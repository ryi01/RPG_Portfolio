using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.AI.Navigation;
using UnityEngine.AI;
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
    [SerializeField][Range(0, 1)] private float enemySpawnDensity = 0.4f;
    [SerializeField] private int maxEnemiesPerRoom = 5;

    [Header("Navigation")]
    public NavMeshSurface navSurface;

    private int[,] mapData;
    
    public Vector2 StartPoint { get;private set; }
    public Vector2 EndPoint { get;private set; }

    public Vector3 WorldStartPoint => new Vector3(StartPoint.x * tileSize, 1.5f, StartPoint.y * tileSize);
    public Vector3 WorldEndPoint => new Vector3(EndPoint.x * tileSize, 1.5f, EndPoint.y * tileSize);

    public void GenerateDungeon()
    {
        GeneratePoint();
        Delaunay();
        DoMST();
        CreateFinalPath();

        CalculateStartAndEnd();

        CreateMap();

        CreateWalls();
        SpawnDungeonObjects();

        BuildNavMesh();
        SpawnEnemies();
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
        int roomSize = UnityEngine.Random.Range(4, 6);
        // 바닥 생성
        foreach (var p in points)
        {
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);

            for(int x = cx - roomSize; x <= cx + roomSize; x++)
            {
                for(int y = cy - roomSize; y <= cy+ roomSize; y++)
                {
                    if (IsInMap(x, y)) mapData[x, y] = 1;
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
                if (mapData[x, y] == 1)
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

        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
        {
            for (int offset = -1; offset <= 1; offset++)
            {
                int ty = y1 + offset;
                if (IsInMap(x, ty) && mapData[x, ty] == 0) mapData[x, ty] = 2;
            }
        }
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            // 복도 선뿐만 아니라 좌우 1칸씩 더 여유를 줍니다
            for (int offset = -1; offset <= 1; offset++)
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
                    if (IsNextToType(x, y, 1) || IsNextToType(x, y, 2))
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
    private void CalculateStartAndEnd()
    {
        if (points == null || points.Count < 2) return;

        float maxDis = -1;
        int startIndex = 0;
        int endIndex = 0;
        for(int i = 0; i < points.Count; i++)
        {
            for(int j = i+1; j < points.Count; j++)
            {
                float dis = Vector2.Distance(points[i], points[j]);
                if(dis > maxDis)
                {
                    maxDis = dis;
                    startIndex = i;
                    endIndex = j;
                }
            }
        }

        StartPoint = points[startIndex];
        EndPoint = points[endIndex];
        Debug.Log($"시작점: {StartPoint}, 보스방: {EndPoint} 선정 완료 (거리: {maxDis})");
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
                if (Vector2.Distance(currentPos, StartPoint) < 5f || currentPos == EndPoint) continue;
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

                if (mapData[x,y] == 1|| mapData[x, y] == 2 && mapData[x, y] < 4)
                {
                    float finalTrapDensity = trapDensity;
                    if (mapData[x, y] == 2) finalTrapDensity *= 0.5f;
                    if (IsNextToType(x, y, 5)) finalTrapDensity *= 1;
                    if(UnityEngine.Random.value < finalTrapDensity)
                    {
                        if(wallCount > 0)
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
        if (IsInMap(x - 1, y) && mapData[x - 1, y] == 3) return Quaternion.Euler(0, 180, 0);
        if (IsInMap(x + 1, y) && mapData[x + 1, y] == 3) return Quaternion.Euler(0, 0, 0);
        if (IsInMap(x, y - 1) && mapData[x, y - 1] == 3) return Quaternion.Euler(0, 90, 0);
        if (IsInMap(x, y + 1) && mapData[x - 1, y + 1] == 3) return Quaternion.Euler(0, -90, 0);
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

    public void SpawnEnemies()
    {
        if (mapData == null || points == null) return;

        foreach(var p in points)
        {
            if (p == StartPoint) continue;
            if (p == EndPoint) continue;
            if (UnityEngine.Random.value > enemySpawnDensity) continue;

            int spawnCount = UnityEngine.Random.Range(1, maxEnemiesPerRoom + 1);
            int cx = Mathf.RoundToInt(p.x);
            int cy = Mathf.RoundToInt(p.y);
            int rSize = 3;

            Transform[] wanderPoints = CreateWayPointsForPoint(cx, cy, rSize);

            for(int i = 0; i < spawnCount; i++)
            {
                int rx = cx + UnityEngine.Random.Range(-rSize, rSize + 1);
                int ry = cy + UnityEngine.Random.Range(-rSize, rSize + 1);

                if(IsInMap(rx, ry) && mapData[rx, ry] ==1)
                {
                    Vector3 spawnPos = new Vector3(rx * tileSize, 0.5f, ry * tileSize);
                    GameObject enemy = Instantiate(enemyPrefabs[0], spawnPos, Quaternion.identity, dungeonParent);

                    EnemyStatComponent stat = enemy.GetComponent<EnemyStatComponent>();
                    if (stat != null) stat.WanderPoints = wanderPoints;
                    NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                    if (agent != null) agent.Warp(spawnPos);
                }   
            }
        }
    }

    private Transform[] CreateWayPointsForPoint(int cx, int cy, int size)
    {
        Transform[] wps = new Transform[4];
        Vector2[] offsets = {new Vector2(-size, -size), new Vector2(-size, size),
                                new Vector2(size, -size), new Vector2(size, size)};
        for(int i = 0; i < 4; i++)
        {
            GameObject wp = new GameObject($"WP_{cx}_{cy}_{i}");
            wp.transform.SetParent(dungeonParent);
            wp.transform.position = new Vector3((cx + offsets[i].x) * tileSize, 0, (cy + offsets[i].y) * tileSize);
            wps[i] = wp.transform;
        }
        return wps;
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
}
