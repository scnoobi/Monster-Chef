using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGen : MonoBehaviour {

    List<AreaNode> world;
    List<GameObject> areaCache;
    public GameObject areaPrefab;
    public int numOfAreasToCreate;
    public enum biome { Forest, Mountain, Lava, Water};
    public int maxNumConnections = 1;

	// Use this for initialization
	void Start () {
        world = new List<AreaNode>();
        areaCache = new List<GameObject>();
        generateMap();
        generateConnections();
        //printWorld();
        GameObject firstArea = (GameObject)Instantiate(areaPrefab);
        firstArea.GetComponent<AreaGen>().areaID = world[0].id;
        firstArea.GetComponent<AreaGen>().nDoors = world[0].getConnections().Count;
        areaCache.Add(firstArea);
	}

    void generateMap() {
        int nAreasCreated = numOfAreasToCreate;
        int j = 0;
        while (nAreasCreated>0)
        {
            AreaNode areaToGen = new AreaNode();
            areaToGen.id = j;
            j++;
            world.Add(areaToGen);
            nAreasCreated--;
        }
    }

    void generateConnections() {
        int connects = 0;
        int maxNConnects = 1;
        for (int i = 0; i < world.Count; i++)
        {
            maxNConnects = Mathf.Max(1, (int)((Random.value - Random.value) * maxNumConnections));
            connects = 0;
            while (connects < maxNumConnections)
            {
                int selectedNode = (int)(Random.value * (world.Count - 1));

                if (!world[i].getConnections().Contains(world[selectedNode]) && selectedNode != i)
                {
                    world[i].addConnection(world[selectedNode]);
                }
                connects++;
            }
        }
    }

    void printWorld() {
        for (int i = 0; i < world.Count; i++)
        {
            for (int j = 0; j < world[i].getConnections().Count; j++)
            {
                Debug.Log(i + " ----> " + world[i].getConnections()[j].id);
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}


}
