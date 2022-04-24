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
    public GameObject waypointCluster;
    public Transform[] waypointList;
    public int waypointIndex;
    public int randomChecks;
    public int randomMax;
    public float chaseSpd = 5, idleSpd = 2, randomSpd = 3;
    public float chaseAccel = 2, idleAccel = 5, randomAccel = 1;
    public float chaseTurn = 500, idleTurn = 1000, randomTurn = 1000;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
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
        m_Agent.speed = idleSpd;
        m_Agent.angularSpeed = idleTurn;
        m_Agent.acceleration = idleAccel;
        while (state == States.idle)
        {
            MoveToWaypoint();
            yield return null;
        }
        NextState();
    }

    private IEnumerator Chase()
    {
        m_Agent.speed = chaseSpd;
        m_Agent.angularSpeed = chaseTurn;
        m_Agent.acceleration = chaseAccel;
        anim.SetBool("Chase", true);
        while (state == States.chase)
        {
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
        m_Agent.speed = chaseSpd;
        m_Agent.angularSpeed = idleTurn;
        while (state == States.investigate)
        {
            float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
            if (dist <= 1f)
                state = States.hunt;
            yield return null;
        }
        NextState();
    }

    private IEnumerator Hunt()
    {
        m_Agent.speed = randomSpd;
        m_Agent.angularSpeed = randomTurn;
        m_Agent.acceleration = idleAccel;
        while (state == States.hunt)
        {
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
        m_Agent.speed = idleSpd;
        m_Agent.angularSpeed = idleTurn;
        m_Agent.acceleration = idleAccel;
        anim.SetBool("Chase", false);
        m_Agent.SetDestination(homePos);
        while (state == States.goHome)
        {
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
        int GrassMask = 1 << NavMesh.GetAreaFromName("Tall Grass");
        if (navHit.mask == GrassMask)
        {
            m_Agent.speed = 3f;
        }
        else
        {
            m_Agent.speed = 30f;
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
