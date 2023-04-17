using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour
{
    public GameObject gridObject;           // The GameObject for the grid system for reference
    public GameObject playerObject;         // The GameObject for the player
    public GameObject enemyObject;          // The GameObject for the enemy
    public GameObject battleSystem;         // The GameObject for the battle system
    public GameObject gridViewCamera;       // The GameObject for the camera that faces the grid

    public State state;                     // The current game state for determining turns

    private GridSystem gridSystem;          // The grid system itself for easier reference
    private List<GameObject> playersUnits;  // The list of units the player controls
    private List<GameObject> enemysUnits;   // The list of units the enemy controls

    private int playersUnitsMoved;          // The number of player units that have already moved
    private int enemysUnitsMoved;           // The number of enemy units that have already moved
    private bool winConditionMet;           // Whether the win condition has been met (Currently, that is when there are no longer any units for a particular side)

    public enum State
    {
        PlayerTurn,
        EnemyTurn,
    }

    // Start is called before the first frame update
    void Start()
    {
        // The player takes the first turn
        state = State.PlayerTurn;
        winConditionMet = false;

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

        StartCoroutine(GridTurnSystem());
}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void checkWinLoseCondition()
    {
        if (enemysUnits.Count == 0)
        {
            // INSERT SCENE FUNCTIONALITY FOR PLAYER WINNING HERE

            print("Player wins!");
            winConditionMet = true;
        }
        else if (playersUnits.Count == 0)
        {
            // INSERT SCENE FUNCTIONALITY FOR PLAYER LOSING HERE

            print("Player loses!");
            winConditionMet = true;
        }
    }

    IEnumerator GridTurnSystem()
    {
        while (!winConditionMet)
        {
            if (state == State.PlayerTurn)
            {
                // The player can then select one of their units that have the "PlayerUnit" Tag

                // Check to see if the player has moved all of their units
                if (playersUnitsMoved >= playerObject.transform.childCount)
                {
                    // Switch to the enemy's turn after all units have moved
                    state = State.EnemyTurn;

                    // Revert the player's unit colors back to their default
                    for (int i = 0; i < playerObject.transform.childCount; i++)
                    {
                        playerObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    }

                    // Prepare the enemy units for their turn
                    for (int i = 0; i < enemyObject.transform.childCount; i++)
                    {
                        enemyObject.transform.GetChild(i).gameObject.GetComponent<Unit>().hasMoved = false;
                    }

                    enemysUnitsMoved = 0;

                    Debug.Log("Starting enemy's turn!");
                }
                else
                {
                    var ray = gridViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Input.GetMouseButtonDown(0) && hit.transform.tag == "PlayerUnit" && !hit.transform.gameObject.GetComponent<Unit>().hasMoved)
                        {
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
                            gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);
                        }
                        else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node")
                        {
                            // Check to see if the unit can move to that node
                            if (hit.transform.gameObject.GetComponent<GridNode>().validMove)
                            {
                                // Move the unit and disable the move indicators for each node
                                gridSystem.moveSelectedUnitMouseClick(hit.transform.gameObject);
                                gridSystem.resetValidMoveNodes();

                                // Wait a moment so you can't double-click accidentally
                                yield return new WaitForSeconds(.3f);

                                // Determine the valid attacks the unit can perform
                                gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "EnemyUnit");

                                // Choose a unit to attack, if any
                                yield return StartCoroutine(chooseEnemyUnitToAttack());

                                // Disable the selected unit indicator for the unit, if it still exists
                                if (gridSystem.selectedUnit != null)
                                {
                                    // Disable the attack indicator for the unit
                                    gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

                                    // Show that the unit cannot be moved the rest of this turn
                                    gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
                                    gridSystem.selectedUnit.GetComponent<SpriteRenderer>().color = Color.gray;

                                    // Do not increment this value if the unit is defeated, or else the player's turn will end early when there are less units on the board
                                    playersUnitsMoved++;

                                    gridSystem.selectedUnit = null;
                                }

                                gridSystem.resetValidAttackNodes();
                            }
                            else
                            {
                                Debug.Log("Invalid move: Node not in moveset!");
                            }

                        }
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

                    // Prepare the player units for their turn
                    for (int i = 0; i < playerObject.transform.childCount; i++)
                    {
                        playerObject.transform.GetChild(i).gameObject.GetComponent<Unit>().hasMoved = false;
                    }

                    playersUnitsMoved = 0;

                    Debug.Log("Starting player's turn!");
                }
                else
                {
                    // Input System for Mouse Clicks (Selecting nodes and moving on grid)
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Input.GetMouseButtonDown(0) && hit.transform.tag == "EnemyUnit" && !hit.transform.gameObject.GetComponent<Unit>().hasMoved)
                        {
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
                            gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);
                        }
                        else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node")
                        {
                            // Check to see if the unit can move to that node
                            if (hit.transform.gameObject.GetComponent<GridNode>().validMove)
                            {
                                // Move the unit and disable the move indicators for each node
                                gridSystem.moveSelectedUnitMouseClick(hit.transform.gameObject);
                                gridSystem.resetValidMoveNodes();

                                //Wait a moment so you can't double-click accidentally
                                yield return new WaitForSeconds(.3f);

                                // Determine the valid attacks the unit can perform
                                gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "PlayerUnit");

                                // Choose a unit to attack, if any
                                yield return StartCoroutine(choosePlayerUnitToAttack());

                                // Disable the selected unit indicator for the unit, if it still exists
                                if (gridSystem.selectedUnit != null)
                                {
                                    // Disable the attack indicator for the unit
                                    gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

                                    // Show that the unit cannot be moved the rest of this turn
                                    gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
                                    gridSystem.selectedUnit.GetComponent<SpriteRenderer>().color = Color.gray;

                                    // Do not increment this value if the unit is defeated, or else the enemy's turn will end early when there are less units on the board
                                    enemysUnitsMoved++;

                                    gridSystem.selectedUnit = null;
                                }

                                gridSystem.resetValidAttackNodes();
                            }
                            else
                            {
                                Debug.Log("Invalid move: Node not in moveset!");
                            }
                        }
                    }
                }
            }

            yield return null;
        }
        
        yield return null;
    }

    IEnumerator chooseEnemyUnitToAttack()
    {
        yield return new WaitForSeconds(1);
        
        bool hasAttacked = false;

        // Check to see if there are any valid attacks
        if (gridSystem.validAttackNodes.Count > 0)
        {
            // End this loop once the unit has attacked
            while (!hasAttacked)
            {
                var ray = gridViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check to see if a valid node has been clicked on
                    if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node" && hit.transform.gameObject.GetComponent<GridNode>().validAttack)
                    {
                        // Check to see if the unit wants to attack
                        if (hit.transform.gameObject.GetComponent<GridNode>().currentUnit == gridSystem.GetComponent<GridSystem>().selectedUnit)
                        {
                            // Rather than having to attack an opposing unit, the unit can "click" on itself, forfeiting the attack action this turn
                            hasAttacked = true;
                        }
                        else
                        {
                            // Start a battle with that unit, where the player starts first
                            yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(gridSystem.selectedUnit, hit.transform.gameObject.GetComponent<GridNode>().currentUnit, "Player"));
                            hasAttacked = true;

                            checkIfUnitDefeated(gridSystem.selectedUnit, hit.transform.gameObject.GetComponent<GridNode>().currentUnit);

                            checkWinLoseCondition();
                        }
                    }
                }

                yield return null;
            }
        }

        yield return null;
    }

    IEnumerator choosePlayerUnitToAttack()
    {
        bool hasAttacked = false;

        // Check to see if there are any valid attacks
        if (gridSystem.validAttackNodes.Count > 0)
        {
            // End this loop once the unit has attacked
            while (!hasAttacked)
            {
                var ray = gridViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check to see if a valid node has been clicked on
                    if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node" && hit.transform.gameObject.GetComponent<GridNode>().validAttack)
                    {
                        // Check to see if the unit wants to attack
                        if (hit.transform.gameObject.GetComponent<GridNode>().currentUnit == gridSystem.GetComponent<GridSystem>().selectedUnit)
                        {
                            // Rather than having to attack an opposing unit, the unit can "click" on itself, forfeiting the attack action this turn
                            hasAttacked = true;
                        }
                        else
                        {
                            // Start a battle with that unit, where the player starts first
                            yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(hit.transform.gameObject.GetComponent<GridNode>().currentUnit, gridSystem.selectedUnit, "Enemy"));
                            hasAttacked = true;

                            // CHECK FOR WIN CONDITION HERE
                            checkIfUnitDefeated(hit.transform.gameObject.GetComponent<GridNode>().currentUnit, gridSystem.selectedUnit);

                            checkWinLoseCondition();
                        }
                    }
                }

                yield return null;
            }
        }

        yield return null;
    }

    private void checkIfUnitDefeated(GameObject playerUnit, GameObject enemyUnit)
    {
        if (playerUnit.GetComponent<Unit>().currentHP <= 0 && enemyUnit.GetComponent<Unit>().currentHP <= 0)
        {
            // INSERT FUNCTIONALITY FOR BOTH UNITS DYING HERE

            playersUnits.Remove(playerUnit);
            Destroy(playerUnit);

            playersUnits.Remove(enemyUnit);
            Destroy(enemyUnit);

            Debug.Log("Both units are defeated!");
        }
        else if (enemyUnit.GetComponent<Unit>().currentHP <= 0)
        {
            // INSERT FUNCTIONALITY FOR PLAYER VICTORY HERE
            enemysUnits.Remove(enemyUnit);
            Destroy(enemyUnit);

            Debug.Log("Enemy unit defeated!");
        }
        else if (playerUnit.GetComponent<Unit>().currentHP <= 0)
        {
            // INSERT FUNCTIONALITY FOR ENEMY VICTORY HERE
            playersUnits.Remove(playerUnit);
            Destroy(playerUnit);

            Debug.Log("Player unit defeated!");
        }
    }
}
