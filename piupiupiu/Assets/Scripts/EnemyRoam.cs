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
    Vector3 direction;

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
        StartCoroutine(ChangeDirection());
        //allPlayers = GameObject.FindObjectsOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = Player.Instance.transform.position; //FindClosestPlayer().transform.position;

        //Pythagoras Rule
        //distance = (float)Math.Sqrt(Math.Pow((playerPos[0] - transform.position.x), 2) + Math.Pow((playerPos[1] - transform.position.y), 2));

        timePastSinceAlive = Time.time - startTime;
        timePastSinceLastRedirection = Time.time - timeOfLastRedirection;
        /*
        if (timePastSinceAlive > randomStartTime)
        {
            if (firstGo && timePastSinceLastRedirection > 0.1f)
            {
                GoToPlayer();
            }
            else if (!firstGo)
            {
                GoToPlayer();
                firstGo = true;
            }
        }
        */

        if (timePastSinceAlive > randomStartTime)
        {
            MoveTowardDirection(direction);
        }
    }

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            direction = playerPos - transform.position;
            yield return new WaitForSeconds(3f);
        }
    }

    private void MoveTowardDirection(Vector3 direction)
    {
        var randomSpeed = UnityEngine.Random.Range(homingSpeedMin, homingSpeedMax);
        //enemyRB2D.velocity = direction.normalized * Time.deltaTime * randomSpeed *2;
        Vector2 enemyVelNorm = enemyRB2D.velocity.normalized;
        float step = randomSpeed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, step);
        //var magnitude = direction.magnitude;
        var magNormalized = direction.normalized;
        //Debug.Log("magNormalized: " + magNormalized);
        transform.position = new Vector3((transform.position.x) + magNormalized.x * step, (transform.position.y) + magNormalized.y * step, transform.position.z);
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
