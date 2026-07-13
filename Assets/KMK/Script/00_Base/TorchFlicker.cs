using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private float baseIntensity = 1.8f;
    [SerializeField] private float flickerAmount = 0.25f;
    [SerializeField] private float flickerSpeed = 7f;

    private float rndSpeed;

    private void Awake()
    {
        rndSpeed = Random.Range(0, 100);
    }

    private void Update()
    {
        if (torchLight == null) return;
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, rndSpeed);
        torchLight.intensity = baseIntensity + (noise - 0.5f) * flickerAmount;
    }
}
