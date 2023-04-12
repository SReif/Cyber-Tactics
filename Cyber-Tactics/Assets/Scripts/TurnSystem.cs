using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public GameObject gridObject;           // The GameObject for the grid system for reference
    public GameObject playerObject;         // The gameobject for the player
    public GameObject enemyObject;          // The gameobject for the enemy

    //private bool showingValidMoves;       // Determines whether the system has already checked for the moves of the selected unit

    private GridSystem gridSystem;          // The grid system itself for easier reference
    private List<GameObject> playersUnits;  // The list of units the player controls
    private List<GameObject> enemysUnits;   // The list of units the enemy controls

    private int playersUnitsMoved;
    private int enemysUnitsMoved;

    private State state;                    // The current game state for determining turns

    private enum State
    {
        PlayerTurn,
        EnemyTurn,
    }

    // Start is called before the first frame update
    void Start()
    {
        // The player takes the first turn
        state = State.PlayerTurn;

        // Retrieve the grid system from the grid system GameObject
        gridSystem = gridObject.GetComponent<GridSystem>();

        // Retrieve the players units
        playersUnits = new List<GameObject>();
        for (int i = 0; i < playerObject.transform.childCount; i++)
        {
            playersUnits.Add(playerObject.transform.GetChild(i).gameObject);
        }

        // Retrieve the enemys units
        enemysUnits = new List<GameObject>();
        for (int i = 0; i < enemyObject.transform.childCount; i++)
        {
            enemysUnits.Add(enemyObject.transform.GetChild(i).gameObject);
        }

        playersUnitsMoved = 0;
        enemysUnitsMoved = 0;

        //showingValidMoves = false;
}

    // Update is called once per frame
    void Update()
    {
        if (state == State.PlayerTurn)
        {
            // The player can then select one of their units that have the "Playerunit" Tag

            if (playersUnitsMoved >= playerObject.transform.childCount)
            {
                // Switch to the enemy's turn after all units have moved
                state = State.EnemyTurn;

                // Revert the player's unit colors back to normal
                for (int i = 0; i < playerObject.transform.childCount; i++)
                {
                    playerObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }

                for (int i = 0; i < enemyObject.transform.childCount; i++)
                {
                    enemyObject.transform.GetChild(i).gameObject.GetComponent<Unit>().hasMoved = false;
                }

                enemysUnitsMoved = 0;

                Debug.Log("Starting enemy's turn!");
            }
            else
            {
                // Input System for Mouse Clicks (Selecting nodes and moving on grid)
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetMouseButtonDown(0) && hit.transform.tag == "PlayerUnit" && !hit.transform.gameObject.GetComponent<Unit>().hasMoved)
                    {
                        //Debug.Log("Unit selected.");

                        // Disable the selected unit indicator for the old object
                        if (gridSystem.selectedUnit != null)
                        {
                            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
                        }

                        // Select the new unit and activate its selected unit indicator
                        gridSystem.selectedUnit = hit.transform.gameObject;
                        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);

                        // Stop showing the valid moves for the previous unit
                        gridSystem.resetValidMoveNodes();

                        // Show the valid moves for the current unit
                        gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showPlayerValidMoves(gridSystem.grid);
                    }
                    else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node")
                    {
                        //Debug.Log("Node selected.");

                        // Check to see if the unit can move to that node
                        if (hit.transform.gameObject.GetComponent<GridNode>().validMove)
                        {
                            if (hit.transform.gameObject.GetComponent<GridNode>().currentUnit != null)
                            {
                                if (hit.transform.gameObject.GetComponent<GridNode>().currentUnit.tag == "EnemyUnit")
                                {
                                    // Defeat the unit (like Chess), remove it from the playersUnit list, and remove it from the game
                                    enemysUnits.Remove(hit.transform.gameObject.GetComponent<GridNode>().currentUnit);
                                    Destroy(hit.transform.gameObject.GetComponent<GridNode>().currentUnit);

                                    checkWinLoseCondition();
                                }
                            }

                            gridSystem.moveSelectedUnitMouseClick(hit.transform.gameObject);

                            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
                            gridSystem.resetValidMoveNodes();

                            //showingValidMoves = false;
                            gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
                            gridSystem.selectedUnit.GetComponent<SpriteRenderer>().color = Color.gray;

                            playersUnitsMoved++;
                        }
                        else
                        {
                            Debug.Log("Invalid move: Node not in moveset!");
                        }
                        
                    }
                    /*
                    else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "EnemyUnit" && gridSystem.selectedUnit != null)
                    {
                        // Check to see if the player unit can move to the enemy unit's node
                        if (hit.transform.gameObject.GetComponent<Unit>().currentNode.GetComponent<GridNode>().validMove)
                        {
                            gridSystem.moveSelectedUnitMouseClick(hit.transform.gameObject);

                            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
                            gridSystem.resetValidMoveNodes();

                            // Defeat the unit (like Chess), remove it from the enemysUnit list, and remove it from the game
                            enemysUnits.Remove(hit.transform.gameObject);
                            Destroy(hit.transform.gameObject);

                            checkWinLoseCondition();

                            //showingValidMoves = false;
                            gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;

                            playersUnitsMoved++;
                        }
                    }
                    */
                }
            }
        }
        else if (state == State.EnemyTurn)
        {
            // The enemy can then select one of their units that have the "EnemyUnit" Tag

            if (enemysUnitsMoved >= enemyObject.transform.childCount)
            {
                // Switch to the player's turn after all units have moved
                state = State.PlayerTurn;

                // Revert the enemy's unit colors back to normal
                for (int i = 0; i < enemyObject.transform.childCount; i++)
                {
                    enemyObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                }

                for (int i = 0; i < playerObject.transform.childCount; i++)
                {
                    playerObject.transform.GetChild(i).gameObject.GetComponent<Unit>().hasMoved = false;
                }

                playersUnitsMoved = 0;

                Debug.Log("Starting player's turn!");
            }
            else
            {
                // Select the newest unit to move once and only show its valid moves once
                /*
                if (!showingValidMoves)
                {
                    gridSystem.selectedUnit = playerUnits[0].transform.GetChild(0).gameObject;
                    gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);
                    gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);

                    showingValidMoves = true;
                }
                */

                // Input System for Mouse Clicks (Selecting nodes and moving on grid)
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetMouseButtonDown(0) && hit.transform.tag == "EnemyUnit" && !hit.transform.gameObject.GetComponent<Unit>().hasMoved)
                    {
                        //Debug.Log("Unit selected.");

                        // Disable the selected unit indicator for the old object
                        if (gridSystem.selectedUnit != null)
                        {
                            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
                        }

                        // Select the new unit and activate its selected unit indicator
                        gridSystem.selectedUnit = hit.transform.gameObject;
                        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);

                        // Stop showing the valid moves for the previous unit
                        gridSystem.resetValidMoveNodes();

                        // Show the valid moves for the current unit
                        gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showEnemyValidMoves(gridSystem.grid);
                    }
                    else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node")
                    {
                        //Debug.Log("Node selected.");

                        // Check to see if the unit can move to that node
                        if (hit.transform.gameObject.GetComponent<GridNode>().validMove)
                        {
                            if (hit.transform.gameObject.GetComponent<GridNode>().currentUnit != null)
                            {
                                if (hit.transform.gameObject.GetComponent<GridNode>().currentUnit.tag == "PlayerUnit")
                                {
                                    // Defeat the unit (like Chess), remove it from the playersUnit list, and remove it from the game
                                    playersUnits.Remove(hit.transform.gameObject.GetComponent<GridNode>().currentUnit);
                                    Destroy(hit.transform.gameObject.GetComponent<GridNode>().currentUnit);

                                    checkWinLoseCondition();
                                }
                            }

                            gridSystem.moveSelectedUnitMouseClick(hit.transform.gameObject);

                            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
                            gridSystem.resetValidMoveNodes();

                            //showingValidMoves = false;
                            gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
                            gridSystem.selectedUnit.GetComponent<SpriteRenderer>().color = Color.gray;

                            enemysUnitsMoved++;
                        }
                        else
                        {
                            Debug.Log("Invalid move: Node not in moveset!");
                        }
                    }
                    /*
                    else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "PlayerUnit" && gridSystem.selectedUnit != null)
                    {
                        Debug.Log("Enemy attacking!");

                        // Check to see if the enemy unit can move to the player unit's node
                        if (hit.transform.gameObject.GetComponent<Unit>().currentNode.GetComponent<GridNode>().validMove)
                        {
                            Debug.Log("Test!");

                            gridSystem.moveSelectedUnitMouseClick(hit.transform.gameObject);

                            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
                            gridSystem.resetValidMoveNodes();

                            // Defeat the unit (like Chess), remove it from the playersUnit list, and remove it from the game
                            playersUnits.Remove(hit.transform.gameObject);
                            Destroy(hit.transform.gameObject);

                            checkWinLoseCondition();

                            //showingValidMoves = false;
                            gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;

                            enemysUnitsMoved++;
                        }
                    }
                    */
                }
            }
        }
    }

    private void checkWinLoseCondition()
    {
        if (enemysUnits.Count == 0)
        {
            //WinLoseManager.Win();
            print("Player wins!");
        }
        else if (playersUnits.Count == 0)
        {
            //WinLoseManager.Lose();
            print("Player loses!");
        }
    }
}
