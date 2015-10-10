using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaGen : MonoBehaviour {

    public int height;
    public int weight;
    public GameObject tile;
    public float threshold;
    public float scale;
    public int seed;
    public Texture2D maskTex;
    public AreaNode node;
    public WorldGen.biome chosenBiome;
    public float islandSizeThreshold = 45;

    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public Texture2D tileTexture;

    public WorldGen world;
    public PrefabLoader texLoader;
    private List<Pos> posOfEdges;
    private float[,] map;
    public GameObject[,] tileMap;
    private List<GameObject> walls;
    private List<GameObject> doors;
    float inNum;


    struct Pos {
        public int i, j;
        public bool left, right, top, bottom;

        public Pos(int i, int j, bool l, bool r, bool t, bool b)
        { 
            this.i = i; 
            this.j = j;
            this.left = l;
            this.right = r;
            this.top = t;
            this.bottom = b;
        }

        public Pos(int i, int j)
        {
            this.i = i;
            this.j = j;
            this.left = false;
            this.right = false;
            this.top = false;
            this.bottom = false;
        }
    }

	// Use this for initialization
	void Start () {
        chosenBiome = WorldGen.biome.Forest;
        doors = new List<GameObject>();
        walls = new List<GameObject>();
        texLoader = GameObject.FindGameObjectWithTag("PrefabLoader").GetComponent<PrefabLoader>();
        map = new float[height, weight];
        seed = 875;
        if (seed == -1)
            seed = (int)(Random.value*1000f);
        tileMap = new GameObject[height, weight];
        createShape();
        addWalls();
        detectIslands();
        addDoors();
        fillAreaWithFillers();
        if (world.previousArea != null)
            spawnCharacterOnCorrectDoor(world.previousArea.GetComponent<AreaGen>());
        else
            spawnCharacterFirstArea();
	}

    void OnEnable()
    {
        if (world != null)
            spawnCharacterOnCorrectDoor(world.previousArea.GetComponent<AreaGen>());
    }

    public List<GameObject> getDoors() { return doors; }

    void createShape() {
        for (int i = 0; i < weight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[i, j] = Mathf.PerlinNoise(((float)(seed + i) / (float)scale), ((float)(seed + j) / (float)scale));
                if (map[i, j] <= threshold)
                {
                    tileMap[i, j] = (GameObject)Instantiate(tile, new Vector3(i * tile.GetComponent<Renderer>().bounds.max.x, j * tile.GetComponent<Renderer>().bounds.max.y, 0), tile.transform.rotation);
                    tileMap[i, j].transform.parent = this.transform;
                }
            }
        }
    }

    List<Pos> detectEdges() {
        List<Pos> posOfEdges = new List<Pos>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight ; j++)
            {
                if(tileMap[i, j] != null){
                    bool blockIsEdgeLeft = (i==0) || tileMap[i - 1, j] == null;
                    bool blockIsEdgeRight = (i == weight-1) || tileMap[i + 1, j] == null;
                    bool blockIsEdgeTop = (j == 0) || tileMap[i , j - 1] == null;
                    bool blockIsEdgeDown = (j == height - 1) || tileMap[i , j + 1] == null;


                    if (blockIsEdgeLeft || blockIsEdgeRight || blockIsEdgeTop || blockIsEdgeDown)
                    {
                        posOfEdges.Add(new Pos(i, j, blockIsEdgeLeft, blockIsEdgeRight, blockIsEdgeTop, blockIsEdgeDown));
                        GameObject delete = tileMap[i, j];
                        GameObject.Destroy(delete);
                    }
                }
            }
        }
        foreach (Pos p in posOfEdges)
        {
            tileMap[p.i, p.j] = null;
        }
        return posOfEdges;
    }

    void detectIslands() {
        int size = 0;
        List<Pos> island = new List<Pos>();
        List<Pos> deleteThese = new List<Pos>();
        List<Pos> fuseThese = new List<Pos>();
        int[,] checkedMap = new int[height, weight];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight; j++)
            {
                checkedMap[i, j] = 0;
            }
        }
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight ; j++)
            {
                if (tileMap[i, j] != null && checkedMap[i, j]==0){
                    checkedMap[i, j] = 1;
                    island = new List<Pos>();
                    size = fillIsland(i, j, checkedMap, island);
                    if (size < islandSizeThreshold)
                    {
                        Debug.Log("delete " + island.Count);
                        deleteThese.InsertRange(deleteThese.Count, island);
                    }
                    else { 
                        Debug.Log("islands with size " + size);
                        fuseThese.Add(island[0]);
                    }
                }
            }
        }
        deleteIsland(deleteThese);
        fuseIslands(fuseThese);
    }

    void deleteIsland(List<Pos> deleteThese) {
        foreach (Pos delete in deleteThese)
        {
            GameObject.Destroy(tileMap[delete.i, delete.j]);
            tileMap[delete.i, delete.j] = null;
            if (walls.Count > 0)
            {
                foreach (GameObject wall in walls)
                {
                    if (wall.GetComponent<Wall>().i == delete.i + 1 || wall.GetComponent<Wall>().i == delete.i - 1 || wall.GetComponent<Wall>().i == delete.i)
                        if (wall.GetComponent<Wall>().j == delete.j + 1 || wall.GetComponent<Wall>().j == delete.j - 1 || wall.GetComponent<Wall>().j == delete.j)
                        {
                            GameObject.Destroy(wall);
                            for (int i = posOfEdges.Count - 1; i >= 0; i--)
                            {
                                if (wall.GetComponent<Wall>().i == posOfEdges[i].i && wall.GetComponent<Wall>().j == posOfEdges[i].j)
                                    posOfEdges.RemoveAt(i);
                            }
                        }
                }
            }
        }
    }

    void fuseIslands(List<Pos> islandRepresentatives)
    {
        List<List<Pos>> segmentedposOfEdges = new List<List<Pos>>();
        Pos closestEdgePoint = new Pos(int.MaxValue,int.MaxValue);
        foreach (Pos representative in islandRepresentatives)
        {
            foreach (Pos edge in posOfEdges)
            {
                Vector2 rep = new Vector2(representative.i, representative.j);
                if (Vector2.Distance(rep, new Vector2(closestEdgePoint.i, closestEdgePoint.j)) > Vector2.Distance(rep, new Vector2(edge.i, edge.j)))
                    closestEdgePoint = edge;
            }

            List<Pos> edgeOfIsland = new List<Pos>();
            foreach (Pos edge in posOfEdges)
            {
                bool sameColumn = edge.i == closestEdgePoint.i;
                bool sameLine = edge.j == closestEdgePoint.j;
                bool left = edge.i == closestEdgePoint.i - 1;
                bool right = edge.i == closestEdgePoint.i + 1;
                bool down = edge.j == closestEdgePoint.j + 1;
                bool up = edge.j == closestEdgePoint.j - 1;

                bool leftStraight = left && sameColumn;
                bool rightStraight = right && sameColumn;
                bool downStraight = down && sameLine;
                bool upStraight = up && sameLine;
                bool leftDiagonalUp = left && up;
                bool leftDiagonalDown = left && down;
                bool rightDiagonalUp = right && up;
                bool rightDiagonalDown = right && down;

                if (leftStraight || rightStraight || downStraight || upStraight || leftDiagonalUp || leftDiagonalDown || rightDiagonalUp || rightDiagonalDown) {
                    edgeOfIsland.Add(closestEdgePoint);
                    closestEdgePoint = edge;
                }
            }
            segmentedposOfEdges.Add(edgeOfIsland);
        }
        List<Pos> closestNodes = new List<Pos>();
        Pos pos1 = new Pos(-1,-1), pos2 = new Pos(-1,-1);
        int i = 0;
        float distance = float.MaxValue;
        foreach (Pos edge in segmentedposOfEdges[i]) {
            foreach (Pos edge2 in segmentedposOfEdges[i+1])
            {
                float testDistance = Vector2.Distance(new Vector2(edge.i,edge.j), new Vector2(edge2.i,edge2.j));
                if (distance > testDistance)
                {
                    distance = testDistance;
                    pos1 = edge;
                    pos2 = edge2;
                }
            }
            closestNodes.Add(pos1);
            closestNodes.Add(pos2);
        }
        getLineBetweenIslands(closestNodes);
    }

    List<List<Pos>> getLineBetweenIslands(List<Pos> closestNodes)
    {
        List<List<Pos>>  allLines = new List<List<Pos>>();
        int j = 0, k = 0;
        for (int i = 0; i < closestNodes.Count; i += 2)
        {
            List<Pos> line = new List<Pos>();
            int x = closestNodes[i].i;
            int y = closestNodes[i].j;
            int dx = x - closestNodes[i + 1].i;
            int dy = y - closestNodes[i + 1].j;
            bool inverted = false;
            int step = (int)Mathf.Sign(dx);
            int gradientStep = (int)Mathf.Sign(dy);
            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = shortest;
                shortest = Mathf.Abs(dx);
                step = gradientStep;
                gradientStep = (int)Mathf.Sign(dx);
            }

            int gradientAccumulation = longest / 2;
            for (int l = 0; l < longest; l++)
            {
                line.Add(new Pos(x, y));
                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }
                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                }
            }
            allLines.Add(line);
            Debug.DrawLine(new Vector2(line[0].i, line[0].j), new Vector2(line[line.Count - 1].i, line[line.Count - 1].j), Color.red, 10.0f, true);
        }
        return allLines;
    }

    void createPassageWayBetweenTwoCloseNodes(List<List<Pos>> allLines)
    {


    }

    int fillIsland(int i, int j, int[,] checkedMap, List<Pos> island)
    {
        int size = checkedMap[i, j];
        island.Add(new Pos(i, j));
        if (i < height - 1 && tileMap[i + 1, j] != null && checkedMap[i + 1, j]==0)
        {
            checkedMap[i + 1, j] = checkedMap[i , j]+1;
            size = Mathf.Max(fillIsland(i + 1, j, checkedMap, island),size);
        }
        if (i > 0 && tileMap[i - 1, j] != null && checkedMap[i - 1, j] == 0)
        {
            checkedMap[i - 1, j] = checkedMap[i, j] + 1;
            size = Mathf.Max(fillIsland(i - 1, j, checkedMap, island), size);
        }
        if (j < weight - 1 && tileMap[i, j + 1] != null && checkedMap[i, j + 1] == 0)
        {
            checkedMap[i, j + 1] = checkedMap[i, j] + 1; ;
            size = Mathf.Max(fillIsland(i, j + 1, checkedMap, island), size);
        }
        if (j > 0 && tileMap[i, j - 1] != null && checkedMap[i, j - 1] == 0)
        {
            checkedMap[i, j - 1] = checkedMap[i, j] + 1;
            size = Mathf.Max(fillIsland(i, j - 1, checkedMap, island), size);
        }
        return size;
    }

    void addWalls()
    {
        posOfEdges = detectEdges();
        foreach (Pos p in posOfEdges) {

            bool hasTileOnTop  = ((p.j != height - 1) && tileMap[p.i, p.j + 1] != null);
            bool hasTileOnBottom = ((p.j != 0) && tileMap[p.i, p.j - 1] != null);
            bool hasTileOnRight = ((p.i != weight - 1) && tileMap[p.i + 1, p.j] != null);
            bool hasTileOnleft = ((p.i != 0) && tileMap[p.i - 1, p.j] != null);

            //Debug.Log( p.i + " " + p.j+ " "+  hasTileOnleft + " " + hasTileOnRight +" "+ hasTileOnTop +" " +hasTileOnBottom);
            bool hasAtleastOneTileNear = hasTileOnleft || hasTileOnRight || hasTileOnBottom || hasTileOnTop;
            
            if (hasAtleastOneTileNear)
            {
                GameObject wall = (GameObject)Instantiate(wallPrefab, new Vector3(p.i * tile.GetComponent<Renderer>().bounds.max.x,
                    p.j * tile.GetComponent<Renderer>().bounds.max.y, 0), wallPrefab.transform.rotation);
                wall.transform.parent = this.transform;
                wall.GetComponent<Wall>().i = p.i;
                wall.GetComponent<Wall>().j = p.j;
                walls.Add(wall);
            }
        }
    }

    void addDoors()     // remove Walls from under.... or change so they arnt even placed there... maybe place the doors first and only then the walls
    {
        int doorsSpawned = 0;
        List<GameObject> wallsToRemove = new List<GameObject>();
        while (doorsSpawned < node.getConnections().Count)
        { 
            int edge = (int)(Random.value * (posOfEdges.Count - 1));
            Pos edgePos= posOfEdges[edge];

            float offsetX = 0, offsetZ = 0;

            bool blockIsEdgeLeft = (edgePos.i == 0) || map[edgePos.i - 1, edgePos.j] > threshold;
            bool blockIsEdgeRight = (edgePos.i == weight - 1) || map[edgePos.i + 1, edgePos.j] > threshold;
            bool blockIsEdgeTop = (edgePos.j == height - 1) || map[edgePos.i, edgePos.j + 1] > threshold;
            bool blockIsEdgeDown = (edgePos.j == 0) || map[edgePos.i, edgePos.j - 1] > threshold;


            bool correctPlacement = false;
            correctPlacement |= (blockIsEdgeLeft && node.getConnections()[doorsSpawned].getDecidedDirection() == Connection.Direction.left);
            correctPlacement |= (blockIsEdgeRight && node.getConnections()[doorsSpawned].getDecidedDirection() == Connection.Direction.right);
            correctPlacement |= (blockIsEdgeTop && node.getConnections()[doorsSpawned].getDecidedDirection() == Connection.Direction.top);
            correctPlacement |= (blockIsEdgeDown && node.getConnections()[doorsSpawned].getDecidedDirection() == Connection.Direction.down);


            if (!correctPlacement)
                continue;

            //Debug.Log(edgePos.i + " " + edgePos.j + " : " + blockIsEdgeLeft.ToString() + blockIsEdgeRight.ToString() + blockIsEdgeTop.ToString() + blockIsEdgeDown.ToString());

            if (blockIsEdgeLeft)
            {
                offsetX += tile.GetComponent<Renderer>().bounds.max.x / 2;
            }
            if (blockIsEdgeRight)
            {
                offsetX -= tile.GetComponent<Renderer>().bounds.max.x / 2;
            }
            
            if (blockIsEdgeTop)
            {
                offsetZ -= tile.GetComponent<Renderer>().bounds.max.y / 2;
            }
            
            if (blockIsEdgeDown)
            {
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y;
            }

            Vector3 posToSpawnDoor = new Vector3(edgePos.i * tile.GetComponent<Renderer>().bounds.max.x + offsetX,
                edgePos.j * tile.GetComponent<Renderer>().bounds.max.y + offsetZ, 0);

            GameObject door = (GameObject)Instantiate(doorPrefab, posToSpawnDoor , doorPrefab.transform.rotation);
            door.transform.parent = this.transform;
            door.GetComponent<Door>().nextArea = node.getConnections()[doorsSpawned].getNode().id;
            door.GetComponent<Door>().area = this;
            door.GetComponent<Door>().i = edgePos.i;
            door.GetComponent<Door>().j = edgePos.j;
            door.GetComponent<Door>().chosenDirection = node.getConnections()[doorsSpawned].getDecidedDirection();
            doors.Add(door);
            doorsSpawned++;
        }
    }

    void destroyShape() {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight; j++)
            {
                GameObject.Destroy(tileMap[i, j]);
            }
        }
    }

    void printArea()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight; j++)
            { Debug.Log(i + " " + j + " " + tileMap[i, j]); }
        }
    }

    public void spawnCharacterOnCorrectDoor(AreaGen prevArea)
    {
        if (prevArea == null)
            return;
        foreach (GameObject door in doors)
        {
            float offsetX = 0f;
            float offsetZ = 0f;
            Door doorScript = door.GetComponent<Door>();

            if (!(doorScript.GetComponent<Door>().nextArea == prevArea.node.id))
                continue;

            if (doorScript.GetComponent<Door>().chosenDirection == Connection.Direction.left)
            {
                offsetX += tile.GetComponent<Renderer>().bounds.max.x *2 ;
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y / 2;
            } else
                if (doorScript.GetComponent<Door>().chosenDirection == Connection.Direction.right)
            {
                offsetX -= tile.GetComponent<Renderer>().bounds.max.x;
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y / 2;
            }
            else
                    if (doorScript.GetComponent<Door>().chosenDirection == Connection.Direction.top)
            {
                offsetZ -= tile.GetComponent<Renderer>().bounds.max.y;
                offsetX += tile.GetComponent<Renderer>().bounds.max.x / 2;
            }
            else
                        if (doorScript.GetComponent<Door>().chosenDirection == Connection.Direction.down)
            {
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y * 2;
                offsetX += tile.GetComponent<Renderer>().bounds.max.x / 2;
            }

            world.player.transform.position = door.transform.position + new Vector3(offsetX, offsetZ, 0);
        }
    }

    public void spawnCharacterFirstArea()
    {
        for (int i = height/2; i < height; i++)
        {
            for (int j = weight/2; j < weight; j++)
            {
                if (tileMap[i, j] != null)
                    world.player.transform.position = tileMap[i, j].transform.position;
            }
        }
    }

    public void TriggerNextArea(int idOfNextArea) {
        world.generateNextArea(idOfNextArea);
    }

    public void fillAreaWithFillers()
    {
        GameObject[] biomeFillers;
        switch (chosenBiome) { 
        
            case WorldGen.biome.Forest:
                biomeFillers = texLoader.forestFillers;
                break;

            default:
                return;
        }
        Vector3 spawnPos = new Vector3(0,0,0);
        GameObject fillerToSpawn = (GameObject)Instantiate(biomeFillers[0], spawnPos, biomeFillers[0].transform.rotation);
        fillerToSpawn.transform.parent = transform;
    }




}
