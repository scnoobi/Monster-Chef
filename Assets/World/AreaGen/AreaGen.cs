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
    public int areaID;
    public int nDoors;

    public GameObject wallPrefab;
    public GameObject doors;
    public Texture2D tileTexture;

    private float[,] map;
    private GameObject[,] tileMap;
    private List<GameObject> walls;
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
        walls = new List<GameObject>();
        map = new float[height, weight];
        if (seed == -1)
            seed = (int)(Random.value*1000f);
        tileMap = new GameObject[height, weight];
        createShape();
        addWalls();
        addDoors();
	}

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
        for (int i = 0; i < weight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(tileMap[i, j] != null){
                    bool blockIsEdgeLeft = (i==0) || tileMap[i - 1, j] == null;
                    bool blockIsEdgeRight = (i == weight-1) || tileMap[i + 1, j] == null;
                    bool blockIsEdgeTop = (j == 0) || tileMap[i , j - 1] == null;
                    bool blockIsEdgeDown = (j == height - 1) || tileMap[i , j + 1] == null;

                    if(blockIsEdgeLeft || blockIsEdgeRight || blockIsEdgeTop || blockIsEdgeDown)
                        posOfEdges.Add(new Pos(i, j, blockIsEdgeLeft, blockIsEdgeRight, blockIsEdgeTop, blockIsEdgeDown));

                    foreach(Pos p in posOfEdges)
                        tileMap[p.i, p.j].GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
        return posOfEdges;
    }

    void addWalls()
    {
        List<Pos> posOfEdges = detectEdges();
        foreach (Pos p in posOfEdges) {
            GameObject wall = (GameObject)Instantiate(wallPrefab, new Vector3(p.i * tile.GetComponent<Renderer>().bounds.max.x,
                p.j * tile.GetComponent<Renderer>().bounds.max.y, 0), wallPrefab.transform.rotation);
            wall.transform.parent = this.transform;
            walls.Add(wall);
        }
    }

    void addDoors()     // remove Walls from under.... or change so they arnt even placed there... maybe place the doors first and only then the walls
    {
        int doorsSpawned = 0;
        List<Pos> posOfEdges = detectEdges();
        while (doorsSpawned < nDoors) {
            int edge = (int)(Random.value * (posOfEdges.Count - 1));
            Pos edgePos= posOfEdges[edge];
            Instantiate(doors, new Vector3(edgePos.i * tile.GetComponent<Renderer>().bounds.max.x,
                edgePos.j * tile.GetComponent<Renderer>().bounds.max.y, 0), doors.transform.rotation);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {
            Debug.Log("apocalyppse");
            destroyShape();
            tileMap = new GameObject[height, weight];
            createShape();
        }

    }
}
