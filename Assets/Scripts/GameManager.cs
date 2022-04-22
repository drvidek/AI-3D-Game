using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private int scoreMax;
    [SerializeField] public int scoreCurrent;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] wispCount = GameObject.FindGameObjectsWithTag("Wisp");
        scoreMax = wispCount.Length;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = scoreCurrent.ToString() + " / " + scoreMax.ToString();
    }
}
