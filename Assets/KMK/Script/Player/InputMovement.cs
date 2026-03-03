using System.Collections;
using UnityEngine;

public class InputMovement : MonoBehaviour
{
    #region 이동
    [Header("이동")]
    private CharacterController cc;
    private PlayerController pc;
    private Vector3 movement;
    private Vector3 targetPos;
    private bool isMoving = false;
    public bool IsMoving => isMoving;

    public System.Action OnArrival;
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
    public void SetTarget(Vector3 pos)
    {
        targetPos = pos;
        isMoving = true;
    }
    public void StopMove()
    {
        isMoving = false;
    }
    public void UpdateClickMove()
    {
        if (isMoving == false) return;
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        if(dir.magnitude < 1.5f)
        {
            CompleteArrive();
            return;
        }
        Vector3 moveDir = dir.normalized;
        RotTarget(moveDir);
        Move(moveDir);
    }
    public void Move(Vector3 dir)
    {
        CollisionFlags flag = cc.Move(dir * pc.StatComp.MoveSpeed * Time.deltaTime);
        if((flag & CollisionFlags.Sides) != 0)
        {
            float dis = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.y), new Vector3(targetPos.x, 0, targetPos.y));
            if (dis < 1.2f)
            {
                CompleteArrive();
            }
        }

    }
    private void CompleteArrive()
    {
        isMoving = false;
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
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentStep = (distance/duration)* Time.deltaTime;
            CollisionFlags flag = cc.Move(dir * currentStep);
            if ((flag & CollisionFlags.Sides) != 0) yield break;
            yield return null;
        }
    }
}
