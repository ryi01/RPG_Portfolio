using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMaterialHandle : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] bossRenderer;
    [SerializeField] private Material chargingMaterial;
    private Dictionary<SkinnedMeshRenderer, Material> originalMats = new Dictionary<SkinnedMeshRenderer, Material>();
    private List<GameObject> activeGhost = new List<GameObject>();
    private List<GameObject> outlineObj = new List<GameObject>();
    private List<Material> createMats = new List<Material>();

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

    public void CreateCharginOutline()
    {
        ClearChargngOutline();
        if (bossRenderer == null || chargingMaterial == null) return;
        foreach (var skin in bossRenderer)
        {
            if (skin == null) continue;
            GameObject outline = new GameObject("ChargingOutline");
            outline.transform.SetParent(skin.transform, false);
            outline.transform.localScale = Vector3.one * 1.02f;
            MeshRenderer mr = outline.AddComponent<MeshRenderer>();
            MeshFilter mf = outline.AddComponent<MeshFilter>();

            Mesh baked = new Mesh();
            skin.BakeMesh(baked);
            mf.mesh = baked;
            Material mat = new Material(chargingMaterial);
            mr.material = mat;
            createMats.Add(mat);
            outlineObj.Add(outline);
        }
    }

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

    public void ClearChargngOutline()
    {
        foreach (var obj in outlineObj) Destroy(obj);
        foreach (var mat in createMats) Destroy(mat);
        outlineObj.Clear();
        createMats.Clear();
    }


    public void CreateGhostTrail()
    {
        if (bossRenderer == null) return;
        foreach (var skin in bossRenderer)
        {
            if (skin == null) continue;
            GameObject ghost = new GameObject("GhostTrail");
            activeGhost.Add(ghost);
            MeshFilter mf = ghost.AddComponent<MeshFilter>();
            MeshRenderer mr = ghost.AddComponent<MeshRenderer>();

            Mesh bakedMesh = new Mesh();
            skin.BakeMesh(bakedMesh);
            mf.mesh = bakedMesh;

            if (originalMats.TryGetValue(skin, out Material origin))
            {
                mr.sharedMaterial = origin;
            }
            else mr.sharedMaterial = skin.sharedMaterial;

            MaterialPropertyBlock ghostProp = new MaterialPropertyBlock();
            ghostProp.SetColor("_BaseColor", Color.cyan * 2f); // 잔상 색상
            ghostProp.SetFloat("_Alpha", 0.5f);               // 반투명
            mr.SetPropertyBlock(ghostProp);
            // 2. 렌더링 순서 조정 (본체 뒤에 그려지도록)
            mr.sortingOrder = -1;

            ghost.transform.position = skin.transform.position;
            ghost.transform.rotation = skin.transform.rotation;

            mr.material.SetColor("_TintColor", Color.cyan * 2f);

            StartCoroutine(FadeOutGhost(mr, 0.5f));
        }

    }
    private IEnumerator FadeOutGhost(MeshRenderer mr, float duration)
    {
        float elapsed = 0;
        Material mat = mr.material;
        Color startColor = Color.cyan * 3.0f; // 밝은 하늘색
        Color endColor = new Color(0.5f, 0f, 1f, 1f) * 3.0f; // 밝은 보라색

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float ratio = elapsed / duration;
            float alpha = Mathf.Lerp(1, 0, ratio);
            Color currentColor = Color.Lerp(startColor, endColor, ratio);

            mat.SetFloat("_Alpha", alpha);

            mat.SetColor("_BaseColor", currentColor);

            yield return null;
        }
        activeGhost.Remove(mr.gameObject);
        Destroy(mat);
        Destroy(mr.gameObject);
    }

    public void ResetAll()
    {
        ClearChargngOutline();
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
}
