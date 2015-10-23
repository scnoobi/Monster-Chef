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

	void Start () {
        world = new List<AreaNode>();
        areaCache = new List<GameObject>();
        generateMap();
        generateConnections();
        generateNextArea(0);
        player = GameObject.FindGameObjectWithTag("Player");
	}

    //generated all the area nodes, these are an abstraction of each level section
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

    //receives a direction, returns the reverse direction
    //ex : left -> right, up -> down
    Connection.Direction reverseDirection(int direction) {
        int newDirection;
        if (direction % 2 == 0)
            newDirection = direction + 1;
        else
            newDirection = direction - 1;

        return (Connection.Direction)newDirection;
    }

    //from the previously generated area nodes, generate the connections between these randomly
    //to do this, first copy the list of AreaNodes, order it randomly and create connections between them
    // after this, go through this scrambled list and add new extra connections between the areas
    // for each connection from area1 to area2, it is also created a connection in reverse from area2 to area1 and these also have reverse directions
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
                int direction = (int)(Random.value * 4);
                mixedWorld[i].addConnection(new Connection(mixedWorld[i + 1],(Connection.Direction) direction));
                mixedWorld[i + 1].addConnection(new Connection(mixedWorld[i], reverseDirection(direction)));
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

                if (!mixedWorld[i].getConnections().Exists(x => x.getNode() == mixedWorld[selectedNode]) && selectedNode != i && mixedWorld[selectedNode].getConnections().Count < maxNConnects)
                {
                    int direction = (int)(Random.value * 4);
                    mixedWorld[i].addConnection(new Connection(mixedWorld[selectedNode], (Connection.Direction)direction));
                    mixedWorld[selectedNode].addConnection(new Connection(mixedWorld[i], reverseDirection(direction)));
                }
                else
                    break;
            }
        }
    }

    //check if the area to generate has already been generated and is in cache
    //if it is then enable it
    //if it isnt then instantiate a gameObject with AreaGen script, initialize its variables and save it in cache
    //also disable the previous area
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

    //print the area nodes and their connections
    void printWorld() {
        for (int i = 0; i < world.Count; i++)
        {
            for (int j = 0; j < world[i].getConnections().Count; j++)
            {
                Debug.Log(i + " ----> " + world[i].getConnections()[j].getNode().id);
            }
        }
    }

}
