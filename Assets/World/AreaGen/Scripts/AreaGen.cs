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

    public enum tileTypes { walkable, door, wall, empty, debug};
    private float[,] map;
    GridNode[,] grid;
    public GameObject[,] tileMap;
    private List<GridNode> walls;
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

        public string toString() {
            return "i " + i + " ; j " + j;  
        }
    }

    class GridNode{

        public Pos pos;
        public tileTypes gridType;
        public Color debugColor;

        public GridNode(int i, int j, tileTypes gridType)
        {
            this.pos = new Pos(i, j);
            this.gridType = gridType;
        }

        public GridNode(Pos pos, tileTypes gridType)
        {
            this.pos = pos;
            this.gridType = gridType;
        }

        public void setDebug(Color color){
            gridType = tileTypes.debug;
            debugColor = color;
        }

    }

	// Use this for initialization
	void Start () {
        chosenBiome = WorldGen.biome.Forest;
        doors = new List<GameObject>();
        walls = new List<GridNode>();
        texLoader = GameObject.FindGameObjectWithTag("PrefabLoader").GetComponent<PrefabLoader>();
        map = new float[height, weight];
        seed = 875;
        if (seed == -1)
            seed = (int)(Random.value*1000f);
        grid = new GridNode[height, weight];
        tileMap = new GameObject[height, weight];

        createShape();
        posOfEdges = detectEdges();
        detectIslands();
        addWalls();
        addDoors();
        createMap();
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
                    grid[i, j] = new GridNode(i, j, tileTypes.walkable);
                }
                else {
                    grid[i, j] = new GridNode(i, j, tileTypes.empty);
                }

            }
        }
    }

    void createMap() {
        for (int i = 0; i < weight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j].gridType == tileTypes.walkable)
                {
                    tileMap[i, j] = (GameObject)Instantiate(tile, new Vector3(i * tile.GetComponent<Renderer>().bounds.max.x, j * tile.GetComponent<Renderer>().bounds.max.y, 0), tile.transform.rotation);
                    tileMap[i, j].transform.parent = this.transform;
                }
                if (grid[i, j].gridType == tileTypes.wall || grid[i, j].gridType == tileTypes.debug)
                    {
                        GameObject wall = (GameObject)Instantiate(wallPrefab, new Vector3(i * tile.GetComponent<Renderer>().bounds.max.x,
                            j * tile.GetComponent<Renderer>().bounds.max.y, 0), wallPrefab.transform.rotation);
                        wall.transform.parent = this.transform;
                        wall.GetComponent<Wall>().i = i;
                        wall.GetComponent<Wall>().j = j;
                    }
            }
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < weight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j].gridType == tileTypes.debug)
                {
                   Gizmos.color = grid[i, j].debugColor;
                   Gizmos.DrawCube(new Vector3(i * tile.GetComponent<Renderer>().bounds.max.x, j * tile.GetComponent<Renderer>().bounds.max.y, 0), new Vector3(1f, 1f, 1f));
                }
            }
        }
    }

    List<Pos> detectEdges()
    {
        List<Pos> posOfEdges = new List<Pos>();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight; j++)
            {
                if (grid[i, j].gridType != tileTypes.empty)
                {
                    bool blockIsEdgeLeft = (i == 0) || grid[i - 1, j].gridType == tileTypes.empty;
                    bool blockIsEdgeRight = (i == weight - 1) || grid[i + 1, j].gridType == tileTypes.empty;
                    bool blockIsEdgeTop = (j == 0) || grid[i, j - 1].gridType == tileTypes.empty;
                    bool blockIsEdgeDown = (j == height - 1) || grid[i, j + 1].gridType == tileTypes.empty;

                    if (blockIsEdgeLeft || blockIsEdgeRight || blockIsEdgeTop || blockIsEdgeDown)
                    {
                        posOfEdges.Add(new Pos(i, j, blockIsEdgeLeft, blockIsEdgeRight, blockIsEdgeTop, blockIsEdgeDown));
                        grid[i, j].gridType = tileTypes.wall;
                    }
                }
            }
        }
        return posOfEdges;
    }

    void addWalls()
    {
        foreach (Pos p in posOfEdges)
        {

            bool hasTileOnTop = ((p.j != height - 1) && grid[p.i, p.j + 1].gridType != tileTypes.empty);
            bool hasTileOnBottom = ((p.j != 0) && grid[p.i, p.j - 1].gridType != tileTypes.empty);
            bool hasTileOnRight = ((p.i != weight - 1) && grid[p.i + 1, p.j].gridType != tileTypes.empty);
            bool hasTileOnleft = ((p.i != 0) && grid[p.i - 1, p.j].gridType != tileTypes.empty);

            bool hasAtleastOneTileNear = hasTileOnleft || hasTileOnRight || hasTileOnBottom || hasTileOnTop;

            if (hasAtleastOneTileNear)
            {
                walls.Add(grid[p.i, p.j]);
            }
        }
    }

    void detectIslands() {
        int size = 0;
        List<Pos> island = new List<Pos>();
        List<Pos> islandEdges = new List<Pos>();
        List<Pos> deleteThese = new List<Pos>();
        List<List<Pos>> fuseThese = new List<List<Pos>>();
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
                if (grid[i, j].gridType == tileTypes.walkable && checkedMap[i, j]==0){
                    checkedMap[i, j] = 1;
                    island = new List<Pos>();
                    islandEdges = new List<Pos>();
                    size = fillIsland(i, j, checkedMap, island, islandEdges);
                    if (size < islandSizeThreshold)
                    {
                        Debug.Log("delete " + island.Count);
                        deleteThese.InsertRange(deleteThese.Count, island);
                    }
                    else { 
                        Debug.Log("islands with size " + size);
                        Debug.Log("fist val "+islandEdges[0].toString());
                        fuseThese.Add(islandEdges);
                    }
                }
            }
        }
        deleteIsland(deleteThese);
        fuseIslands(fuseThese);
    }

    int fillIsland(int i, int j, int[,] checkedMap, List<Pos> island, List<Pos> islandEdges)
    {
        int size = checkedMap[i, j];
        island.Add(new Pos(i, j));
        if (i < height - 1 && checkedMap[i + 1, j] == 0)
        {
            if (grid[i + 1, j].gridType == tileTypes.walkable)
            {
                checkedMap[i + 1, j] = checkedMap[i, j] + 1;
                size = Mathf.Max(fillIsland(i + 1, j, checkedMap, island, islandEdges), size);
            }
        }

        if (i < height - 1 && grid[i + 1, j].gridType == tileTypes.wall)
        {
            islandEdges.Add(new Pos(i + 1, j));
        }

        if (i > 0 && grid[i - 1, j].gridType == tileTypes.walkable && checkedMap[i - 1, j] == 0)
        {
            if (grid[i - 1, j].gridType == tileTypes.walkable)
            {
                checkedMap[i - 1, j] = checkedMap[i, j] + 1;
                size = Mathf.Max(fillIsland(i - 1, j, checkedMap, island, islandEdges), size);
            }
        }

        if (i > 0 && grid[i - 1, j].gridType == tileTypes.wall)
        {
            islandEdges.Add(new Pos(i - 1, j));
        }

        if (j < weight - 1 && grid[i, j + 1].gridType == tileTypes.walkable && checkedMap[i, j + 1] == 0)
        {
            if (grid[i, j + 1].gridType == tileTypes.walkable)
            {
            checkedMap[i, j + 1] = checkedMap[i, j] + 1; ;
            size = Mathf.Max(fillIsland(i, j + 1, checkedMap, island, islandEdges), size);
            }
        }

        if (j < weight - 1 && grid[i, j + 1].gridType == tileTypes.wall)
        {
            islandEdges.Add(new Pos(i, j + 1));
        }

        if (j > 0 && grid[i, j - 1].gridType == tileTypes.walkable && checkedMap[i, j - 1] == 0)
        {
            if (grid[i, j - 1].gridType == tileTypes.walkable)
            {
            checkedMap[i, j - 1] = checkedMap[i, j] + 1;
            size = Mathf.Max(fillIsland(i, j - 1, checkedMap, island, islandEdges), size);
            }
        }

        if (j > 0 && grid[i, j - 1].gridType == tileTypes.wall)
        {
            islandEdges.Add(new Pos(i, j - 1));
        }

        return size;
    }

    void deleteIsland(List<Pos> deleteThese) {
        foreach (Pos delete in deleteThese)
        {
            GameObject.Destroy(tileMap[delete.i, delete.j]);
            grid[delete.i, delete.j].gridType = tileTypes.empty;
            if (walls.Count > 0)
            {
                foreach (GridNode wall in walls)
                {
                    if (wall.pos.i == delete.i + 1 || wall.pos.i == delete.i - 1 || wall.pos.i == delete.i)
                        if (wall.pos.j == delete.j + 1 || wall.pos.j == delete.j - 1 || wall.pos.j == delete.j)
                        {
                            wall.gridType = tileTypes.empty;
                            for (int i = posOfEdges.Count - 1; i >= 0; i--)
                            {
                                if (wall.pos.i == posOfEdges[i].i && wall.pos.j == posOfEdges[i].j)
                                    posOfEdges.RemoveAt(i);
                            }
                        }
                }
            }
        }
    }

    void fuseIslands(List<List<Pos>> edgesOfIslands)
    {
        List<Pos> closestNodes = new List<Pos>();
        Pos pos1 = new Pos(-1,-1), pos2 = new Pos(-1,-1);
        float distance = float.MaxValue;
        Color[] colors = { Color.red, Color.green, Color.cyan };
        for (int i = 0; i < edgesOfIslands.Count; i++)
        {
            foreach (Pos edge in edgesOfIslands[i])
            {
                //grid[edge.i, edge.j].setDebug(colors[i]);
            }
        }
        for (int i = 0; i < edgesOfIslands.Count - 1; i++)
        {
            foreach (Pos edge in edgesOfIslands[i])
            {
                foreach (Pos edge2 in edgesOfIslands[i + 1])
                {
                    float testDistance = Vector2.Distance(new Vector2(edge.i, edge.j), new Vector2(edge2.i, edge2.j));
                    if (distance > testDistance)
                    {
                        distance = testDistance;
                        pos1 = edge;
                        pos2 = edge2;
                    }
                }
            }
            grid[pos1.i, pos1.j].setDebug(colors[i]);
            grid[pos2.i, pos2.j].setDebug(colors[i]);
            closestNodes.Add(pos1);
            closestNodes.Add(pos2);
            Debug.DrawLine(new Vector2(pos1.i * tile.GetComponent<Renderer>().bounds.max.x, pos1.j * tile.GetComponent<Renderer>().bounds.max.y),
                new Vector2(pos2.i * tile.GetComponent<Renderer>().bounds.max.x, pos2.j * tile.GetComponent<Renderer>().bounds.max.y),
                Color.green, 50.0f, true);
        }

        getLineBetweenIslands(closestNodes);
    }

    List<List<Pos>> getLineBetweenIslands(List<Pos> closestNodes)
    {
        List<List<Pos>>  allLines = new List<List<Pos>>();
        List<Pos> line;
        for (int i = 0; i < closestNodes.Count; i += 2)
        {
            line = new List<Pos>();
            int x = closestNodes[i].i;
            int y = closestNodes[i].j;
            int dx =  closestNodes[i + 1].i - x;
            int dy =  closestNodes[i + 1].j - y;
            Debug.Log("dx " + dx);
            Debug.Log("dy " + dy);
            bool inverted = false;
            int step = (int)Mathf.Sign(dx);
            int gradientStep = (int)Mathf.Sign(dy);
            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);
                step = (int)Mathf.Sign(dy);
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
                    gradientAccumulation -= longest;
                }
            }
            allLines.Add(line);
            Debug.Log("lineSize"+line.Count);
            Debug.DrawLine(new Vector2(line[0].i * tile.GetComponent<Renderer>().bounds.max.x, line[0].j * tile.GetComponent<Renderer>().bounds.max.y),
                new Vector2(line[line.Count - 1].i * tile.GetComponent<Renderer>().bounds.max.x, line[line.Count - 1].j * tile.GetComponent<Renderer>().bounds.max.y),
                Color.red, 50.0f, true);
        }
        return allLines;
    }

    void createPassageWayBetweenTwoCloseNodes(List<List<Pos>> allLines)
    {


    }

    void addDoors()
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
