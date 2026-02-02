using UnityEngine;

public class InputMovement : MonoBehaviour
{
    #region 이동
    [Header("이동")]
    private CharacterController controller;
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;
    private Vector3 movement;
    #endregion
    #region 중력
    [Header("중력")]
    [SerializeField] private float grav = 20;
    private float vSpeed = 0;
    #endregion
    [Header("애니메이션")]
    private Animator animator;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        GravityDown();
    }

    private void Move()
    {
        // 입력 이동 방향
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 입력값에 따라 dir 설정
        Vector3 dir = new Vector3(h, 0, v).normalized;
        movement = dir;

        // 회전처리
        Vector3 targetDir = movement.normalized;
        if(targetDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);

            controller.Move(movement * speed * Time.deltaTime);
        }

    }

    private void GravityDown()
    {
        vSpeed -= grav * Time.deltaTime;
        if (vSpeed < -grav) vSpeed = -grav;

        Vector3 vMove = new Vector3(0, vSpeed * Time.deltaTime, 0);

        // Move 함수는 flag를 반환함 => 충돌 확인 가능
        CollisionFlags flag = controller.Move(vMove);
        // 바닥에서 떠있다면
        if ((flag & CollisionFlags.Below) != 0)
        {
            vSpeed = 0;
        }
    }
}
