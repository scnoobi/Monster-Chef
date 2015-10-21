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

    public enum tileTypes { walkable, door, wall, empty};
    private float[,] map;
    GridNode[,] grid;
    public GameObject[,] tileMap;
    private List<GameObject> doors;
    List<List<Pos>> allLinesBetweenIslands;
    float inNum;

    //struct that holds 2 indexes for position in a matrix
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

    //struct that works as an abstraction for a tile/gridnode
    class GridNode{

        public Pos pos;
        public tileTypes gridType;
        public bool debugMode = false;
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
            debugMode = true;
            debugColor = color;
        }
    }

	void Start () {
        chosenBiome = WorldGen.biome.Forest;
        doors = new List<GameObject>();
        texLoader = GameObject.FindGameObjectWithTag("PrefabLoader").GetComponent<PrefabLoader>();
        map = new float[height, weight];
        if (seed == -1)
            seed = (int)(Random.value*1000f);
        grid = new GridNode[height, weight];
        tileMap = new GameObject[height, weight];

        createShape();
        posOfEdges = detectEdges();
        detectIslands();
        createPassageWayBetweenTwoCloseNodes(allLinesBetweenIslands);
        posOfEdges = detectEdges();
        addDoors();
        createMap();
        fillAreaWithFillers();

        if (world.previousArea != null)
            spawnCharacterOnCorrectDoor(world.previousArea.GetComponent<AreaGen>());
        else
            spawnCharacterFirstArea();
	}

    //when an area is enabled means the character just got into it and should spawn on the correct door
    void OnEnable()
    {
        if (world != null)
            spawnCharacterOnCorrectDoor(world.previousArea.GetComponent<AreaGen>());
    }

    public List<GameObject> getDoors() { return doors; }

    //use perlin noise, load it into a matrix and depending on a values load it with a walkable gridNode or an empty gridNode
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

    //goes through the matrix with all the gridNode and if has type walkable, instantiates a walkable gameObject, if type is wall, instantiates a wall gameObject
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
                if (grid[i, j].gridType == tileTypes.wall)
                    {
                        GameObject wall = (GameObject)Instantiate(wallPrefab, new Vector3(i * tile.GetComponent<Renderer>().bounds.max.x,
                            j * tile.GetComponent<Renderer>().bounds.max.y, 0), wallPrefab.transform.rotation);
                        wall.transform.parent = this.transform;
                    }
            }
        }
    }

    //detects the edges of the map
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
        //clean out lonely useless walls
        for (int i = posOfEdges.Count - 1; i >= 0; i--){
            Pos pos = posOfEdges[i];
            bool atleastOneWalkableTileNearby = ((pos.i != 0) && grid[pos.i - 1, pos.j].gridType == tileTypes.walkable) ||
                ((pos.i != weight - 1) && grid[pos.i + 1, pos.j].gridType == tileTypes.walkable) ||
                ((pos.j != 0) && grid[pos.i, pos.j - 1].gridType == tileTypes.walkable) ||
                ((pos.j != height - 1) && grid[pos.i, pos.j + 1].gridType == tileTypes.walkable);

            if (!atleastOneWalkableTileNearby)
            {
                grid[pos.i, pos.j].gridType = tileTypes.empty;
                posOfEdges.RemoveAt(i);
            }

        }

        return posOfEdges;
    }

    //detects if there are separate landmasses
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
                        deleteThese.InsertRange(deleteThese.Count, island);
                    }
                    else { 
                        fuseThese.Add(islandEdges);
                    }
                }
            }
        }
        deleteIsland(deleteThese);
        fuseIslands(fuseThese);
    }

    //recursively fills an island and find the edges of that specific island
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

    //delete a specific island
    void deleteIsland(List<Pos> deleteThese) {
        foreach (Pos delete in deleteThese)
        {
            if (grid[delete.i, delete.j].gridType == tileTypes.wall) {
                for (int i = posOfEdges.Count - 1; i >= 0; i--)
                {
                    if (posOfEdges[i].i == delete.i && posOfEdges[i].j == delete.j)
                        posOfEdges.RemoveAt(i);
                }
            }
            grid[delete.i, delete.j].gridType = tileTypes.empty;
        }
    }

    //fuse all islands
    void fuseIslands(List<List<Pos>> edgesOfIslands)
    {
        List<Pos> closestNodes = new List<Pos>();
        Pos pos1 = new Pos(-1,-1), pos2 = new Pos(-1,-1);
        float distance = float.MaxValue;
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
            closestNodes.Add(pos1);
            closestNodes.Add(pos2);
            pos1 = new Pos(-1, -1);  pos2 = new Pos(-1, -1);
            distance = float.MaxValue;
        }
        allLinesBetweenIslands = getLineBetweenIslands(closestNodes);
    }

    //get a line between the closest two wall points
    List<List<Pos>> getLineBetweenIslands(List<Pos> closestNodes)
    {
        List<List<Pos>>  allLines = new List<List<Pos>>();
        List<Pos> line;
        for (int i = 0; i < closestNodes.Count; i += 2)
        {
            int x0 = closestNodes[i].i;
            int y0 = closestNodes[i].j;
            int x1 = closestNodes[i + 1].i;
            int y1 = closestNodes[i + 1].j;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int x = x0;
            int y = y0;
            int n = 1 + dx + dy;
            int x_inc = (x1 > x0) ? 1 : -1;
            int y_inc = (y1 > y0) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;
            line = new List<Pos>();
            for (; n > 0; --n)
            {
                line.Add(new Pos(x, y));

                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                }
            }
            /*
            Debug.Log("start "+line[0].toString());
            Debug.Log("end "+line[line.Count - 1].toString());
            Debug.DrawLine(new Vector2(line[0].i * tile.GetComponent<Renderer>().bounds.max.x, line[0].j * tile.GetComponent<Renderer>().bounds.max.y),
    new Vector2(line[line.Count - 1].i * tile.GetComponent<Renderer>().bounds.max.x, line[line.Count - 1].j * tile.GetComponent<Renderer>().bounds.max.y),
    Color.red, 50.0f, true);
             */
            allLines.Add(line);
        }
        return allLines;
    }

    //get lines between islands and create a passageway with walkable nodes
    void createPassageWayBetweenTwoCloseNodes(List<List<Pos>> allLines)
    {
        int radius = 4;
        for (int i = 0; i < allLines.Count; i++)
        {
            foreach (Pos pos in allLines[i])
            {
                drawCircle( pos, radius);
            }
        }
    }

    //make a walkable circle
    void drawCircle(Pos pos, int r) {

        for (int x = -r; x <= r; x++) {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = pos.i + x;
                    int drawY = pos.j + y;
                    if (isInsideGridMap(drawX, drawY))
                        grid[drawX, drawY].gridType = tileTypes.walkable;
                }

            }
        
        }
    }

    bool isInsideGridMap(int i, int j)
    {
        return i >= 0 && i < height && j >= 0 && j < weight;

    }

    void addDoors()
    {
        int doorsSpawned = 0;
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

    //first area the character does not come from other doors so spawn inside in an walkable tile
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

    //TODO
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

    void OnDrawGizmos()
    {
        for (int i = 0; i < weight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (grid[i, j].debugMode)
                {
                    Gizmos.color = grid[i, j].debugColor;
                    Gizmos.DrawCube(new Vector3(i * tile.GetComponent<Renderer>().bounds.max.x, j * tile.GetComponent<Renderer>().bounds.max.y, 0), new Vector3(1f, 1f, 1f));
                }
            }
        }
    }


}
