using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //This scripts gets attached to the Canvas game object
    private bool paused;
    private bool playerTurn;

    public GameObject turnPane;
    public GameObject unitPane;
    public GameObject pausePane;
    public GameObject winPane;
    public GameObject losePane;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true;
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextTurn();
        }
    }
    
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
    
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    //Pauses game
    private void Pause()
    {
        if(paused)
        {
            Time.timeScale = 1;
            pausePane.SetActive(false);
            paused = false;
        }

        else
        {
            pausePane.SetActive(true);
            paused = true;
            Time.timeScale = 0;
        }
    }

    //Changes current turn text
    private void NextTurn()
    {
        if(!playerTurn)
        {
            TextMeshProUGUI tempText = turnPane.GetComponentInChildren<TextMeshProUGUI>();
            tempText.text = "Player Turn";
            tempText.color = Color.blue;
            playerTurn = true;
        }

        else
        {
            TextMeshProUGUI tempText = turnPane.GetComponentInChildren<TextMeshProUGUI>();
            tempText.text = "Enemy Turn";
            tempText.color = Color.red;
            playerTurn = false;
        }
    }
}
