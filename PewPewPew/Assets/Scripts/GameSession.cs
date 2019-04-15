using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour {

    [Range(0.1f, 10f)][SerializeField] float gameSpeed = 1f;
    int pointsPerEnemyDestroyed;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI multText;
    [SerializeField] bool isAutoPlayEnabled;

    int multiplier;

    [SerializeField] int currentScore = 0;
    //int currentHealth = 0;

    private void Awake()
    {
        int gameStatusCount = FindObjectsOfType<GameSession>().Length;
        if (gameStatusCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        scoreText.text = currentScore.ToString();
        Application.targetFrameRate = 60;
        multiplier = 1;
        //healthText.text = currentHealth.ToString();
    }

    // Update is called once per frame
    void Update () {
		Time.timeScale = gameSpeed;
    }

    public int AddToScore(int points)
    {
        currentScore += points * multiplier;
        scoreText.text = currentScore.ToString();
        return points*multiplier;
    }

    public void AddToMult()
    {
        multiplier += 1;
        multText.text = "x" + multiplier.ToString();
    }

    public string GetScore()
    {
        return scoreText.text;
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }

    public bool IsAutoPlayEnabled()
    {
        return isAutoPlayEnabled;
    }
}
