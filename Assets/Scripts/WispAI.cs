using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class WispAI : MonoBehaviour
{
    GameManager gameManager;
    Image selectIcon;
    NavMeshAgent m_Agent;
    Vector3 homePos;

    void Start()
    {
        selectIcon = GetComponentInChildren<Image>();
        m_Agent = GetComponent<NavMeshAgent>();
        homePos = transform.position;
    }

    void Update()
    {
        RandomMove();
        if (PlayerAI.targetObject != this.gameObject)
            selectIcon.enabled = false;
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
        if (dist <= 0.5f)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 currentPos = m_Agent.transform.position;
            float distFromHome = Vector3.Distance(homePos, randomOffset + currentPos);

            while (distFromHome > 10)
            {
                randomOffset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                currentPos = m_Agent.transform.position;
                distFromHome = Vector3.Distance(homePos, randomOffset + currentPos);
            }

            m_Agent.SetDestination(randomOffset + currentPos);
        }
    }
}
