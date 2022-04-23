using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = new Vector3(player.position.x, player.position.y + 2.5f, player.position.z - 28f);
            transform.position = newPosition;
        }
    }
}
