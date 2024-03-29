using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the SceneManager game object
public class WinLoseManager : MonoBehaviour
{
    //[System.NonSerialized] public GameObject[] enemies; //array of enemy units
    //[System.NonSerialized] public GameObject[] allies; //array of player units
    [SerializeField] private GameObject playerLeader;       // Player leader unit
    [SerializeField] private bool doesPlayerLeaderExist;    // Whether or not the lose condition for the player losing their leader applies in this level; if false, leave playerLeader blank.

    [SerializeField] private GameObject enemyLeader;        // Enemy leader unit
    [SerializeField] private bool doesEnemyLeaderExist;     // Whether or not the win condition for the player defeating the enemy leader applies in this level; if false, leave enemyLeaderBlank

    [SerializeField] private GameObject objectiveNode;      // The node used for the win/lose tile objective condition (Only supports singular tile objective)
    [SerializeField] private bool isPlayerObjective;        // Whether or not the player is completing the tile objective; if true, the player is. If false, the enemy is.

    public GridSystem gridSystem;
    public UIManager uiManager;
    public bool winLoseConditionMet;        // Whether or not the win condition has been met
    public bool win, lose;

    private TurnSystem turnSystem;

    // Start is called before the first frame update
    public void Start()
    {
        //Instantiating variables
        if(GameObject.Find("UIManager") != null)
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }

        if (GameObject.Find("Grid Turn System") != null)
        {
            turnSystem = GameObject.Find("Grid Turn System").GetComponent<TurnSystem>();
        }

        winLoseConditionMet = false;
        win = false;
        lose = false;

        //Fill unit arrays
        //enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        //allies = GameObject.FindGameObjectsWithTag("PlayerUnit");
    }

    // Update is called once per frame
    public void Update()
    {
        // Check to see if there are no more units in the scenario for either side
        if (turnSystem.enemysUnits.Count == 0 && turnSystem.playersUnits.Count == 0)
        {
            winLoseConditionMet = true;

            Lose();
            Debug.Log("No more allies or enemies!");
        }
        else if (turnSystem.enemysUnits.Count == 0)
        {
            winLoseConditionMet = true;

            Win();
            Debug.Log("No more enemies!");
        }
        else if (turnSystem.playersUnits.Count == 0)
        {
            winLoseConditionMet = true;

            Lose();
            Debug.Log("No more allies!");
        }

        // Check to see if there are no more leader units in the scenario for either side.
        if (enemyLeader == null && playerLeader == null && doesPlayerLeaderExist && doesEnemyLeaderExist)
        {
            winLoseConditionMet = true;

            Lose();
            Debug.Log("Enemy and player leaders defeated!");
        }
        else if (enemyLeader == null && doesEnemyLeaderExist)
        {
            winLoseConditionMet = true;

            Win();
            Debug.Log("Enemy leader defeated!");
        }
        else if (playerLeader == null && doesPlayerLeaderExist)
        {
            winLoseConditionMet = true;

            Lose();
            Debug.Log("Player leader defeated!");
        }

        // Check to see if a unit has moved onto the objective node, if it exists
        if (objectiveNode != null && objectiveNode.transform.Find("Unit Slot").childCount > 0)
        {
            if (isPlayerObjective && objectiveNode.transform.Find("Unit Slot").GetChild(0).gameObject.tag == "PlayerUnit")
            {
                winLoseConditionMet = true;

                Win();
                Debug.Log("Player completed tile objective!");
            }
            else if (!isPlayerObjective && objectiveNode.transform.Find("Unit Slot").GetChild(0).gameObject.tag == "EnemyUnit")
            {
                winLoseConditionMet = true;

                Lose();
                Debug.Log("Enemy completed tile objective!");
            }
        }
    }

    //If win conditions are met, this function will execute
    public void Win()
    {
        win = true;
        uiManager.OpenPanel(uiManager.winPane);
        uiManager.ClosePanel(uiManager.turnPane);
        uiManager.ClosePanel(uiManager.viewedUnitPane);
        uiManager.ClosePanel(uiManager.selectedUnitPane);
        //uiManager.Pause();
    }

    //If lose conditions are met, this function will execute
    public void Lose()
    {
        lose = true; 
        uiManager.OpenPanel(uiManager.losePane);
        uiManager.ClosePanel(uiManager.turnPane);
        uiManager.ClosePanel(uiManager.selectedUnitPane);
        uiManager.ClosePanel(uiManager.viewedUnitPane);
        //uiManager.Pause();
    }


}
