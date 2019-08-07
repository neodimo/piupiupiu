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
    
    public string position;
    public Vector3 target;
    public float xOffset;
    public float yOffset;
    Vector3 originalPos;
    //Rigidbody2D enemyRB2D;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    
    private void OnEnable()
    {
        startTime = Time.time;
        randomStartTime = 1f;//UnityEngine.Random.Range(1f, 2f);
        firstGo = false;
        atTarget = false;
        originalPos = transform.position;


        if (gameObject.transform.position.x < -45) //if left
        {
            position = "left";
            xOffset = 92;
            yOffset = 0;
            transform.up = Vector3.right;
        }
        else if (gameObject.transform.position.x > 45) //if right
        {
            position = "right";
            xOffset = -92;
            yOffset = 0;
            transform.up = Vector3.left;
        }
        else if (gameObject.transform.position.y < -45) //if bottom
        {
            position = "bottom";
            xOffset = 0;
            yOffset = 92;
            transform.up = Vector3.up;
        }
        else if (gameObject.transform.position.y > 45) //if top right
        {
            position = "top";
            xOffset = 0;
            yOffset = -92;
            transform.up = Vector3.down;

        }

        /*
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
        else if(position == "right")
        {
            xOffset = -92;
            yOffset = 0;
        }
        else
        {
            xOffset = 0;
            yOffset = 0;
        }*/
        target = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
        //Debug.Log("ENABLE//// xOffset: " + xOffset + " - yOffset: " + yOffset + " - target: " + target + " - position: " + position);
        //enemyRB2D = gameObject.GetComponent<Rigidbody2D>();
    }

    /*
    private void Start()
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
        else if (position == "right")
        {
            xOffset = -92;
            yOffset = 0;
        }
        else
        {
            xOffset = 0;
            yOffset = 0;
        }
        target = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, transform.position.z);
        Debug.Log("START//// xOffset: " + xOffset + " - yOffset: " + yOffset + " - target: " + target + " - position: " + position);
    } */

    private void OnDisable()
    {
        position = "";
        xOffset = 0;
        yOffset = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        //Pythagoras Rule
        //distance = (float)Math.Sqrt(Math.Pow((playerPos[0] - transform.position.x), 2) + Math.Pow((playerPos[1] - transform.position.y), 2));

        timePastSinceAlive = Time.time - startTime;
        timePastSinceLastRedirection = Time.time - timeOfLastRedirection;


        //Debug.Log("xOffset: " + xOffset + " - yOffset: " + yOffset + " - target: " + target + " - position: " + position);
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
