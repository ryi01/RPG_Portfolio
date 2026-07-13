using System.Collections;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float shirinkDuration = 0.3f;

    protected GameObject player;
    private bool isEnding;
    private float lifeTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lifeTimer = lifeTime;
    }
    private void Update()
    {
        if (isEnding) return;
        lifeTimer -= Time.deltaTime;
        if(lifeTimer <= 0f)
        {
            StartCoroutine(ShrinkAndDestroy());
            return;
        }
        if (player == null || gameObject == null) return;
        Vector3 targetPos = player.transform.position + Vector3.up * 1.5f;
        Vector3 targetDir = (targetPos - transform.position);
        if (targetDir.sqrMagnitude < 0.01f) return;
        targetDir.Normalize();
        transform.forward = targetDir;
        transform.position += targetDir * moveSpeed * Time.deltaTime;
    }
    private IEnumerator ShrinkAndDestroy()
    {
        isEnding = true;
        Vector3 startScale = transform.localScale;
        float elapsed = 0;
        while(elapsed < shirinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shirinkDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
