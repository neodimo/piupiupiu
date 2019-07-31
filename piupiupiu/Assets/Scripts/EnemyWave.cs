using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] [Range(1, 600)] float homingSpeedMin = 12;
    [SerializeField] [Range(1, 600)] float homingSpeedMax = 15;
    float startTime;
    float timePastSinceAlive;
    float timePastSinceLastRedirection;
    float timeOfLastRedirection;
    float randomStartTime;
    bool firstGo;
    bool atTarget;
    float xOffset;
    float yOffset;
    public string position;
    Vector3 target;
    Vector3 originalPos;
    //Rigidbody2D enemyRB2D;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.time;
        randomStartTime = 1f;//UnityEngine.Random.Range(1f, 2f);
        firstGo = false;
        atTarget = false;
        originalPos = transform.position;

        if (position == "top")
        {
            xOffset = 0;
            yOffset = -92;
        }
        else if (position == "bottom")
        {
            xOffset = 0;
            yOffset = 92;
        }
        else if (position == "left")
        {
            xOffset = 92;
            yOffset = 0;
        }
        else
        {
            xOffset = -92;
            yOffset = 0;
        }
        target = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
        //enemyRB2D = gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        
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
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime*25);
                if (position == "top" || position == "bottom")
                {
                    if (transform.position.y == target.y || transform.position.y == -target.y)
                    {
                        target.y = -target.y;
                        transform.Rotate(0, 0, 180);
                    }
                }
                else if (position == "left" || position == "right")
                {
                    if (transform.position.x == target.x || transform.position.x == -target.x)
                    {
                        target.x = -target.x;
                        transform.Rotate(0, 0, 180);
                    }
                }
            }
            else if (!firstGo)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime*25);
                firstGo = true;
            }
        }
    }
}
