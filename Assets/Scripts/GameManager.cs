using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private Text scoreText;    //our text asset to display the score
    [SerializeField] private int scoreMax;  //our maximum score
    [SerializeField] public int scoreCurrent;   //our current score

    [Header("Game Objects")]
    [SerializeField]
    GameObject[] wisps; //all wisps in the scene
    [SerializeField]
    GameObject[] doors; //all doors in the scene
    [SerializeField]
    GameObject[] gems;  //all gems in the scene
    [SerializeField]
    GameObject[] enemies;   //all enemies in the scene
    [SerializeField]
    PlayerAI player;    //the player

    [Header("Gem Colours")]
    [SerializeField]
    public Color32[] gemColour; //our possible gem colours

    [Header("Game End Panels")]
    [SerializeField]
    GameObject winPanel;    //our win panel
    [SerializeField]
    GameObject losePanel;   //our lose panel

    void Start()
    {
        //get all our game objects for our lists
        player = GameObject.Find("Player").GetComponent<PlayerAI>();
        wisps = GameObject.FindGameObjectsWithTag("Wisp");
        doors = GameObject.FindGameObjectsWithTag("Door");
        gems = GameObject.FindGameObjectsWithTag("Gem");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //set our max score to the number of wisps
        scoreMax = wisps.Length;
        //update our score display
        UpdateScore();
    }

    public void UpdateScore()
    {
        //update the text display
        scoreText.text = scoreCurrent.ToString() + " / " + scoreMax.ToString() + "\nWisps";
        //if we hit max score, trigger game end in a win state
        if (scoreCurrent == scoreMax)
            GameEnd(true);
    }

    public void GameEnd(bool win)
    {
        //trigger the panels according to win state
        winPanel.SetActive(win);
        losePanel.SetActive(!win);
        //deactivate the player
        player.PlayerActive(false);
    }

    public void Restart()
    {
        //trigger restart on all wisps
        for (int i = 0; i < wisps.Length; i++)
        {
            WispAI wisp = wisps[i].GetComponent<WispAI>();
            wisp.Restart();
        }
        //trigger restart on all doors
        for (int i = 0; i < doors.Length; i++)
        {
            DoorController door = doors[i].GetComponent<DoorController>();
            door.Restart();
        }
        //trigger restart on all gems
        for (int i = 0; i < gems.Length; i++)
        {
            GemController gem = gems[i].GetComponent<GemController>();
            gem.Restart();
        }
        //trigger restart on all enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyAI enemy = enemies[i].GetComponent<EnemyAI>();
            enemy.Restart();
        }
        //trigger restart on the player
        player.Restart();
        //reset the score to 0
        scoreCurrent = 0;
        //update the score
        UpdateScore();
    }

    public void QuitGame()
    {
        //if we're in the editor, end play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        //quit the game
        Application.Quit();
    }

}
