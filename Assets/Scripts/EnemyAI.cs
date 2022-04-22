using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    NavMeshAgent m_Agent;
    [SerializeField] private Transform player;
    [SerializeField] private float chaseDist;
    Animator anim;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > chaseDist)
        {
            RandomMove();
            anim.SetBool("Chase", false);
        }
        else
        {
            m_Agent.SetDestination(player.position);
            anim.SetBool("Chase", true);
        }
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
        }
    }
}
