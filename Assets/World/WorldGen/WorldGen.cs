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
        generateNextArea(0);
        //printWorld();
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
        int maxNConnects = 1;
        List<AreaNode> mixedWorld = new List<AreaNode>(world);

        for (int k = 0; k < mixedWorld.Count; k++)
        {
            AreaNode temp = mixedWorld[k];
            int randomIndex = Random.Range(k, mixedWorld.Count);
            mixedWorld[k] = mixedWorld[randomIndex];
            mixedWorld[randomIndex] = temp;
        }

        for (int i = 0; i < mixedWorld.Count; i++)
        {
            if (i + 1 <= mixedWorld.Count - 1)
            {
                mixedWorld[i].addConnection(mixedWorld[i + 1]);
                mixedWorld[i + 1].addConnection(mixedWorld[i]);
            }
        }


        for (int i = 0; i < mixedWorld.Count; i++)
        {
            float u1 = Random.value;//these are uniform(0,1) random doubles
            float u2 = Random.value;
            float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                         Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
            float randNormal = 0 + 1 * randStdNormal;
            
            maxNConnects = Mathf.Max(1, Mathf.Abs((int)(randNormal * maxNumConnections)));
            maxNConnects = Mathf.Min(maxNConnects, maxNumConnections);
            while (mixedWorld[i].getConnections().Count < maxNConnects)
            {
                int selectedNode = (int)(Random.value * (mixedWorld.Count - 1));

                if (!mixedWorld[i].getConnections().Contains(mixedWorld[selectedNode]) && selectedNode != i && mixedWorld[selectedNode].getConnections().Count < maxNConnects)
                {
                    mixedWorld[i].addConnection(mixedWorld[selectedNode]);
                    mixedWorld[selectedNode].addConnection(mixedWorld[i]);
                }
                else
                    break;
            }
        }
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
