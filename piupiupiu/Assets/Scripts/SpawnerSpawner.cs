using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSpawner : MonoBehaviour
{
    public static SpawnerSpawner Instance;

    [SerializeField] GameObject[] spawnerPool;
    Vector2 playerPos;
    //Player[] allPlayers;
    Player closestPlayerClass;
    string playerState;
    float timeSincePlayerDied;
    float startTime;
    float timeSinceStarted;
    [SerializeField] float timeBetweenSpawners;
    int percentageBasedValue;
    bool spawning;
    //bool horizontal = true;
    [SerializeField] bool spawn = true;
    [SerializeField] public bool demoMode = false;

    int[] weights;
    int weightTotal;

    void Awake()
    {
        weights = new int[spawnerPool.Length]; //number of things

        weights[0] = 25;
        weights[1] = 35;
        weights[2] = 20;
        weights[3] = 40;

        weightTotal = 0;
        foreach (int w in weights)
        {
            weightTotal += w;
        }

        Instance = this;
    }

    int RandomWeighted()
    {
        int result = 0, total = 0;
        int randVal = Random.Range(0, weightTotal + 1);
        if (weights != null)
        {
            for (result = 0; result < weights.Length; result++)
            {
                total += weights[result];
                if (total >= randVal) break;
            }
        }
        return result;
    }

    public void Init()
    {
        startTime = Time.time;
        spawning = true;
        //allPlayers = GameObject.FindObjectsOfType<Player>();
        //closestPlayerClass = FindObjectOfType<Player>();
        playerPos = Player.Instance.transform.position; //FindClosestPlayer().transform.position;
        percentageBasedValue = RandomWeighted();
        if (spawn == true)
        {
            StartCoroutine(SpawnSpawners());
        }
    }

    void Update()
    {
        
    }

    IEnumerator SpawnSpawners()
    {
        yield return new WaitForSeconds(3);

        while ((spawning && Level.Instance.enemies.Count < 200 && !demoMode) || (spawning && Level.Instance.enemies.Count < 30 && demoMode))
        {
            percentageBasedValue = RandomWeighted();
            playerState = Player.Instance.ProcessState();
            while (playerState != "Sucking")
            {
                percentageBasedValue = RandomWeighted();
                playerState = Player.Instance.ProcessState();
                playerPos = Player.Instance.transform.position; //FindClosestPlayer().transform.position;
                GameObject currentSpawner;
                if (spawnerPool[percentageBasedValue].name == "Enemy Spawner Homing" || spawnerPool[percentageBasedValue].name == "Enemy Spawner Roaming" || spawnerPool[percentageBasedValue].name == "Enemy Spawner Smart")
                {
                    float randomPosFromPlayerRadiusX;
                    float randomPosFromPlayerRadiusY;

                    if (!demoMode)
                    {
                        randomPosFromPlayerRadiusX = Mathf.Clamp(UnityEngine.Random.Range(playerPos.x - 10, playerPos.x + 10), -45, 45);
                        randomPosFromPlayerRadiusY = Mathf.Clamp(UnityEngine.Random.Range(playerPos.y - 10, playerPos.y + 10), -45, 45);
                    }
                    else
                    {
                        randomPosFromPlayerRadiusX = Mathf.Clamp(UnityEngine.Random.Range(playerPos.x - 40, playerPos.x + 40), -45, 45);
                        randomPosFromPlayerRadiusY = Mathf.Clamp(UnityEngine.Random.Range(playerPos.y - 40, playerPos.y + 40), -45, 45);
                    }
                    Vector2 randomPosFromPlayerRadius = new Vector2(randomPosFromPlayerRadiusX, randomPosFromPlayerRadiusY);
                    currentSpawner = Instantiate(spawnerPool[percentageBasedValue], randomPosFromPlayerRadius, Quaternion.identity) as GameObject;
                }
                else if (spawnerPool[percentageBasedValue].name == "Enemy Spawner Wave")
                {
                    var randomPosFromPlayerRadiusX = Mathf.Clamp(UnityEngine.Random.Range(playerPos.x - 20, playerPos.x + 20), -47, 30);
                    var randomPosFromPlayerRadiusY = Mathf.Clamp(UnityEngine.Random.Range(playerPos.y - 20, playerPos.y + 20), -47, 46.5f);
                    Vector2 randomPosFromPlayerRadius = new Vector2(randomPosFromPlayerRadiusX, randomPosFromPlayerRadiusY);
                    currentSpawner = Instantiate(spawnerPool[percentageBasedValue], randomPosFromPlayerRadius, Quaternion.identity) as GameObject;
                    var enemyCount = currentSpawner.GetComponent<EnemySpawner>().EnemyCount();

                }
                else
                {
                    float randomPosFromPlayerRadiusX;
                    float randomPosFromPlayerRadiusY;

                    if (!demoMode)
                    {
                        randomPosFromPlayerRadiusX = Mathf.Clamp(UnityEngine.Random.Range(playerPos.x - 10, playerPos.x + 10), -40, 40);
                        randomPosFromPlayerRadiusY = Mathf.Clamp(UnityEngine.Random.Range(playerPos.y - 10, playerPos.y + 10), -40, 40);
                    }
                    else
                    {
                        randomPosFromPlayerRadiusX = Mathf.Clamp(UnityEngine.Random.Range(playerPos.x - 40, playerPos.x + 40), -40, 40);
                        randomPosFromPlayerRadiusY = Mathf.Clamp(UnityEngine.Random.Range(playerPos.y - 40, playerPos.y + 40), -40, 40);
                    }

                    Vector2 randomPosFromPlayerRadius = new Vector2(randomPosFromPlayerRadiusX, randomPosFromPlayerRadiusY);
                    currentSpawner = Instantiate(spawnerPool[percentageBasedValue], randomPosFromPlayerRadius, Quaternion.identity) as GameObject;
                }

                timeSinceStarted = Time.time - startTime;
                if (demoMode)
                {
                    timeBetweenSpawners = 6f;
                    currentSpawner.GetComponent<EnemySpawner>().enemyCountMin = 1;
                    currentSpawner.GetComponent<EnemySpawner>().enemyCountMax = 2;
                }
                else
                {
                    if (timeSinceStarted < 30f)
                    {
                        timeBetweenSpawners = 5f;
                    }
                    else if (timeSinceStarted > 30f && timeSinceStarted < 60f)
                    {
                        timeBetweenSpawners = 3f;
                        currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(10);
                    }
                    else if (timeSinceStarted > 60f && timeSinceStarted < 90f)
                    {
                        timeBetweenSpawners = 2f;
                        currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(15);
                    }
                    else if (timeSinceStarted > 90f)
                    {
                        timeBetweenSpawners = 1f;
                        currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(20);
                    }
                    else if (timeSinceStarted > 120f)
                    {
                        timeBetweenSpawners = 1f;
                        currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(25);
                    }
                    else if (timeSinceStarted > 150f)
                    {
                        timeBetweenSpawners = 1f;
                        currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(30);
                    }
                }
                yield return new WaitForSeconds(timeBetweenSpawners);
            }
            yield return new WaitForSeconds(6f);
        }
    }

    /* 
     * ENABLE WHEN MAKING MULTIPLAYER
    private GameObject FindClosestPlayer()
    {
        float distanceToClosestPlayer = Mathf.Infinity;
        Player closestPlayer = null;
        if (allPlayers.Length == 0) { return this.gameObject; }
        foreach (Player currentPlayer in allPlayers)
        {
            float distanceToPlayer = (currentPlayer.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToPlayer < distanceToClosestPlayer)
            {
                distanceToClosestPlayer = distanceToPlayer;
                closestPlayer = currentPlayer;
            }
        }
        return closestPlayer.gameObject;
    }
    */
}
