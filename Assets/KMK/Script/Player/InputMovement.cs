using UnityEngine;

public class InputMovement : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 targetPos;
    private bool isMoving;
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
        mainCam = Camera.main;
        targetPos = transform.position; 
    }
    public void Move(Vector3 dir)
    {
        movement = dir;
        cc.Move(movement * pc.StatComp.MoveSpeed * Time.deltaTime);
        Vector3 targetDir = movement.normalized;
        targetDir.y = 0; 

        pc.SetAnimFloat("Move", movement.magnitude);
    }
    public void RotTarget(Vector3 dir)
    {
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
}
