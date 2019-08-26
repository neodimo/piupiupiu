using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineToClosestEnemy : MonoBehaviour
{
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Level.Instance.EnemyCount() > 0 && Player.Instance.closestEnemyFound != null) //(GameObject.FindObjectOfType<Enemy>())
        {
            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, Player.Instance.closestEnemyFound.transform.position);
            //Debug.DrawRay(gameObject.transform.position, Player.Instance.closestEnemyFound.transform.position - gameObject.transform.position);
        }
        else
        {
            lr.enabled = false;
        }
    }
}
