using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int enemyCount;
    int enemyDeathCount;
    //SceneLoader sceneLoader;
    EnemySpawner enemySpawner;

    private void Awake()
    {
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
