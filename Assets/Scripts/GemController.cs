using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemController : MonoBehaviour
{
    private Color32 gemColour;
    public int gemIndex;
    private Renderer myRenderer;
    GameManager gameManager;
    public Light myLight;
    Image selectIcon;

    // Start is called before the first frame update
    void Start()
    {
        selectIcon = GetComponentInChildren<Image>();
        myRenderer = GetComponentInChildren<Renderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetColour();
    }

    public void GetColour()
    {
        gemColour = gameManager.gemColour[gemIndex];
        gemColour.a = 200;
        myRenderer.material.color = gemColour;
        myLight.color = gemColour;
    }

    public void Restart()
    {
        this.gameObject.SetActive(true);
        selectIcon.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerAI.targetObject != this.gameObject)
            selectIcon.enabled = false;
    }
}
