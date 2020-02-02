using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainLevel : MonoBehaviour
{


    public void LoadMainLevelNow()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

}
