using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemController : MonoBehaviour
{
    private Color32 gemColour;  //my colour
    public int gemIndex;    //my index
    private Renderer myRenderer;    //my mesh
    GameManager gameManager;    //the game manager
    public Light myLight;   //my light
    Image targetIcon;   //my icon for when I am targeted

    // Start is called before the first frame update
    void Start()
    {
        //fetch components
        targetIcon = GetComponentInChildren<Image>();
        myRenderer = GetComponentInChildren<Renderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //set my colour
        GetColour();
    }

    public void GetColour()
    {
        //set my colour based on my gem index
        gemColour = gameManager.gemColour[gemIndex];
        gemColour.a = 200;
        myRenderer.material.color = gemColour;
        myLight.color = gemColour;
    }

    public void Restart()
    {
        //activate myself by default
        this.gameObject.SetActive(true);
        //deactivate my target icon by default
        targetIcon.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if the player is not targeting me
        if (PlayerAI.targetObject != this.gameObject)
            //disable my target icon
            targetIcon.enabled = false;
    }
}
