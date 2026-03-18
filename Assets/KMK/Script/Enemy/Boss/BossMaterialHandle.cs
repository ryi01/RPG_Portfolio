using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 sharedMaterial
: 원본 머티리얼과 같이 사용됨
: 바꾸면 같은 머티리얼을 쓰는 애들이 다같이 바뀜
: 복사 X 가벼움
material
: 접근하는 순간 복사본이 생김
: 특정 오브젝트만 따로 바뀜
: 편하지만 많이 쓰면 무거움
materialPropertyBlock
: 머티리얼을 복사하지 않고
: 오브젝트별 값만 다르게 보이게함
: 잔상, 색변경 하이라이트에 좋음
 */
public class BossMaterialHandle : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] bossRenderer;
    [SerializeField] private Material chargingMaterial;
    private Dictionary<SkinnedMeshRenderer, Material> originalMats = new Dictionary<SkinnedMeshRenderer, Material>();
    private Dictionary<GameObject, Coroutine> ghostCoroutines = new Dictionary<GameObject, Coroutine>();
    private List<GameObject> activeGhost = new List<GameObject>();
    private List<GameObject> outlineObj = new List<GameObject>();
    private List<Material> createMats = new List<Material>();

    // 기본 material을 originalMats에 저장
    public void SetOriginMats()
    {
        originalMats.Clear();
        if (bossRenderer == null) return;
        foreach (var skin in bossRenderer)
        {
            if (skin == null) continue;
            originalMats[skin] = skin.sharedMaterial;
        }
    }

    // outline 만들기
    public void CreateCharginOutline()
    {
        ClearChargingOutline();
        if (bossRenderer == null || chargingMaterial == null) return;

        // 보스의 skinRenderer 갯수만큼 skin 생성 후,
        // ChargingOutline으로 Material 생성
        foreach (var skin in bossRenderer)
        {
            if (skin == null) continue;
            GameObject outline = new GameObject("ChargingOutline");
            outline.transform.SetParent(skin.transform, false);
            outline.transform.localScale = Vector3.one * 1.02f;
            // 실제로 화면에 그려주고
            MeshRenderer mr = outline.AddComponent<MeshRenderer>();
            // mesh를 화면에 띄우기 위한 도구 중 하나. 모양을 잡아줌
            MeshFilter mf = outline.AddComponent<MeshFilter>();
            // mesh를 만들고
            Mesh baked = new Mesh();
            // 보스 외형 대로 메쉬를 bake해서
            skin.BakeMesh(baked);
            // 모양을 만들어준다 
            mf.mesh = baked;
            // material을 설정해 실제로 보여줌
            Material mat = new Material(chargingMaterial);
            mr.material = mat;
            createMats.Add(mat);
            outlineObj.Add(outline);
        }
    }

    // 충전하는 외형 material을 조절
    public void UpdateCharginColor(float ratio)
    {
        Color effectColor = Color.Lerp(Color.red, Color.yellow, ratio);
        foreach (var obj in outlineObj)
        {
            if (obj == null) continue;

            var mr = obj.GetComponent<MeshRenderer>();
            if (mr == null) continue;

            mr.material.SetColor("_GlowColor", effectColor);
        }
    }

    // 메모리 정리
    public void ClearChargingOutline()
    {
        foreach (var obj in outlineObj)
        {
            if (obj != null) Destroy(obj);
        }
        foreach (var mat in createMats)
        {
            if(mat != null) Destroy(mat);
        }
        outlineObj.Clear();
        createMats.Clear();
    }
    // 잔상만들기
    public void CreateGhostTrail()
    {
        if (bossRenderer == null) return;
        foreach (var skin in bossRenderer)
        {
            if (skin == null) continue;
            GameObject ghost = new GameObject("GhostTrail");
            activeGhost.Add(ghost);
            // 잔상을 만들어서
            MeshFilter mf = ghost.AddComponent<MeshFilter>();
            MeshRenderer mr = ghost.AddComponent<MeshRenderer>();

            Mesh bakedMesh = new Mesh();
            skin.BakeMesh(bakedMesh);
            mf.mesh = bakedMesh;
            // originalMats를 key로 찾아서 있으면 => 새로만든 Material에 경우 존재 하지 않음
            if (originalMats.TryGetValue(skin, out Material origin))
            {
                // 오리진으로
                mr.sharedMaterial = origin;
            }
            // 없으면 sharedMaterial로 
            else mr.sharedMaterial = skin.sharedMaterial;
            // MaterialPropertyBlock : 일부 값만 개별적으로 변경
            MaterialPropertyBlock ghostProp = new MaterialPropertyBlock();
            ghostProp.SetColor("_BaseColor", Color.cyan * 2f); // 잔상 색상
            ghostProp.SetFloat("_Alpha", 0.5f);               // 반투명
            mr.SetPropertyBlock(ghostProp);
            // 2. 렌더링 순서 조정 (본체 뒤에 그려지도록)
            mr.sortingOrder = -1;

            ghost.transform.position = skin.transform.position;
            ghost.transform.rotation = skin.transform.rotation;

            Material runtimeMat = mr.material;
            runtimeMat.SetColor("_TintColor", Color.cyan * 2f);

            Coroutine co = StartCoroutine(FadeOutGhost(ghost, mr, runtimeMat, 0.5f));
            ghostCoroutines[ghost] = co;
        }

    }

    private IEnumerator FadeOutGhost(GameObject ghost, MeshRenderer mr, Material mat, float duration)
    {
        if (ghost == null || mr == null || mat == null) yield break;
        float elapsed = 0;

        Color startColor = Color.cyan * 3.0f; // 밝은 하늘색
        Color endColor = new Color(0.5f, 0f, 1f, 1f) * 3.0f; // 밝은 보라색

        while (elapsed < duration)
        {
            if (ghost == null || mr == null || mat == null) yield break;
            elapsed += Time.deltaTime;
            float ratio = elapsed / duration;
            float alpha = Mathf.Lerp(1, 0, ratio);
            Color currentColor = Color.Lerp(startColor, endColor, ratio);

            mat.SetFloat("_Alpha", alpha);

            mat.SetColor("_BaseColor", currentColor);

            yield return null;
        }
        ghostCoroutines.Remove(ghost);
        activeGhost.Remove(ghost);
        if(mat != null) Destroy(mat);
        if (ghost != null) Destroy(ghost);
    }

    public void ResetAll()
    {
        ClearChargingOutline();

        foreach(var pair in ghostCoroutines)
        {
            if (pair.Value != null) StopCoroutine(pair.Value);
        }
        ghostCoroutines.Clear();

        foreach (var ghost in activeGhost)
        {
            if (ghost != null) Destroy(ghost);
        }
        activeGhost.Clear();

        foreach (var pair in originalMats)
        {
            if (pair.Key != null)
            {
                pair.Key.sharedMaterial = pair.Value;
            }
        }
        originalMats.Clear();
    }
    private void OnDisable()
    {
        ResetAll();
    }
}
