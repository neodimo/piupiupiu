using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInNOut : MonoBehaviour
{
    public float speed;
    [SerializeField] float zoomAmount;
    float scaleAnim;
    float scaleInitX;
    bool switchFlag = false;


    // Start is called before the first frame update
    void Start()
    {
        scaleInitX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (switchFlag == false)
        {
            scaleAnim = Mathf.SmoothStep(scaleInitX, scaleInitX + zoomAmount, Mathf.PingPong((speed * Time.time), 1));
            //scaleAnim = Mathf.Lerp(scaleInitX, scaleInitX + zoomAmount, Mathf.PingPong((speed*Time.time), 1));
            if (scaleAnim >= scaleInitX + zoomAmount)
            {
                switchFlag = true;
            }
        }
        else if (switchFlag == true)
        {
            scaleAnim = Mathf.SmoothStep(scaleInitX + zoomAmount, scaleInitX, Mathf.PingPong((speed * Time.time), 1));
            //scaleAnim = Mathf.Lerp(scaleInitX+zoomAmount, scaleInitX, Mathf.PingPong((speed * Time.time), 1));
            if (scaleAnim <= scaleInitX)
            {
                switchFlag = false;
            }
        }
        //Debug.Log(0.5f * Time.time);
        transform.localScale = new Vector3(scaleAnim, scaleAnim, transform.localScale.z);
    }
}
