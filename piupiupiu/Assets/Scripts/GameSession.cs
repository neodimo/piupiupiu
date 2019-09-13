using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using MoreMountains.NiceVibrations;
using System;

public class GameSession : MonoBehaviour {


    [Range(0f, 10f)][SerializeField] float gameSpeed = 1f;
    int pointsPerEnemyDestroyed;
    public static GameSession Instance;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI multText;
    [SerializeField] TextMeshProUGUI highScoreTitleText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] bool isAutoPlayEnabled;

    // PlayerData for save data
    [SerializeField] public int currentScore;
    public int highScore;
    public int multiplier;

    private void Awake()
    {
        int gameStatusCount = FindObjectsOfType<GameSession>().Length;
        if (gameStatusCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        MMVibrationManager.iOSInitializeHaptics();
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
        if (currentScore > highScore)
        {
            highScore = currentScore;
            highScoreText.text = highScore.ToString();
            highScoreText.text = currentScore.ToString();
        }
        
        //Score Text Viewability
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            scoreText.gameObject.SetActive(false);
            multText.gameObject.SetActive(false);
            if (highScoreText != null)
            {
                highScoreTitleText.gameObject.SetActive(true);
                highScoreText.gameObject.SetActive(true);
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            scoreText.gameObject.SetActive(true);
            multText.gameObject.SetActive(true);
            if (highScoreText != null)
            {
                highScoreTitleText.gameObject.SetActive(false);
                highScoreText.gameObject.SetActive(false);
            }
        }
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
        StartCoroutine(ResetCurrentScore());
        
        //Destroy(gameObject);
    }

    IEnumerator ResetCurrentScore()
    {
        yield return new WaitForSeconds(0.25f);
        currentScore = 0;
        scoreText.text = currentScore.ToString();
        multiplier = 1;
        multText.text = "x" + multiplier.ToString();
    }

    public bool IsAutoPlayEnabled()
    {
        return isAutoPlayEnabled;
    }
}
