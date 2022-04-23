using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] Enemy;
    public GameObject[] powerups;
    public GameObject fuelGallon;
    public GameObject beginForest, city, forest, militaryBase;
    public GameObject bossPrefab;

    public float timeToSpawn = 3f;
    public float timeToSpawnFuelGallon = 10f;
    public float minHorizontalPos;
    public float maxHorizontalPos;
    public float minVerticalPos;
    public float maxVerticalPos;
    public float playerBulletSpeed = 150f;

    private Transform forestStart, militaryBaseStart, bossStart;
    private Player player;

    private float currentTime_FuelGallon = 0f;
    private float maxForwardPos;
    private float currentTime = 0;

    private bool canSpawn = false;

    private int machineChoiceToSpawnEnemy = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        forestStart = GameObject.Find("ForestStart").GetComponent<Transform>();
        militaryBaseStart = GameObject.Find("MilitaryBaseStart").GetComponent<Transform>();
        bossStart = GameObject.Find("BossStart").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn && player != null && maxForwardPos > player.transform.position.z + 40f)
        {
            EnemySpawn();
            FuelGallonGenerator();
        }
    }

    private void EnemySpawn()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= timeToSpawn)
        {
            currentTime = 0;
            float randomHorizontal = Random.Range(player.xMin, player.xMax);
            float randomVertical = Random.Range(player.yMin, player.yMax);
            float randomForward = Random.Range(player.transform.position.z + 40f, maxForwardPos);

            int result = Random.Range(0, machineChoiceToSpawnEnemy);

            if (result == 0 || result == 1)
            {
                Instantiate(Enemy[0], new Vector3(randomHorizontal, randomVertical, randomForward), Quaternion.Euler(0, 180, 0));
            }
            else if (result >= 2 && result <= 9)
            {
                Instantiate(Enemy[1], new Vector3(randomHorizontal, randomVertical, randomForward), Quaternion.Euler(0, 180, 0));
            }
            else
            {
                Instantiate(Enemy[2], new Vector3(randomHorizontal, randomVertical, randomForward), Quaternion.Euler(0, 180, 0));
            }
        }
    }

    public void PowerupGenerator(Transform enemyPos)
    {
        int machineChoice = Random.Range(0, 3);

        if (machineChoice == 2)
        {
            int powerup = Random.Range(0, powerups.Length);

            Instantiate(powerups[powerup], enemyPos.position, Quaternion.identity);
        }
    }

    public void FuelGallonGenerator()
    {
        currentTime_FuelGallon += Time.deltaTime;

        if (currentTime_FuelGallon >= timeToSpawnFuelGallon)
        {
            currentTime_FuelGallon = 0;

            float randomHorizontal = Random.Range(player.xMin, player.xMax);
            float randomVertical = Random.Range(player.yMin, player.yMax);
            float randomForward = Random.Range(player.transform.position.z + 40f, player.transform.position.z + 500f);

            Instantiate(fuelGallon, new Vector3(randomHorizontal, randomVertical, randomForward), Quaternion.identity);
        }
    }

    public void ReachedTheCity()
    {
        maxForwardPos = forestStart.position.z;
        canSpawn = true;
        Destroy(beginForest.gameObject);
    }

    public void ReachedTheForest()
    {
        
        if (player != null)
        {
            maxForwardPos = militaryBaseStart.position.z;
            player.xMin = -30f;
            player.xMax = 30f;
            player.yMin = 3f;
            player.yMax = 40f;
            machineChoiceToSpawnEnemy = 9;
            Destroy(city.gameObject);
        }
    }

    public void ReachedTheMilitaryBase()
    {
        if (player != null)
        {
            maxForwardPos = bossStart.position.z;
            player.xMin = -40f;
            player.xMax = 40f;
            player.yMin = 5f;
            player.yMax = 45f;
            Destroy(forest.gameObject);
            machineChoiceToSpawnEnemy = 19;
        }
    }

    public void ReachedTheBossArea()
    {
        if (player != null)
        {
            canSpawn = false;
            player.forwardSpeed = 0f;
            player.canConsumeFuel = false;
            Instantiate(bossPrefab, new Vector3(player.transform.position.x, player.transform.position.y, 7000f), Quaternion.Euler(0f, 180f, 0f));
        }
    }

    public void ChangeDifficulty(float currentDifficulty, float currentTimeToSpawn)
    {
        if (player != null)
        {
            if (currentDifficulty <= 1.25f)
            {
                player.forwardSpeed *= currentDifficulty;
                player.horizontalSpeed *= currentDifficulty;
                player.verticalSpeed *= currentDifficulty;

                player.initialHSpeed *= currentDifficulty;
                player.initialVSpeed *= currentDifficulty;
            }
            else
            {
                player.forwardSpeed *= 1.25f;
                player.horizontalSpeed *= 1.25f;
                player.verticalSpeed *= 1.25f;

                player.initialHSpeed *= 1.25f;
                player.initialVSpeed *= 1.25f;
            }

            playerBulletSpeed *= (currentDifficulty + 0.05f);
            timeToSpawn -= currentTimeToSpawn;
        }
        
        //currentDifficulty += 0.05f;

            //Debug.Log("Forward Speed: " + player.forwardSpeed + "\nH_Speed: " + player.horizontalSpeed + "\nV_Speed: " + player.verticalSpeed + "\nPlayer Bullet Speed: " + playerBulletSpeed + "\nTime to Spawn: " + timeToSpawn);
    }
}
