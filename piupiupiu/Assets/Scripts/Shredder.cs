using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "Enemy")
        {
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
