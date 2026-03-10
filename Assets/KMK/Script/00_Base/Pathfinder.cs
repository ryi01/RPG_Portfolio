
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isNotWall;
    public Vector3 worldPos;
    public int gridX, gridY;

    public int gCost;
    public int hCost;
    public int FCost => gCost + hCost;

    public Node parent;

    public Node(bool _w, Vector3 _wp, int _gX, int _gY)
    {
        isNotWall = _w;
        worldPos = _wp;
        gridX = _gX;
        gridY = _gY;
    }
}
// F = H + G => 현재 = 미래 + 과거
public class Pathfinder : MonoBehaviour
{
    private Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // 시작점과 타겟 지점의 index를 구함
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closeSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                // 새로 들어온 노드가 현재 FCost보다 작거나
                // 새로 들어온 노드가 현재 FCost랑 같을 때, hCost가 작다면=> 앞으로 갈 거리가 작은 경우
                if (openSet[i].FCost < currentNode.FCost 
                    || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            // 타겟지점에 도달하면
            if (currentNode == targetNode) return RetracePath(startNode, targetNode);

            // 주변 이웃 노드 탐색 => 8방향
            foreach(Node neighbor in GetNeighbors(currentNode))
            {
                // 벽이거나 이미 탐색한 위치라면 패스
                if (!neighbor.isNotWall || closeSet.Contains(neighbor)) continue;
                // 총 비용 계산 : F = G + H
                // newMoveCostToNeighbor : 새로운 경로의 비용
                int newMoveCostToNeighbor = currentNode.gCost + GetDistnaceNode(currentNode, neighbor);
                // 더 짧은 경로가 있거나 처음 본 경우 
                // A B C  => 1번 : S -> C -> E 
                // D S E  => 2번 : S -> E
                // F G H  => 결론 : 이런 식으로 갈수도 있기에 gCost를 통해 거리를 확인함
                if(newMoveCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMoveCostToNeighbor;
                    neighbor.hCost = GetDistnaceNode(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    // 경로 역추적
    // 도착지 -> 시작점으로 되돌아감
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            path.Add(currentNode);
            // 내 상위 Node로 변경
            currentNode = currentNode.parent;
        }
        // 뒤에서부터 들어갔기에 역순으로 뒤집어줘야함 
        path.Reverse();
        return path;
    }

    // 두 노드 사이 거리 계산(8방향이동)
    // A = (2, 3) B = (7, 6) : disX, disY = 5, 3
    // 10과 14인 이유 : 직선 이동 = 10 대각선이동 = 14 
    // 대각선 거리 = 루트2 = 1.41414.... => 10 * 1.4 => 14
    // 직선은 소수점 자리를 피하기 위해 10으로 계산함 => 비율을 맞춤
    // disX와 disY중에 작은값을 대각선이동으로 취급
    // => x랑 y축으로 동시에 이동해야하기에 공통된 값만 사용
    // 대각선 이동을 하고 남은 값을 직선이동으로 변경
    int GetDistnaceNode(Node nodeA, Node nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        // 대각선 이동을 하고 남은 길이를 직선이동으로 취급
        return (disX > disY) ? 14 * disY + 10 * (disX - disY) : 14 * disX + 10 * (disY - disX);
    }

    // 주변 8개 노드 가져오기
    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if(checkX >= 0 && checkX < grid.GridSizeX && 
                    checkY >= 0 && checkY < grid.GridSizeY)
                {
                    Node neighbor = grid.GridNode[checkX, checkY];
                    if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                    {
                        if (!grid.GridNode[node.gridX + x, node.gridY].isNotWall ||
                            !grid.GridNode[node.gridX, node.gridY + y].isNotWall) continue;
                    }
                    if(neighbor.isNotWall)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }
        return neighbors;
    }
}
