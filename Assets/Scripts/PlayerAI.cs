using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerAI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    NavMeshAgent m_Agent;
    public static GameObject targetObject;
    bool following;
    RaycastHit m_HitInfo = new RaycastHit();
    bool linking = false;
    public float walkSpeed, jumpSpeed;
    Transform modelTransform;
    public Vector3 homePos;
    public bool active;
    [SerializeField] private Animator myAnim;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        modelTransform = GetComponentInChildren<Renderer>().transform;
        myAnim = GetComponentInChildren<Animator>();
        homePos = transform.position;
    }

    public void PlayerActive(bool b)
    {
        active = b;
    }

    void Update()
    {
        if (active)
        ClickMove();
        if (following && targetObject != null)
        {
            m_Agent.destination = targetObject.transform.position;
        }

        if (m_Agent.remainingDistance > 0.5f)
            myAnim.SetBool("Moving", true);
        else
            myAnim.SetBool("Moving", false);
    }

    private void FixedUpdate()
    {
        if (m_Agent.isOnOffMeshLink && linking == false)
        {
            linking = true;
            m_Agent.speed = jumpSpeed;
            myAnim.SetTrigger("Jump");
        }
        else if (m_Agent.isOnNavMesh && linking == true)
        {
            linking = false;
            m_Agent.velocity = Vector3.zero;
            m_Agent.speed = walkSpeed;
        }
    }

    public void Restart()
    {
        m_Agent.Warp(homePos);
        m_Agent.destination = homePos;
        following = false;
        targetObject = null;
    }

    void ClickMove()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                

                if (m_HitInfo.rigidbody != null)
                {
                    following = true;
                    targetObject = m_HitInfo.rigidbody.gameObject;
                    Image image = targetObject.GetComponentInChildren<Image>();
                    image.enabled = true;
                }
                else
                {
                    following = false;
                    targetObject = null;
                    m_Agent.destination = m_HitInfo.point;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active)
        {
            GameObject hitObject = other.gameObject;
            if (hitObject == targetObject)
                targetObject = null;
            switch (hitObject.tag)
            {
                case "Wisp":
                    hitObject.SetActive(false);
                    gameManager.scoreCurrent++;
                    gameManager.UpdateScore();
                    break;
                case "Gem":
                    GemController hitGem = hitObject.GetComponent<GemController>();
                    int unlockIndex = hitGem.gemIndex;
                    hitObject.SetActive(false);
                    GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
                    for (int i = 0; i < doors.Length; i++)
                    {
                        DoorController door = doors[i].GetComponent<DoorController>();
                        door.UnlockCheck(unlockIndex);
                    }
                    break;
                case "Enemy":
                    Debug.Log("Enemy hit");
                    gameManager.GameEnd(false);
                    break;
                default:
                    break;
            }
        }
    }

}
