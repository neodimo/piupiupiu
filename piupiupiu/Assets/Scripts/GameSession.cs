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

    Vector3 InStartHSTPos;
    Vector3 InStartHSPos;

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

        LoadScore();

        MMVibrationManager.iOSInitializeHaptics();
    }

    private void Start()
    {
        scoreText.text = String.Format("{0:n0}", currentScore);
        Application.targetFrameRate = 60;
        multiplier = 1;
        //healthText.text = currentHealth.ToString();

        InStartHSTPos = highScoreTitleText.gameObject.transform.position;
        InStartHSPos = highScoreText.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update () {
		Time.timeScale = gameSpeed;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            highScoreText.text = String.Format("{0:n0}", highScore);
        }
        
        //Score Text Viewability
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            scoreText.gameObject.SetActive(false);
            multText.gameObject.SetActive(false);
            if (highScoreText != null)
            {
                highScoreTitleText.gameObject.transform.position = InStartHSTPos;
                highScoreTitleText.fontSize = 50f;

                highScoreText.gameObject.transform.position = InStartHSPos;
                highScoreText.alignment = TextAlignmentOptions.Right;
                highScoreText.fontSize = 115f;

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
                // Set High Score Position When In Game
                Vector3 InGameHSTPos = new Vector3(InStartHSTPos.x-875, InStartHSTPos.y, InStartHSTPos.z);
                highScoreTitleText.gameObject.transform.position = InGameHSTPos;
                highScoreTitleText.fontSize = 35f;

                Vector3 InGameHSPos = new Vector3(InStartHSPos.x + 90, InStartHSPos.y, InStartHSPos.z);
                highScoreText.gameObject.transform.position = InGameHSPos;
                highScoreText.alignment = TextAlignmentOptions.Left;
                highScoreText.fontSize = 80f;

                //highScoreTitleText.gameObject.SetActive(false);
                //highScoreText.gameObject.SetActive(false);
            }
        }
    }

    public int AddToScore(int points)
    {
        currentScore += points * multiplier;
        scoreText.text = String.Format("{0:n0}", currentScore);
        return points*multiplier;
    }

    public void AddToMult()
    {
        multiplier += 1;
        multText.text = "x" + String.Format("{0:n0}", multiplier);
    }

    public int GetScore()
    {
        return currentScore;
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

    public void SaveScore()
    {
        SaveGame.SaveScore(this);
    }

    public void LoadScore()
    {
        ScoreData data;
        if (SaveGame.LoadScore() != null)
        {
            data = SaveGame.LoadScore();
        }
        else
        {
            SaveScore();
            Debug.Log("No save found, creating new one");
            data = SaveGame.LoadScore();
        }

        if (highScoreText != null)
        {
            highScore = data.highScore;
            highScoreText.text = String.Format("{0:n0}", highScore);
        }
    }

    private void OnApplicationQuit()
    {
        SaveScore();
    }
}
