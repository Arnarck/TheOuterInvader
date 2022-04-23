using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bullet : MonoBehaviour
{
    public GameObject particlePrefab;
    public GameObject explosionPrefab;
    public AudioSource soundEffect;

    public float speed = 150f;
    public float timeToDestroy = 2;

    private SpawnManager spawnManager;
    private UIManager uiManager;
    private Rigidbody rigidBody;
    private Player playerRef;

    private int hitPoints = 1;

    // Start is called before the first frame update
    void Start()
    {
        soundEffect.volume = PlayerPrefs.GetFloat("SoundEffects");
        soundEffect.Play();

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();

        speed = spawnManager.playerBulletSpeed;
        Destroy(this.gameObject, timeToDestroy);

        if (playerRef != null)
        {
            hitPoints = playerRef.playerShotHP;
        }
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = Vector3.forward * speed;
    }

    public void LoseLife(int hp)
    {
        hitPoints -= hp;

        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basic_Enemy"))
        {
            uiManager.UpdateScore(100);
            spawnManager.PowerupGenerator(other.transform);

            Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(other.gameObject);

            LoseLife(1);
        }
        else if (other.CompareTag("Faster_Enemy"))
        {
            uiManager.UpdateScore(150);
            spawnManager.PowerupGenerator(other.transform);

            Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(other.gameObject);

            LoseLife(1);
        }
        else if (other.CompareTag("Stronger_Enemy"))
        {
            Enemy strongerEnemy = other.gameObject.GetComponent<Enemy>();

            if (strongerEnemy != null)
            {
                Instantiate(particlePrefab, transform.position, Quaternion.identity);
                
                int hits = strongerEnemy.hitPoints;

                strongerEnemy.hitPoints -= hitPoints;


                if (strongerEnemy.hitPoints <= 0)
                {
                    uiManager.UpdateScore(200);
                    spawnManager.PowerupGenerator(other.transform);

                    Destroy(strongerEnemy.gameObject);
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                }

                LoseLife(hits);
            }
        }
        else if (other.CompareTag("Boss"))
        {
            Boss boss = other.gameObject.GetComponent<Boss>();

            if (boss != null)
            {
                Instantiate(particlePrefab, transform.position, Quaternion.identity);
                boss.LoseLife(hitPoints);
            }

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Missile"))
        {
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            LoseLife(2);
        }
    }
}
