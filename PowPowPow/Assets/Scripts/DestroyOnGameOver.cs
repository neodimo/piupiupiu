using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnGameOver : MonoBehaviour
{
    GameObject player;
    Player closestPlayerClass;
    string playerState;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            player = FindObjectOfType<Player>().gameObject;
            closestPlayerClass = FindObjectOfType<Player>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerState = closestPlayerClass.ProcessState();
        if (playerState == "Dead")
        {
            Destroy(gameObject);
        }
    }
}
