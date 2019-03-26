using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour//, IDragHandler//, IPointerDownHandler//, IPointerUpHandler
{

    [Header("Player")]
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
    private bool hasSuckedOnce = false;

    float sinceLastDashPress;
    float lastDashPress;
    public bool godMode;
    Toggle m_Toggle;
    [SerializeField] Joystick joystick;
    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = .5f;
    [Header("Effects")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosionSound;
    //[SerializeField] [Range(0, 1)] float shootingVolume = 0.2f;
    [SerializeField] [Range(0, 1)] float explosionVolume = 0.1f;
    [SerializeField] RuntimeAnimatorController[] statusSprites;

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

    VectorGridForce2 vectorGridForce;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    Enemy[] allEnemies;

    bool debugGodModeToggle;

    bool hasPressedSpace = false;

    float playerBaseScaleX;
    float playerBaseScaleY;

    Rigidbody2D rb2D;

    public Vector2 playerPos;

    //Children
    [SerializeField] GameObject body;
    [SerializeField] GameObject gun;

    // Use this for initialization
    void Start() {

        playerBaseScaleX = transform.localScale.x;
        playerBaseScaleY = transform.localScale.y;
        SetupBoundaries();
        allEnemies = GameObject.FindObjectsOfType<Enemy>();
        animator = GetComponent<Animator>();
        vectorGridForce = gameObject.GetComponent<VectorGridForce2>();

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

        godModeToggle = GameObject.FindGameObjectWithTag("GodModeToggle");
        rb2D = GetComponent<Rigidbody2D>();
        lastDashPress = Time.time - 5;
        canDash = true;
    }

    // Update is called once per frame
    void Update() {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Rotate(Move());
            Suck();
            Jump();
            ProcessDash();
        }
        Fire();
        ProcessState();
    }

    public String ProcessState()
    {
        if (godModeToggle.GetComponent<Toggle>().isOn)
        {
            godMode = true;
            if (isDashing)
            {
                animator.runtimeAnimatorController = statusSprites[2];
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
            else
            {
                animator.runtimeAnimatorController = statusSprites[1];
                vectorGridForce.m_ForceScale = .9f;
                vectorGridForce.m_Radius = .2f;
                return "Normal God";
            }
        }
        if (!godModeToggle.GetComponent<Toggle>().isOn)
        {
            godMode = false;
            if (isDashing)
            {
                animator.runtimeAnimatorController = statusSprites[2];
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
            else
            {
                animator.runtimeAnimatorController = statusSprites[0];
                vectorGridForce.m_ForceScale = .6f;
                vectorGridForce.m_Radius = .2f;
                return "Normal";
            }
        }
        else return "Null";
    }

    private void ProcessDash()
    {
        sinceLastDashPress = Time.time - lastDashPress;

        //KeyBoardInput
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            canDash = true;
        }
        if (Input.GetAxis("Horizontal") > 0 && canDash && !isDashing && dashSliderGauge.value > 0) // && sinceLastDashPress > dashCooldown)
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
        /*
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
        }*/

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
        //Debug.Log(dashDirection);
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
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            //float normalizedScale = Mathf.Abs(playerBaseScaleY - transform.localScale.y);
            //Vector3 transformOffset = new Vector3(transform.position.x, (transform.position.y+2)+(2*normalizedScale), transform.position.z);
            Vector3 gunTransform = transform.GetChild(0).GetChild(0).position;
            GameObject laser = Instantiate(laserPrefab, gun.transform.position, body.transform.rotation) as GameObject;
            laser.transform.localScale = transform.localScale*3;
            laser.GetComponent<Rigidbody2D>().velocity = body.transform.up * projectileSpeed;
            yield return new WaitForSeconds(projectileFiringPeriod);
        }

    }

    private Vector3 Move()
    {
        /*
        //x position
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * playerSpeed;
        var xPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);

        //y position
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * playerSpeed;
        var yPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        x
        transform.position = new Vector2(xPos, yPos);
        */

        moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);
        var deltaX = Vector3.right * Time.deltaTime * playerSpeed;
        var deltaY = Vector3.up * Time.deltaTime * playerSpeed;

        if (moveVector != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
            transform.Translate(moveVector * playerSpeed * Time.deltaTime, Space.World);
            var xPos = Mathf.Clamp(transform.position.x, xMin, xMax);
            var yPos = Mathf.Clamp(transform.position.y, yMin, yMax);

            transform.position = new Vector3(xPos, yPos, transform.position.z);
        }

        var direction = new Vector3(moveVector[0], moveVector[1], transform.position.z);
        return direction;
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
            }
            else
            {
                isSucking = false;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    public void Rotate(Vector3 direction)
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
        if (GameObject.FindObjectOfType<Enemy>())
        {
            Transform enemyTransform = FindClosestEnemy().transform;
            diff = enemyTransform.position - transform.position;
            body.transform.up = diff;
        }
        else
        {
            body.transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
        }
    }

    private void Jump()
    {
        //z position. this is faked with scaling.
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
        Enemy closestEnemy = null;
        allEnemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;
                //Debug.Log("Distance to Closest Enemy = " + distanceToClosestEnemy);
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
            Destroy(other.gameObject);
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
        damageDealer.Hit();
        if (health <= 0)
        {
            if (!godMode && !isDashing)
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

    public Vector2 GetPos()
    {
        return transform.position;
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position, explosionVolume);
        Destroy(explosion, 1f);
        FindObjectOfType<SceneLoader>().GameOver();
    }

    public void ToggleGodMode(bool value)
    {
        if (value == true)
        {
            //godMode = true;
            //GetComponent<Animator>().runtimeAnimatorController = statusSprites[2];
            Debug.Log("God Mode On");
        }
        if (value == false)
        {
            //godMode = false;
            //GetComponent<Animator>().runtimeAnimatorController = statusSprites[0];
            Debug.Log("God Mode Off");
        }
    }

    private void SetupBoundaries()
    {
        //Camera gameCamera = Camera.main;
        xMin = -49.15f; //gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = 49.15f; //gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = -49.15f; //gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = 49.15f; //gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }
}
