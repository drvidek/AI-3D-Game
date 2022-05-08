using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float zoom;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, zoom, player.position.z);
    }
}
