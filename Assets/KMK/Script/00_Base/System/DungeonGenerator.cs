using System;
using System.Collections.Generic;
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

    public Triangle(Vector2 a, Vector2 b, Vector3 c)
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
        circleRadius = Vector3.Distance(a, circleCenter);
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
    [SerializeField] private List<Vector2> points = new List<Vector2>();
    [SerializeField] private List<Edge> finalEdges = new List<Edge>();

    [Header("Decoration")]
    public GameObject[] obstaclePrefabs;
    public GameObject[] propPrefabs;
    public GameObject itemPrefabs;
    public GameObject[] trapPrefab;
    [SerializeField][Range(0, 1)] private float decorationDensity = 0.1f; 
    [SerializeField][Range(0, 1)] private float itemBoxDensity = 0.3f; 
    [SerializeField][Range(0, 1)] private float trapDensity = 0.2f;

    [Header("Enemy Spawn")]
    public GameObject[] enemyPrefabs;
    [SerializeField] private int maxEnemiesPerRoom = 5;

    [Header("Navigation")]
    public NavMeshSurface navSurface;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private QuestData dungeonQuestData;

    [Header("Grid")]
    [SerializeField] private Grid grid;

    private int[,] mapData;
    private int roomSize;
    
    public Vector2 StartPoint { get;private set; }
    public Vector2 EndPoint { get;private set; }

    public Vector3 WorldStartPoint => new Vector3(StartPoint.x, 0, StartPoint.y) * tileSize + Vector3.up * 1.5f;
    public Vector3 WorldEndPoint => new Vector3(EndPoint.x, 0, EndPoint.y) * tileSize + Vector3.up * 1.5f;

    private void OnEnable()
    {
        BossController.OnBossDeath += SpawnPortal;
    }
    private void OnDisable()
    {
        BossController.OnBossDeath -= SpawnPortal;
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
        CreateDoors();
        SpawnDungeonObjects();

        BuildNavMesh();

        SpawnEnemies();
        SpawnBossInRoom();

        OnDungeonGenerationComplete();
    }
    
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
            Vector2 newPoint = new Vector2(Mathf.RoundToInt(rx), Mathf.RoundToInt(ry));

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
        List<Edge> allEdges = new List<Edge>();
        foreach(var tri in triangleList)
        {
            Edge[] triEdges = { new Edge(tri.a, tri.b), new Edge(tri.b, tri.c), new Edge(tri.c, tri.a) };
            foreach (var e in triEdges)
            {
                bool exits = false;
                foreach(var ae in allEdges)
                {
                    if(ae.Equal(e))
                    {
                        exits = true;
                        break;
                    }
                }
                if (!exits) allEdges.Add(e);
            }
        }

        List<Vector2> reachedPonints = new List<Vector2>();
        List<Vector2> unreachedPoints = new List<Vector2>(points);
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
                    Vector2 pUnreached = unreachedPoints[i];
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
        List<Edge> allDelaunaryEdges = new List<Edge>();
        foreach(var tri in triangleList)
        {
            Edge[] triEdges = { new Edge(tri.a, tri.b), new Edge(tri.b, tri.c), new Edge(tri.c, tri.a) };
            foreach(var e in triEdges)
            {
                bool exits = false;
                foreach(var ae in allDelaunaryEdges)
                {
                    if(ae.Equal(e))
                    {
                        exits = true;
                        break;
                    }
                }
                if (!exits) allDelaunaryEdges.Add(e);
            }
        }

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
        dungeonParent = new GameObject("DungenParent").transform;
        mapData = new int[mapWidth, mapHeight];
        roomSize = UnityEngine.Random.Range(4, 6);
      
        // 바닥 생성
        foreach (var p in points)
        {
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);
            int currentRoomSize = GetRoomSize(p);
            int markValue = (p == EndPoint) ? 9 : 1;
            for (int x = cx - currentRoomSize; x <= cx + currentRoomSize; x++)
            {
                for(int y = cy - currentRoomSize; y <= cy+ currentRoomSize; y++)
                {
                    if (IsInMap(x, y)) mapData[x, y] = markValue;
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
                if (mapData[x, y] == 1 || mapData[x, y] == 9)
                {
                    Instantiate(roomPrefab, pos, Quaternion.identity, dungeonParent);
                }
                else if (mapData[x, y] == 2)
                {
                    Instantiate(corridorPrefab, pos, Quaternion.identity, dungeonParent);
                }
            }
        }
    }
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
                if (IsInMap(x, ty) && mapData[x, ty] == 0) mapData[x, ty] = 2;
            }
        }
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            // 복도 선뿐만 아니라 좌우 1칸씩 더 여유를 줍니다
            for (int offset = -halfWidth; offset <= corridorWidth - halfWidth; offset++)
            {
                int tx = x2 + offset;
                if (IsInMap(tx, y) && mapData[tx, y] == 0) mapData[tx, y] = 2;
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
                    if (IsNextToType(x, y, 1) || IsNextToType(x, y, 2) || IsNextToType(x, y, 9))
                    {
                        // 벽 높이가 2이므로 Y좌표는 1.0f (바닥 0 위에 안착)
                        Vector3 wallPos = new Vector3(x * tileSize, 0f, y * tileSize);
                        GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, dungeonParent);
                        wall.name = $"Wall_{x}_{y}";
                        wall.transform.localScale = new Vector3(0.5f, 0.8f, 1.4f);

                        // 생성된 벽 자리 마킹 (중복 생성 방지)
                        mapData[x, y] = 3;
                    }
                }
            }
        }
    }
    private void CreateDoors()
    {
        int bx = Mathf.RoundToInt(EndPoint.x);
        int by = Mathf.RoundToInt(EndPoint.y);
        int currentSize = GetRoomSize(EndPoint);
        // 1. 반복문 범위를 배열의 최대 크기 내로 안전하게 고정합니다.
        int minX = Mathf.Max(0, bx - currentSize);
        int maxX = Mathf.Min(mapData.GetLength(0) - 1, bx + currentSize);

        int minY = Mathf.Max(0, by - currentSize);
        int maxY = Mathf.Min(mapData.GetLength(1) - 1, by + currentSize);
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (mapData[x, y] == 9)
                {
                    if(IsNextToType(x, y, 2))
                    {
                        Vector3 doorPos = new Vector3(x * tileSize, 0.5f, y * tileSize);
                        Instantiate(doorPrefab, doorPos, Quaternion.identity, dungeonParent);

                        mapData[x, y] = 8;
                        return;
                    }
                }
            }
        }
    }

    private bool IsNextToType(int x, int y, int targetType)
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
                    if (mapData[nx, ny] == targetType)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
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


    private void SpawnDungeonObjects()
    {
        if (mapData == null) return;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (mapData[x, y] > 3) continue;
                Vector2 currentPos = new Vector2(x, y);
                if (Vector2.Distance(currentPos, StartPoint) <= 5f || currentPos == EndPoint) continue;
                int wallCount = CountWall(x, y);
                if (mapData[x, y]  == 1 && wallCount >=2)
                {
                    if(UnityEngine.Random.value < itemBoxDensity)
                    {
                        PlaceObject(itemPrefabs, x, y, 5);
                        continue;
                    }
                }
                if (wallCount > 0 && (mapData[x, y] == 1 || mapData[x, y] == 2) )
                {
                    if (UnityEngine.Random.value < decorationDensity)
                    {
                        GameObject randomProp = propPrefabs[UnityEngine.Random.Range(0, propPrefabs.Length)];
                        PlaceObject(randomProp, x, y, 4);
                        continue;
                    }
                }

                if ((mapData[x,y] == 1|| mapData[x, y] == 2) && mapData[x, y] < 4)
                {
                    float finalTrapDensity = trapDensity;
                    if (mapData[x, y] == 2) finalTrapDensity *= 0.5f;
                    if (IsNextToType(x, y, 5)) finalTrapDensity *= 1;
                    if(UnityEngine.Random.value < finalTrapDensity)
                    {
                        if(GetWallDirection(x, y) != Vector2Int.zero)
                        {
                            PlaceObject(trapPrefab[0], x, y, 7);
                        }
                        else
                        {
                            PlaceObject(trapPrefab[1], x, y, 7, 0.3f);
                        }
                        continue;
                    }
                }
            }
        }
    }

    private void PlaceObject(GameObject prefab, int x, int y, int typeMask, float posY = 0.1f)
    {
        Vector3 pos = new Vector3(x * tileSize, posY, y * tileSize);
        Quaternion rot = Quaternion.identity;
        if(typeMask == 7)
        {
            Vector3 offset = Vector3.zero;
            rot = GetTrapRot(x, y);
        }
        else
        {
            rot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        }
        GameObject obj = Instantiate(prefab, pos, rot, dungeonParent);
        obj.name = $"{prefab.name}_{x}_{y}";
        mapData[x, y] = typeMask;
    }
    private Quaternion GetTrapRot(int x, int y)
    {
        // 벽과 같은 방향을 보도록 변경
        var direction = new[]
        {
            new { dx = -1, dy = 0, rot = Quaternion.Euler(0, 90, 0) },
            new { dx = 1, dy = 0, rot = Quaternion.Euler(0, -90, 0) },
            new { dx = 0, dy = -1, rot = Quaternion.Euler(0, 0, 0) },
            new { dx = 0, dy = 1, rot = Quaternion.Euler(0, 180, 0) }
        };
        foreach(var dir in direction)
        {
            int nx = x + dir.dx;
            int ny = y + dir.dy;
            if(IsInMap(nx, ny) && mapData[nx, ny] == 3)
            {
                return dir.rot;
            }
        }
        return Quaternion.identity;
    }
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
                if (IsInMap(nx, ny) && mapData[nx, ny] == 3) count++;
            }
        }
        return count;
    }
    private Vector2Int GetWallDirection(int x, int y)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        Vector2Int foundDIr = Vector2Int.zero;
        int count = 0;

        foreach(var dir in dirs)
        {
            if(IsInMap(x + dir.x, y + dir.y) && mapData[x+dir.x, y+dir.y] == 3)
            {
                count++;
                foundDIr = dir;
            }
        }

        return (count == 1) ? foundDIr : Vector2Int.zero;
    }

    public void SpawnEnemies()
    {
        if (mapData == null || points == null) return;

        foreach(var p in points)
        {
            if (p == EndPoint) continue;
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);
            if (Vector2.Distance(p, StartPoint) < 2f || p == EndPoint) continue;

            int spawnCount = UnityEngine.Random.Range(5, maxEnemiesPerRoom + 1);

            for(int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPos = new Vector3(cx * tileSize, 0.5f, cy * tileSize);
                GameObject enemy = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity, dungeonParent);
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = true;
                }
                EnemyStatComponent stat = enemy.GetComponent<EnemyStatComponent>();
                if (stat != null)
                {
                    stat.WayPoint = CreateWayPointsForPoint(cx, cy, roomSize);
                }
            }
        }
    }

    private Transform CreateWayPointsForPoint(int cx, int cy, int size)
    {
        float range = size * 0.8f;
        Vector3 randomPos = new Vector3(
            (cx + UnityEngine.Random.Range(-range, range)) * tileSize,
            0.5f,
            (cy + UnityEngine.Random.Range(-range, range)) * tileSize
        );
        GameObject wp = new GameObject("WayPoint");
        wp.transform.position = randomPos;
        wp.transform.SetParent(dungeonParent);
        return wp.transform;
    }

    public void BuildNavMesh()
    {
        if(navSurface == null)
        {
            navSurface = dungeonParent.gameObject.AddComponent<NavMeshSurface>();

            navSurface.collectObjects = CollectObjects.Children;
        }

        navSurface.BuildNavMesh();
    }

    public void SpawnBossInRoom()
    {
        Vector3 spawnPos = new Vector3(EndPoint.x * tileSize, 0.5f, EndPoint.y * tileSize);
        GameObject bossObj = Instantiate(bossPrefab, spawnPos, Quaternion.identity, dungeonParent);

        BossController boss = bossObj.GetComponent<BossController>();
        if(boss != null)
        {
            boss.QuestData = dungeonQuestData;
        }
    }
    private int GetRoomSize(Vector2 roomPos)
    {
        return (roomPos == EndPoint) ? roomSize + 2 : roomSize;
    }

    private void SpawnPortal()
    {
        Vector3 spawnPos = new Vector3(EndPoint.x * tileSize, 0.5f, EndPoint.y * tileSize);
        GameManager.Instance.SpawnPortal("VillageScene", spawnPos);
    }

    private void OnDungeonGenerationComplete()
    {
        grid.transform.position = Vector3.zero;
        Vector2 dungeonSize = new Vector2(mapWidth * tileSize, mapHeight * tileSize);
        grid.Init(dungeonSize, tileSize / 2);
    }

}
