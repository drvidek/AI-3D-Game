using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Color32 myColour;
    public int gemIndex;
    [SerializeField] private Renderer myRenderer;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(myRenderer);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetColour();
    }

    public void GetColour()
    {
        myColour = gameManager.gemColour[gemIndex];
        myColour.a = 255;
        myRenderer.material.color = myColour;
    }

    public void UnlockCheck(int key)
    {
        if (key == gemIndex)
        {
            Debug.Log("Unlock");
            Destroy(this.gameObject);
        }
    }
}
