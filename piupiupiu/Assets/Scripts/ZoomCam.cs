using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCam : MonoBehaviour
{
    public float speed;
    float posX;
    float posY;
    float posZ;
    bool switchFlag = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (switchFlag == false)
        {
            posZ = Mathf.Lerp(-100f, -250f, Mathf.PingPong((0.5f*Time.time), 1));
            if (posZ <= -250f)
            {
                switchFlag = true;
            }
        }
        else if (switchFlag == true)
        {
            posZ = Mathf.Lerp(-250f, -100f, Mathf.PingPong((0.5f * Time.time), 1));
            if (posZ >= -100f)
            {
                switchFlag = false;
            }
        }
        Debug.Log(0.5f * Time.time);
        transform.position = new Vector3(transform.position.x, transform.position.y, posZ);
    }
}
