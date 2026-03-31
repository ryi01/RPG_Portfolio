using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class InputMovement : MonoBehaviour
{
    #region 이동
    [Header("이동")]
    private CharacterController cc;
    private PlayerController pc;
    private Vector3 targetPos;
    private bool isMoving = false;
    public bool IsMoving => isMoving;

    public System.Action OnArrival;
    [SerializeField] private Pathfinder pathfinder;
    private List<Node> currentPath;
    private int targetIndex;
    #endregion
    #region 중력
    [Header("중력")]
    [SerializeField] private float grav = 20;
    private float vSpeed = 0;
    #endregion
    [SerializeField] private LineRenderer pathVisualizerPrefab;
    [SerializeField] private TrailRenderer dashTrail;
    private LineRenderer pathLine;

    private Coroutine forceCoroutine;
    private readonly List<Collider> ignoredEnemyCols = new List<Collider>();
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        pathLine = Instantiate(pathVisualizerPrefab, transform);
        dashTrail.emitting = false;
        targetPos = transform.position;
    }
    public void FindPath(Vector3 pos, bool isGrid = true)
    {
        // 이동 시작 시 이전 로직 정리
        StopMove();
        if (isGrid && GameManager.Instance.IsPathFindingEnable)
        {
            // 경로 탐색 시작
            currentPath = pathfinder.FindPath(transform.position, pos);
            if (currentPath != null && currentPath.Count > 0)
            {
                targetIndex = 0;
                isMoving = true;
                pathLine.enabled = true;
                DrawPath();
                targetPos = currentPath[targetIndex].worldPos;
            }
            else isMoving = false;
        }
        else
        {
            targetPos = pos;
            isMoving = true;
        }
    }
    // 강제 이동 중단 => 키입력, 피격, 대화시작 등
    public void StopMove()
    {
        HidePathLine();
        isMoving = false;
        currentPath = null;
        targetIndex = 0;
        pc.Animator.SetFloat("Move", 0);
    }
    public void UpdateClickMove()
    {
        if (isMoving == false) return;
        if (pathLine != null && pathLine.enabled)
        {
            pathLine.SetPosition(0, transform.position + Vector3.up * 0.1f);
        }
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
       
        if (dir.magnitude < 0.3f)
        {
            if(GameManager.Instance.IsPathFindingEnable)
            {
                targetIndex++;
                if (currentPath != null && targetIndex < currentPath.Count)
                {
                    targetPos = currentPath[targetIndex].worldPos;
                    DrawPath();
                }
                else
                {
                    CompleteArrive();
                    return;
                }
            }
            else
            {
                CompleteArrive();
                return;
            }
        }
        RotTarget(dir.normalized);
        Move(dir.normalized);
    }
    public void Move(Vector3 dir)
    {
        CollisionFlags flag = cc.Move(dir * pc.StatComp.MoveSpeed * Time.deltaTime);
        if((flag & CollisionFlags.Sides) != 0)
        {
            float dis = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPos.x, 0, targetPos.z));
            if(dis < 1.2f)
            {
                CompleteArrive();
            }
            else
            {
                StopMove();
            }
        }

    }
    // 목적지 도착 여부
    private void CompleteArrive()
    {
        HidePathLine();
        isMoving = false;
        pc.Animator.SetFloat("Move", 0);
        OnArrival?.Invoke();
        OnArrival = null;
    }
    public void RotTarget(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, pc.StatComp.RotSpeed * Time.deltaTime);
    }

    public void LookAtInstant(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    public void GravityDown()
    {
        vSpeed -= grav * Time.deltaTime;
        if (vSpeed < -grav) vSpeed = -grav;

        Vector3 vMove = new Vector3(0, vSpeed * Time.deltaTime, 0);

        // Move 함수는 flag를 반환함 => 충돌 확인 가능
        CollisionFlags flag = cc.Move(vMove);
        // 바닥에서 떠있다면
        if ((flag & CollisionFlags.Below) != 0)
        {
            vSpeed = 0;
        }
    }
    public Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Environment");
        if(Physics.Raycast(ray, out RaycastHit hitinfo, 100f, layerMask))
        {
            return hitinfo.point;
        }
        Plane ground = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if(ground.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return transform.position + transform.forward;
    }

    public void Push(Vector3 dir, float distance, float duration, bool isJump = false, bool useTrail = false)
    {
        StartPush(dir, distance, duration, isJump, useTrail, false);
    }
    public void PushByBoss(Vector3 dir, float distance, float duration, bool isJump = false, bool useTrail = false)
    {
        StartPush(dir, distance, duration, isJump, useTrail, true);
    }
    private void StartPush(Vector3 dir, float distance, float duration, bool isJump, bool useTrail, bool ignoreEnemyCollision)
    {
        if(forceCoroutine != null)
        {
            StopCoroutine(forceCoroutine);
            RestoreIgnoredEnemyCollisions();
            forceCoroutine = null;
        }

        StopMove();

        dir.y = 0;
        if (dir.sqrMagnitude <= 0.001f) return;
        dir.Normalize();
        pc.CameraShakeController.ShakeCam(0.1f, 0.2f);

        if (dashTrail != null)
            dashTrail.emitting = useTrail;

        forceCoroutine = StartCoroutine(OnForce(dir, distance, duration, isJump, ignoreEnemyCollision));
    }
    IEnumerator OnForce(Vector3 dir, float distance, float duration, bool isJump, bool ignoreEnemyCollision)
    {
        if(ignoreEnemyCollision)
        {
            IgnoreNearbyEnemyCollisions(true, 2.5f);
        }
        float elapsed = 0;
        Vector3 startPos = transform.position;
        if (Physics.Raycast(startPos, dir, out RaycastHit hit, distance, LayerMask.GetMask("Wall")))
        {
            distance = Mathf.Max(hit.distance - 0.2f, 0);
        }
        Vector3 targetPos = startPos + (dir * distance);
        float height = isJump ? 1f : 0f;
        Vector3 prevPos = startPos;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            float yOffset = Mathf.Sin(t * Mathf.PI) * height;
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, t);
            pos.y = baseY + yOffset;
            Vector3 delta = pos - prevPos;

            cc.Move(delta);
            prevPos = pos;
            yield return null;
        }
        dashTrail.emitting = false;

        RestoreIgnoredEnemyCollisions();
        forceCoroutine = null;
    }
    private void IgnoreNearbyEnemyCollisions(bool ignore, float radius)
    {
        RestoreIgnoredEnemyCollisions();

        Collider playerCol = cc;
        if (playerCol == null) return;
        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayerMask);
        foreach(Collider hit in hits)
        {
            if (!hit) continue;
            if (hit.transform == transform) continue;

            Physics.IgnoreCollision(playerCol, hit, ignore);
            ignoredEnemyCols.Add(hit);
        }
    }

    private void RestoreIgnoredEnemyCollisions()
    {
        Collider playerCol = cc;
        if (playerCol == null) return;
        for(int i = 0; i < ignoredEnemyCols.Count; i++)
        {
            Collider enemyCol = ignoredEnemyCols[i];
            if (!enemyCol) continue;

            Physics.IgnoreCollision(playerCol, enemyCol, false);
        }
        ignoredEnemyCols.Clear();
    }
    private void DrawPath()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            pathLine.positionCount = 0;
            return;
        }

        int remainNodes = currentPath.Count - targetIndex;
        pathLine.positionCount = remainNodes + 1;
        Vector3 startPos = transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.25f);
        pathLine.SetPosition(0, startPos);

        for(int i = 0; i < remainNodes; i++)
        {
            pathLine.SetPosition(i + 1, currentPath[targetIndex + i].worldPos + Vector3.up * 0.25f);
        }
    }

    public void HidePathLine()
    {
        if(pathLine != null)
        {
            pathLine.positionCount = 0;
            pathLine.enabled = false;
        }
    }
}
