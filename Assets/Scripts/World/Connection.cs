using UnityEngine;
using System.Collections;

public class Connection {

    AreaNode arriveNode;
    Direction decidedDirection;

    public enum Direction { left, right, top, down};

    public Connection(AreaNode arrNode, Direction decDirection)
    {
        this.arriveNode = arrNode;
        this.decidedDirection = decDirection;
    }

    public AreaNode getNode() { return arriveNode; }
    public Direction getDecidedDirection() { return decidedDirection; }
}
