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

    public float xMin;
    public float yMin;
    public float xMax;
    public float yMax;

    private void Awake()
    {
        Instance = this;
        enemyCount = 0;
        enemyDeathCount = 0;
    }

    private void Start()
    {
        spawnerSpawner.Init();
        EnemySetupBoundaries();
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

    public List<GameObject> GetNeighbors(GameObject enemy, float radius)
    {
        List<GameObject> neighborsFound = new List<GameObject>();

        foreach(var otherEnemy in enemies)
        {
            if (otherEnemy == enemy)
            {
                continue;
            }
            
            if(Vector3.Distance(enemy.transform.position, otherEnemy.transform.position) <= radius && otherEnemy.name == enemy.name)
            {
                neighborsFound.Add(otherEnemy);
            }
        }

        return neighborsFound;
    }

    public List<GameObject> EnemiesAlive()
    {
        return enemies;
    }

    private void EnemySetupBoundaries()
    {
        xMin = -49.15f;
        xMax = 48.15f;
        yMin = -49.15f;
        yMax = 48.15f;
    }
}
