using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int enemyCountMin, enemyCountMax;
    GameObject enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EmitEnemies()
    {
        int multCount = UnityEngine.Random.Range(enemyCountMin, enemyCountMax);
        GameObject[] multPrefabs = new GameObject[multCount];

        for (int counter = 0; counter < multCount; counter++)
        {
            float randOffsetX = UnityEngine.Random.Range(-1f, 1f);
            float randOffsetY = UnityEngine.Random.Range(-1f, 1f);
            Vector3 offsetPos = new Vector3(transform.position.x + randOffsetX, transform.position.y + randOffsetY, transform.position.z);
            multPrefabs[counter] = Instantiate(enemyPrefab, offsetPos, Quaternion.identity) as GameObject;

            float randVelX = UnityEngine.Random.Range(-2f, 2f);
            float randVelY = UnityEngine.Random.Range(-2f, 2f);
            Vector2 multVel = new Vector2(randVelX, randVelY);
            multPrefabs[counter].GetComponent<Rigidbody2D>().velocity = multVel;
            Destroy(multPrefabs[counter], 4f);
        }
    }
}

public enum EnemyType { Homing, Roming, Hectic }