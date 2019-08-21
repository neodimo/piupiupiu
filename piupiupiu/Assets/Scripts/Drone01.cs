using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone01 : MonoBehaviour
{

    Coroutine firingCoroutine;
    [SerializeField] float projectileSpeed;
    [SerializeField] float projectileFiringPeriod;
    [SerializeField] GameObject laserPrefab;
    Transform playerBody;
    Transform droneBody;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = Player.Instance.transform.GetChild(3);
        droneBody = Player.Instance.transform.GetChild(4).transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Player.Instance.transform.GetChild(3).transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, playerBody.transform.rotation, Time.deltaTime * 5);
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1") || Player.Instance.ProcessState() == "Dead")
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (Player.Instance.ProcessState() != "Dead")
        {
            GameObject laser = ObjectPooler.SharedInstance.GetPooledObject(laserPrefab, droneBody.transform.position, transform.rotation) as GameObject;//Instantiate(laserPrefab, gunMiddle.transform.position, body.transform.rotation) as GameObject;
            laser.transform.localScale = transform.localScale * 3;
            laser.transform.localScale = laser.transform.localScale * -1;
            //laser.transform.rotation = Quaternion.Euler(0, 0, 180);
            laser.GetComponent<Rigidbody2D>().velocity = -droneBody.transform.up * projectileSpeed;
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }
}
