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
    float minDist = 0.5f;
    float randomDist = 3f;
    float maxDistFromHome = 5f;
    Renderer myRenderer;
    Light myLight;

    void Start()
    {
        selectIcon = GetComponentInChildren<Image>();
        myRenderer = GetComponentInChildren<Renderer>();
        myLight = GetComponentInChildren<Light>();
        m_Agent = GetComponent<NavMeshAgent>();
        homePos = transform.position;
    }

    void Update()
    {
        //myLight.enabled = CheckVisible();
        RandomMove();
        if (PlayerAI.targetObject != this.gameObject)
            selectIcon.enabled = false;
    }

    public void Restart()
    {
        this.gameObject.SetActive(true);
        selectIcon.enabled = false;
        transform.position = homePos;
    }

    bool CheckVisible()
    {
        if (myRenderer.isVisible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void RandomMove()
    {
        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        if (dist <= minDist)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-randomDist, randomDist), 0f, Random.Range(-randomDist, randomDist));
            Vector3 currentPos = m_Agent.transform.position;
            float distFromHome = Vector3.Distance(homePos, randomOffset + currentPos);

            while (distFromHome > maxDistFromHome)
            {
                randomOffset = new Vector3(Random.Range(-randomDist, randomDist), 0f, Random.Range(-randomDist, randomDist));
                currentPos = m_Agent.transform.position;
                distFromHome = Vector3.Distance(homePos, randomOffset + currentPos);
            }
            m_Agent.SetDestination(randomOffset + currentPos);
        }
    }
}
