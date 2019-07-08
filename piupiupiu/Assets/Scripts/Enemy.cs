using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.CGF.Systems.ObjectTransform;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] float health = 100;
    [SerializeField] int points = 5;
    //FOR SHOOTING
    /*
    //float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 5f;
    */
    [Header("Death Effects")]
    [SerializeField] GameObject explosionPrefab;
    //[SerializeField] AudioClip explosionSound;
    //[SerializeField] AudioClip enemyShootingSound;
    //[SerializeField] [Range(0, 1)] float shootingVolume = 0.05f;
    //[SerializeField] [Range(0, 1)] float explosionVolume = 0.1f;
    [Header("Multiplier")]
    [SerializeField] GameObject multPrefab;
    [SerializeField] [Range(1, 20)] int multCountMin = 1;
    [SerializeField] [Range(1, 20)] int multCountMax = 5;
    [Header("Points Pop Up")]
    [SerializeField] GameObject popUpText;

    int currentPoints;
    bool dealerIsPlayerBody;
    Player closestPlayerClass;
    string playerState;

    //List<GameObject> popUpList;

    Level level;

    Coroutine firingCoroutine;

    private void Awake()
    {
        level = Level.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(CheckForLevel());

        Level.Instance.AddToEnemyCount(gameObject);
        closestPlayerClass = Player.Instance; //FindObjectOfType<Player>();
        //popUpList = new List<GameObject>();

        //FOR SHOOTING
        //shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    //
    IEnumerator CheckForLevel()
    {
        yield return new WaitForSeconds(.01f);

        // check for it
        if (level == null)
        {
            level = Level.Instance;
        }

        // add
        if (level != null)
        {
            level.AddToEnemyCount(gameObject);
        }
        else
        {
            StartCoroutine(CheckForLevel());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //FOR SHOOTING
        //CountDownAndShoot();
        playerState = closestPlayerClass.ProcessState();
    }

    //FOR SHOOTING
    /*
    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }
    

    private void Fire()
    {
        Vector3 transformOffset = new Vector3(transform.position.x, (transform.position.y - 1.5f), transform.position.z);
        Quaternion enemyRotation = transform.rotation;
        GameObject laser = Instantiate(laserPrefab, transformOffset, enemyRotation) as GameObject;
        AudioSource.PlayClipAtPoint(enemyShootingSound, Camera.main.transform.position, shootingVolume);
        //TODO orient forward vector
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
    }
    */

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (other.gameObject.tag == "Player")
        {
            dealerIsPlayerBody = true;
        }
        else dealerIsPlayerBody = false;
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (other.gameObject.tag == "Player")
        {
            dealerIsPlayerBody = true;
        }
        else dealerIsPlayerBody = false;
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            GameObject explosion = CGFObjectPoolingManager.Instance.InstantiatePoolObject(explosionPrefab, transform.position, explosionPrefab.transform.rotation) as GameObject; //Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
            if (!dealerIsPlayerBody && playerState != "Sucking")
            {
                EmitMultipliers();
            }
            //AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionVolume);
            currentPoints = GameSession.Instance.AddToScore(points);  //FindObjectOfType<GameSession>().AddToScore(points);
            PopUpPoints();
            //Destroy(gameObject);
            Level.Instance.EnemyDestroyed(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void EmitMultipliers()
    {
        int multCount = UnityEngine.Random.Range(multCountMin, multCountMax);
        GameObject[] multPrefabs = new GameObject[multCount];

        for (int counter = 0; counter < multCount; counter++)
        {
            float randOffsetX = UnityEngine.Random.Range(-1f, 1f);
            float randOffsetY = UnityEngine.Random.Range(-1f, 1f);
            Vector3 offsetPos = new Vector3(transform.position.x + randOffsetX, transform.position.y + randOffsetY, transform.position.z);
            multPrefabs[counter] = CGFObjectPoolingManager.Instance.InstantiatePoolObject(multPrefab, offsetPos, Quaternion.identity);
            //Instantiate(multPrefab, offsetPos, Quaternion.identity) as GameObject;

            float randVelX = UnityEngine.Random.Range(-2f, 2f);
            float randVelY = UnityEngine.Random.Range(-2f, 2f);
            Vector2 multVel = new Vector2(randVelX, randVelY);
            multPrefabs[counter].GetComponent<Rigidbody2D>().velocity = multVel;
        }
    }

    private void PopUpPoints()
    {
        GameObject textClone;
        textClone = CGFObjectPoolingManager.Instance.InstantiatePoolObject(popUpText, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity) as GameObject; //Instantiate(popUpText, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);

        textClone.GetComponent<TextMeshPro>().text = "+" + currentPoints.ToString();

        //Destroy(textClone, .5f);
    }
}
