using UnityEngine;

public class BossLinearShotSkillAttack : EnemySkillAttack
{
    [SerializeField] private GameObject linearProjectilePrefab;
    [SerializeField] private int phaseOneShotCount = 1;
    [SerializeField] private int phaseTwoShotCount = 3;
    [SerializeField] private float angleStep = 10f;
    private int currentShotCnt;
    private void OnEnable()
    {
        currentShotCnt = phaseOneShotCount;
        if (owner.BossPhase != null) owner.BossPhase.OnPhaseTwoStarted += SetBulletCount;
    }
    private void OnDisable()
    {
        if (owner.BossPhase != null) owner.BossPhase.OnPhaseTwoStarted -= SetBulletCount;
    }
    private void SetBulletCount()
    {
        currentShotCnt = phaseTwoShotCount;
    }


    public override void Attack()
    {
        if (linearProjectilePrefab == null) return;
        FireSpreadShot();
    }

    private void FireSpreadShot()
    {
        Vector3 targetPos = owner.Player.transform.position;
        targetPos.y = attackTransform.position.y;
        Vector3 dir = (targetPos - attackTransform.position).normalized;
        if (dir == Vector3.zero) return;
        Quaternion baseRot = Quaternion.LookRotation(dir);
        if (currentShotCnt <= 1)
        {
            SpawnBullet(baseRot);
            return;
        }

        float startAngle = -angleStep * (currentShotCnt - 1) * 0.5f;
        for (int i = 0; i < currentShotCnt; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rot = baseRot * Quaternion.Euler(0, currentAngle, 0);
            SpawnBullet(rot);
        }
    }
    private void SpawnBullet(Quaternion rot)
    {
        Vector3 spawnPos = attackTransform.position;
        spawnPos.y = owner.Player.transform.position.y + 0.8f;
        GameObject bullet = Instantiate(linearProjectilePrefab, spawnPos, rot);
        if (bullet != null)
        {
            if (bullet.TryGetComponent(out BulletCollision collision))
            {
                collision.Owner = GetComponent<BaseController>();
            }
        }
    }

    public void OnFirstLinearShot()
    {
        PlayEffect();
        Attack();
    }
}
