using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GamePhaseManager : Singleton<GamePhaseManager>
{

    public GameObject loseScreen;

    
    public void LoseGame()
    {
        StartCoroutine(HandleGameLose());
    }
 

    public void ExitGameToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        Time.timeScale = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        loseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ExitGameToMenu();
    }

    private IEnumerator HandleGameLose()
    {
        loseScreen.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(5f);
        ExitGameToMenu();
    }
}
