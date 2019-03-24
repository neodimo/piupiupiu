using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoam : MonoBehaviour
{
    Player[] allPlayers;
    Vector3 playerPos;
    float distance;
    [SerializeField] [Range(1, 600)] float homingSpeedMin = 12;
    [SerializeField] [Range(1, 600)] float homingSpeedMax = 15;
    float startTime;
    float timePastSinceAlive;
    float timePastSinceLastRedirection;
    float timeOfLastRedirection;
    float randomStartTime;
    bool firstGo;
    Rigidbody2D enemyRB2D;

    private void Awake()
    {
        startTime = Time.time;
        randomStartTime = UnityEngine.Random.Range(1f, 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        firstGo = false;
        enemyRB2D = gameObject.GetComponent<Rigidbody2D>();
        allPlayers = GameObject.FindObjectsOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = FindClosestPlayer().transform.position;

        //Pythagoras Rule
        //distance = (float)Math.Sqrt(Math.Pow((playerPos[0] - transform.position.x), 2) + Math.Pow((playerPos[1] - transform.position.y), 2));

        timePastSinceAlive = Time.time - startTime;
        timePastSinceLastRedirection = Time.time - timeOfLastRedirection;

        if (timePastSinceAlive > randomStartTime)
        {
            if (firstGo && timePastSinceLastRedirection > 2.5f)
            {
                GoToPlayer();
            }
            else if (!firstGo)
            {
                GoToPlayer();
                firstGo = true;
            }
        }
    }

    private void GoToPlayer()
    {
        Vector3 direction = playerPos - transform.position;
        var randomSpeed = UnityEngine.Random.Range(homingSpeedMin, homingSpeedMax);
        enemyRB2D.velocity = direction.normalized * Time.deltaTime * randomSpeed;
        Vector2 enemyVelNorm = enemyRB2D.velocity.normalized;
        var rotationDirection = 0;
        if (enemyVelNorm.x < 0)
        {
            rotationDirection = 1;
        }
        else
        {
            rotationDirection = -1;
        }
        enemyRB2D.angularVelocity = UnityEngine.Random.Range(360, 400) * rotationDirection;
        timeOfLastRedirection = Time.time;
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
