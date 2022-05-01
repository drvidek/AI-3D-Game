using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GapController : MonoBehaviour
{
    [SerializeField] private OffMeshLink myLink;
    [SerializeField] private GameObject myDoor;


    // Start is called before the first frame update
    void Start()
    {
        myLink = GetComponentInChildren<OffMeshLink>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.tag == "Door")
        {
            myDoor = other.gameObject;
            myLink.activated = false;
        }
    }

    private void Update()
    {
        if (myDoor != null)
        {
            myLink.activated = !myDoor.activeSelf;
        }
    }

}
