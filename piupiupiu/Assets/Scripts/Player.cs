using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.NiceVibrations;

public class Player : MonoBehaviour//, IDragHandler//, IPointerDownHandler//, IPointerUpHandler
{
    public static Player Instance;
    Level level;
    [Header("Player")]
    public bool godMode;
    [SerializeField] bool canFire = true;
    [SerializeField] float playerSpeed = 20;
    //[SerializeField] float padding = 0;
    [SerializeField] float jumpSpeed = 0.1f;
    [SerializeField] float maxJumpHeight = 4f;
    [SerializeField] float health = 200f;
    [SerializeField] float dashSpeed = 20f;
    // Swipe Detection Vectors
    private Vector2 startPos;
    private Vector2 dashDirectionTouch;
    private Vector2 dashDirectionMouse;
    private bool directionChosen;
    private Vector2 lastPosition;
    public float minSwipeToDash = 2;
    private bool minSwipeToDashCheck = false;
    public float dashCooldown = 3f;

    private bool isDashing = false;
    private bool isSucking = false;
    private bool isFiring = false;
    private bool hasSuckedOnce = false;
    private bool isDead = false;

    float sinceLastDashPress;
    float sinceLastSuck;
    float timeofSuck;
    float lastDashPress;
    
    Toggle m_Toggle;
    [SerializeField] Joystick joystick;
    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = .5f;
    [Header("Effects")]
    [SerializeField] GameObject explosionPrefab;
    //[SerializeField] AudioClip explosionSound;
    //[SerializeField] [Range(0, 1)] float shootingVolume = 0.2f;
    //[SerializeField] [Range(0, 1)] float explosionVolume = 0.1f;
    //[SerializeField] RuntimeAnimatorController[] statusSprites;

    [Header("VectorGridForce")]
    [SerializeField]
    [Range(0, 1)]
    float defaultScale = .6f;
    [SerializeField]
    [Range(0, 1)]
    float defaultRadius = .2f;

    GameObject[] sliders;
    Slider suckSliderGauge;
    Slider dashSliderGauge;
    bool canDash;
    bool isTouchInput = false;
    bool isMouseInput = false;

    Vector2 mouseStartPos;
    Vector2 mouseEndPos;
    Vector2 currentMousePos;

    Vector3 moveVector;

    Coroutine firingCoroutine;
    Coroutine suckingCoroutine;

    GameObject godModeToggle;

    Animator animator;

    SpriteRenderer spriteRenderer;

    CircleCollider2D circleCollider2D;

    VectorGridForce2 vectorGridForce;

    string colliderType = "2d";

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    List<GameObject> allEnemies = new List<GameObject>();
    public GameObject closestEnemyFound;
    //Enemy[] allEnemies;

    bool debugGodModeToggle;

    bool hasPressedSpace = false;
    bool canMove;

    float playerBaseScaleX;
    float playerBaseScaleY;

    Rigidbody2D rb2D;

    public Vector2 playerPos;

    public String playerState;

    //Children
    [SerializeField] GameObject body;
    [SerializeField] GameObject gunMiddle;
    [SerializeField] GameObject gunLeft01;
    [SerializeField] GameObject gunLeft02;
    [SerializeField] GameObject gunRight01;
    [SerializeField] GameObject gunRight02;
    GameObject[] guns;

    GameSession gameSession;
    string score;

    int axisCounter = 0;
    bool startShooting = false;
    bool stopShooting = false;

    bool isMouse = false;
    bool isController = false;

    //
    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            canMove = true;
        }

        playerBaseScaleX = transform.localScale.x;
        playerBaseScaleY = transform.localScale.y;
        SetupBoundaries();
        allEnemies = Level.Instance.EnemiesAlive(); //GameObject.FindObjectsOfType<Enemy>();
        animator = GetComponent<Animator>();
        vectorGridForce = gameObject.GetComponent<VectorGridForce2>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        gameSession = GameSession.Instance;

        sliders = GameObject.FindGameObjectsWithTag("Slider");

        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i].name == "SuckSlider")
            {
                suckSliderGauge = sliders[i].GetComponent<Slider>();
            }
            else if (sliders[i].name == "DashSlider") 
            {
                dashSliderGauge = sliders[i].GetComponent<Slider>();
            }
        }

        if (GameObject.FindGameObjectWithTag("GodModeToggle") != null)
        {
            godModeToggle = GameObject.FindGameObjectWithTag("GodModeToggle");
        }
        else
        {
            godModeToggle = null;
        }
        rb2D = GetComponent<Rigidbody2D>();
        lastDashPress = Time.time - 5;
        canDash = true;

        guns = new GameObject[] {gunMiddle, gunLeft01, gunRight01};
    }

    // Update is called once per frame
    void Update() {
        playerState = ProcessState();
        //Debug.Log("r_horizontal = " + Input.GetAxis("Horizontal_R") + " r_vertical = " + Input.GetAxis("Vertical_R"));
        if (isDead == false)
        {
            if (canMove)
            {
                if (Level.Instance.EnemyCount() > 0)
                {
                    closestEnemyFound = FindClosestEnemy();
                }
                Move();
                Rotate();
                Suck();
                Jump();
                //ProcessDash();
            }
            score = gameSession.GetScore();
            Fire();
            
        }
        
        //Debug.Log(playerState);
    }

    public String ProcessState()
    {
        if (godModeToggle != null)
        {
            if (godMode)
            {
                //godMode = true;
                if (isDashing)
                {
                    //animator.runtimeAnimatorController = statusSprites[2];
                    vectorGridForce.m_ForceScale = 1.5f;
                    vectorGridForce.m_Radius = .3f;
                    return "Dashing God";
                }
                else if (isSucking)
                {
                    vectorGridForce.m_ForceScale = -.5f;
                    vectorGridForce.m_Radius = .4f;
                    return "Sucking God";
                }
                else if (isFiring)
                {
                    vectorGridForce.m_ForceScale = -.5f;
                    vectorGridForce.m_Radius = .4f;
                    return "Firing God";
                }
                else
                {
                    //animator.runtimeAnimatorController = statusSprites[1];
                    vectorGridForce.m_ForceScale = .9f;
                    vectorGridForce.m_Radius = .2f;
                    return "Normal God";
                }

            }
            else if (!godMode)
            {
                //godMode = false;
                if (isDashing)
                {
                    //animator.runtimeAnimatorController = statusSprites[2];
                    vectorGridForce.m_ForceScale = 1.3f;
                    vectorGridForce.m_Radius = .3f;
                    return "Dashing";
                }
                else if (isSucking)
                {
                    vectorGridForce.m_ForceScale = -.2f;
                    vectorGridForce.m_Radius = .4f;
                    return "Sucking";
                }
                else if (isDead)
                {
                    StartCoroutine(SendOutWave());
                    return "Dead";
                }
                else if (isFiring)
                {
                    return "Firing";
                }
                else
                {
                    //animator.runtimeAnimatorController = statusSprites[0];
                    vectorGridForce.m_ForceScale = .6f;
                    vectorGridForce.m_Radius = .2f;
                    return "Normal";
                }
            }
            else return "Normal";
        }
        else if (godModeToggle == null)
        {

            if (isDashing)
            {
                //animator.runtimeAnimatorController = statusSprites[2];
                vectorGridForce.m_ForceScale = 1.3f;
                vectorGridForce.m_Radius = .3f;
                return "Dashing";
            }
            else if (isSucking)
            {
                vectorGridForce.m_ForceScale = -.2f;
                vectorGridForce.m_Radius = .4f;
                return "Sucking";
            }
            else if (isDead)
            {
                StartCoroutine(SendOutWave());
                return "Dead";
            }
            else if (isFiring)
            {
                vectorGridForce.m_ForceScale = defaultScale; //.6f;
                vectorGridForce.m_Radius = defaultRadius;
                return "Firing";
            }
            else
            {
                //animator.runtimeAnimatorController = statusSprites[0];
                vectorGridForce.m_ForceScale = defaultScale; //.6f;
                vectorGridForce.m_Radius = defaultRadius; //.2f;
                return "Normal";
            }
        }
        else return "Normal";
    }

    IEnumerator SendOutWave()
    {
        vectorGridForce.m_ForceScale = 2f;
        vectorGridForce.m_Radius = .25f;
        yield return new WaitForSeconds(.2f);
        vectorGridForce.m_ForceScale = 0f;
        vectorGridForce.m_Radius = 0f;
    }

    private void ProcessDash()
    {
        sinceLastDashPress = Time.time - lastDashPress;

        //KeyBoardInput
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            canDash = true;
        }
        if (Input.GetAxis("Horizontal") > 0 && canDash && !isDashing && dashSliderGauge.value > 0)
        {
            canDash = false;
            StartCoroutine(Dash(new Vector2(dashSliderGauge.value*1, 0)));
            lastDashPress = Time.time;
            directionChosen = true;
        }
        else if (Input.GetAxis("Horizontal") < 0 && canDash && !isDashing && dashSliderGauge.value > 0)
        {
            canDash = false;
            StartCoroutine(Dash(new Vector2(dashSliderGauge.value*-1, 0)));
            lastDashPress = Time.time;
            directionChosen = true;
        }
        else if (Input.GetAxis("Vertical") > 0 && canDash && !isDashing && dashSliderGauge.value > 0)
        {
            canDash = false;
            StartCoroutine(Dash(new Vector2(0, dashSliderGauge.value*1)));
            lastDashPress = Time.time;
            directionChosen = true;
        }
        else if (Input.GetAxis("Vertical") < 0 && canDash && !isDashing && dashSliderGauge.value > 0)
        {
            canDash = false;
            StartCoroutine(Dash(new Vector2(0, dashSliderGauge.value*- 1)));
            lastDashPress = Time.time;
            directionChosen = true;
        }

        //TouchInput
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    directionChosen = false;
                    break;

                case TouchPhase.Moved:
                    dashDirectionTouch = touch.position - startPos;
                    if (Math.Abs(touch.position.x - startPos.x) > minSwipeToDash || Math.Abs(touch.position.y - startPos.y) > minSwipeToDash)
                    {
                        if (dashSliderGauge.value > 0)
                        {
                            minSwipeToDashCheck = true;
                            isTouchInput = true;
                            directionChosen = true;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    isTouchInput = true;
                    directionChosen = true;
                    break;
            }
        }

        //MouseInput
        else if (Input.mousePresent)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseStartPos = Input.mousePosition;
                directionChosen = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mouseEndPos = Input.mousePosition;
                dashDirectionMouse = mouseEndPos - mouseStartPos;
            }

            if (Math.Abs(dashDirectionMouse.x) > minSwipeToDash || Math.Abs(dashDirectionMouse.y) > minSwipeToDash)
            {
                directionChosen = true;
                if (dashSliderGauge.value > 0)
                {
                    minSwipeToDashCheck = true;
                }
            }
            else
            {
                directionChosen = false;
            }
            isMouseInput = true;
        }

        if (minSwipeToDashCheck)
        {
            minSwipeToDashCheck = false;
            Debug.Log("directionChosen = " + directionChosen);
            if (directionChosen)
            {
                if (dashSliderGauge.value > 0)
                {
                    if (isTouchInput)
                    {
                        StartCoroutine(Dash(dashDirectionTouch));
                        dashDirectionTouch = Vector2.zero;
                        Debug.Log("You are touching!");
                    }
                    else if (isMouseInput)
                    {
                        StartCoroutine(Dash(dashDirectionMouse));
                        dashDirectionMouse = Vector2.zero;
                        Debug.Log("You are clicking!");
                    }
                }
            }
        }
        else
        {
            dashSliderGauge.value += .5f*Time.deltaTime;
        }

    }

    IEnumerator Dash(Vector2 dashDirection)
    {
        isDashing = true;
        lastDashPress = Time.time;
        if (isTouchInput)
        {
            rb2D.velocity = dashDirection.normalized * dashSpeed;
        }
        else
        {
            rb2D.velocity = dashDirection.normalized * dashSpeed;
        }

        dashSliderGauge.value = 0;
        yield return new WaitForSeconds(1f);
        isDashing = false;
        directionChosen = false;
        isTouchInput = false;
    }

    private void Fire()
    {
        if (canFire == true)
        {
            

            if ((Input.GetAxis("Vertical_R") > 0.2f || Input.GetAxis("Vertical_R") < -0.2f) && axisCounter == 0)
            {
                if (Input.GetAxis("Horizontal_R") < 0.2f && Input.GetAxis("Horizontal_R") > -0.2f)
                {
                    startShooting = true;
                }
                axisCounter++;
            }
            else if ((Input.GetAxis("Horizontal_R") > 0.2f || Input.GetAxis("Horizontal_R") < -0.2f) && axisCounter == 0)
            {
                if (Input.GetAxis("Vertical_R") < 0.2f && Input.GetAxis("Vertical_R") > -0.2f)
                {
                    startShooting = true;
                }
                axisCounter++;
            }
            else if ((Input.GetAxis("Horizontal_R") < 0.2f && Input.GetAxis("Horizontal_R") > -0.2f) && (Input.GetAxis("Vertical_R") < 0.2f && Input.GetAxis("Vertical_R") > -0.2f))
            {
                if (axisCounter > 0)
                {
                    stopShooting = true;
                }
                startShooting = false;
                axisCounter = 0;
            }
            /*
            else
            {
                Debug.Log("not in range of axis checks");
                stopShooting = true;
                startShooting = false;
                axisCounter = 0;
            }
            */
            if (Input.GetButtonDown("Fire1") || startShooting == true)
            {
                isFiring = true;
                firingCoroutine = StartCoroutine(FireContinuously());
                startShooting = false;
            }
            if (Input.GetButtonUp("Fire1") || stopShooting == true || isDead)
            {
                StopCoroutine(firingCoroutine);
                isFiring = false;
                stopShooting = false;
            }
        }
    }

    IEnumerator FireContinuously()
    {
        while (!isDead)
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            if (Convert.ToInt32(score) >= 0 && Convert.ToInt32(score) < 9999)
            {
                GameObject laser = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject;
                laser.transform.localScale = transform.localScale * 3;
                laser.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                yield return new WaitForSeconds(projectileFiringPeriod);
            }
            if (Convert.ToInt32(score) > 9999 && Convert.ToInt32(score) < 199999)
            {
                GameObject laserMiddle = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject;
                laserMiddle.transform.localScale = transform.localScale * 3;
                laserMiddle.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                GameObject laserLeft01 = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunLeft01.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunLeft01.transform.position, body.transform.rotation) as GameObject;
                laserLeft01.transform.localScale = transform.localScale * 3;
                laserLeft01.transform.up = body.transform.up / 2;
                laserLeft01.GetComponent<Rigidbody2D>().velocity = laserLeft01.transform.up * projectileSpeed;
                GameObject laserRight01 = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunRight01.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunRight01.transform.position, body.transform.rotation) as GameObject;
                laserRight01.transform.localScale = transform.localScale * 3;
                laserRight01.transform.up = body.transform.up * 2;
                laserRight01.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                yield return new WaitForSeconds(projectileFiringPeriod/5);
            }
            if (Convert.ToInt32(score) > 199999)
            {
                GameObject laserMiddle = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject; //Instantiate(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject;
                laserMiddle.transform.localScale = transform.localScale * 3;
                laserMiddle.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                GameObject laserLeft01 = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunLeft01.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunLeft01.transform.position, body.transform.rotation) as GameObject;
                laserLeft01.transform.localScale = transform.localScale * 3;
                laserLeft01.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                GameObject laserLeft02 = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunLeft02.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunLeft02.transform.position, body.transform.rotation) as GameObject;
                laserLeft02.transform.localScale = transform.localScale * 3;
                laserLeft02.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                GameObject laserRight01 = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunRight01.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunRight01.transform.position, body.transform.rotation) as GameObject;
                laserRight01.transform.localScale = transform.localScale * 3;
                laserRight01.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                GameObject laserRight02 = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, gunRight02.transform.position, body.transform.rotation) as GameObject;//Instantiate(laserPrefab, gunRight02.transform.position, body.transform.rotation) as GameObject;
                laserRight02.transform.localScale = transform.localScale * 3;
                laserRight02.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
                yield return new WaitForSeconds(projectileFiringPeriod / 5);
            }
        }

    }

    void Move()
    {
        if (joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            isMouse = false;
            isController = true;
            moveVector = (Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical"));
        }
        else
        {
            isController = false;
            isMouse = true;
            moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);
        }
        
        //var deltaX = Vector3.right * Time.deltaTime * playerSpeed;
        //var deltaY = Vector3.up * Time.deltaTime * playerSpeed;

        if (moveVector != Vector3.zero)
        {
            transform.Translate(moveVector * playerSpeed * Time.deltaTime, Space.World);
            var xPos = Mathf.Clamp(transform.position.x, xMin, xMax);
            var yPos = Mathf.Clamp(transform.position.y, yMin, yMax);

            transform.position = new Vector3(xPos, yPos, transform.position.z);
        }

        //var direction = new Vector3(moveVector[0], moveVector[1], transform.position.z);
        //return direction;
    }

    private void Suck()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            if (suckSliderGauge.value > 0)
            {
                hasSuckedOnce = true;
                suckingCoroutine = StartCoroutine(SuckContinuously());
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (hasSuckedOnce==true)
            {
                StopCoroutine(suckingCoroutine);
            }
            isSucking = false;
        }
    }

    IEnumerator SuckContinuously()
    {
        while (true)
        {
            if (suckSliderGauge.value > 0)
            {
                isSucking = true;
                suckSliderGauge.value -= .35f;
                timeofSuck = Time.time;
            }
            else
            {
                isSucking = false;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    public void Rotate()
    {
        /* Trig Answer
        // get the angle
        Vector3 norTar = (enemyTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(norTar.y, norTar.x) * Mathf.Rad2Deg;
        // rotate to angle
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0, 0, angle - 90);
        transform.rotation = rotation;
        */

        //Simple Answer
        Vector3 diff;

        //Rotation is pointed directly at the closest enemy.
        if (isMouse)
        {
            if (Level.Instance.EnemyCount() > 0)
            {
                Transform enemyTransform = closestEnemyFound.transform;
                diff = enemyTransform.position - transform.position;
                body.transform.up = diff;
            }
            else if (moveVector != Vector3.zero)
            {
                body.transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
            }
        }
        else if (isController)
        {
            Vector3 moveVectorController = new Vector3(Input.GetAxis("Horizontal_R"), -Input.GetAxis("Vertical_R"), 0);
            if (moveVectorController != Vector3.zero)
            {
                body.transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVectorController);
            }
        }

        //body.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(Input.GetAxis("Horizontal_R"), -Input.GetAxis("Vertical_R"), 0));
    }

    private void Jump()
    {
        //JUMP!
        float zScaleX;
        float zScaleY;

        if (Input.GetKey(KeyCode.Space))
        {
            zScaleX = Mathf.Clamp(transform.localScale.x + (jumpSpeed * Time.deltaTime), playerBaseScaleX, maxJumpHeight);
            zScaleY = Mathf.Clamp(transform.localScale.y + (jumpSpeed * Time.deltaTime), playerBaseScaleY, maxJumpHeight);
            hasPressedSpace = true;
        }
        else if (hasPressedSpace)
        {
            zScaleX = Mathf.Clamp(transform.localScale.x - (jumpSpeed * Time.deltaTime), playerBaseScaleX, maxJumpHeight);
            zScaleY = Mathf.Clamp(transform.localScale.y - (jumpSpeed * Time.deltaTime), playerBaseScaleY, maxJumpHeight);
        }
        else
        {
            zScaleX = playerBaseScaleX;
            zScaleY = playerBaseScaleY;
        }

        transform.localScale = new Vector2(zScaleX, zScaleY);
    }

    public GameObject FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        
        GameObject closestEnemy = null;
        allEnemies =  Level.Instance.EnemiesAlive(); //GameObject.FindObjectsOfType<Enemy>(); 
        foreach (GameObject currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;
            }
        }
        return closestEnemy.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            ProcessHit(damageDealer);
        }
        if (other.gameObject.tag == "Multiplier")
        {
            ProcessMultiplier();
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }

    private void ProcessMultiplier()
    {
        FindObjectOfType<GameSession>().AddToMult();
        suckSliderGauge.value += .03f;
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit(colliderType);
        sinceLastSuck = Time.time - timeofSuck;
        if (health <= 0)
        {
            if (!godMode && !isDashing && !isSucking && sinceLastSuck > 3)
            {
                Die();
            }
            else
            {
                return;
            }
        }
    }

    public float GetHealth()
    {
        return health;
    }

    private void Die()
    {
        spriteRenderer.enabled = false;
        isDead = true;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        explosion.transform.up = Vector3.forward;
        //AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionVolume);
        Destroy(explosion, 1f);
        FindObjectOfType<SceneLoader>().GameOver();
        circleCollider2D.enabled = false;
    }

    public void ToggleGodMode(bool value)
    {
        if (value == true)
        {
            Debug.Log("God Mode On");
        }
        if (value == false)
        {
            Debug.Log("God Mode Off");
        }
    }

    private void SetupBoundaries()
    {
        xMin = -49.15f;
        xMax = 48.15f;
        yMin = -49.15f;
        yMax = 48.15f;
    }
}
