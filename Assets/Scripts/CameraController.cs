using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;  //the player's transform
    [SerializeField] private float zoom;    //the camera distance from the player

    // Update is called once per frame
    void Update()
    {
        //set our position to the player's x and z position, using zoom for our height
        transform.position = new Vector3(player.position.x, zoom, player.position.z);
    }
}
