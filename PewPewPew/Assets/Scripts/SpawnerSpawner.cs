using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] SpawnerPool;
    Vector2 playerPos;
    Player[] allPlayers;
    float timeSincePlayerDied;
    float startTime;
    float timeSinceStarted;
    float timeBetweenSpawners;
    int percentageBasedValue;
    bool spawning;

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
        percentageBasedValue = RandomWeighted();
        StartCoroutine(SpawnSpawners());
    }

    void Update()
    {
        percentageBasedValue = RandomWeighted();
    }

    IEnumerator SpawnSpawners()
    {
        yield return new WaitForSeconds(3);

        while (spawning)
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
            }
            else if (timeSinceStarted > 60f && timeSinceStarted < 90f)
            {
                timeBetweenSpawners = 2f;
            }
            yield return new WaitForSeconds(timeBetweenSpawners);
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
