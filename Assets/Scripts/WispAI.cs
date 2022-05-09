using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class WispAI : MonoBehaviour
{
    GameManager gameManager;    //our game manager script
    Image targetIcon;   //my icon to indicate when I am targeted
    NavMeshAgent m_Agent;   //my navmesh agent
    Vector3 homePos;    //my starting position
    float minDist = 0.5f;   //minimum distance to destination
    float randomDist = 3f;  //radius to check from my current position for next move
    float maxDistFromHome = 5f; //maximum distance I will travel from home

    void Start()
    {
        //fetch components
        targetIcon = GetComponentInChildren<Image>();
        m_Agent = GetComponent<NavMeshAgent>();
        //set my starting position
        homePos = transform.position;
    }

    void Update()
    {
        //trigger move method
        RandomMove();
        //if i am not currently targeted by the player
        if (PlayerAI.targetObject != this.gameObject)
            //turn off my target icon
            targetIcon.enabled = false;
    }

    public void Restart()
    {
        //make sure i am active
        this.gameObject.SetActive(true);
        //turn off my icon by default
        targetIcon.enabled = false;
        //return to my home position
        transform.position = homePos;
    }

    void RandomMove()
    {
        //get the current distance from my position my destination
        float dist = Vector3.Distance(m_Agent.transform.position, m_Agent.destination);
        //if it's less that the minimum distance
        if (dist <= minDist)
        {
            //get a new position within a 3m radius
            Vector3 randomOffset = new Vector3(Random.Range(-randomDist, randomDist), 0f, Random.Range(-randomDist, randomDist));
            //grab our current position
            Vector3 currentPos = m_Agent.transform.position;
            //fnd out the distance to our new position from home
            float distFromHome = Vector3.Distance(homePos, randomOffset + currentPos);

            //while we exceed the maximum distance from home, keep trying a new destination
            while (distFromHome > maxDistFromHome)
            {
                randomOffset = new Vector3(Random.Range(-randomDist, randomDist), 0f, Random.Range(-randomDist, randomDist));
                currentPos = m_Agent.transform.position;
                distFromHome = Vector3.Distance(homePos, randomOffset + currentPos);
            }
            //set our new destination
            m_Agent.SetDestination(randomOffset + currentPos);
        }
    }
}
