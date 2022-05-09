using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GapController : MonoBehaviour
{
    [SerializeField] private OffMeshLink myLink;    //my offmesh link
    [SerializeField] private GameObject myDoor; //the door on top of me


    // Start is called before the first frame update
    void Start()
    {
        //fetch components
        myLink = GetComponentInChildren<OffMeshLink>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if we are in contact with a door
        if (other.tag == "Door")
        {
            //set that door to my door
            myDoor = other.gameObject;
            //deactivate my link
            myLink.activated = false;
        }
    }

    private void Update()
    {
        //if I have a door
        if (myDoor != null)
        {
            //set my active status to opposite my door's status
            myLink.activated = !myDoor.activeSelf;
        }
    }

}
