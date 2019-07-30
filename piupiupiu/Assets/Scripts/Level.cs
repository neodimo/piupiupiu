using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public SpawnerSpawner spawnerSpawner;
    public static Level Instance;
    public int enemyCount;
    int enemyDeathCount;
    public List<GameObject> enemies = new List<GameObject>();
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
        spawnerSpawner.Init();
        //sceneLoader = FindObjectOfType<SceneLoader>();
    }

    public void AddToEnemyCount(GameObject enemy)
    {
        enemies.Add(enemy);
        enemyCount = enemies.Count;
    }

    public void EnemyDestroyed(GameObject enemy)
    {
        //enemyCount--;
        enemyDeathCount++;
        enemies.Remove(enemy);
        enemyCount = enemies.Count;
        /*
        if (enemyCount <= 0)
        {
            sceneLoader.LoadNextScene();
        }
        */
    }

    public int EnemyCount()
    {
        return enemyCount;
    }

    public List<GameObject> EnemiesAlive()
    {
        return enemies;
    }
}
