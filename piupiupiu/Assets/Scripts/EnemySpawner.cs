using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int enemyCountMin, enemyCountMax;
    int enemyCount;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Sprite[] sprites;
    GameObject[] tiles;
    bool horizontal = true;
    string position;


    // Start is called before the first frame update
    void Start()
    {
        enemyCount = UnityEngine.Random.Range(enemyCountMin, enemyCountMax);
        EmitEnemies();
        if (enemyPrefab.name == "EnemyWave")
        {
            enemyCount = 6;
            tiles = new GameObject[enemyCount];
            float tileOffset = 0;
            for (int spriteCount = 0; spriteCount < enemyCount; spriteCount++)
            {
                Vector3 tilePos = new Vector3(transform.position.x + tileOffset, transform.position.y, transform.position.z);
                tiles[spriteCount] = Instantiate(tilePrefab, tilePos, Quaternion.identity) as GameObject;
                tiles[spriteCount].transform.parent = gameObject.transform;
                if (spriteCount == 0)
                {
                    tiles[spriteCount].GetComponent<SpriteRenderer>().sprite = sprites[0];
                }
                else if (spriteCount == enemyCount - 1)
                {
                    tiles[spriteCount].GetComponent<SpriteRenderer>().sprite = sprites[2];
                }
                else
                {
                    tiles[spriteCount].GetComponent<SpriteRenderer>().sprite = sprites[1];
                }
                tileOffset += 5;
            }
            if (Random.value < 0.5f)
            {
                horizontal = true;
            }
            else horizontal = false;

            if (gameObject.transform.position.x < 0 && gameObject.transform.position.y < 0) //if bottom left
            {
                if (!horizontal)
                {
                    gameObject.transform.Rotate(0, 0, -90);
                    transform.position = new Vector3(-46, -15, transform.position.z);
                    position = "left";
                }
                else
                {
                    transform.position = new Vector3(-42, -46, transform.position.z);
                    position = "bottom";
                }
            }
            else if (gameObject.transform.position.x < 0 && gameObject.transform.position.y > 0) //if top left
            {
                if (!horizontal)
                {
                    gameObject.transform.Rotate(0, 0, 90);
                    transform.position = new Vector3(-46, 15, transform.position.z);
                    position = "left";
                }
                else
                {
                    transform.position = new Vector3(-42, 46, transform.position.z);
                    position = "top";
                }
            }
            else if (gameObject.transform.position.x > 0 && gameObject.transform.position.y < 0) //if bottom right
            {
                if (!horizontal)
                {
                    gameObject.transform.Rotate(0, 0, -90);
                    transform.position = new Vector3(46, -15, transform.position.z);
                    position = "right";
                }
                else
                {
                    transform.position = new Vector3(15, -46, transform.position.z);
                    position = "bottom";
                }
            }
            else if (gameObject.transform.position.x > 0 && gameObject.transform.position.y > 0) //if top right
            {
                if (!horizontal)
                {
                    gameObject.transform.Rotate(0, 0, 90);
                    transform.position = new Vector3(46, 15, transform.position.z);
                    position = "right";
                }
                else
                {
                    transform.position = new Vector3(15, 46, transform.position.z);
                    position = "top";
                }
            }
        }
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

        //int enemyCount = UnityEngine.Random.Range(enemyCountMin, enemyCountMax);
        GameObject[] enemyPrefabs = new GameObject[enemyCount];
        float waveEnemyOffsetX = 0;
        float waveEnemyOffsetY = 0;
        for (int counter = 0; counter < enemyCount; counter++)
        {
            if (enemyPrefab.name == "EnemyHoming" || enemyPrefab.name == "EnemyRoaming")
            {
                yield return new WaitForSeconds(.2f);
                float randOffsetX = UnityEngine.Random.Range(-3f, 3f);
                float randOffsetY = UnityEngine.Random.Range(-3f, 3f);
                Vector3 offsetPos = new Vector3(transform.position.x + randOffsetX, transform.position.y + randOffsetY, transform.position.z); //implement maybe?

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
            else if (enemyPrefab.name == "EnemyWave")
            {
                yield return new WaitForSeconds(.05f);
                Vector3 enemyWavePos;
                if (transform.rotation.z == 0)
                {
                    enemyWavePos = new Vector3(transform.position.x + waveEnemyOffsetX, transform.position.y, transform.position.z);
                    waveEnemyOffsetX += 5;
                }
                else if (transform.rotation.z > 0)
                {
                    enemyWavePos = new Vector3(transform.position.x, transform.position.y + waveEnemyOffsetY, transform.position.z);
                    waveEnemyOffsetY += 5;
                }
                else
                {
                    enemyWavePos = new Vector3(transform.position.x, transform.position.y + waveEnemyOffsetY, transform.position.z);
                    waveEnemyOffsetY -= 5;
                }

                //Emit enemy.
                enemyPrefabs[counter] = Instantiate(enemyPrefab, enemyWavePos, Quaternion.identity) as GameObject;

                //Orient enemy direction.
                if (position == "top")
                {
                    enemyPrefabs[counter].transform.up = Vector3.down;
                    enemyPrefabs[counter].GetComponent<EnemyWave>().position = "top";
                }
                else if (position == "bottom")
                {
                    enemyPrefabs[counter].transform.up = Vector3.up;
                    enemyPrefabs[counter].GetComponent<EnemyWave>().position = "bottom";
                }
                else if (position == "left")
                {
                    enemyPrefabs[counter].transform.up = Vector3.right;
                    enemyPrefabs[counter].GetComponent<EnemyWave>().position = "left";
                }
                else
                {
                    enemyPrefabs[counter].transform.up = Vector3.left;
                    enemyPrefabs[counter].GetComponent<EnemyWave>().position = "right";
                }
            }
        }
        if (tilePrefab)
        {
            for (int counter = 0; counter < enemyCount; counter++)
            {
                Destroy(tiles[counter], 1f);
            }
        }
        Destroy(gameObject, 1f);
    }

    public void UpdateEnemySpawnCount(int EnemySpawnCount)
    {
        enemyCountMin += EnemySpawnCount - 5;
        enemyCountMax += EnemySpawnCount;
    }

    public int EnemyCount()
    {
        return enemyCount;
    }

    public string WaveEmitterPosition()
    {
        return position;
    }
}

//public enum EnemyType { Homing, Roming, Hectic }