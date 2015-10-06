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
    }

	// Use this for initialization
	void Start () {
        chosenBiome = WorldGen.biome.Forest;
        doors = new List<GameObject>();
        walls = new List<GameObject>();
        texLoader = GameObject.FindGameObjectWithTag("PrefabLoader").GetComponent<PrefabLoader>();
        map = new float[height, weight];
        if (seed == -1)
            seed = (int)(Random.value*1000f);
        tileMap = new GameObject[height, weight];
        createShape();
        addWalls();
        addDoors();
        fillAreaWithFillers();
        if (world.previousArea !=null)
            spawnCharacterOnCorrectDoor(world.previousArea.GetComponent<AreaGen>());
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

    void printArea() {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < weight; j++)
            { Debug.Log(i +" "+ j+ " "+ tileMap[i,j]); }
        }
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

            bool blockIsEdgeLeft = (edgePos.i == 0) || tileMap[edgePos.i - 1, edgePos.j] == null;
            bool blockIsEdgeRight = (edgePos.i == weight - 1) || tileMap[edgePos.i + 1, edgePos.j] == null;
            bool blockIsEdgeTop = (edgePos.j == height - 1)  || tileMap[edgePos.i, edgePos.j + 1] == null;
            bool blockIsEdgeDown = (edgePos.j == 0) || tileMap[edgePos.i, edgePos.j - 1] == null;

            if (blockIsEdgeLeft)
            {
                offsetX += tile.GetComponent<Renderer>().bounds.max.x / 2;
            }else
            if (blockIsEdgeRight)
            {
                offsetX -= tile.GetComponent<Renderer>().bounds.max.x / 2;
            }
            else
            if (blockIsEdgeTop)
            {
                offsetZ -= tile.GetComponent<Renderer>().bounds.max.y / 2;
            }
            else
            if (blockIsEdgeDown)
            {
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y;
            }

            Vector3 posToSpawnDoor = new Vector3(edgePos.i * tile.GetComponent<Renderer>().bounds.max.x + offsetX,
                edgePos.j * tile.GetComponent<Renderer>().bounds.max.y + offsetZ, 0);

            foreach(GameObject wallInDoorPlace in walls){
                if (wallInDoorPlace.transform.position == posToSpawnDoor)
                {
                    wallsToRemove.Add(wallInDoorPlace);
                }
            }

            GameObject door = (GameObject)Instantiate(doorPrefab, posToSpawnDoor , doorPrefab.transform.rotation);
            door.transform.parent = this.transform;
            door.GetComponent<Door>().nextArea = node.getConnections()[doorsSpawned].id;
            door.GetComponent<Door>().area = this;
            door.GetComponent<Door>().i = edgePos.i;
            door.GetComponent<Door>().j = edgePos.j;
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

            bool blockIsEdgeLeft = (doorScript.i == 0) || tileMap[doorScript.i - 1, doorScript.j] == null;
            bool blockIsEdgeRight = (doorScript.i == weight - 1) || tileMap[doorScript.i + 1, doorScript.j] == null;
            bool blockIsEdgeDown = (doorScript.j == 0) || tileMap[doorScript.i, doorScript.j - 1] == null;
            bool blockIsEdgeTop  = (doorScript.j == height - 1) || tileMap[doorScript.i, doorScript.j + 1] == null;

            Debug.Log(doorScript.i + " " + doorScript.j + " : " + blockIsEdgeLeft.ToString() + blockIsEdgeRight.ToString() + blockIsEdgeTop.ToString() + blockIsEdgeDown.ToString());


            if (blockIsEdgeLeft)
            {
                offsetX += tile.GetComponent<Renderer>().bounds.max.x *2 ;
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y / 2;
            } else
            if (blockIsEdgeRight)
            {
                offsetX -= tile.GetComponent<Renderer>().bounds.max.x;
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y / 2;
            }
            else
            if (blockIsEdgeTop)
            {
                offsetZ -= tile.GetComponent<Renderer>().bounds.max.y;
                offsetX += tile.GetComponent<Renderer>().bounds.max.x / 2;
            }
            else
            if (blockIsEdgeDown)
            {
                offsetZ += tile.GetComponent<Renderer>().bounds.max.y * 2;
                offsetX += tile.GetComponent<Renderer>().bounds.max.x / 2;
            }

            world.player.transform.position = door.transform.position + new Vector3(offsetX, offsetZ, 0);
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
    }




}
