using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private int scoreMax;
    [SerializeField] public int scoreCurrent;

    [SerializeField]
    GameObject[] wisps;
    [SerializeField]
    GameObject[] doors;
    [SerializeField]
    GameObject[] gems;
    [SerializeField]
    GameObject[] enemies;

    [SerializeField]
    PlayerAI player;

    [SerializeField]
    public Color32[] gemColour;

    [SerializeField]
    GameObject winPanel;
    [SerializeField]
    GameObject losePanel;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerAI>();
        wisps = GameObject.FindGameObjectsWithTag("Wisp");
        doors = GameObject.FindGameObjectsWithTag("Door");
        gems = GameObject.FindGameObjectsWithTag("Gem");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        scoreMax = wisps.Length;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = scoreCurrent.ToString() + " / " + scoreMax.ToString() + "\nWisps";
        if (scoreCurrent == scoreMax)
            GameEnd(true);
    }

    public void GameEnd(bool win)
    {
        winPanel.SetActive(win);
        losePanel.SetActive(!win);
        player.PlayerActive(false);
    }

    public void Restart()
    {

        for (int i = 0; i < wisps.Length; i++)
        {
            WispAI wisp = wisps[i].GetComponent<WispAI>();
            wisp.Restart();
            Debug.Log("Restarted Wisps");
        }

        for (int i = 0; i < doors.Length; i++)
        {
            DoorController door = doors[i].GetComponent<DoorController>();
            door.Restart();
            Debug.Log("Restarted Doors");

        }

        for (int i = 0; i < gems.Length; i++)
        {
            GemController gem = gems[i].GetComponent<GemController>();
            gem.Restart();
            Debug.Log("Restarted Gems");

        }

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyAI enemy = enemies[i].GetComponent<EnemyAI>();
            enemy.Restart();
            Debug.Log("Restarted Enemies");

        }

        player.Restart();
        Debug.Log("Restarted Player");

        scoreCurrent = 0;
        UpdateScore();
            Debug.Log("Restarted All");
        

    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

}
