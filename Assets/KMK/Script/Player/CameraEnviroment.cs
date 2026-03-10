using UnityEngine;

public class CameraEnviroment : MonoBehaviour
{
    [SerializeField] private Camera cam;

    public void ChangeToDungeon()
    {
        cam.backgroundColor = new Color(0.1f, 0.11f, 0.13f);
        RenderSettings.fog = true;
    }

    public void ChnageToTown()
    {
        cam.backgroundColor = new Color(0.56f, 0.7f, 0.83f);
        RenderSettings.fog = false;
    }
}
