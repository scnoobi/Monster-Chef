using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaNode  {

    List<AreaNode> connections;
    public int id;
    //biome biomeOfArea;

    public AreaNode() {
        connections = new List<AreaNode>();
    }

    public bool hasNode(int idToCheck)
    {
        bool hasNode = false;
        foreach (AreaNode n in connections) {
            if (n.id == idToCheck)
                hasNode = true;
        }
        return hasNode;
    }

    public void addConnection(AreaNode areaNode) { connections.Add(areaNode); }

    public List<AreaNode> getConnections() { return connections; }
}
