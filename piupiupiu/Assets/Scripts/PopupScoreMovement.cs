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
        var newPosition = Vector3.MoveTowards(transform.position, Camera.main.transform.position, Time.deltaTime * 300f);
        transform.position = new Vector3(newPosition.x, newPosition.y + (10 * Time.deltaTime), newPosition.z);
    }
}
