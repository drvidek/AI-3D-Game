using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{

    public enum States  //our state machine
    {
        idle,
        chase,
        investigate,
        hunt,
        goHome
    }
    [Header("States + behaviours")]
    public States state;    //to store my current state
    [SerializeField] private Vector3 homePos;    //my home position
    [SerializeField] private float chaseDist;   //the minimum distance to trigger chase state
    RaycastHit rayToPlayer = new RaycastHit();  //to check whether the player is in line of sight
    [SerializeField] private int randomChecks;    //my current number of random checks completed
    [SerializeField] private int randomMax;   //my maximum number of random checks

    [Header("Waypoints")]
    [SerializeField] private GameObject waypointCluster;  //my waypoint cluster parent object
    [SerializeField] private Transform[] waypointList;    //my list of waypoints
    [SerializeField] private int waypointIndex;   //my currently targeted waypoint

    [Header("Speeds")]
    [SerializeField] private float normSpd;   //my speeds and accels
    [SerializeField] private float normAccel;
    [SerializeField] private float chaseSpd = 5f, idleSpd = 2f, randomSpd = 3f, slowSpd = 1.5f;
    [SerializeField] private float chaseAccel = 2f, idleAccel = 5f, randomAccel = 1f, slowAccel = 10f;
    [SerializeField] private float chaseTurn = 500f, idleTurn = 1000f, randomTurn = 1000f;

    [Header("Components")]
    Animator anim;  //my animator
    NavMeshAgent m_Agent;   //my navmesh agent
    [SerializeField] private Transform player;  //the player's transform component

    void Start()
    {
        //fetch components and waypoints
        m_Agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;
        waypointList = waypointCluster.GetComponentsInChildren<Transform>();

        //set our home position
        homePos = transform.position;
        //trigger our state machine
        state = States.idle;
        NextState();
    }

    void Update()
    {
        //if the player is visible, set our state machine to Chase
        if (PlayerVisible())
            state = States.chase;
    }

    public void Restart()
    {
        //reset to our starting position
        m_Agent.Warp(homePos);
        m_Agent.destination = homePos;
        //reset our state to Idle
        state = States.idle;
        //turn off our chase animation by default
        anim.SetBool("Chase", false);
        //reset our waypoint index
        waypointIndex = 0;
        //reset our random check count
        randomChecks = 0;
        //trigger our state machine
        NextState();
    }

    void NextState()
    {
        //start the appropriate co-routine based on our state
        switch (state)
        {
            case States.idle:
                StartCoroutine(Idle());
                break;
            case States.chase:
                StartCoroutine(Chase());
                break;
            case States.investigate:
                StartCoroutine(Investigate());
                break;
            case States.hunt:
                StartCoroutine(Hunt());
                break;
            case States.goHome:
                StartCoroutine(GoHome());
                break;
            default:
                break;
        }
    }

    private IEnumerator Idle()
    {
        //set our default speeds for this state
        normSpd = idleSpd;
        m_Agent.angularSpeed = idleTurn;
        normAccel = idleAccel;
        //while we're in Idle state
        while (state == States.idle)
        {
            //check for a navmesh modifier on speed
            ChangeAreaSpeed();
            //move to your next waypoint
            MoveToWaypoint();
            //loop each frame we are in this state
            yield return null;
        }
        //when we break Idle state, trigger the next state
        NextState();
    }

    private IEnumerator Chase()
    {
        //set our default speeds for this state
        normSpd = chaseSpd;
        m_Agent.angularSpeed = chaseTurn;
        normAccel = chaseAccel;
        //set our animator to chase
        anim.SetBool("Chase", true);
        while (state == States.chase)
        {
            //check for a navmesh modifier on speed
            ChangeAreaSpeed();
            //target the player
            m_Agent.SetDestination(player.position);
            //get our current distance from the player
            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
            //if the player breaks line of sight or the distance from the player is larger than maximum distance
            if (!PlayerVisible() || dist > chaseDist)
                //move to Investigate state
                state = States.investigate;
            //loop each frame we are in this state
            yield return null;
        }
        //when we break Idle state, trigger the next state
        NextState();
    }

    private IEnumerator Investigate()
    {
        //set our default speeds for this state
        normSpd = chaseSpd;
        m_Agent.angularSpeed = idleTurn;
        while (state == States.investigate)
        {
            //check for a navmesh modifier on speed
            ChangeAreaSpeed();
            //get our distance from the destination (the player's last known location)
            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
            //if our distance is less than 1 metre
            if (dist <= 1f)
                //move to Hunt state
                state = States.hunt;
            //loop each frame we are in this state
            yield return null;
        }
        //when we break Idle state, trigger the next state
        NextState();
    }

    private IEnumerator Hunt()
    {
        //set our default speeds for this state
        normSpd = randomSpd;
        m_Agent.angularSpeed = randomTurn;
        normAccel = randomAccel;
        while (state == States.hunt)
        {
            //check for a navmesh modifier on speed
            ChangeAreaSpeed();
            //Complete random checks
            RandomMove();
            //if we exceed maximum check count
            if (randomChecks >= randomMax)
            {
                //move to Go Home state
                state = States.goHome;
                //reset our random check count
                randomChecks = 0;
            }
            //loop each frame we are in this state
            yield return null;
        }
        //when we break Idle state, trigger the next state
        NextState();
    }

    private IEnumerator GoHome()
    {
        //set our default speeds for this state
        normSpd = idleSpd;
        m_Agent.angularSpeed = idleTurn;
        normAccel = idleAccel;
        //reset our animator to walk
        anim.SetBool("Chase", false);
        //set our destination to our home position
        m_Agent.SetDestination(homePos);
        while (state == States.goHome)
        {
            //check for a navmesh modifier on speed
            ChangeAreaSpeed();
            //if we reach our destination, go back to Idle state
            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
            if (dist <= 1f)
                state = States.idle;
            //loop each frame we are in this state
            yield return null;
        }
        //when we break Idle state, trigger the next state
        NextState();

    }

    private bool PlayerVisible()
    {
        //get direction from the enemy to the player
        Vector3 playerDir = player.position - transform.position;
        //if we can cast a ray to the player with our maximum chase distance
        if (Physics.Raycast(transform.position, playerDir, out rayToPlayer, chaseDist))
        {
            //make sure we land a hit on something, and make sure that something is the player - return true. Else, return false.
            return (rayToPlayer.rigidbody != null && rayToPlayer.rigidbody.gameObject.tag == "Player");
        }
        else
            return false;
    }

    void ChangeAreaSpeed()
    {
        //establish a container for our navmesh hit data
        NavMeshHit navHit;
        //check our position on the navmesh
        m_Agent.SamplePathPosition(-1, 0.0f, out navHit);
        //set the navmesh Slow area to an int
        int SlowFloor = 1 << NavMesh.GetAreaFromName("Slow");
        //if our navmesh hit matches this int
        if (navHit.mask == SlowFloor)
        {
            //slow down the agent
            m_Agent.speed = slowSpd;
            m_Agent.acceleration = slowAccel;
        }
        else
        {
            //keep our speed to our current default speed
            m_Agent.speed = normSpd;
            m_Agent.acceleration = normAccel;
        }
    }

    void RandomMove()
    {
        //check the distance to our current destination
        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        //if we're within 1m
        if (dist <= 1f)
        {
            //set a new random destination to check
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 currentPos = m_Agent.transform.position;
            m_Agent.SetDestination(randomOffset + currentPos);
            //increase our number of random checks
            randomChecks++;
        }
    }

    void MoveToWaypoint()
    {
        //set our destination to our current waypoint's position
        m_Agent.destination = waypointList[waypointIndex].position;
        //get our distance from our destination
        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        //if we're within minimum distance or we cannot complete our path
        if (dist <= 1f || m_Agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            //move to our next waypoint
            waypointIndex++;
            //loop back to 0 if we exceed our maximum
            if (waypointIndex > waypointList.Length - 1)
                waypointIndex = 0;
        }
    }
}
