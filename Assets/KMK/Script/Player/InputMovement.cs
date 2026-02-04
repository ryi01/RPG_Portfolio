using System.Collections;
using UnityEngine;

public class InputMovement : MonoBehaviour
{
    #region 이동
    [Header("이동")]
    private CharacterController cc;
    private PlayerController pc;
    private Vector3 movement;
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
    }
    public void Move(Vector3 dir)
    {
        movement = dir;
        cc.Move(movement * pc.StatComp.MoveSpeed * Time.deltaTime);
        movement.y = 0; 

        pc.Animator.SetFloat("Move", movement.normalized.magnitude);
    }
    public void RotTarget(Vector3 dir)
    {
        if (dir == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, pc.StatComp.RotSpeed * Time.deltaTime);
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
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if(ground.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return transform.position;
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
            cc.Move(dir * currentStep);
            yield return null;
        }
    }
}
