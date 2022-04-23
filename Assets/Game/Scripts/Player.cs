using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject muzzleFlashPrefab;
    public GameObject bulletPrefab;
    public AudioSource[] audios;
    
    public int playerShotHP = 1;

    public float forwardSpeed = 100f;
    public float horizontalSpeed = 40f;
    public float verticalSpeed = 40f;
    public float inclination = 60f;
    public float xMin = -9f, xMax = 9f, yMin = 2f, yMax = 30f;
    public float timeToReduceFuel = 0.5f;
    public float initialHSpeed = 40, initialVSpeed = 40;

    public bool canConsumeFuel = true;

    private int hitPoints = 30;
    private int fuel = 100;
    private int shieldPoints = 10;

    private float fireRate = 0.25f;
    private float currentTime = 0f;
    private float currentTime_Fuel = 0f;

    private UIManager uiManager;

    //Powerups
    private Rigidbody rigidBody;

    private bool shieldIsActive = false;
    private bool isDashSpeedUpActivated = false;
    private bool isContinuousShotActivated = false;
    private bool isPenetratingShotActivated = false;

    private float dashSpeedUpPowerDownTime = 0f;
    private float continuousShotPowerDownTime = 0f;
    private float penetratingShotPowerDownTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 7f, -550f);

        audios[0].volume = PlayerPrefs.GetFloat("SoundEffects");
        audios[1].volume = PlayerPrefs.GetFloat("SoundEffects");

        audios[0].Play();

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        uiManager.SetMaxHealth(hitPoints);
        uiManager.SetMaxFuel(fuel);

        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!uiManager.GameIsPaused)
        {
            currentTime_Fuel += Time.deltaTime;   
            Shot();

            if (isContinuousShotActivated || isDashSpeedUpActivated || isPenetratingShotActivated)
            {
                PowerupsPowerDownRoutine();
            }

            LoseFuel();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal * horizontalSpeed, vertical * verticalSpeed, forwardSpeed);

        rigidBody.velocity = movement;
        rigidBody.rotation = Quaternion.Euler(0, 0, rigidBody.velocity.x * -inclination);

        //Limita a movimentação do player, para que ele não atravesse o cenário
        rigidBody.position = new Vector3
        (
            Mathf.Clamp(rigidBody.position.x, xMin, xMax), 
            Mathf.Clamp(rigidBody.position.y, yMin, yMax), 
            rigidBody.position.z
        );
    }

    private void Shot()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= fireRate && Input.GetKeyDown(KeyCode.Space))
        {
            currentTime = 0;
            Instantiate(muzzleFlashPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z +5f), Quaternion.identity);
            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basic_Enemy") || other.CompareTag("Faster_Enemy"))
        {
            LoseLife(3);
            Destroy(other.gameObject);
            Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
        }
        else if (other.CompareTag("Stronger_Enemy"))
        {
            LoseLife(6);
            Destroy(other.gameObject);
            Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
        }

    }

    public void LoseLife(int hits)
    {
        if (!uiManager.whoWillDie.Equals("Boss") && !uiManager.whoWillDie.Equals("Player"))
        {
            if (shieldIsActive)
            {
                shieldPoints -= hits;

                if (shieldPoints <= 0)
                {
                    hitPoints += shieldPoints;
                    shieldPoints = 0;

                    audios[1].Play();
                    uiManager.DeactivateShield();
                    shieldIsActive = false;
                }
                else
                {
                    uiManager.UpdateShield(shieldPoints);
                }
            }
            else
            {
                hitPoints -= hits;
            }

            if (hitPoints <= 0)
            {
                hitPoints = 0;
                uiManager.SlowMotion(this.tag);
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            uiManager.SetHealth(hitPoints);
        }
    }

    public void LoseFuel()
    {
        if (currentTime_Fuel > timeToReduceFuel && canConsumeFuel)
        {
            currentTime_Fuel = 0f;
            fuel -= 1;

            if (fuel <= 0)
            {
                uiManager.SetFuel(0);
                uiManager.SlowMotion(this.tag);

                audios[0].Stop();
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                uiManager.SetFuel(fuel);
            }
        }
    }

    public void FillTheTank(int fuelPoints)
    {
        if (!uiManager.whoWillDie.Equals("Player"))
        {
            fuel += fuelPoints;

            if (fuel > 100)
            {
                fuel = 100;
            }

            uiManager.SetFuel(fuel);
        }
    }

    public void ContinuousShotPowerupRoutine()
    {
        continuousShotPowerDownTime = 15f;   
        isContinuousShotActivated = true;
        fireRate = 0.05f;
    }

    public void DashSpeedUpPowerupRoutine()
    {
        dashSpeedUpPowerDownTime = 10f;
        isDashSpeedUpActivated = true;
        horizontalSpeed = initialHSpeed * 1.15f;
        verticalSpeed = initialVSpeed * 1.15f;
    }

    public void RestoreLifePowerupRoutine()
    {
        hitPoints += 5;
        
        if (hitPoints > 30)
        {
            hitPoints = 30;
        }

        uiManager.SetHealth(hitPoints);
    }

    public void ShieldPowerupRoutine()
    {
        shieldPoints = 10;

        if (shieldIsActive)
        {
            uiManager.UpdateShield(shieldPoints);
        }
        else
        {
            uiManager.ActivateShield(shieldPoints);
            shieldIsActive = true;
        }
    }

    public void PenetratingShotPowerupRoutine()
    {
        playerShotHP = 4;
        isPenetratingShotActivated = true;
        penetratingShotPowerDownTime = 15f;
    }

    private void PowerupsPowerDownRoutine()
    {
        if (isContinuousShotActivated)
        {
            if (continuousShotPowerDownTime > 0)
            {
                continuousShotPowerDownTime -= Time.deltaTime;
            }
            else
            {
                audios[1].Play();
                isContinuousShotActivated = false;
                fireRate = 0.25f;
            }
        }

        if (isDashSpeedUpActivated)
        {
            if (dashSpeedUpPowerDownTime > 0)
            {
                dashSpeedUpPowerDownTime -= Time.deltaTime;
            }
            else
            {
                audios[1].Play();
                isDashSpeedUpActivated = false;
                horizontalSpeed = initialHSpeed;
                verticalSpeed = initialVSpeed;
            }
        }

        if (isPenetratingShotActivated)
        {
            if (penetratingShotPowerDownTime > 0)
            {
                penetratingShotPowerDownTime -= Time.deltaTime;
            }
            else
            {
                audios[1].Play();
                isPenetratingShotActivated = false;
                playerShotHP = 1;
            }
        }
    }
}
