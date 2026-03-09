
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isWall;
    public Vector3 worldPos;
    public int gridX, gridY;

    public int gCost;
    public int hCost;
    public int FCost => gCost + hCost;

    public Node parent;

    public Node(bool _w, Vector3 _wp, int _gX, int _gY)
    {
        isWall = _w;
        worldPos = _wp;
        gridX = _gX;
        gridY = _gY;
    }
}

public class Pathfinder : MonoBehaviour
{
    private Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

/*    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        List<Node> closeSet = new List<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost) )
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                
            }

            
        }
    }*/
}
