using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAISmart : MonoBehaviour
{
    //

    Player[] allPlayers;
    Vector2 playerPos;
    //float distance;
    float startTime;
    float timePast;
    float actualSpeed = 0;

    Vector3 wanderTarget;
    public Vector3 acceleration;
    public Vector3 velocity;
    FlockAISmartBrain brain;

    private void Awake()
    {
        startTime = Time.time;
    }

    void Start()
    {
        brain = FindObjectOfType<FlockAISmartBrain>();
        velocity = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
    }

    void OnEnable()
    {

    }

    void Update()
    {
        playerPos = Player.Instance.transform.position;

        timePast = Time.time - startTime;
        if (timePast > .5)
        {
            acceleration = Combine();
            acceleration = Vector3.ClampMagnitude(acceleration, brain.maxAcceleration);
            velocity = velocity + acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, brain.maxVelocity);
            if (Level.Instance.EnemyCount() > 0)
            {
                if (Player.Instance.closestEnemyFound == gameObject || Player.Instance.ProcessState() == "Firing" || Player.Instance.ProcessState() == "Firing God")
                {
                    velocity = velocity * 3;
                }
            }
            
            transform.position = transform.position + velocity * Time.deltaTime;
            float xPos = Mathf.Clamp(transform.position.x, Level.Instance.xMin, Level.Instance.xMax);
            float yPos = Mathf.Clamp(transform.position.y, Level.Instance.yMin, Level.Instance.yMax);

            transform.position = new Vector3(xPos, yPos, transform.position.z);

        }
    }

    virtual protected Vector3 Combine()
    {
        float avoidancePriority = brain.avoidancePriority;
        float separationPriority = brain.separationPriority;
        if (Level.Instance.EnemyCount() > 0)
        {
            if (Player.Instance.closestEnemyFound == gameObject || Player.Instance.ProcessState() == "Firing" || Player.Instance.ProcessState() == "Firing God")
            {
                avoidancePriority = brain.avoidancePriority * 1000;
                //separationPriority = brain.separationPriority * 6000;
            }
        }

        Vector3 finalVec = brain.cohesionPriority * Cohesion() + brain.toPlayerPriority * GoToPlayer()
            + brain.alignmentPriority * Alignment() + brain.separationPriority * Separation()
            + avoidancePriority * Avoidance();

        /*
         * Vector3 finalVec = brain.cohesionPriority * Cohesion() + brain.wanderPriority * Wander()
            + brain.alignmentPriority * Alignment() + brain.separationPriority * Separation()
            + brain.avoidancePriority * Avoidance();
        */
        return finalVec;
    }

    protected Vector3 Wander()
    {
        float jitter = brain.wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(RandomBinomial() * jitter, RandomBinomial() * jitter, 0);
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= brain.wanderRadius;
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(-brain.wanderDistance, brain.wanderDistance, 0);
        targetInLocalSpace -= transform.position;
        return targetInLocalSpace.normalized;
    }

    private Vector3 GoToPlayer()
    {
        Vector3 towardPlayer = new Vector3();
        towardPlayer = Player.Instance.transform.position - transform.position;
        towardPlayer = Vector3.Normalize(towardPlayer);
        return towardPlayer;
    }

    Vector3 Cohesion()
    {
        Vector3 cohesionVector = new Vector3();
        int countEnemies = 0;
        var neighbors = Level.Instance.GetNeighbors(gameObject, brain.cohesionRadius);
        if (neighbors.Count == 0)
            return cohesionVector;
        foreach (var enemy in neighbors)
        {
            if (isInFOV(enemy.transform.position))
            {
                cohesionVector += enemy.transform.position;
                countEnemies++;
            }
        }

        if(countEnemies == 0)
        {
            return cohesionVector;
        }

        cohesionVector /= countEnemies;
        cohesionVector = cohesionVector - transform.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    Vector3 Alignment()
    {
        Vector3 alignVector = new Vector3();
        var neighbors = Level.Instance.GetNeighbors(gameObject, brain.cohesionRadius);
        if (neighbors.Count == 0)
            return alignVector;

        foreach (var enemy in neighbors)
        {
            if (isInFOV(enemy.transform.position))
            {
                alignVector += enemy.GetComponent<FlockAISmart>().velocity;
            }
        }

        return alignVector.normalized;
    }

    Vector3 Separation()
    {
        Vector3 separateVector = new Vector3();
        var neighbors = Level.Instance.GetNeighbors(gameObject, brain.cohesionRadius);
        if (neighbors.Count == 0)
            return separateVector;

        foreach (var enemy in neighbors)
        {
            if (isInFOV(enemy.transform.position))
            {
                Vector3 movingTowards = transform.position - enemy.transform.position;
                if (movingTowards.magnitude > 0)
                {
                    separateVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }

        return separateVector.normalized;
    }

    Vector3 Avoidance()
    {
        Vector3 avoidVector = new Vector3();
        float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
        if (distance > brain.avoidanceRadius)
            return avoidVector;

        avoidVector += RunAway(Player.Instance.transform.position);

        return avoidVector.normalized;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (transform.position - target).normalized * brain.maxVelocity;
        return neededVelocity - velocity;
    }

    bool isInFOV(Vector3 vec)
    {
        return Vector3.Angle(velocity, vec - transform.position) <= brain.maxFOV;
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

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }
}
