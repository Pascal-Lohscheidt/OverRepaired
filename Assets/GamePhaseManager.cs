using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GamePhaseManager : Singleton<GamePhaseManager>
{

    public GameObject loseScreen;

    
    public void LoseGame()
    {
        StartCoroutine(HandleGameLose());
    }
 

    public void ExitGameToMenu()
    {
        SceneManager.LoadScene(0);
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
        yield return new WaitForSeconds(5f);
        ExitGameToMenu();
    }
}
