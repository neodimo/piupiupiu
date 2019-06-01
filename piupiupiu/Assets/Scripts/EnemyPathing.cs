using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        //Pythagoras Rule
        //distance = (float)Math.Sqrt(Math.Pow((playerPos[0] - transform.position.x), 2) + Math.Pow((playerPos[1] - transform.position.y), 2));

        timePastSinceAlive = Time.time - startTime;
        timePastSinceLastRedirection = Time.time - timeOfLastRedirection;

        if (timePastSinceAlive > randomStartTime)
        {
            if (firstGo && timePastSinceLastRedirection > 2.5f)
            {
                //GoToPlayer();
            }
            else if (!firstGo)
            {
                //GoToPlayer();
                firstGo = true;
            }
        }
    }
}
