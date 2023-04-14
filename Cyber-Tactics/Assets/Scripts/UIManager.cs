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

    private string unitType;

    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private GridSystem gridSystem;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true;
        paused = false;
        turnSystem = GameObject.Find("Turn System").GetComponent<TurnSystem>();
        gridSystem = GameObject.Find("Square Grid System").GetComponent<GridSystem>();
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

        ToggleStatsPane();

        //NextTurn();
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
    private void ToggleStatsPane()
    {
        if (gridSystem.selectedUnit != null)
        {
            OpenPanel(unitPane);
            unitType = gridSystem.selectedUnit.GetComponent<Unit>().unitID;
            unitPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = unitType;
        }

        if (gridSystem.selectedUnit == null)
        {
            ClosePanel(unitPane);
        }
    }

    //Changes current turn text
    //Requires Enum State to be Public and State state to be public
    private void NextTurn()
    {
        if(!playerTurn)
        {
            TextMeshProUGUI tempText = turnPane.GetComponentInChildren<TextMeshProUGUI>();
            tempText.text = "Player Turn";
            tempText.color = Color.red;
            playerTurn = true;
        }

        else
        {
            TextMeshProUGUI tempText = turnPane.GetComponentInChildren<TextMeshProUGUI>();
            tempText.text = "Enemy Turn";
            tempText.color = Color.blue;
            playerTurn = false;
        }

        /*if (turnSystem.state == TurnSystem.State.PlayerTurn)
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
        }*/
    }
}
