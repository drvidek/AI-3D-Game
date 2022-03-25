using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();

    [SerializeField] bool _isRandomPos = false;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (_isRandomPos)
        {
            RandomMove();
        }
        else
        {
            ClickMove();
        }

        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        Debug.Log(dist);
    }

    void ClickMove()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                m_Agent.destination = m_HitInfo.point;
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
