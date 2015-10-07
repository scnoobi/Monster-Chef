using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaNode  {

    List<Connection> connections;
    public int id;
    //biome biomeOfArea;

    public AreaNode() {
        connections = new List<Connection>();
    }

    public bool hasNode(int idToCheck)
    {
        bool hasNode = false;
        foreach (Connection n in connections)
        {
            if (n.getNode().id == idToCheck)
                hasNode = true;
        }
        return hasNode;
    }

    public void addConnection(Connection connect) { connections.Add(connect); }

    public List<Connection> getConnections() { return connections; }
}
