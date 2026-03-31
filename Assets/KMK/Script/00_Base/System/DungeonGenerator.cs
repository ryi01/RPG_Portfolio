using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
#region 필요한 데이터
public class Triangle
{
    public Vector2 a, b, c;
    public Vector2 circleCenter;
    public float circleRadius;

    public Triangle(Vector2 a, Vector2 b, Vector2 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        CaculateCircle();
    }

    // 수학로직 : 외접원 구하기
    private void CaculateCircle()
    {
        float x1 = a.x;
        float x2 = b.x;
        float x3 = c.x;
        float y1 = a.y;
        float y2 = b.y;
        float y3 = c.y;

        float d = 2 * (x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));

        if (Mathf.Abs(d) < 0.001f) return;

        // 각 점에서의 x^2 + y^2 구하기
        float aSq = a.sqrMagnitude;
        float bSq = b.sqrMagnitude;
        float cSq = c.sqrMagnitude;
        // 센터 구하기
        float centerX = (aSq * (y2 - y3) + bSq * (y3 - y1) + cSq * (y1 - y2)) / d;
        float centerY = (aSq * (x3 - x2) + bSq * (x1 - x3) + cSq * (x2 - x1)) / d;

        circleCenter = new Vector2(centerX, centerY);
        circleRadius = Vector2.Distance(a, circleCenter);
    }
    public bool IsPointInsideCircle(Vector2 point)
    {
        return Vector2.Distance(point, circleCenter) < circleRadius;
    }
}
public struct Edge
{
    public Vector2 u;
    public Vector2 v;

    public Edge(Vector2 u, Vector2 v)
    {
        this.u = u;
        this.v = v;
    }

    public bool Equal(Edge other)
    {
        return (u == other.u && v == other.v) || (u == other.v && v == other.u);
    }
}
#endregion

public class DungeonGenerator : MonoBehaviour
{
    #region 변수
    [Header("Dungeon Settings")]
    [SerializeField] private int pointCount = 10;
    [SerializeField] private float areaSize = 50;
    [SerializeField] private float minDis = 5f;
    [SerializeField] private int mapWidth = 60;
    [SerializeField] private int mapHeight = 60;
    [SerializeField] private float tileSize = 2.0f;

    [SerializeField] private List<Triangle> triangleList = new List<Triangle>();
    [SerializeField] private List<Edge> mstEdges = new List<Edge>();

    [Header("3D Prefabs")]
    public GameObject roomPrefab;
    public GameObject corridorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    [Header("Generation Data")]
    [SerializeField] private Transform dungeonParent;
    [SerializeField] private List<Vector2Int> points = new List<Vector2Int>();
    [SerializeField] private List<Edge> finalEdges = new List<Edge>();

    [Header("Decoration")]
    public GameObject[] obstaclePrefabs;
    public GameObject[] propPrefabs;
    public GameObject itemPrefabs;
    public GameObject dungeonDustPrefab;
    public GameObject triggerPrefab;
    public GameObject torchPrefab;
    public GameObject[] trapPrefab;
    [SerializeField][Range(0, 1)] private float decorationDensity = 0.1f; 
    [SerializeField][Range(0, 1)] private float itemBoxDensity = 0.3f; 
    [SerializeField][Range(0, 1)] private float trapDensity = 0.2f;
    [SerializeField][Range(0, 1)] private float torchChance = 0.4f;

    [Header("Enemy Spawn")]
    public GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] meleeEnemyPrefabs;
    [SerializeField] private GameObject[] rangedEnemyPrefabs;
    [SerializeField] private GameObject[] eliteEnemyPrefabs;

    [SerializeField] private int spawnPointsPerRoom = 6;
    [SerializeField] private int patrolPointsPerRoom = 4;
    [SerializeField] private float minPointDistance = 2;

    [SerializeField] private Vector2Int meleeSpawnRange = new Vector2Int(3, 5);
    [SerializeField] private Vector2Int rangedSpawnRange = new Vector2Int(2, 4);
    [SerializeField] private Vector2Int eliteSpawnRange = new Vector2Int(1, 2);
    #region 방 정보
    [System.Serializable]
    public class RoomSpawnData
    {
        public Vector2Int RoomKey;
        public List<Transform> RoomWayPoints;
        public List<Vector3> SpawnPoints;

        public EnumTypes.RoomSpawnType RoomType;
        public int SpawnCount;
        public GameObject[] EnemySet;
    }
    #endregion

    [Header("Navigation")]
    public NavMeshSurface navSurface;

    [Header("Grid")]
    [SerializeField] private GridAStar grid;

    private int[,] mapData;
    private int roomSize;
    
    public Vector2 StartPoint { get;private set; }
    public Vector2 EndPoint { get;private set; }

    public Vector3 WorldStartPoint => new Vector3(StartPoint.x * tileSize, 0, StartPoint.y * tileSize);
    public Vector3 WorldEndPoint => new Vector3(EndPoint.x * tileSize, 0, EndPoint.y * tileSize);
    public Transform DungeonParent => dungeonParent;

    private Dictionary<Vector2Int, List<Transform>> wayPoints = new Dictionary<Vector2Int, List<Transform>>();
    private Dictionary<Vector2Int, List<Vector3>> roomSpawnPoints = new Dictionary<Vector2Int, List<Vector3>>();

    public bool IsGenerationCompleted { get; private set; } = false;
    enum TileType
    {
        Empty = 0,
        Room = 1,
        Corridor = 2,
        Wall = 3,
        Prop = 4,
        Item = 5,
        Trap = 6,
        Door = 7,
        BossRoom = 8
    }
    #endregion
    private void OnEnable()
    {
        BossQuestComponent.OnBossDeath += SpawnPortal;
    }
    private void OnDisable()
    {
        BossQuestComponent.OnBossDeath -= SpawnPortal;
    }
    public void GenerateDungeon()
    {
        GeneratePoint();
        Delaunay();
        DoMST();
        CreateFinalPath();

        CalculateStartAndEnd();

        CreateMap();

        CreateWalls();
        // CreateDoors();
        CreateBossRoomTrigger();

        SpawnDungeonObjects();

        OnDungeonGenerationComplete();

        BuildNavMesh();

        SpawnEnemies();
        SpawnBossInRoom();

        SpawnAtmosphere();

        IsGenerationCompleted = true;
    }
    #region 맵 생성 공식
    private void GeneratePoint()
    {
        points.Clear();
        triangleList.Clear();

        int count = 0;
        while(points.Count < pointCount&&count < 1000)
        {
            count++;
            float rx = UnityEngine.Random.Range(5, areaSize - 5);
            float ry = UnityEngine.Random.Range(5, areaSize - 5);

            // 랜덤값 바탕으로 포인트 구하기
            Vector2Int newPoint = new Vector2Int(Mathf.RoundToInt(rx), Mathf.RoundToInt(ry));

            bool isTooClose = false;
            foreach(var p in points)
            {
                if(Vector2.Distance(p, newPoint) < minDis)
                {
                    isTooClose = true;
                    break;
                }
            }
            if (!isTooClose) points.Add(newPoint);
        }
    }
    private void SetupSuperTriangle()
    {
        triangleList.Clear();
        float margin = areaSize * 2;
        Vector2 v1 = new Vector2(areaSize / 2, areaSize * 2);
        Vector2 v2 = new Vector2(-margin, -margin);
        Vector2 v3 = new Vector2(margin + areaSize, -margin);

        triangleList.Add(new Triangle(v1, v2, v3));
    }

    private void Delaunay()
    {
        SetupSuperTriangle();
        
        foreach (var point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            foreach (var tri in triangleList)
            {
                // 외접원에 들어가 있는지 확인
                if (tri.IsPointInsideCircle(point))
                {
                    badTriangles.Add(tri);
                }
            }


            // 경계 추출
            List<Edge> polygon = new List<Edge>();
            foreach (var tri in badTriangles)
            {
                // 삼각형의 선 추출
                Edge[] edges = { new Edge(tri.a, tri.b), new Edge(tri.b, tri.c), new Edge(tri.c, tri.a) };
                foreach (var e in edges)
                {
                    bool isShared = false;
                    foreach (var otherTri in badTriangles)
                    {
                        if (otherTri == tri) continue;
                        if (IsEdgeInTriangle(otherTri, e))
                        {
                            isShared = true;
                            break;
                        }
                    }
                    if (!isShared) polygon.Add(e);
                }

            }
            // 불량 삼각형은 지움
            foreach (var tri in badTriangles)
            {
                triangleList.Remove(tri);
            }
            // 재구성
            foreach (var e in polygon)
            {
                triangleList.Add(new Triangle(e.u, e.v, point));
            }
        }

        for(int i = triangleList.Count -1; i >= 0; i--)
        {
            Triangle tri = triangleList[i];
            if (IsSuperTriangle(tri.a) || IsSuperTriangle(tri.b) || IsSuperTriangle(tri.c))
                triangleList.Remove(tri);
        }
    }
    private bool IsSuperTriangle(Vector2 v)
    {
        float margin = areaSize * 0.5f;
        return v.x < -margin || v.x > areaSize + margin || v.y < -margin || v.y > areaSize + margin;
    }   
    private bool IsEdgeInTriangle(Triangle tri, Edge edge)
    {
        Edge[] triEdges = { new Edge(tri.a, tri.b), new Edge(tri.b, tri.c), new Edge(tri.c, tri.a) };
        foreach(var e in triEdges)
        {
            if(e.Equal(edge))
            {
                return true;
            }
        }
        return false;
    }

    private void DoMST()
    {
        mstEdges.Clear();
        if (points.Count == 0) return;
        List<Edge> allEdges = CollectUniqueEdge(triangleList);
        List<Vector2Int> reachedPonints = new List<Vector2Int>();
        List<Vector2Int> unreachedPoints = new List<Vector2Int>(points);
        reachedPonints.Add(unreachedPoints[0]);
        unreachedPoints.RemoveAt(0);
        while(unreachedPoints.Count > 0)
        {
            float minDis = float.MaxValue;
            Edge bestEdge = new Edge();
            int bestPointIndex = -1;

            foreach(var pReached in reachedPonints)
            {
                for(int i = 0; i < unreachedPoints.Count;i++)
                {
                    Vector2Int pUnreached = unreachedPoints[i];
                    foreach(var edge in allEdges)
                    {
                        if((edge.u == pReached) && (edge.v == pUnreached) ||
                            (edge.u == pUnreached && edge.v == pReached))
                        {
                            float dist = Vector2.Distance(pReached, pUnreached);

                            if(dist < minDis)
                            {
                                minDis = dist;
                                bestEdge = edge;
                                bestPointIndex = i;
                            }
                        }
                    }
                }
            }

            if (bestPointIndex != -1)
            {
                mstEdges.Add(bestEdge);
                reachedPonints.Add(unreachedPoints[bestPointIndex]);
                unreachedPoints.RemoveAt(bestPointIndex);
            }
            else break;
        }
    }

    private void CreateFinalPath()
    {
        finalEdges.Clear();
        finalEdges.AddRange(mstEdges);
        List<Edge> allDelaunaryEdges = CollectUniqueEdge(triangleList);

        List<Edge> remainEdges = new List<Edge>();
        foreach(var e in allDelaunaryEdges)
        {
            bool isMST = false;
            foreach(var mstE in mstEdges)
            {
                if(mstE.Equal(e))
                {
                    isMST = true;
                    break;
                }
            }
            if (!isMST) remainEdges.Add(e);
        }

        foreach(var edge in remainEdges)
        {
            if(UnityEngine.Random.value < 0.15f)
            {
                finalEdges.Add(edge);
            }
        }
    }

    private void CreateMap()
    {
        if (dungeonParent != null) DestroyImmediate(dungeonParent.gameObject);
        dungeonParent = new GameObject("DungeonParent").transform;
        mapData = new int[mapWidth, mapHeight];
        roomSize = UnityEngine.Random.Range(4, 6);
      
        // 바닥 생성
        foreach (var p in points)
        {
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);
            int currentRoomSize = GetRoomSize(p);
            TileType type = (p == EndPoint) ? TileType.BossRoom : TileType.Room;
            for (int x = cx - currentRoomSize; x <= cx + currentRoomSize; x++)
            {
                for(int y = cy - currentRoomSize; y <= cy+ currentRoomSize; y++)
                {
                    if (IsInMap(x, y)) mapData[x, y] = (int)type;
                }
            }
        }

        foreach(var edge in finalEdges)
        {
            MarkCorridorInData(edge);
        }

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                if (mapData[x, y] == (int)TileType.Room || mapData[x, y] == (int)TileType.BossRoom)
                {
                    Instantiate(roomPrefab, pos, Quaternion.identity, dungeonParent);
                }
                else if (mapData[x, y] == (int)TileType.Corridor)
                {
                    Instantiate(corridorPrefab, pos, Quaternion.identity, dungeonParent);
                }
            }
        }
    }

    private List<Edge> CollectUniqueEdge(List<Triangle> triangles)
    {
        List<Edge> result = new List<Edge>();
        foreach (var tri in triangleList)
        {
            Edge[] triEdges = { new Edge(tri.a, tri.b), new Edge(tri.b, tri.c), new Edge(tri.c, tri.a) };
            foreach (var e in triEdges)
            {
                bool exits = false;
                foreach (var ae in result)
                {
                    if (ae.Equal(e))
                    {
                        exits = true;
                        break;
                    }
                }
                if (!exits) result.Add(e);
            }
        }

        return result;
    }

    #endregion
    #region 벽, 바닥 생성
    private void MarkCorridorInData(Edge edge)
    {
        int x1 = Mathf.RoundToInt(edge.u.x);
        int y1 = Mathf.RoundToInt(edge.u.y);
        int x2 = Mathf.RoundToInt(edge.v.x);
        int y2 = Mathf.RoundToInt(edge.v.y);

        int corridorWidth = 4;
        int halfWidth = corridorWidth / 2;

        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
        {
            for (int offset = -halfWidth; offset <= corridorWidth - halfWidth; offset++)
            {
                int ty = y1 + offset;
                if (IsInMap(x, ty) && mapData[x, ty] == 0) mapData[x, ty] = (int)TileType.Corridor;
            }
        }
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            // 복도 선뿐만 아니라 좌우 1칸씩 더 여유를 줍니다
            for (int offset = -halfWidth; offset <= corridorWidth - halfWidth; offset++)
            {
                int tx = x2 + offset;
                if (IsInMap(tx, y) && mapData[tx, y] == 0) mapData[tx, y] = (int)TileType.Corridor;
            }
        }
    }
    private bool IsInMap(int x, int y) => x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;

    public void CreateWalls()
    {
        if (mapData == null) return;
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                if (mapData[x, y] == 0)
                {
                    if (IsNextToType(x, y, TileType.Room) || IsNextToType(x, y, TileType.Corridor) || IsNextToType(x, y, TileType.BossRoom))
                    {
                        // 벽 높이가 2이므로 Y좌표는 1.0f (바닥 0 위에 안착)
                        Vector3 wallPos = new Vector3(x * tileSize, 0f, y * tileSize);
                        GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, dungeonParent);
                        wall.name = $"Wall_{x}_{y}";
                        wall.transform.localScale = new Vector3(0.5f, 0.8f, 1.4f);
                        if (UnityEngine.Random.value < torchChance)
                        {
                            TryAttachWallDecoration(wall, x, y);
                        }
                        // 생성된 벽 자리 마킹 (중복 생성 방지)
                        mapData[x, y] = (int)TileType.Wall;
                    }
                }
            }
        }
    }
    private bool IsNextToType(int x, int y, TileType targetType)
    {
        for(int ox = -1; ox <= 1; ox++)
        {
            for (int oy = -1; oy <= 1; oy++)
            {
                if (ox == 0 && oy == 0) continue; // 자기 자신 제외

                int nx = x + ox;
                int ny = y + oy;

                // 범위 확인 : 맵 바깥을 참조해 에러가 나지 않게 방
                if (IsInMap(nx, ny))
                {
                    // [수정 핵심] mapData가 1(방)이거나 2(복도)이면 true
                    if (mapData[nx, ny] == (int)targetType)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion
    #region 보스방 설정
    // BFS 사용
    // 어떤 점에서 가장 먼 곳을 찾고 그 점에서 다시 가장 먼 곳을 찾음
    // => 던전의 시작과 끝 찾기
    // 1. 모든 방을 Node, 복도를 Edge로 하는 그래프 만들기
    // 2. 0번 방에서 출발해 가장 멀리 떨어진 방 A 찾기
    // 3. 방 A에서 출발해서 가장 멀리 떨어진 B찾기
    private void CalculateStartAndEnd()
    {
        if (points == null || points.Count < 2) return;
        // MST 정보를 인접 리스트로 변환(길의 연결 상태를 추적하기 위함)
        Dictionary<Vector2, List<Vector2>> graph = new Dictionary<Vector2, List<Vector2>>();
        foreach(var edge in mstEdges)
        {
            if(!graph.ContainsKey(edge.u)) graph[edge.u] = new List<Vector2>();
            if(!graph.ContainsKey(edge.v)) graph[edge.v] = new List<Vector2>();
            graph[edge.u].Add(edge.v);
            graph[edge.v].Add(edge.u);
        }

        // 특정 지점에서 그래프를 따라 가장 먼 방을 찾는 BFS 함수 정의
        Vector2 FindFarthest(Vector2 startNode)
        {
            Queue<Vector2> queue = new Queue<Vector2>();
            Dictionary<Vector2, int> distances = new Dictionary<Vector2, int>();
            queue.Enqueue(startNode);
            distances[startNode] = 0;

            Vector2 farthestNode = startNode;
            int maxDist = 0;

            while (queue.Count > 0)
            {
                Vector2 current = queue.Dequeue();
                // 현재 노드가 지금까지 찾은 노드 보다 더 멀으면 갱신
                if (distances[current] > maxDist)
                {
                    maxDist = distances[current];
                    farthestNode = current;
                }
                // 연결된 이웃 방들을 탐색
                if (graph.ContainsKey(current))
                {
                    foreach (var neighbor in graph[current])
                    {
                        if (!distances.ContainsKey(neighbor))
                        {
                            distances[neighbor] = distances[current] + 1;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return farthestNode;
        }
        // 첫번째 BFS : 임의의 방에서 가장 먼 끝점 A를 찾음
        Vector2 startCandiate = FindFarthest(points[0]);
        // 두번째 BFS : A에서 가장 먼 끝점 B를 찾음 
        Vector2 endCandiate = FindFarthest(startCandiate);

        StartPoint = startCandiate;
        EndPoint = endCandiate;
    }

    #endregion
    #region 오브젝트 배치
    private void SpawnDungeonObjects()
    {
        if (mapData == null) return;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (mapData[x, y] > (int)TileType.Wall) continue;

                Vector2 currentPos = new Vector2(x, y);
                if (Vector2.Distance(currentPos, StartPoint) <= 5f || currentPos == EndPoint) continue;

                int wallCount = CountWall(x, y);
                // 아이템 생성
                if (mapData[x, y] == (int)TileType.Room && wallCount >= 2)
                {
                    if (UnityEngine.Random.value < itemBoxDensity)
                    {
                        PlaceObject(itemPrefabs, x, y, TileType.Item);
                        continue;
                    }
                }
                // 장식
                if (wallCount > 0 && (mapData[x, y] == (int)TileType.Room || mapData[x, y] == (int)TileType.Corridor))
                {
                    if (UnityEngine.Random.value < decorationDensity)
                    {
                        GameObject randomProp = propPrefabs[UnityEngine.Random.Range(0, propPrefabs.Length)];
                        PlaceObject(randomProp, x, y, TileType.Prop);
                        continue;
                    }
                }

                // 트랩
                if ((mapData[x, y] == (int)TileType.Room || mapData[x, y] == (int)TileType.Corridor) && mapData[x, y] < (int)TileType.Prop)
                {

                    if (UnityEngine.Random.value < trapDensity)
                    {
                        if (GetDirectionByType(x, y, TileType.Wall) != Vector2Int.zero)
                        {
                            PlaceObject(trapPrefab[0], x, y, TileType.Trap);
                        }
                        else
                        {
                            PlaceObject(trapPrefab[1], x, y, TileType.Trap, 0.3f);
                        }
                    }
                }
            }
        }
    }

    private void PlaceObject(GameObject prefab, int x, int y, TileType typeMask, float posY = 0.1f)
    {
        Vector3 pos = new Vector3(x * tileSize, posY, y * tileSize);
        Quaternion rot = Quaternion.identity;

        if(typeMask == TileType.Trap)
        {
            Vector3 offset = Vector3.zero;
            rot = GetRotationByType(x, y, TileType.Wall);
        }
        else
        {
            rot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        }
        GameObject obj = Instantiate(prefab, pos, rot, dungeonParent);
        obj.name = $"{prefab.name}_{x}_{y}";
        mapData[x, y] = (int)typeMask;

        if(typeMask == TileType.Item && grid != null)
        {
            grid.RegisterObjectNode(pos, true);
        }
    }

    private void TryAttachWallDecoration(GameObject wall, int x, int y)
    {
        Vector2Int dir = GetInsideDirection(x, y);
        if (dir == Vector2Int.zero) return;
        Renderer renderer = wall.GetComponentInChildren<Renderer>();
        if (renderer == null) return;
        Vector3 dir3 = new Vector3(dir.x, 0, dir.y);
        Bounds bounds = renderer.bounds;

        float halfDepth = Mathf.Abs(dir.x) > 0 ? bounds.extents.x : bounds.extents.z;
        float surfaceOffset = 0.05f;
        float height = bounds.extents.y + 0.2f;

        Vector3 pos = wall.transform.position + dir3 * (halfDepth + surfaceOffset) + Vector3.up * height;

        Quaternion rotation = DirectionToRotation(-dir);

        Instantiate(torchPrefab, pos, rotation, dungeonParent);
    }

    #endregion
    #region 벽 위치 및 방향 확인
    private int CountWall(int x, int y)
    {
        int count = 0;
        for (int ox = -1; ox <= 1; ox++)
        {
            for (int oy = -1; oy <= 1; oy++)
            {
                if (ox == 0 && oy == 0) continue;
                int nx = x + ox;
                int ny = y + oy;
                if (IsInMap(nx, ny) && mapData[nx, ny] == (int)TileType.Wall) count++;
            }
        }
        return count;
    }
    private Vector2Int GetDirectionByType(int x, int y, TileType type)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach(var dir in dirs)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if(IsInMap(nx, ny) && mapData[nx, ny] == (int)type)
            {
                return dir;
            }
        }

        return Vector2Int.zero;
    }

    private Vector2Int GetInsideDirection(int x, int y)
    {
        Vector2Int dir = GetDirectionByType(x, y, TileType.BossRoom);
        if (dir != Vector2Int.zero) return dir;

        dir = GetDirectionByType(x, y, TileType.Room);
        if (dir != Vector2Int.zero) return dir;

        dir = GetDirectionByType(x, y, TileType.Corridor);
        if (dir != Vector2Int.zero) return dir;

        return Vector2Int.zero;
    }
    private Quaternion GetRotationByType(int x, int y, TileType type)
    {
        Vector2Int dir = GetDirectionByType(x, y, type);
        return DirectionToRotation(dir);
    }

    private Quaternion DirectionToRotation(Vector2Int dir)
    {
        if (dir == Vector2Int.left) return Quaternion.Euler(0, 90, 0);
        if (dir == Vector2Int.right) return Quaternion.Euler(0, -90, 0);
        if (dir == Vector2Int.down) return Quaternion.Euler(0, 0, 0);
        if (dir == Vector2Int.up) return Quaternion.Euler(0, 180, 0);

        return Quaternion.identity;
    }

    #endregion
    #region 적 생성
    public void SpawnEnemies()
    {
        if (mapData == null || points == null) return;
        int roomOrder = 0;
        foreach(var p in points)
        {
            if (p == StartPoint) continue;
            if (p == EndPoint) continue;
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);

            RoomSpawnData roomData = CreateRoomSpawnData(cx, cy, roomOrder);
            SpawnRoomEnemies(roomData);

            roomOrder++;
        }
    }
    private RoomSpawnData CreateRoomSpawnData(int cx, int cy, int roomOrder)
    {
        Vector2Int roomKey = new Vector2Int(cx, cy);

        if(!roomSpawnPoints.TryGetValue(roomKey, out List<Vector3> spawnPoints))
        {
            spawnPoints = CreateSpawnPointForRoom(cx, cy, roomSize);
            roomSpawnPoints.Add(roomKey, spawnPoints);
        }

        if (!wayPoints.TryGetValue(roomKey, out List<Transform> wayPoint))
        {
            wayPoint = CreateWayPointsForPoint(cx, cy, roomSize);
            wayPoints.Add(roomKey, wayPoint);
        }

        RoomSpawnData data = new RoomSpawnData
        {
            RoomKey = roomKey,
            SpawnPoints = spawnPoints,
            RoomWayPoints = wayPoint
        };

        DecideRoomSpawnRule(data, roomOrder);

        return data;
    }
    private void DecideRoomSpawnRule(RoomSpawnData data, int roomOrder)
    {
        int typeIndex = roomOrder % 3;
        switch(typeIndex)
        {
            case 0:
                data.RoomType = EnumTypes.RoomSpawnType.Melee;
                data.SpawnCount = UnityEngine.Random.Range(meleeSpawnRange.x, meleeSpawnRange.y + 1);
                data.EnemySet = meleeEnemyPrefabs;
                break;
            case 1:
                data.RoomType = EnumTypes.RoomSpawnType.Ranged;
                data.SpawnCount = UnityEngine.Random.Range(rangedSpawnRange.x, rangedSpawnRange.y + 1);
                data.EnemySet = rangedEnemyPrefabs;
                break;
            case 2:
                data.RoomType = EnumTypes.RoomSpawnType.Elite;
                data.SpawnCount = UnityEngine.Random.Range(eliteSpawnRange.x, eliteSpawnRange.y + 1);
                data.EnemySet = eliteEnemyPrefabs;
                break;
        }
    }
    // 적 생성 위치를 만드는 함수
    private List<Vector3> CreateSpawnPointForRoom(int cx, int cy, int size)
    {
        return CreateRandomRoomPoints(cx, cy, size, spawnPointsPerRoom, 2);
    }
    private List<Transform> CreateWayPointsForPoint(int cx, int cy, int size)
    {
        List<Transform> result = new List<Transform>();
        int targetCount = UnityEngine.Random.Range(3, patrolPointsPerRoom + 1);
        List<Vector3> positions = CreateRandomRoomPoints(cx, cy, size, targetCount, minPointDistance);
        for(int i = 0; i < positions.Count; i++)
        {
            GameObject wp = new GameObject($"WayPoint_{cx}_{cy}_{i}");
            wp.transform.position = positions[i];
            wp.transform.SetParent(dungeonParent);

            result.Add(wp.transform);
        }
        return result;
    }

    private void SpawnRoomEnemies(RoomSpawnData data)
    {
        if (data.EnemySet == null || data.EnemySet.Length == 0) return;
        if (data.SpawnPoints == null || data.SpawnPoints.Count == 0) return;
        List<Vector3> avaliableSpawnPoints = new List<Vector3>(data.SpawnPoints);
        int spawnCount = Mathf.Min(data.SpawnCount, avaliableSpawnPoints.Count);

        for(int i = 0; i < spawnCount; i++)
        {
            int pointIndex = UnityEngine.Random.Range(0, avaliableSpawnPoints.Count);
            Vector3 spawnPos = avaliableSpawnPoints[pointIndex];
            avaliableSpawnPoints.RemoveAt(pointIndex);

            GameObject enemyPrefab = data.EnemySet[i % data.EnemySet.Length];

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, dungeonParent);

            EnemyStatComponent stat = enemy.GetComponent<EnemyStatComponent>();
            if (stat != null && data.RoomWayPoints != null && data.RoomWayPoints.Count > 0)
            {
                Transform patrolPoint = data.RoomWayPoints[UnityEngine.Random.Range(0, data.RoomWayPoints.Count)];
                stat.RoamCenter = patrolPoint.position;
            }
        }
    }
    // 랜덤 포인트 생성 공통함수
    private List<Vector3> CreateRandomRoomPoints(int cx, int cy, int size, int targetCount, float minDistance)
    {
        List<Vector3> result = new List<Vector3>();
        int attempts = 0;
        while (result.Count < targetCount && attempts < targetCount * 10)
        {
            attempts++;
            int rx = UnityEngine.Random.Range(cx - size, cx + size);
            int ry = UnityEngine.Random.Range(cy - size, cy + size);

            if (!IsInMap(rx, ry)) continue;
            if (mapData[rx, ry] != (int)TileType.Room && mapData[rx, ry] != (int)TileType.BossRoom) continue;

            Vector3 pos = new Vector3(rx * tileSize, 0.5f, ry * tileSize);

            bool isClose = false;

            foreach (var p in result)
            {
                if (Vector3.Distance(p, pos) < minDistance)
                {
                    isClose = true;
                    break;
                }
            }

            if (isClose) continue;
            result.Add(pos);
        }
        return result;
    }
    #endregion
    public void BuildNavMesh()
    {
        if(navSurface == null)
        {
            navSurface = dungeonParent.gameObject.AddComponent<NavMeshSurface>();

            navSurface.collectObjects = CollectObjects.Children;

            navSurface.layerMask = LayerMask.GetMask("Environment");
        }

        navSurface.BuildNavMesh();
    }

    #region 보스 방 생성

    public void SpawnBossInRoom()
    {
        Vector3 spawnPos = new Vector3(EndPoint.x * tileSize, 0.5f, EndPoint.y * tileSize);
        var questData = GameManager.Instance.QuestManager.GetCurrentQuestData();
        GameObject bossObj = Instantiate(questData.BossPrefab, spawnPos, Quaternion.identity, dungeonParent);

        BossQuestComponent questComp = bossObj.GetComponent<BossQuestComponent>();
        if(questComp != null)
        {
            questComp.SetQuestData(questData);
        }
    }
    private void CreateBossRoomTrigger()
    {
        Vector3 triggerPos = new Vector3(EndPoint.x * tileSize, 1f, EndPoint.y * tileSize);

        GameObject triggerObj = Instantiate(triggerPrefab, triggerPos, Quaternion.identity, dungeonParent);

        int size = GetRoomSize(EndPoint);
        triggerObj.transform.localScale = new Vector3(size * tileSize * 2, 2f, size * tileSize * 2);
        
    }
    private int GetRoomSize(Vector2 roomPos)
    {
        return (roomPos == EndPoint) ? roomSize + 2 : roomSize;
    }
    private void OnDungeonGenerationComplete()
    {
        Vector2 dungeonSize = new Vector2(mapWidth * tileSize, mapHeight * tileSize);
        grid.transform.position = new Vector3(dungeonSize.x / 2f, 0, dungeonSize.y / 2f);
        grid.Init(dungeonSize, tileSize / 2, dungeonParent.position);
    }
    #endregion
    #region 먼지 생성
    private void SpawnAtmosphere()
    {
        SpawnRoomDust();
        SpawnCorridorDusts();
    }

    private void SpawnRoomDust()
    {
        if (points == null || dungeonDustPrefab == null) return;

        foreach(var p in points)
        {
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);

            int currentRoomSize = GetRoomSize(p);

            Vector3 roomCenter = new Vector3(cx * tileSize, 0f, cy * tileSize);

            SpawnRoomDust(roomCenter, currentRoomSize);
        }    
    }

    private void SpawnRoomDust(Vector3 roomCenter, int roomRadius)
    {
        float worldRoomWidth = (roomRadius * 2 + 1) * tileSize;
        SpawnDustVolume(roomCenter, new Vector3(worldRoomWidth * 0.8f, 2, worldRoomWidth * 0.8f), 2, 0.16f, $"RoomDust_{roomCenter.x}_{roomCenter.z}");
    }

    private void SpawnCorridorDusts()
    {
        if (mapData == null || dungeonDustPrefab == null) return;

        int space = 6;

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                if (mapData[x, y] != (int)TileType.Corridor) continue;
                if ((x + y) % space != 0) continue;
                bool left = IsInMap(x - 1, y) && mapData[x - 1, y] == (int)TileType.Corridor;
                bool right = IsInMap(x + 1, y) && mapData[x + 1, y] == (int)TileType.Corridor;
                bool up = IsInMap(x, y + 1) && mapData[x, y + 1] == (int)TileType.Corridor;
                bool down = IsInMap(x, y - 1) && mapData[x, y - 1] == (int)TileType.Corridor;

                bool horizontal = left || right;
                bool vertical = up || down;

                Vector3 corridorCenter = new Vector3(x * tileSize, 0, y * tileSize);
                SpawnCorridorDust(corridorCenter, horizontal, vertical);
            }
        }
    }

    private void SpawnCorridorDust(Vector3 corridorCenter, bool horizontal, bool vertical)
    {
        Vector3 scale = new Vector3(tileSize * 3f, 1.5f, tileSize * 3f);
        if(horizontal && !vertical)
        {
            scale = new Vector3(tileSize * 4f, 1.2f, tileSize * 2f);
        }
        else if(!horizontal && vertical)
        {
            scale = new Vector3(tileSize * 2f, 1.2f, tileSize * 4f);
        }
        else if(horizontal && vertical)
        {
            scale = new Vector3(tileSize * 3f, 1.2f, tileSize * 3f);
        }

        SpawnDustVolume(corridorCenter, scale, 1.5f, 0.14f, $"CorridorDust_{corridorCenter.x}_{corridorCenter.z}");
    }
    private void SpawnDustVolume(Vector3 center, Vector3 scale, float height, float startSize, string dustName)
    {
        if (dungeonDustPrefab == null) return;
        Vector3 spawnPos = center + Vector3.up * height;
        GameObject dust = Instantiate(dungeonDustPrefab, spawnPos, Quaternion.identity, dungeonParent);
        dust.name = dustName;

        ParticleSystem ps = dust.GetComponent<ParticleSystem>();
        if (ps == null) return;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = scale;

        var main = ps.main;
        main.startSize = startSize;
    }
    #endregion

    private void SpawnPortal()
    {
        Vector3 spawnPos = new Vector3(EndPoint.x * tileSize, 0.5f, EndPoint.y * tileSize);
        GameManager.Instance.SpawnPortal("VillageScene", spawnPos);
    }

    public void ClearDungeon()
    {
        IsGenerationCompleted = false;

        roomSpawnPoints.Clear();
        wayPoints.Clear();

        if (dungeonParent != null)
        {
            grid.ClearGrid();
            // 1. 하위 오브젝트들까지 확실히 정리하기 위해 부모를 즉각 파괴하거나 정리
            Destroy(dungeonParent.gameObject);

            // 2. [중요] 변수를 반드시 null로 초기화하여 중복 파괴 오류 방지
            dungeonParent = null;
        }
    }
}
