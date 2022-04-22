using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerAI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    NavMeshAgent m_Agent;
    public static GameObject targetObject;
    bool following;
    RaycastHit m_HitInfo = new RaycastHit();

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        ClickMove();
        if (following)
        {
            m_Agent.destination = targetObject.transform.position;
        }

    }

    void ClickMove()
    {
        
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                Debug.Log("ray cast");
                if (m_HitInfo.rigidbody != null)
                {
                    Debug.Log("click orb");
                    following = true;
                    targetObject = m_HitInfo.rigidbody.gameObject;
                    Image image = targetObject.GetComponentInChildren<Image>();
                    image.enabled = true;
                }
                else
                {
                    Debug.Log("click floor");
                    following = false;
                    targetObject = null;
                    m_Agent.destination = m_HitInfo.point;
                }
            }
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        GameObject hitObject = other.gameObject;
        Debug.Log(hitObject);
        switch (hitObject.tag)
        {
            case "Wisp":
                Destroy(hitObject);
                gameManager.scoreCurrent++;
                gameManager.UpdateScore();
                break;
            default:
                break;
        }
    }


}
