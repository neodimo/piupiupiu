using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    Player[] allPlayers;
    Vector2 playerPos;
    float distance;
    [SerializeField] [Range(1, 30)] float homingSpeedMin = 12;
    [SerializeField] [Range(1, 30)] float homingSpeedMax = 15;
    float startTime;
    float timePast;
    float actualSpeed = 0;
    float accelleration = 2f;

    private void Awake()
    {
        startTime = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        allPlayers = GameObject.FindObjectsOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = FindClosestPlayer().transform.position;

        //Pythagoras Rule
        //distance = (float)Math.Sqrt(Math.Pow((playerPos[0] - transform.position.x), 2) + Math.Pow((playerPos[1] - transform.position.y), 2));

        timePast = Time.time - startTime;
        if (timePast > .5)
        {
            distance = Vector2.Distance(transform.position, playerPos);
            GoToPlayer();
        }
    }

    private void GoToPlayer()
    {
        float randSpeed = UnityEngine.Random.Range(homingSpeedMin, homingSpeedMax);

        if (actualSpeed < randSpeed)
        {
            actualSpeed += accelleration;
            transform.position = Vector2.MoveTowards(transform.position, playerPos, actualSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPos, randSpeed * Time.deltaTime);
        }
    }

    private void SlowToStop()
    {
        if (actualSpeed != 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPos, actualSpeed * Time.deltaTime);
            actualSpeed -= accelleration;
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
