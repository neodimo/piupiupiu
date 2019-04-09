using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    public float speed;
    public string axis = "z";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (axis == "x")
        {
            transform.Rotate(Vector3.left, speed * Time.deltaTime);
        }
        else if (axis == "y")
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}
