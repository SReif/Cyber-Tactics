using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour
{
    public GameObject gridObject;                   // The GameObject for the grid system for reference
    public GameObject playerObject;                 // The GameObject for the player
    public GameObject enemyObject;                  // The GameObject for the enemy
    public GameObject battleSystem;                 // The GameObject for the battle system
    public GameObject gridViewCamera;               // The GameObject for the camera that faces the grid

    public State state;                             // The current game state for determining turns

    private GridSystem gridSystem;                  // The grid system itself for easier reference
    private List<GameObject> playersUnits;          // The list of units the player controls
    private List<GameObject> enemysUnits;           // The list of units the enemy controls
    private List<GameObject> enemysUnitsNotMoved;   // The list of units the enemy has moved yet

    private int playersUnitsMoved;                  // The number of player units that have already moved
    private int enemysUnitsMoved;                   // The number of enemy units that have already moved
    private bool winConditionMet;                   // Whether the win condition has been met (Currently, that is when there are no longer any units for a particular side)

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

        Debug.Log("Player units retrieved.");

        // Retrieve the enemys units
        enemysUnits = new List<GameObject>();
        for (int i = 0; i < enemyObject.transform.childCount; i++)
        {
            enemysUnits.Add(enemyObject.transform.GetChild(i).gameObject);
        }

        Debug.Log("Enemy units retrieved.");

        playersUnitsMoved = 0;
        enemysUnitsMoved = 0;
        enemysUnitsNotMoved = enemysUnits;

        Debug.Log("Beginning grid turn system. Player goes first.");

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
                    enemysUnitsNotMoved = enemysUnits;

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
                                gridSystem.moveSelectedUnit(hit.transform.gameObject);

                                // Wait a moment so you can't double-click accidentally
                                yield return new WaitForSeconds(.3f);

                                // Determine the valid attacks the unit can perform
                                gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "EnemyUnit");

                                // Choose a unit to attack, if any
                                yield return StartCoroutine(chooseUnitToAttack("Player"));

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

                                Debug.Log("Unit has taken its turn.");
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
                    enemysUnitsNotMoved = new List<GameObject>();

                    Debug.Log("Starting player's turn!");
                }
                else
                {
                    yield return StartCoroutine(calculateBetterEnemyAI());

                    Debug.Log("Unit has taken its turn.");
                }
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator enemyAIRandomlyChooseUnitToAttack()
    {
        yield return new WaitForSeconds(.3f);

        bool hasAttacked = false;

        // Check to see if there are any valid attacks
        if (gridSystem.validAttackNodes.Count > 0)
        {
            // End this loop once the unit has attacked
            while (!hasAttacked)
            {
                // Select a random valid attack for the enemy unit
                int index = Random.Range(0, gridSystem.validAttackNodes.Count);
                GameObject node = gridSystem.validAttackNodes[index];

                if (node.GetComponent<GridNode>().currentUnit == gridSystem.GetComponent<GridSystem>().selectedUnit)
                {
                    // Rather than having to attack an opposing unit, the unit can select itself, forfeiting the attack action this turn
                    hasAttacked = true;
                }
                else
                {
                    // Start a battle with that unit, where the enemy starts first
                    yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(node.GetComponent<GridNode>().currentUnit, gridSystem.selectedUnit, "Enemy"));

                    hasAttacked = true;
                    checkIfUnitDefeated(gridSystem.selectedUnit, node.GetComponent<GridNode>().currentUnit);
                    checkWinLoseCondition();
                }

                yield return null;
            }
        }

        yield return null;
    }

    IEnumerator chooseUnitToAttack(string currentUnitSide)
    {
        yield return new WaitForSeconds(.3f);

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
                            Debug.Log("Opposing unit detected, commence battle.");


                            if (currentUnitSide == "Player")
                            {
                                // Start a battle with that unit, where the player starts first
                                yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(gridSystem.selectedUnit, hit.transform.gameObject.GetComponent<GridNode>().currentUnit, "Player"));

                                hasAttacked = true;
                                checkIfUnitDefeated(gridSystem.selectedUnit, hit.transform.gameObject.GetComponent<GridNode>().currentUnit);
                                checkWinLoseCondition();
                            }
                            else
                            {
                                // Start a battle with that unit, where the player starts first
                                yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(hit.transform.gameObject.GetComponent<GridNode>().currentUnit, gridSystem.selectedUnit, "Enemy"));

                                hasAttacked = true;
                                checkIfUnitDefeated(hit.transform.gameObject.GetComponent<GridNode>().currentUnit, gridSystem.selectedUnit);
                                checkWinLoseCondition();
                            }


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

    IEnumerator calculateRandomizedEnemyAI()
    {
        // Select a random enemy unit (Repeat until you find a unit that has not moved yet.)
        int index = Random.Range(0, enemysUnits.Count);
        while (enemysUnits[index].GetComponent<Unit>().hasMoved)
        {
            index = Random.Range(0, enemysUnits.Count);
        }

        gridSystem.selectedUnit = enemysUnits[index];
        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);
        gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);

        yield return new WaitForSeconds(.3f);

        // Move the enemy unit to a random valid move
        index = Random.Range(0, gridSystem.validMoveNodes.Count);
        GameObject node = gridSystem.validMoveNodes[index];
        gridSystem.moveSelectedUnit(node);
        gridSystem.resetValidMoveNodes();

        yield return new WaitForSeconds(.3f);

        // Find all of the valid attack nodes for the enemy unit and select one
        gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "PlayerUnit");
        yield return StartCoroutine(enemyAIRandomlyChooseUnitToAttack());

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

    IEnumerator calculateBetterEnemyAI()
    {
        // Select a random enemy unit (Repeat until you find a unit that has not moved yet.)
        int index = Random.Range(0, enemysUnits.Count);
        while (enemysUnits[index].GetComponent<Unit>().hasMoved)
        {
            index = Random.Range(0, enemysUnits.Count);
        }

        gridSystem.selectedUnit = enemysUnits[index];
        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);
        gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);

        // Store the valid moves nodes that lead to an attack on a player unit
        List<GameObject> targets = new List<GameObject>();

        // For each valid move node, check to see if a valid attack at that location can attack a player unit
        for (int k = 0; k < gridSystem.validMoveNodes.Count; k++)
        {
            GameObject validMoveNode = gridSystem.validMoveNodes[k];

            List<List<Vector2>> unitAttackSet = gridSystem.selectedUnit.GetComponent<Unit>().calculateAdjacentSquareAttackSet();

            // For each attack in the attack set...
            for (int i = 0; i < unitAttackSet.Count; i++)
            {
                for (int j = 0; j < unitAttackSet[i].Count; j++)
                {
                    // Check to see if the attack is outside of the grid
                    if (unitAttackSet[i][j].x + validMoveNode.GetComponent<GridNode>().nodeGridPos.x < gridSystem.grid.GetLength(0) && unitAttackSet[i][j].x + validMoveNode.GetComponent<GridNode>().nodeGridPos.x >= 0
                        && unitAttackSet[i][j].y + validMoveNode.GetComponent<GridNode>().nodeGridPos.y < gridSystem.grid.GetLength(1) && unitAttackSet[i][j].y + validMoveNode.GetComponent<GridNode>().nodeGridPos.y >= 0)
                    {
                        // Obtain the node that the unit wants to try to attack
                        GameObject node = gridSystem.grid[(int)(unitAttackSet[i][j].x + validMoveNode.GetComponent<GridNode>().nodeGridPos.x),
                            (int)(unitAttackSet[i][j].y + validMoveNode.GetComponent<GridNode>().nodeGridPos.y)];

                        // Determine if there is a unit on the node
                        if (node.GetComponent<GridNode>().currentUnit != null)
                        {
                            // Check to see if the unit is an enemy unit
                            if (node.GetComponent<GridNode>().currentUnit.tag == "PlayerUnit")
                            {
                                /*
                                // Change the color of the node to show that it is a valid attack
                                node.transform.Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = true;

                                // This is used later to ensure that you cannot select a node outside of the attackset
                                node.GetComponent<GridNode>().validAttack = true;

                                validAttackNodes.Add(node);
                                */

                                targets.Add(validMoveNode);

                                // If the unit attackset does not allow the unit to attack over units, prevent it from calculating more attacks past this node
                                if (unitAttackSet.Count > 1)
                                {
                                    break;
                                }
                            }
                            else if (node.GetComponent<GridNode>().currentUnit == transform.gameObject)
                            {
                                /*
                                // Change the color of the node to show that it is a valid attack
                                node.transform.Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = true;

                                // This is used later to ensure that you cannot select a node outside of the moveset
                                node.GetComponent<GridNode>().validAttack = true;

                                validAttackNodes.Add(node);
                                */

                                targets.Add(validMoveNode);
                            }
                        }
                    }
                }
            }
        }

        if (targets.Count != 0)
        {
            // If there are multiple targets, pick a random one
            index = Random.Range(0, targets.Count);
            GameObject moveNode = targets[index];
            gridSystem.moveSelectedUnit(moveNode);
            gridSystem.resetValidMoveNodes();

            yield return new WaitForSeconds(.3f);

            // Find all of the valid attack nodes for the enemy unit and select one
            gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "PlayerUnit");
            yield return StartCoroutine(enemyAIChooseUnitToAttack());

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
            yield return StartCoroutine(calculateRandomizedEnemyAI());
        }

        yield return null;
    }

    IEnumerator enemyAIChooseUnitToAttack()
    {
        yield return new WaitForSeconds(.3f);

        bool hasAttacked = false;

        // Check to see if there are any valid attacks
        if (gridSystem.validAttackNodes.Count > 0)
        {
            for (int i = 0; i < gridSystem.validAttackNodes.Count; i++)
            {
                GameObject node = gridSystem.validAttackNodes[i];

                if (node.GetComponent<GridNode>().currentUnit != gridSystem.GetComponent<GridSystem>().selectedUnit)
                {
                    // Start a battle with that unit, where the enemy starts first
                    yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(node.GetComponent<GridNode>().currentUnit, gridSystem.selectedUnit, "Enemy"));

                    hasAttacked = true;
                    checkIfUnitDefeated(gridSystem.selectedUnit, node.GetComponent<GridNode>().currentUnit);
                    checkWinLoseCondition();
                }

                if (hasAttacked)
                {
                    break;
                }
            }
        }

        yield return null;
    }
}

