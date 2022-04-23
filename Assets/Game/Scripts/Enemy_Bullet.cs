using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    public GameObject particlePrefab;
    public GameObject explosionPrefab;
    public GameObject particleCollisionPrefab;
    public AudioSource soundEffect;

    public float speed = 30f;
    public float timeToDestroy = 1.2f;

    private UIManager uiManager;
    private Rigidbody rigidBody;
    
    private bool moveWithTransform = false;

    // Start is called before the first frame update
    void Start()
    {
        soundEffect.volume = PlayerPrefs.GetFloat("SoundEffects");

        soundEffect.Play();

        Destroy(this.gameObject, timeToDestroy);

        rigidBody = GetComponent<Rigidbody>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (this.CompareTag("Stronger_Enemy"))
        {
            speed /= 1.5f;
        }
        else if (this.CompareTag("Faster_Enemy"))
        {
            speed *= 1.5f;
        }

        //if (this.CompareTag("Double_Shot") || this.CompareTag("Triple_Shot"))
        //{
        //    moveWithTransform = true;
        //}
    }

    private void Update()
    {
        if (!uiManager.GameIsPaused && moveWithTransform)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        }
    }

    private void FixedUpdate()
    {
        if (!moveWithTransform)
        {
            rigidBody.velocity = Vector3.back * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                if (this.CompareTag("Missile"))
                {
                    player.LoseLife(6);
                    Destroy(this.gameObject);
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(particlePrefab, transform.position, Quaternion.Euler(0f, 180f, 0f));

                    if (this.CompareTag("Stronger_Enemy_Shot"))
                    {
                        player.LoseLife(2);
                    }
                    else
                    {
                        player.LoseLife(1);
                    }
                    Destroy(this.gameObject);
                }
            }
        }
        else if (other.CompareTag("Player_Shot"))
        {
            if (this.CompareTag("Missile"))
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(particleCollisionPrefab, transform.position, Quaternion.identity);

                Player_Bullet playerShot = other.gameObject.GetComponent<Player_Bullet>();

                if (playerShot != null)
                {
                    playerShot.LoseLife(1);
                }
            }

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("BigBangArea"))
        {
            Destroy(this.gameObject);
        }
    }

//    private void OnDestroy()
//    {
//        if (this.CompareTag("Missile"))
//        {
//            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
//        }
//    }
}
