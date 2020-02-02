using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainLevel : MonoBehaviour
{


    public void LoadMainLevelNow()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

}
