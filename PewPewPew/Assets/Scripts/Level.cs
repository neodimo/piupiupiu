using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    int enemyCount;
    //SceneLoader sceneLoader;
    EnemySpawner enemySpawner;

    private void Start()
    {
        //sceneLoader = FindObjectOfType<SceneLoader>();
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
