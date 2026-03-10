using UnityEngine;
// A* 알고리즘
// Node <-> Node 거리를 계산해야함 => 중앙 좌표를 통해 구할 수 있음
public class Grid : MonoBehaviour
{
    // 장애물 레이어
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkRadius = 0.8f;
    // 맵 크기 전체
    private Vector2 gridWorldSize;
    // 노드의 반지름 => 실제 노드 크기 = 반지름 * 2
    private float nodeRadius;

    private Node[,] grid;
    public Node[,] GridNode => grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    public int GridSizeX => gridSizeX;
    public int GridSizeY => gridSizeY;

    public void Init(Vector2 mapSize, float nodeRadius)
    {
        gridWorldSize = mapSize;
        this.nodeRadius = nodeRadius;
        // 실제 노드 크기
        nodeDiameter = nodeRadius * 2;
        // 전체 맵 크기를 노드크기로 나눠 x,y축의 노드를 결정
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        grid = null;
        CreateGrid();
    }

    private void CreateGrid()
    {
        // 노드 배열 생성
        grid = new Node[gridSizeX, gridSizeY];

        // 현재 오브젝트 기준으로 왼쪽 아래 모서리 좌표
        // 현재오브젝트(정중앙) - 가로 절반 - 세로절반
        Vector3 worldBootmLeft = Vector3.zero;
        // 모든 좌표에 대해 노드 생성
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // 현재 노드의 월드 좌표 계산
                // 노드의 중심점을 구하기 위해 nodeRadius 더함
                // 작은 격자 안에 중심점을 찾아야함 => NodeRadius를 더함
                Vector3 worldPoint = worldBootmLeft + Vector3.right * (x * nodeDiameter)
                                + Vector3.forward * (y * nodeDiameter);
                // 장애물 확인
                // 반지름 범위내에 장애물 레이어가 있는가
                // 중앙 좌표를 쓰는 이유 : 노드 중앙에서 nodeRadius 범위 안에 있는가
                float radius = checkRadius * nodeRadius;
                bool isNoWall = !(Physics.CheckSphere(worldPoint, radius, obstacleLayer));
                // 노드 생성
                grid[x, y] = new Node(isNoWall, worldPoint, x, y);
            }
        }
        Debug.Log($"그리드 생성");
    }

    // 월드 좌표를 받아 해당하는 gridNode 반환
    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        // 월드 좌표를 0~1비율로 변환
        // 맵 안에 X위치가 몇 %인지 구함 
        // ex) -5~5 : -5 = 0%, 0 = 50%, 5 = 100%
        // + gridWorldSize.x / 2 : -5~5가 아닌 0~10으로 변경 
        float percentX = Mathf.Clamp01(worldPos.x / gridWorldSize.x);
        float percentY = Mathf.Clamp01(worldPos.z / gridWorldSize.y);
        // 비율을 이용해 실재 grid index 계산
        // percent를 통해 grid index를 구함
        // ex) gridSizeX = 10, percentX = 0.7 => (10 - 1) * 0.7 =6.3 = 6
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        // 해당 노드 반환 
        return grid[x, y];
    }

}
