using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupScoreMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, Time.deltaTime * 300f);
    }
}
