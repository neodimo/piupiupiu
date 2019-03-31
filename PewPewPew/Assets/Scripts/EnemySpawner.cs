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
        StartCoroutine("WaitToEmit");
    }

    IEnumerator WaitToEmit()
    {
        yield return new WaitForSeconds(2);

        int enemyCount = UnityEngine.Random.Range(enemyCountMin, enemyCountMax);
        GameObject[] enemyPrefabs = new GameObject[enemyCount];
        for (int counter = 0; counter < enemyCount; counter++)
        {
            yield return new WaitForSeconds(.2f);
            float randOffsetX = UnityEngine.Random.Range(-3f, 3f);
            float randOffsetY = UnityEngine.Random.Range(-3f, 3f);
            Vector3 offsetPos = new Vector3(transform.position.x + randOffsetX, transform.position.y + randOffsetY, transform.position.z);

            enemyPrefabs[counter] = Instantiate(enemyPrefab, transform.position, Quaternion.identity) as GameObject; //Instantiate(enemyPrefab, offsetPos, Quaternion.identity) as GameObject;

            float randVelX = UnityEngine.Random.Range(-200f, 200f);
            float randVelY = UnityEngine.Random.Range(-200f, 200f);
            Vector2 enemyVel = new Vector2(randVelX, randVelY) * Time.deltaTime;
            enemyPrefabs[counter].GetComponent<Rigidbody2D>().velocity = enemyVel;
            Vector2 enemyVelNorm = enemyVel.normalized;
            var rotationDirection = 0;
            if (enemyVelNorm.x > 0)
            {
                rotationDirection = 1;
            }
            else
            {
                rotationDirection = -1;
            }
            enemyPrefabs[counter].GetComponent<Rigidbody2D>().angularVelocity = UnityEngine.Random.Range(360, 400) * rotationDirection;
        }
        Destroy(gameObject, 1f);
    }

    public void UpdateEnemySpawnCount(int EnemySpawnCount)
    {
        enemyCountMin += EnemySpawnCount - 5;
        enemyCountMax += EnemySpawnCount;
    }
}

//public enum EnemyType { Homing, Roming, Hectic }