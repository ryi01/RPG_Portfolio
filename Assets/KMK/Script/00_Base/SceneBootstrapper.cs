using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Excute()
    {
        string mainSceneName = "PersistentScene";
        string startVillageScene = "VillageScene";

        if(!IsSceneLoad(mainSceneName))
        {
            SceneManager.LoadScene(mainSceneName, LoadSceneMode.Additive);
        }
        if(SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().name == mainSceneName)
        {
            SceneManager.LoadScene(startVillageScene, LoadSceneMode.Additive);
        }
    }

    private static bool IsSceneLoad(string scene)
    {
        for(int i = 0; i < SceneManager.sceneCount;i++)
        {
            if (SceneManager.GetSceneAt(i).name == scene) return true;
        }

        return false;
    }
}
