using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Vector2 gridWorldSize;
    [SerializeField] private float nodeRadius;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // 현재 오브젝트 기준으로 왼쪽 아래 모서리 좌표
        // 현재오브젝트(정중앙) - 가로 절반 - 세로절반
        Vector3 worldBootmLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBootmLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool isNoWall = !(Physics.CheckSphere(worldPoint, nodeRadius, obstacleLayer));
                grid[x, y] = new Node(isNoWall, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
}
