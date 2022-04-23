using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public GameObject collectedPrefab;
    public GameObject bigBangAreaPrefab;
    public float rotateSpeed = 150f;

    private Transform playerPos;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (!this.CompareTag("Fuel_Gallon"))
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = PlayerPrefs.GetFloat("SoundEffects");
            audioSource.Play();
        }

        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        
        if (playerPos != null && playerPos.position.z > transform.position.z + 10f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("Fuel_Gallon") && other.CompareTag("Player_Shot"))
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            
            if (player != null)
            {
                player.FillTheTank(25);
            }

            Instantiate(collectedPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                if (this.CompareTag("ContinuousShot_Powerup"))
                {
                    player.ContinuousShotPowerupRoutine();
                }
                else if (this.CompareTag("DashSpeedUp_Powerup"))
                {
                    player.DashSpeedUpPowerupRoutine();
                }
                else if (this.CompareTag("Health_Powerup"))
                {
                    player.RestoreLifePowerupRoutine();
                }
                else if (this.CompareTag("Shield_Powerup"))
                {
                    player.ShieldPowerupRoutine();
                }
                else if (this.CompareTag("BigBang_Powerup"))
                {
                    Instantiate(bigBangAreaPrefab, new Vector3(0, 3f, transform.position.z), Quaternion.identity);
                }
                else if (this.CompareTag("PenetratingShot_Powerup"))
                {
                    player.PenetratingShotPowerupRoutine();
                }
                else if (this.CompareTag("Fuel_Gallon"))
                {
                    player.FillTheTank(25);
                }

                Instantiate(collectedPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
}
