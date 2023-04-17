using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This scripts gets attached to the Canvas game object as it makes calls to the child objects
public class UIManager : MonoBehaviour
{
    private bool paused;

    public GameObject turnPane;
    public GameObject unitPane;
    public GameObject pausePane;
    public GameObject winPane;
    public GameObject losePane;

    private string unitType;
    private int unitHp;
    private int unitPRes;
    private int unitMRes;
    private int unitPDmg;
    private int unitMDmg;

    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private GridSystem gridSystem;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiating variables if necessary
        paused = false;
        if(GameObject.Find("Turn System") != null)
        {
            turnSystem = GameObject.Find("Turn System").GetComponent<TurnSystem>();
        }
        
        if(GameObject.Find("Square Grid System") != null)
        {
            gridSystem = GameObject.Find("Square Grid System").GetComponent<GridSystem>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if(GameObject.Find("Square Grid System") != null)
        {
            ToggleStatsPane();
            NextTurn();
        }
    }
    
    //Opens specified panel
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
    
    //Closes specified panel
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    //Pauses game
    public void Pause()
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

    //Toggles the Stats pane on/off depending if a unit is selected or not
    //Stats pane is filled with seleced unit's attributes
    private void ToggleStatsPane()
    {
        if (gridSystem.selectedUnit != null)
        {
            OpenPanel(unitPane);
            unitType = gridSystem.selectedUnit.GetComponent<Unit>().unitID;
            unitHp = gridSystem.selectedUnit.GetComponent<Unit>().currentHP;
            unitPRes = gridSystem.selectedUnit.GetComponent<Unit>().baseDefense;
            unitMRes = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicDefense;
            unitPDmg = gridSystem.selectedUnit.GetComponent<Unit>().baseAttack;
            unitMDmg = gridSystem.selectedUnit.GetComponent<Unit>().baseMagic;

            GameObject.Find("Title Text").GetComponent<TextMeshProUGUI>().text = unitType;
            GameObject.Find("Health Number Text").GetComponent<TextMeshProUGUI>().text = "" + unitHp;
            GameObject.Find("PR Number Text").GetComponent<TextMeshProUGUI>().text = "" + unitPRes;
            GameObject.Find("MR Number Text").GetComponent<TextMeshProUGUI>().text = "" + unitMRes;
            GameObject.Find("PDmg Number Text").GetComponent<TextMeshProUGUI>().text = "" + unitPDmg;
            GameObject.Find("MDmg Number Text").GetComponent<TextMeshProUGUI>().text = "" + unitMDmg;
        }

        if (gridSystem.selectedUnit == null)
        {
            ClosePanel(unitPane);
        }
    }

    //Changes current turn text at top of the game screen to either player/enemy
    private void NextTurn()
    {

        if (turnSystem.state == TurnSystem.State.PlayerTurn)
        {
            TextMeshProUGUI tempText = turnPane.GetComponentInChildren<TextMeshProUGUI>();
            tempText.text = "Player Turn";
            tempText.color = Color.red;
        }

        else
        {
            TextMeshProUGUI tempText = turnPane.GetComponentInChildren<TextMeshProUGUI>();
            tempText.text = "Enemy Turn";
            tempText.color = Color.blue;
        }
    }
}
