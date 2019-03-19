using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoam : MonoBehaviour
{
    Player[] allPlayers;
    Vector2 playerPos;
    float distance;
    [SerializeField] [Range(1, 30)] float homingSpeedMin = 12;
    [SerializeField] [Range(1, 30)] float homingSpeedMax = 15;
    float startTime;
    float timePastSinceAlive;
    float timePastSinceLastRedirection;
    float timeOfLastRedirection;
    float actualSpeed = 0;
    float accelleration = 2f;

    private void Awake()
    {
        startTime = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        timePastSinceLastRedirection = 0;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = FindClosestPlayer().transform.position;

        //Pythagoras Rule
        //distance = (float)Math.Sqrt(Math.Pow((playerPos[0] - transform.position.x), 2) + Math.Pow((playerPos[1] - transform.position.y), 2));

        timePastSinceAlive = Time.time - startTime;
        timePastSinceLastRedirection = Time.time - timeOfLastRedirection;
        Debug.Log("Time Past Since Last Redirection: " + timePastSinceLastRedirection);
        Debug.Log("Hello");
        if (timePastSinceAlive > .5)
        {
            if (timePastSinceLastRedirection > 3)
            {
                distance = Vector2.Distance(transform.position, playerPos);
                GoToPlayer();
            }
        }
    }

    private void GoToPlayer()
    {
        var deltaX = playerPos.x - transform.position.x;
        var deltaY = playerPos.y - transform.position.y;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(deltaX, deltaY) * Time.deltaTime * 5;
        timeOfLastRedirection = Time.time;
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
        allPlayers = GameObject.FindObjectsOfType<Player>();
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
