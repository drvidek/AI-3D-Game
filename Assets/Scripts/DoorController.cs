using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    private Color32 myColour;   //my colour
    public int gemIndex;    //my gem index
    [SerializeField] private Renderer myRenderer;   //my mesh
    GameManager gameManager;    //the game manager

    void Start()
    {
        //fetch components
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //set my colour
        GetColour();
    }

    public void GetColour()
    {
        //use my gem index to set my colour
        myColour = gameManager.gemColour[gemIndex];
        myColour.a = 255;
        myRenderer.material.color = myColour;
    }

    public void Restart()
    {
        //set my status to active
        this.gameObject.SetActive(true);
    }

    public void UnlockCheck(int key)
    {
        //if the input gem index is the same as mine
        if (key == gemIndex)
        {
            //deactivate myself
            this.gameObject.SetActive(false);
        }
    }
}
