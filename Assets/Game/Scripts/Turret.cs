using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject shotPrefab;

    public int maxNumberOfShots = 5;

    public float timeToShoot = 5f;

    private UIManager uiManager;
    private Transform playerPos;

    private float fireRate = 0.2f;
    private float currentTimeToShoot = 0f;
    private float currentTime = 0f;

    private int currentNumberOfShots = 0;

    private bool canShoot = false;


    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!uiManager.GameIsPaused)
        {
            Shot();
        }   
    }

    private void Shot()
    {
        if (canShoot)
        {
            currentTimeToShoot += Time.deltaTime;

            if (currentTimeToShoot >= fireRate)
            {
                currentTimeToShoot = 0f;
                currentNumberOfShots++;
                Instantiate(shotPrefab, transform.position, Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f));
            }
            
            if (currentNumberOfShots >= maxNumberOfShots)
            {
                canShoot = false;
                currentNumberOfShots = 0;
            }
        }
        else
        {
            currentTime += Time.deltaTime;

            if (currentTime >= timeToShoot)
            {
                currentTime = 0f;
                canShoot = true;
            }
        }
    }

    private bool PlayerIsNear()
    {
        float difference = transform.position.z - playerPos.transform.position.z;

        if (difference <= 80f && difference >= -10f)
        {
            return true;
        }

        return false;
    }
}
