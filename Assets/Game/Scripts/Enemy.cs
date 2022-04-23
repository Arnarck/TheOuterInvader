using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyBullet;
    public GameObject explosionPrefab;

    public float speed = 7.5f;
    public float timeToShoot = 120f;

    public int hitPoints = 1;

    private Transform playerPos;
    private UIManager uiManager;
    private AudioSource audioSource;

    private int currentTime;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("SoundEffects");
        audioSource.Play();

        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (this.CompareTag("Stronger_Enemy"))
        {
            timeToShoot *= 1.5f;
            speed /= 1.5f;
            hitPoints = 2;
        }
        else if (this.CompareTag("Faster_Enemy"))
        {
            timeToShoot /= 1.5f;
            speed *= 1.5f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!uiManager.GameIsPaused)
        {
            Shot();

            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

            if (playerPos != null && playerPos.transform.position.z > transform.position.z + 20f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Shot()
    {
        currentTime++;
        if (currentTime >= timeToShoot)
        {
            currentTime = 0;
            Instantiate(enemyBullet, transform.position, Quaternion.Euler(0, 0, 0));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BigBangArea"))
        {
            if (this.CompareTag("Basic_Enemy"))
            {
                uiManager.UpdateScore(100);
            }
            else if (this.CompareTag("Faster_Enemy"))
            {
                uiManager.UpdateScore(150);
            }
            else if (this.CompareTag("Stronger_Enemy"))
            {
                uiManager.UpdateScore(200);
            }

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
