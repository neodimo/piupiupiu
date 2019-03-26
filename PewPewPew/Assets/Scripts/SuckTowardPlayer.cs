using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckTowardPlayer : MonoBehaviour
{
    Player[] allPlayers;
    Vector2 playerPos;
    Player closestPlayerClass;
    GameObject closestPlayerObj;
    string playerState;
    [SerializeField] [Range(1, 100)] float homingSpeedMin = 12;
    [SerializeField] [Range(1, 100)] float homingSpeedMax = 15;
    float startTime;
    float timePast;
    float actualSpeed = 0;
    float accelleration = 6f;

    // Start is called before the first frame update
    void Start()
    {
        allPlayers = GameObject.FindObjectsOfType<Player>();
        closestPlayerClass = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        closestPlayerObj = FindClosestPlayer();
        playerState = closestPlayerClass.ProcessState();
        playerPos = FindClosestPlayer().transform.position;
        SuckToPlayer();
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

    private void SuckToPlayer()
    {
        var distanceToPlayer = playerPos - (new Vector2(transform.position.x, transform.position.y));
        if (playerState == "Sucking")
        {
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
}
