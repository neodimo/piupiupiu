using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnerPool;
    Vector2 playerPos;
    Player[] allPlayers;
    Player closestPlayerClass;
    string playerState;
    float timeSincePlayerDied;
    float startTime;
    float timeSinceStarted;
    float timeBetweenSpawners;
    int percentageBasedValue;
    bool spawning;
    [SerializeField] bool spawn = true;

    int[] weights;
    int weightTotal;

    void Awake()
    {
        weights = new int[SpawnerPool.Length]; //number of things

        weights[0] = 60;
        weights[1] = 40;

        weightTotal = 0;
        foreach (int w in weights)
        {
            weightTotal += w;
        }
    }

    int RandomWeighted()
    {
        int result = 0, total = 0;
        int randVal = Random.Range(0, weightTotal + 1);
        for (result = 0; result < weights.Length; result++)
        {
            total += weights[result];
            if (total >= randVal) break;
        }
        return result;
    }

    void Start()
    {
        startTime = Time.time;
        timeBetweenSpawners = 5f;
        spawning = true;
        allPlayers = GameObject.FindObjectsOfType<Player>();
        closestPlayerClass = FindObjectOfType<Player>();
        playerPos = FindClosestPlayer().transform.position;
        percentageBasedValue = RandomWeighted();
        if (spawn == true)
        {
            StartCoroutine(SpawnSpawners());
        }
    }

    void Update()
    {
        percentageBasedValue = RandomWeighted();
        playerState = closestPlayerClass.ProcessState();
    }

    IEnumerator SpawnSpawners()
    {
        yield return new WaitForSeconds(3);

        while (spawning)
        {
            while (playerState != "Sucking")
            {
                playerPos = FindClosestPlayer().transform.position;
                var randomPosFromPlayerRadiusX = Mathf.Clamp(UnityEngine.Random.Range(playerPos.x - 10, playerPos.x + 10), -40, 40);
                var randomPosFromPlayerRadiusY = Mathf.Clamp(UnityEngine.Random.Range(playerPos.y - 10, playerPos.y + 10), -40, 40);
                Vector2 randomPosFromPlayerRadius = new Vector2(randomPosFromPlayerRadiusX, randomPosFromPlayerRadiusY);
                var currentSpawner = Instantiate(SpawnerPool[percentageBasedValue], randomPosFromPlayerRadius, Quaternion.identity) as GameObject;
                timeSinceStarted = Time.time - startTime;
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
                    currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(20);
                }
                else if (timeSinceStarted > 90f)
                {
                    timeBetweenSpawners = 1f;
                    currentSpawner.GetComponent<EnemySpawner>().UpdateEnemySpawnCount(30);
                }
                yield return new WaitForSeconds(timeBetweenSpawners);
            }
            yield return new WaitForSeconds(6f);
        }
    }

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
}
