using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using MoreMountains.NiceVibrations;
using System;
using UnityEngine.iOS;
using UnityEngine.UI;

public class GameSession : MonoBehaviour {
    public static GameSession Instance;

    [Range(0f, 10f)][SerializeField] float gameSpeed = 1f;
    [SerializeField] bool isAutoPlayEnabled;

    [Header("Main Menu UI")]
    [SerializeField] TextMeshProUGUI highScoreTitleTextStartMenu;
    [SerializeField] TextMeshProUGUI highScoreTextStartMenu;

    [Header("Main Level UI")]
    [SerializeField] TextMeshProUGUI highScoreTitleTextMainLevel;
    [SerializeField] TextMeshProUGUI highScoreTextMainLevel;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI multText;

    [Header("ExpBar")]
    [SerializeField] GameObject expBarObjNonIphoneX;
    [SerializeField] GameObject expBarObjIphoneX;
    [SerializeField] TextMeshProUGUI expText;
    [SerializeField] TextMeshProUGUI levelText;
    Image expBarImageComponent;
    float expBarPercentage = 0;
    [SerializeField] Sprite[] expBarSprites;


    [Header("Player Data")]
    // PlayerData for save data
    [SerializeField] public int currentScore;
    public int highScore;
    public int multiplier;
    public int level;
    int[] levelEXP = new int[1000];
    public float exp;

    public static DeviceGeneration generation;
    bool GoodForIphoneXAndOn;

    //Vector3 InStartHSTPos;
    //Vector3 InStartHSPos;

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
        //LoadExpAndLvl();
        level = 1;

        CheckDeviceCompatability();

        if (GoodForIphoneXAndOn)
        {
            MMVibrationManager.iOSInitializeHaptics();
        }
    }

    private void CheckDeviceCompatability()
    {
        if (generation.ToString() == "iPhone11Pro" || generation.ToString() == "iPhone11" || generation.ToString() == "iPhone11ProMax"
            || generation.ToString() == "iPhoneXS" || generation.ToString() == "iPhoneXSMax" || generation.ToString() == "iPhoneXR" || generation.ToString() == "iPhoneX")
        {
            GoodForIphoneXAndOn = true;
        }
    }

    private void Start()
    {
        scoreText.text = String.Format("{0:n0}", currentScore);
        Application.targetFrameRate = 60;
        multiplier = 1;

        ExperiencePerLevel();
        expText.text = exp + " / " + levelEXP[level];

        expBarImageComponent = expBarObjIphoneX.GetComponent<Image>();
        expBarImageComponent.overrideSprite = expBarSprites[(int)expBarPercentage];

        //healthText.text = currentHealth.ToString();

        //InStartHSTPos = highScoreTitleTextStartMenu.gameObject.transform.position;
        //InStartHSPos = highScoreTextStartMenu.gameObject.transform.position;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start Menu")
        {
            scoreText.gameObject.SetActive(false);
            multText.gameObject.SetActive(false);
            highScoreTitleTextMainLevel.gameObject.SetActive(false);
            highScoreTextMainLevel.gameObject.SetActive(false);

            highScoreTitleTextStartMenu.gameObject.SetActive(true);
            highScoreTextStartMenu.gameObject.SetActive(true);
        }
        else if (scene.name == "Main Level")
        {
            highScoreTitleTextStartMenu.gameObject.SetActive(false);
            highScoreTextStartMenu.gameObject.SetActive(false);

            highScoreTitleTextMainLevel.gameObject.SetActive(true);
            highScoreTextMainLevel.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            multText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update () {
		Time.timeScale = gameSpeed;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            highScoreTextStartMenu.text = String.Format("{0:n0}", highScore);
            highScoreTextMainLevel.text = String.Format("{0:n0}", highScore);
        }
        
    }

    public void ExperiencePerLevel()
    {
        //after each level increase experience requirements. lvl 1 = 100 lvl 2 = 150 etc
        levelEXP[0] = 50;
        for (int i = 1; i < levelEXP.Length; i++) {
            levelEXP[i] = levelEXP[i - 1] * 2; //(Convert.ToInt32((100*i)/Math.Pow(i, i)));
        }
    }

    public void AddOneExperience()
    {
        expBarPercentage += 1;
        expBarImageComponent.overrideSprite = expBarSprites[(int)expBarPercentage];
    }

    public void AddExperience(int experience)
    {
        exp += experience;
        if (exp > levelEXP[level])
        {
            level += 1;
            levelText.text = level.ToString();
        }
        expBarPercentage = (exp / levelEXP[level])*100;
        expText.text = exp + " / " + levelEXP[level];
        expBarImageComponent.overrideSprite = expBarSprites[(int)expBarPercentage];
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

        if (highScoreTextStartMenu != null)
        {
            highScore = data.highScore;
            highScoreTextStartMenu.text = String.Format("{0:n0}", highScore);
            highScoreTextMainLevel.text = String.Format("{0:n0}", highScore);
        }
    }

    private void OnApplicationQuit()
    {
        SaveScore();
    }
}
