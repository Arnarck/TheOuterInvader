using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public SpawnManager spawnManager;

    private Transform playerPos;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPos != null && playerPos.position.z >= transform.position.z)
        {
            if (this.name == "CityStart")
            {
                spawnManager.ReachedTheCity();
            }
            else if (this.name == "ForestStart")
            {
                spawnManager.ReachedTheForest();
            }
            else if (this.name == "MilitaryBaseStart")
            {
                spawnManager.ReachedTheMilitaryBase();
            }
            else if (this.name == "BossStart")
            {
                spawnManager.ReachedTheBossArea();
            }
            Destroy(this.gameObject);
        }
    }
}
