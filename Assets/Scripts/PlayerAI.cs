using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerAI : MonoBehaviour
{
    [Header("Speeds + Movement")]
    [SerializeField] private bool active;   //whether we are in game mode or not
    [SerializeField] private float walkSpeed, jumpSpeed;    //our speeds
    public static GameObject targetObject;  //our target object
    [SerializeField] private Vector3 homePos;   //our home position

    [Header("Components")]
    [SerializeField] private Animator myAnim;   //my animator
    [SerializeField] private GameManager gameManager;   //the game manager
    NavMeshAgent m_Agent;   //my navmesh agent
    bool following; //whether i am following something
    RaycastHit m_HitInfo = new RaycastHit();    //to store our screen to click raycast
    bool linking = false;   //whether we are on a navmesh link

    void Start()
    {
        //fetch components
        m_Agent = GetComponent<NavMeshAgent>();
        myAnim = GetComponentInChildren<Animator>();
        //set my home position
        homePos = transform.position;
    }

    public void PlayerActive(bool b)
    {
        //set active to our input bool
        active = b;
    }

    void Update()
    {
        //if we're active, allow click to move
        if (active)
            ClickMove();
        //if we're following something and our target exists
        if (following && targetObject != null)
        {
            //set our destination to our target's current position
            m_Agent.destination = targetObject.transform.position;
        }

        //if we have not reached our destination
        if (m_Agent.remainingDistance > 0.5f)
            //activate our movement animation
            myAnim.SetBool("Moving", true);
        //else, deactivate our movement animation
        else
            myAnim.SetBool("Moving", false);
    }

    private void FixedUpdate()
    {
        //when we are on a mesh link for the first frame
        if (m_Agent.isOnOffMeshLink && linking == false)
        {
            //toggle our linking bool
            linking = true;
            //set our current movement to zero
            m_Agent.velocity = Vector3.zero;
            //set our speed to our jump speed
            m_Agent.speed = jumpSpeed;
            //trigger our jump animation
            myAnim.SetTrigger("Jump");
        }
        //else if we're on the navmesh surface for the first frame
        else if (m_Agent.isOnNavMesh && linking == true)
        {
            //toggle our linking bool
            linking = false;
            //set our current movement to zero
            m_Agent.velocity = Vector3.zero;
            //set our speed back to our walk speed
            m_Agent.speed = walkSpeed;
        }
    }

    public void Restart()
    {
        //warp home
        m_Agent.Warp(homePos);
        //reset our destination to home
        m_Agent.destination = homePos;
        //turn off our following bool
        following = false;
        //turn off our linking bool
        linking = false;
        //remove any target object
        targetObject = null;
    }

    void ClickMove()
    {
        //on right click
        if (Input.GetMouseButtonDown(0))
        {
            //cast a ray to determine where we've clicked
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if we hit something
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                //if we hit a gem or a wisp
                if (m_HitInfo.rigidbody != null && (m_HitInfo.rigidbody.tag == "Wisp" || m_HitInfo.rigidbody.tag == "Gem"))
                {
                    //activate our following bool
                    following = true;
                    //set our target object to the clicked object
                    targetObject = m_HitInfo.rigidbody.gameObject;
                    //enable that object's target icon
                    Image image = targetObject.GetComponentInChildren<Image>();
                    image.enabled = true;
                }
                else
                {
                    //we are not following anything
                    following = false;
                    targetObject = null;
                    //our destination is the point we clicked
                    m_Agent.destination = m_HitInfo.point;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if we are in an active game
        if (active)
        {
            //get the game object we collided with
            GameObject hitObject = other.gameObject;
            //if we hit our target object, set our target to null
            if (hitObject == targetObject)
                targetObject = null;
            //switch based on the tag of the object we hit
            switch (hitObject.tag)
            {
                case "Wisp":
                    //deactivate the wisp
                    hitObject.SetActive(false);
                    //increase our score by 1
                    gameManager.scoreCurrent++;
                    //update the score indicator and check for a win
                    gameManager.UpdateScore();
                    break;
                case "Gem":
                    //get the gem controller from the gem we hit
                    GemController hitGem = hitObject.GetComponent<GemController>();
                    //get it's gem index
                    int unlockIndex = hitGem.gemIndex;
                    //deactivate the gem
                    hitObject.SetActive(false);
                    //get all the doors in the game
                    GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
                    //for each door
                    for (int i = 0; i < doors.Length; i++)
                    {
                        //get the door component
                        DoorController door = doors[i].GetComponent<DoorController>();
                        //check if your gem ID matches the collected gem
                        door.UnlockCheck(unlockIndex);
                    }
                    break;
                case "Enemy":
                    //trigger a game end in fail state
                    gameManager.GameEnd(false);
                    break;
                default:
                    break;
            }
        }
    }

}
