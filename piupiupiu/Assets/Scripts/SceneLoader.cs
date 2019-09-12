using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    [SerializeField] float secondsToLoadGameOver = 2f;

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    public void LoadStartScene()
    {
        FindObjectOfType<GameSession>().ResetGame();
        SceneManager.LoadScene(0);
    }

    public void LoadFirstLevel()
    {
        FindObjectOfType<GameSession>().ResetGame();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        StartCoroutine(WaitAndLoad());
    }

    public IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(secondsToLoadGameOver);
        SceneManager.LoadScene("Game Over");
    }
}
