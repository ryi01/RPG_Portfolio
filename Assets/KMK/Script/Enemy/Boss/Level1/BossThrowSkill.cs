using System.Collections;
using System.Reflection;
using UnityEngine;


public class BossThrowSkill : EnemySkillAttack
{
    [SerializeField]private GameObject rockPrefab;

    private GameObject rock;

    [SerializeField] private Transform throwingHand;

    [SerializeField] private float speed = 1.5f;

    [SerializeField] private float height = 3f;

    private Coroutine throwRockCoroutine;

    private Vector3 lockedTargetPosition;
    private bool isLockedTarget = false;
    public void LockTargetPos()
    {
        if (owner == null || owner.Player == null) return;

        lockedTargetPosition = owner.Player.transform.position;
        isLockedTarget = true;
    }
    public void SpawnRock()
    {
        if (rock != null)
        {
            Destroy(rock);
        }
        PlayImpactSFX();
        rock = Instantiate(rockPrefab, attackTransform.position, attackTransform.rotation, attackTransform);
        rock.GetComponent<BulletCollision>().InitSet(owner, cameraEffect, this);
        rock.SetActive(true);
    }

    public void GrabRock()
    {
        rock.transform.SetParent(throwingHand);
        rock.transform.localPosition = Vector3.zero;
        rock.transform.localRotation = Quaternion.identity;
    }

    public void ThrowRock()
    {
        rock.transform.SetParent(null);
        if (throwRockCoroutine != null)
        {
            StopCoroutine(throwRockCoroutine);
        }
        throwRockCoroutine = StartCoroutine(ThrowRockCoroutine());

    }

    public void RemoveRock()
    {
        if (throwRockCoroutine != null)
        {
            StopCoroutine(throwRockCoroutine);
            throwRockCoroutine = null;
        }

        if (rock != null)
        {
            Destroy(rock);
            rock = null;
        }
    }

    public void HideRock()
    {
        if (rock != null)
        {
            rock.SetActive(false);
        }
    }

    private IEnumerator ThrowRockCoroutine()
    {
        if (rock == null || owner == null || owner.Player == null) yield break;

        Vector3 initPosition = rock.transform.position;
        Vector3 targetPosition = isLockedTarget ? lockedTargetPosition : owner.Player.transform.position;

        Vector3 midPoint = (initPosition + targetPosition) * 0.5f;
        midPoint.y += height;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            if (rock == null) yield break;
            rock.transform.position = Parabola(initPosition, midPoint, targetPosition, t);

            Vector3 forwardVector = CalculateParabolaDirection(initPosition, midPoint, targetPosition, t);
            if (forwardVector.sqrMagnitude > 0.001f)
            {
                rock.transform.rotation = Quaternion.LookRotation(forwardVector, Vector3.up);
            }

            yield return null;
        }

        if (rock != null)
        {
            Destroy(rock);
            rock = null;
        }

        throwRockCoroutine = null;
    }


    private Vector3 Parabola(Vector3 start, Vector3 mid, Vector3 end, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 result = (uu * start) + (2 * u * t * mid) + (tt * end);
        return result;
    }

    private Vector3 CalculateParabolaDirection(Vector3 start, Vector3 mid, Vector3 end, float t)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        Vector3 tangent = 2 * ((u * (mid - start)) + (t * (end - mid)));

        return tangent.normalized;
    }
}

