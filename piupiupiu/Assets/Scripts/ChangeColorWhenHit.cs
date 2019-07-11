using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorWhenHit : MonoBehaviour
{
    int hitCount;

    // Start is called before the first frame update
    void Start()
    {
        hitCount = 4;
        //gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCount == 4)
        {
            var currentColor = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            Debug.Log(currentColor);
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor * .6f);
            hitCount--;
        }
        else if (hitCount == 3)
        {
            var currentColor = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            Debug.Log(currentColor);
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor * .6f);
            hitCount--;
        }
        else if (hitCount == 2)
        {
            var currentColor = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            Debug.Log(currentColor);
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor * .6f);
            hitCount--;
        }
        else if (hitCount == 1)
        {
            var currentColor = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            Debug.Log(currentColor);
            gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor * .6f);
            hitCount--;
        }
    }
}
