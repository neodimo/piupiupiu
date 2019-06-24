using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Instance;
    int enemyCount;
    int enemyDeathCount;
    //SceneLoader sceneLoader;
    EnemySpawner enemySpawner;

    private void Awake()
    {
        Instance = this;
        enemyCount = 0;
        enemyDeathCount = 0;
    }

    private void Start()
    {
        //sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public void AddToEnemyCount()
    {
        enemyCount++;
    }

    public void EnemyDestroyed()
    {
        enemyCount--;
        enemyDeathCount++;
        /*
        if (enemyCount <= 0)
        {
            sceneLoader.LoadNextScene();
        }
        */
    }
}
