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
    
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
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
        isMoving = false;
        pc.Animator.SetFloat("Move", 0);
    }
    public void UpdateClickMove()
    {
        if (isMoving == false) return;
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        Debug.DrawLine(transform.position, targetPos, Color.red); // 플레이어에서 목표까지 선 그리기
        Debug.DrawRay(targetPos, Vector3.up * 2, Color.green);    // 목표 지점에 기둥 세우기
        if (dir.magnitude < 0.3f)
        {
            if(GameManager.Instance.IsPathFindingEnable)
            {
                targetIndex++;
                if (currentPath != null && targetIndex < currentPath.Count)
                {
                    targetPos = currentPath[targetIndex].worldPos;
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
    private Coroutine forceCoroutine;
    public void Push(Vector3 dir, float distance, float duration)
    {
        if(forceCoroutine != null) StopCoroutine(forceCoroutine); 
        forceCoroutine = StartCoroutine(OnForce(dir, distance, duration));
    }
    IEnumerator OnForce(Vector3 dir, float distance, float duration)
    {
        float elapsed = 0;
        Vector3 startPos = transform.position;
        if(Physics.Raycast(startPos, dir, out RaycastHit hit, distance, LayerMask.GetMask("Wall")))
        {
            distance = hit.distance - 0.2f;
        }
        Vector3 targetPos = startPos + (dir * distance);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }
}
