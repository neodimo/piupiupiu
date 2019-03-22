using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoam : MonoBehaviour
{
    Player[] allPlayers;
    Vector3 playerPos;
    float distance;
    [SerializeField] [Range(1, 30)] float homingSpeedMin = 12;
    [SerializeField] [Range(1, 30)] float homingSpeedMax = 15;
    float startTime;
    float timePastSinceAlive;
    float timePastSinceLastRedirection;
    float timeOfLastRedirection;
    float actualSpeed = 0;
    float accelleration = 2f;
    float randomStartTime;
    bool firstGo;

    private void Awake()
    {
        startTime = Time.time;
        randomStartTime = UnityEngine.Random.Range(1f, 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        firstGo = false;
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
        //var deltaX = playerPos.x - transform.position.x;
        //var deltaY = playerPos.y - transform.position.y;
        Vector3 direction = playerPos - transform.position;
        var randomSpeed = UnityEngine.Random.Range(400, 550);
        gameObject.GetComponent<Rigidbody2D>().velocity = direction.normalized * Time.deltaTime * randomSpeed;
        //Debug.Log("deltaX: " + deltaX + "deltaY: " + deltaY);
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
