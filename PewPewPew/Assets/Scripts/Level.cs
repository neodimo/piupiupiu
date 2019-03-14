using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int enemyCount;
    //SceneLoader sceneLoader;
    EnemySpawnerOld enemySpawner;

    private void Start()
    {
        //sceneLoader = FindObjectOfType<SceneLoader>();
        enemySpawner = FindObjectOfType<EnemySpawnerOld>();
        CountEnemies();
    }

    public void CountEnemies()
    {
        enemyCount = enemySpawner.EnemyCount();
    }

    public void EnemyDestroyed()
    {
        enemyCount--;
        /*
        if (enemyCount <= 0)
        {
            sceneLoader.LoadNextScene();
        }
        */
    }
}
