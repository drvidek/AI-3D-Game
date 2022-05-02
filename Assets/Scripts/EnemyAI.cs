using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum States
    {
        idle,
        chase,
        investigate,
        hunt,
        goHome
    }
    public States state;
    Vector3 homePos;
    NavMeshAgent m_Agent;
    [SerializeField] private Transform player;
    [SerializeField] private float chaseDist;
    RaycastHit rayToPlayer = new RaycastHit();
    Animator anim;
    public Transform modelTransform;
    public GameObject waypointCluster;
    public Transform[] waypointList;
    public int waypointIndex;
    public int randomChecks;
    public int randomMax;
    public float normSpd;
    public float normAccel;
    public float chaseSpd = 5f, idleSpd = 2f, randomSpd = 3f, slowSpd = 1.5f;
    public float chaseAccel = 2f, idleAccel = 5f, randomAccel = 1f, slowAccel = 10f;
    public float chaseTurn = 500f, idleTurn = 1000f, randomTurn = 1000f;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Transform[] transArray = GetComponentsInChildren<Transform>();
        modelTransform = transArray[1];
        player = GameObject.Find("Player").transform;
        homePos = transform.position;
        state = States.idle;
        waypointList = waypointCluster.GetComponentsInChildren<Transform>();
        NextState();
    }

    void Update()
    {
        if (PlayerVisible())
            state = States.chase;
    }

    public void Restart()
    {
        m_Agent.Warp(homePos);
        m_Agent.destination = homePos;
        state = States.idle;
        anim.SetBool("Chase", false);
        waypointIndex = 0;
        randomChecks = 0;
        NextState();
    }

    void NextState()
    {
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
        normSpd = idleSpd;
        m_Agent.angularSpeed = idleTurn;
        normAccel = idleAccel;
        while (state == States.idle)
        {
            ChangeAreaSpeed();
            MoveToWaypoint();
            yield return null;
        }
        NextState();
    }

    private IEnumerator Chase()
    {
        normSpd = chaseSpd;
        m_Agent.angularSpeed = chaseTurn;
        normAccel = chaseAccel;
        anim.SetBool("Chase", true);
        while (state == States.chase)
        {
            ChangeAreaSpeed();
            m_Agent.SetDestination(player.position);

            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);

            if (!PlayerVisible() || dist > chaseDist)
                state = States.investigate;
            yield return null;
        }
        NextState();
    }

    private IEnumerator Investigate()
    {
        normSpd = chaseSpd;
        m_Agent.angularSpeed = idleTurn;
        while (state == States.investigate)
        {
            ChangeAreaSpeed();
            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
            if (dist <= 1f)
                state = States.hunt;
            yield return null;
        }
        NextState();
    }

    private IEnumerator Hunt()
    {
        normSpd = randomSpd;
        m_Agent.angularSpeed = randomTurn;
        normAccel = idleAccel;
        while (state == States.hunt)
        {
            ChangeAreaSpeed();
            RandomMove();
            if (randomChecks >= randomMax)
            {
                state = States.goHome;
                randomChecks = 0;
            }
            yield return null;
        }
        NextState();
    }

    private IEnumerator GoHome()
    {
        normSpd = idleSpd;
        m_Agent.angularSpeed = idleTurn;
        normAccel = idleAccel;
        anim.SetBool("Chase", false);
        m_Agent.SetDestination(homePos);
        while (state == States.goHome)
        {
            ChangeAreaSpeed();
            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
            if (dist <= 1f)
                state = States.idle;
            yield return null;
        }
        NextState();

    }

    private bool PlayerVisible()
    {
        Vector3 playerDir = player.position - transform.position;
        if (Physics.Raycast(transform.position, playerDir, out rayToPlayer, chaseDist))
        {
            return (rayToPlayer.rigidbody != null && rayToPlayer.rigidbody.gameObject.tag == "Player");
        }
        else
            return false;
    }

    void ChangeAreaSpeed()
    {
        NavMeshHit navHit;
        m_Agent.SamplePathPosition(-1, 0.0f, out navHit);
        int GrassMask = 1 << NavMesh.GetAreaFromName("Slow");
        if (navHit.mask == GrassMask)
        {
            m_Agent.speed = slowSpd;
            m_Agent.acceleration = slowAccel;
        }
        else
        {
            m_Agent.speed = normSpd;
            m_Agent.acceleration = normAccel;
        }
    }

    void RandomMove()
    {
        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        if (dist <= 1f)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 currentPos = m_Agent.transform.position;
            m_Agent.SetDestination(randomOffset + currentPos);
            randomChecks++;
        }
    }

    void MoveToWaypoint()
    {
        m_Agent.destination = waypointList[waypointIndex].position;
        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        if (dist <= 1f || m_Agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            waypointIndex++;
            if (waypointIndex > waypointList.Length - 1)
                waypointIndex = 0;
        }
    }
}
