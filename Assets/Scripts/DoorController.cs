using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    private Color32 myColour;
    public int gemIndex;
    [SerializeField] private Renderer myRenderer;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetColour();
    }

    public void GetColour()
    {
        myColour = gameManager.gemColour[gemIndex];
        myColour.a = 255;
        myRenderer.material.color = myColour;
    }

    public void Restart()
    {
        this.gameObject.SetActive(true);
    }
    
    public void UnlockCheck(int key)
    {
        if (key == gemIndex)
        {
            this.gameObject.SetActive(false);
        }
    }
}
