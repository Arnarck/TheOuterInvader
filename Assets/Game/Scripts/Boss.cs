//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject shotPrefab;
    public GameObject missilePrefab;
    public GameObject explosionPrefab;
    public GameObject shieldDestroyedPrefab;

    public float timeToMove = 10f;
    public float timeToShot = 3f;
    public float timeToLaunchMissile = 6f;

    public int numberOfShots = 16;
    public int numberOfMissiles = 8;

    private Player playerRef;
    private Rigidbody rigidBody;
    private UIManager uiManager;
    private AudioSource audioSource;

    private int hitPoints = 100;
    private int currentNumberOfShots = 0;
    private int currentNumberOfMissiles = 0;

    private bool canMove = false;
    private bool canShot = false;
    private bool canLaunchMissile = false;

    private float currentTime_Movement = 0;
    private float currentTime_Missile = 0;
    private float currentTime_MissileRate = 0;
    private float currentTime_Shot = 0;
    private float currentTime_FireRate = 0;
    private float fireRate = 0.25f;
    private float missileRate = 1f;

    private Vector3 newPos;
    private Vector3 previouslyPos;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("SoundEffects");
        audioSource.Play();

        playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        previouslyPos = transform.position;
        uiManager.ActivateBossHealth(hitPoints);
    }

    // Update is called once per frame
    void Update()
    {
        if (!uiManager.GameIsPaused)
        {
            Shot();
            Launch_Missile();
            Movement();
        }
        
    }

    private void FixedUpdate()
    {
        if (!uiManager.GameIsPaused)
        {
            Movement();
        }
    }

    private void Shot()
    {
        if (canShot)
        {

            currentTime_FireRate += Time.deltaTime;

            if (currentTime_FireRate >= fireRate)
            {
                currentNumberOfShots++;
                currentTime_FireRate = 0f;
                Instantiate(shotPrefab, transform.position, Quaternion.identity);
            }

            if (currentNumberOfShots >= numberOfShots)
            {
                currentNumberOfShots = 0;
                canShot = false;
            }
        }
        else
        {
            currentTime_Shot += Time.deltaTime;

            if (currentTime_Shot >= timeToShot)
            {
                currentTime_Shot = 0f;
                canShot = true;
            }
        }
    }

    private void Launch_Missile()
    {
        if (canLaunchMissile)
        {
            currentTime_MissileRate += Time.deltaTime;

            if (currentTime_MissileRate >= missileRate)
            {
                Instantiate(missilePrefab, transform.GetChild(currentNumberOfMissiles).transform.position, Quaternion.Euler(0f, 180f, 0f));
                currentNumberOfMissiles++;
                currentTime_MissileRate = 0f;
            }

            if (currentNumberOfMissiles >= numberOfMissiles)
            {
                currentNumberOfMissiles = 0;
                canLaunchMissile = false;
            }
        }
        else
        {
            currentTime_Missile += Time.deltaTime;

            if (currentTime_Missile >= timeToLaunchMissile)
            {
                currentTime_Missile = 0f;
                canLaunchMissile = true;
            }
        }
    }

    private void Movement()
    {
        if (canMove && playerRef != null)
        {
            float xPos = Random.Range(playerRef.xMin, playerRef.xMax);
            float yPos = Random.Range(playerRef.yMin, playerRef.yMax);
            newPos = new Vector3(xPos, yPos, 0f);

            if (newPos.y < previouslyPos.y)
            {
                newPos.y = -newPos.y;
            }

            rigidBody.velocity = newPos;
            previouslyPos = newPos;
            canMove = false;
        }
        else
        {
            currentTime_Movement += Time.deltaTime;

            if (currentTime_Movement >= timeToMove)
            {
                canMove = true;
                currentTime_Movement = 0f;
            }
        }

        rigidBody.position = new Vector3
        (
            Mathf.Clamp(rigidBody.position.x, playerRef.xMin, playerRef.xMax),
            Mathf.Clamp(rigidBody.position.y, playerRef.yMin + 5f, playerRef.yMax),
            rigidBody.position.z
        );
    }

    public void LoseLife(int hits)
    {
        hitPoints -= hits;

        if (hitPoints <= 0)
        {
            hitPoints = 0;
            uiManager.UpdateScore(500);
            uiManager.SlowMotion(this.tag);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        uiManager.UpdateBossHealth(hitPoints);
    }
}
