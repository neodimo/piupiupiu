using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowPlayer : MonoBehaviour
{
    GameObject player;
    Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            player = Player.Instance.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            playerPos = player.transform.position;
            transform.position = new Vector3(playerPos.x*.8f, (playerPos.y*.8f), transform.position.z);
        }
    }
}
