using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int enemyCountMin, enemyCountMax;
    [SerializeField] GameObject enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        EmitEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EmitEnemies()
    {
        int enemyCount = UnityEngine.Random.Range(enemyCountMin, enemyCountMax);
        GameObject[] enemyPrefabs = new GameObject[enemyCount];

        for (int counter = 0; counter < enemyCount; counter++)
        {
            float randOffsetX = UnityEngine.Random.Range(-3f, 3f);
            float randOffsetY = UnityEngine.Random.Range(-3f, 3f);
            Vector3 offsetPos = new Vector3(transform.position.x + randOffsetX, transform.position.y + randOffsetY, transform.position.z);
            enemyPrefabs[counter] = Instantiate(enemyPrefab, offsetPos, Quaternion.identity) as GameObject;

            float randVelX = UnityEngine.Random.Range(-7f, 7f);
            float randVelY = UnityEngine.Random.Range(-7f, 7f);
            Vector2 enemyVel = new Vector2(randVelX, randVelY);
            enemyPrefabs[counter].GetComponent<Rigidbody2D>().velocity = enemyVel;
        }
    }
}

//public enum EnemyType { Homing, Roming, Hectic }