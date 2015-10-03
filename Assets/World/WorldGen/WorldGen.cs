using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGen : MonoBehaviour {

    List<AreaNode> world;
    List<GameObject> areaCache;
    GameObject currentArea;
    public GameObject previousArea;
    public GameObject areaPrefab;
    public int numOfAreasToCreate;
    public enum biome { Forest, Mountain, Lava, Water};
    public int maxNumConnections = 1;
    public GameObject player;

	// Use this for initialization
	void Start () {
        world = new List<AreaNode>();
        areaCache = new List<GameObject>();
        generateMap();
        generateConnections();
       // printWorld();
        generateNextArea(0);
        player = GameObject.FindGameObjectWithTag("Player");
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
        List<AreaNode> worldDisconnected = world;
        int maxNConnects = 1;
        for (int i = 0; i < world.Count; i++)
        {
            maxNConnects = Mathf.Max(1, (int)((Random.value - Random.value) * maxNumConnections));
            while (world[i].getConnections().Count < maxNConnects)
            {
                int selectedNode;
                if (world[i].getConnections().Count == 0 && worldDisconnected.Count > 0)
                    selectedNode = (int)(Random.value * (world.Count - 1));//Random.Range(worldDisconnected[0].id, worldDisconnected[worldDisconnected.Count - 1].id);
                else
                    selectedNode = (int)(Random.value * (world.Count - 1));

                if (!world[i].getConnections().Contains(world[selectedNode]) && selectedNode != i)
                {
                    world[i].addConnection(world[selectedNode]);
                    world[selectedNode].addConnection(world[i]);

                    if (worldDisconnected.Contains(world[i]))
                        worldDisconnected.Remove(world[i]);
                    if (worldDisconnected.Contains(world[selectedNode]))
                        worldDisconnected.Remove(world[selectedNode]);
                }
            }
        }
        //TODO: check if all areas are reachable
    }

    public void generateNextArea(int idOfNextArea) {
        bool isCached = false;
        GameObject areaToSpawn;
        AreaGen areaGn = null;
        if (currentArea != null)
            areaGn = currentArea.GetComponent<AreaGen>();
        foreach (GameObject area in areaCache)
        {
            if (area.GetComponent<AreaGen>().node.id == idOfNextArea){
                isCached = true;
                currentArea.SetActive(false);
                previousArea = currentArea;
                currentArea = area;
                currentArea.SetActive(true);
                return;
            }
        }
        if (!isCached) {
            areaToSpawn = (GameObject)Instantiate(areaPrefab);
            AreaGen areaGen = areaToSpawn.GetComponent<AreaGen>();
            areaGen.world = this;
            areaGen.node = world[idOfNextArea];
            areaCache.Add(areaToSpawn);
            if (currentArea != null)
            {
                previousArea = currentArea;
                currentArea.SetActive(false);
            }
            currentArea = areaToSpawn;
        }
    }

    void findInslands() { 
    
    
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
