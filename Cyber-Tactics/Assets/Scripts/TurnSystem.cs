using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour
{
    private AudioManager audioManager;
    private WinLoseManager winLoseManager;

    public GameObject gridObject;                   // The GameObject for the grid system for reference
    public GameObject battleSystem;                 // The GameObject for the battle system
    public GameObject gridViewCamera;               // The GameObject for the camera that faces the grid
    public List<GameObject> playersUnits;           // The list of units the player controls
    public List<GameObject> enemysUnits;            // The list of units the enemy controls
    public State state;                             // The current game state for determining turns
    public GameObject viewedUnit;                   // The unit that the player is currently viewing the stats of

    private GridSystem gridSystem;                  // The grid system itself for easier reference
    private List<GameObject> enemysUnitsNotMoved;   // The list of units the enemy has moved yet
    private int playersUnitsMoved;                  // The number of player units that have already moved
    private int enemysUnitsMoved;                   // The number of enemy units that have already moved

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

        // Retrieve the grid system from the grid system GameObject
        gridSystem = gridObject.GetComponent<GridSystem>();

        playersUnitsMoved = 0;
        enemysUnitsMoved = 0;
        enemysUnitsNotMoved = enemysUnits;
        viewedUnit = null;

        Debug.Log("Beginning grid turn system. Player goes first.");

        // Locate the AudioManager and play music
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.Play("Battle Theme - Gorandora");

        // Locate the Win/Lose Manager
        winLoseManager = GameObject.Find("SceneManager").GetComponent<WinLoseManager>();

        StartCoroutine(GridTurnSystem());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GridTurnSystem()
    {
        while (!winLoseManager.winLoseConditionMet)
        {
            if (state == State.PlayerTurn)
            {
                // The player can then select one of their units that have the "PlayerUnit" Tag

                // Check to see if the player has moved all of their units
                if (playersUnitsMoved >= playersUnits.Count)
                {
                    // Switch to the enemy's turn after all units have moved
                    state = State.EnemyTurn;

                    // Revert the player's unit colors back to their default
                    for (int i = 0; i < playersUnits.Count; i++)
                    {
                        playersUnits[i].gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                    }

                    // Prepare the enemy units for their turn
                    for (int i = 0; i < enemysUnits.Count; i++)
                    {
                        enemysUnits[i].gameObject.GetComponent<Unit>().hasMoved = false;
                    }

                    enemysUnitsMoved = 0;
                    enemysUnitsNotMoved = enemysUnits;

                    viewedUnit = null;

                    yield return new WaitForSeconds(1f);
                    Debug.Log("Starting enemy's turn!");
                }
                else
                {
                    var ray = gridViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        // Selecting a new unit to move that has not moved yet
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
                                    gridSystem.selectedUnit.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

                                   // Do not increment this value if the unit is defeated, or else the player's turn will end early when there are less units on the board
                                   playersUnitsMoved++;

                                    gridSystem.selectedUnit = null;
                                }

                                Debug.Log("Unit has taken its turn.");
                            }
                        }
                        else if (Input.GetMouseButtonDown(1) && (hit.transform.tag == "PlayerUnit" || hit.transform.tag == "EnemyUnit"))
                        {
                            // Allow the player to view a unit's stats

                            if (viewedUnit != hit.transform.gameObject)
                            {
                                viewedUnit = hit.transform.gameObject;
                            }
                            else if (viewedUnit == hit.transform.gameObject)
                            {
                                viewedUnit = null;
                            }
                        }
                    }
                }
            }
            else if (state == State.EnemyTurn)
            {
                // The enemy can then select one of their units that have the "EnemyUnit" Tag
                if (enemysUnitsMoved >= enemysUnits.Count)
                {
                    // Switch to the player's turn after all units have moved
                    state = State.PlayerTurn;

                    // Revert the enemy's unit colors back to normal
                    for (int i = 0; i < enemysUnits.Count; i++)
                    {
                        enemysUnits[i].gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                    }

                    // Prepare the player units for their turn
                    for (int i = 0; i < playersUnits.Count; i++)
                    {
                        playersUnits[i].gameObject.GetComponent<Unit>().hasMoved = false;
                    }

                    playersUnitsMoved = 0;
                    enemysUnitsNotMoved = new List<GameObject>();

                    viewedUnit = null;

                    yield return new WaitForSeconds(1f);
                    Debug.Log("Starting player's turn!");
                }
                else
                {
                    // Perform a random enemy unit's turn, if it has not already moved
                    yield return StartCoroutine(calculateAggressiveEnemyAI());

                    Debug.Log("Unit has taken its turn.");
                }
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator enemyAIRandomlyChooseUnitToAttack()
    {
        yield return new WaitForSeconds(1f);

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

                if (node.transform.Find("Unit Slot").GetChild(0).gameObject == gridSystem.GetComponent<GridSystem>().selectedUnit)
                {
                    // Rather than having to attack an opposing unit, the unit can select itself, forfeiting the attack action this turn
                    hasAttacked = true;
                    gridSystem.resetValidAttackNodes();
                }
                else
                {
                    // Start a battle with that unit, where the enemy starts first
                    yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(node.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit, "Enemy"));

                    hasAttacked = true;
                    gridSystem.resetValidAttackNodes();

                    checkIfUnitDefeated(node.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit);
                    //checkWinLoseCondition();
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
                        if (hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject == gridSystem.GetComponent<GridSystem>().selectedUnit)
                        {
                            // Rather than having to attack an opposing unit, the unit can "click" on itself, forfeiting the attack action this turn
                            hasAttacked = true;
                            gridSystem.resetValidAttackNodes();
                        }
                        else
                        {
                            Debug.Log("Opposing unit detected, commence battle.");

                            if (currentUnitSide == "Player")
                            {
                                // Start a battle with that unit, where the player starts first
                                yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(gridSystem.selectedUnit, hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject, "Player"));

                                hasAttacked = true;
                                gridSystem.resetValidAttackNodes();

                                checkIfUnitDefeated(gridSystem.selectedUnit, hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject);
                                //checkWinLoseCondition();
                            }
                            else
                            {
                                // Start a battle with that unit, where the enemy starts first
                                yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit, "Enemy"));
                                
                                hasAttacked = true;
                                gridSystem.resetValidAttackNodes();

                                checkIfUnitDefeated(hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit);
                                //checkWinLoseCondition();
                            }
                        }
                    }
                    else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "EnemyUnit" && hit.transform.parent.transform.parent.GetComponent<GridNode>().validAttack)
                    {
                        Debug.Log("Opposing unit detected, commence battle.");

                        if (currentUnitSide == "Player")
                        {
                            // Start a battle with that unit, where the player starts first
                            yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(gridSystem.selectedUnit, hit.transform.gameObject, "Player"));

                            hasAttacked = true;
                            gridSystem.resetValidAttackNodes();

                            checkIfUnitDefeated(gridSystem.selectedUnit, hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject);
                            //checkWinLoseCondition();
                        }
                        else
                        {
                            // Start a battle with that unit, where the player starts first
                            yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit, "Enemy"));

                            hasAttacked = true;
                            gridSystem.resetValidAttackNodes();

                            checkIfUnitDefeated(hit.transform.gameObject.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit);
                            //checkWinLoseCondition();
                        }
                    }
                    else if (Input.GetMouseButtonDown(1) && (hit.transform.tag == "PlayerUnit" || hit.transform.tag == "EnemyUnit"))
                    {
                        // Allow the player to view the unit's stats, even when they are performing their attack action

                        if (viewedUnit != hit.transform.gameObject)
                        {
                            viewedUnit = hit.transform.gameObject;
                        }
                        else if (viewedUnit == hit.transform.gameObject)
                        {
                            viewedUnit = null;
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

            // Remove the player unit from the player unit list, update the size of the list, and destroy the player unit entirely
            playersUnits.Remove(playerUnit);
            playersUnits.TrimExcess();
            Destroy(playerUnit);

            // Remove the enemy unit from the enemy unit list, update the size of the list, and destroy the enemy unit entirely
            enemysUnits.Remove(enemyUnit);
            enemysUnits.TrimExcess();
            Destroy(enemyUnit);

            Debug.Log("Both units are defeated!");
        }
        else if (enemyUnit.GetComponent<Unit>().currentHP <= 0)
        {
            // INSERT FUNCTIONALITY FOR PLAYER VICTORY HERE

            // Remove the enemy unit from the enemy unit list, update the size of the list, and destroy the enemy unit entirely
            enemysUnits.Remove(enemyUnit);
            enemysUnits.TrimExcess();
            Destroy(enemyUnit);

            Debug.Log("Enemy unit defeated!");
        }
        else if (playerUnit.GetComponent<Unit>().currentHP <= 0)
        {
            // INSERT FUNCTIONALITY FOR ENEMY VICTORY HERE

            // Remove the player unit from the player unit list, update the size of the list, and destroy the player unit entirely
            playersUnits.Remove(playerUnit);
            playersUnits.TrimExcess();
            Destroy(playerUnit);

            Debug.Log("Player unit defeated!");
        }
    }

    IEnumerator calculateRandomizedEnemyAI()
    {
        // Indicate that the selected enemy unit is acting and calculate all of its valid moves
        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);
        gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);

        yield return new WaitForSeconds(1f);

        // Move the enemy unit to a random valid move
        int index = Random.Range(0, gridSystem.validMoveNodes.Count);
        GameObject node = gridSystem.validMoveNodes[index];
        gridSystem.moveSelectedUnit(node);
        gridSystem.resetValidMoveNodes();

        yield return new WaitForSeconds(1f);

        // Find all of the valid attack nodes for the enemy unit and select one
        gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "PlayerUnit");
        yield return StartCoroutine(enemyAIRandomlyChooseUnitToAttack());

        if (gridSystem.selectedUnit != null)
        {
            // Disable the attack indicator for the unit
            gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

            // Show that the unit cannot be moved the rest of this turn
            gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
            gridSystem.selectedUnit.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

            // Do not increment this value if the unit is defeated, or else the enemy's turn will end early when there are less units on the board
            enemysUnitsMoved++;

            gridSystem.selectedUnit = null;
        }

    }

    IEnumerator calculateAggressiveEnemyAI()
    {
        // Select a random enemy unit (Repeat until you find a unit that has not moved yet.)
        int index = Random.Range(0, enemysUnits.Count);

        while (enemysUnits[index].GetComponent<Unit>().hasMoved)
        {
            index = Random.Range(0, enemysUnits.Count);
        }

        // Indicate that the selected enemy unit is acting and calculate all of its valid moves
        gridSystem.selectedUnit = enemysUnits[index];
        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);
        gridSystem.validMoveNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidMoves(gridSystem.grid);

        // Store the valid moves nodes that lead to an attack on a player unit
        List<GameObject> targets = new List<GameObject>();

        // For each valid move node, check to see if a valid attack at that location can attack a player unit
        for (int k = 0; k < gridSystem.validMoveNodes.Count; k++)
        {
            GameObject validMoveNode = gridSystem.validMoveNodes[k];

            string unitAttackID = gridSystem.selectedUnit.GetComponent<Unit>().unitAttackID;
            List <List<Vector2>> unitAttackSet = gridSystem.selectedUnit.GetComponent<Unit>().selectAttackSet(gridSystem.grid);

            Vector2 gridPos = validMoveNode.GetComponent<GridNode>().nodeGridPos;

            // For each attack in the attack set...
            for (int i = 0; i < unitAttackSet.Count; i++)
            {
                for (int j = 0; j < unitAttackSet[i].Count; j++)
                {
                    // Check to see if the attack is outside of the grid
                    if (unitAttackSet[i][j].x + gridPos.x < gridSystem.grid.GetLength(0) && unitAttackSet[i][j].x + gridPos.x >= 0
                        && unitAttackSet[i][j].y + gridPos.y < gridSystem.grid.GetLength(1) && unitAttackSet[i][j].y + gridPos.y >= 0)
                    {
                        // Obtain the node that the unit wants to try to attack
                        GameObject node = gridSystem.grid[(int)(unitAttackSet[i][j].x + gridPos.x), (int)(unitAttackSet[i][j].y + gridPos.y)];

                        GameObject unitSlot = node.transform.Find("Unit Slot").gameObject;

                        // Determine if there is a unit on the node
                        if (unitSlot.transform.childCount != 0)
                        {
                            // Check to see if the unit is an enemy unit
                            if (unitSlot.transform.GetChild(0).tag == "PlayerUnit")
                            {

                                targets.Add(validMoveNode);

                                // If the unit attackset does not allow the unit to attack over units, prevent it from calculating more attacks past this node
                                if (unitAttackSet.Count > 1)
                                {
                                    break;
                                }
                            }

                            // THIS COMMENTED OUT SECTION MAKES IT SO AN ENEMY UNIT HAS THE CHANCE OF NOT ATTACKING ANOTHER (ARCHIVED)
                            /*
                            else if (unitSlot.transform.GetChild(0).gameObject == transform.gameObject)
                            {
                                // Add itself as a place it can attack, indicating that the enemy unit will not attack anything
                                targets.Add(validMoveNode);
                            }
                            */
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(1f);

        if (targets.Count != 0)
        {
            // If there are multiple targets, pick a random one

            // INSERT DECISION MAKING FOR WHICH VALID TARGET MOVE NODE TO CHOOSE HERE.

            index = Random.Range(0, targets.Count);
            GameObject moveNode = targets[index];
            gridSystem.moveSelectedUnit(moveNode);
            gridSystem.resetValidMoveNodes();

            yield return new WaitForSeconds(1f);

            // Find all of the valid attack nodes for the enemy unit and select one
            gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "PlayerUnit");
            yield return StartCoroutine(enemyAIChooseUnitToAttack());

            if (gridSystem.selectedUnit != null)
            {
                // Disable the attack indicator for the unit
                gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

                // Show that the unit cannot be moved the rest of this turn
                gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
                gridSystem.selectedUnit.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

                // Do not increment this value if the unit is defeated, or else the enemy's turn will end early when there are less units on the board
                enemysUnitsMoved++;

                gridSystem.selectedUnit = null;
            }
        }
        else
        {
            // If there are no valid attacks to perform, move the enemy unit near the closest player unit
            yield return StartCoroutine(calculateMoveCloserEnemyAI());
        }

        yield return null;
    }

    IEnumerator enemyAIChooseUnitToAttack()
    {
        bool hasAttacked = false;

        // Check to see if there are any valid attacks
        if (gridSystem.validAttackNodes.Count > 0)
        {
            // INSERT DECISION MAKING FOR WHICH VALID TARGET ATTACK NODE TO CHOOSE HERE.

            // Option 1:
                // Enemy AI chooses which player unit to attack based on the addition of all of its current stats, producing a "threat" score.
                    // If a player unit has 3/5 HP, 1 PHY ATK, 2 PHY DEF, 0 MAG ATK, and 0 MAG DEF, the score would be 6.
                    // If another player unit has 1/3 HP, 2 PHY ATK, 1 PHY DEF, 0 MAG ATk, and 0 MAG DEF, the score would be 4.
                    // The enemy unit would attack the player unit with the lowest overall threat score

            // Option 2:
                // The enemy AI compares its overall stats to the different units:
                    // For each stat, the enemy AI checks to see if it has a higher stat, and marks it as 1.
                        // Perhaps if the player unit HP is under a certain threshold, it marks it 
                    // If the enemy unit has a lower stat, it marks it as 0.
                    // The enemy adds all of these values together to produce a "battle success" score.
                        // The enemy unit will attack the player unit with the highest battle success score because it knows it has more stats that are better.
                    // This system is malleable because you can change the weight of the scores.
                        // If you want the enemy unit to prioritize player units with lower HP, you can increase the score for that stat.

            // Option 3:
                // The enemy AI compares its ATK stats to the player unit's DEF stats and compares its DEF stats to the player unit's ATK stats.
                    // This system will produce a "battle success" score based on the comparisons between its stats.
                    // If the enemy unit has a higher PHY ATK than the player unit's PHY DEF, calculate the difference and add it to the overall success score.
                    // If the enemy unit has a higher PHY DEF than the player unit's PHY ATK, do the same thing as above.
                    // If the enemy unit has higher current HP than the player unit, do the same thing as above.
                    // MAG ATK to MAG DEF and vice versa are calculated the same as the PHY to ATK and vice versa.
                // The enemy unit will choose the unit with the highest "battle success" score associated with it.
                
                // Look into finding the MAX modifier for each stat and adding it to its associated base stat to see how well a unit could do at its best

            // Either choose a player unit at random if there is a tie or pick the first in the list


            for (int i = 0; i < gridSystem.validAttackNodes.Count; i++)
            {
                GameObject node = gridSystem.validAttackNodes[i];

                if (node.transform.Find("Unit Slot").GetChild(0).gameObject != gridSystem.GetComponent<GridSystem>().selectedUnit)
                {
                    viewedUnit = node.transform.Find("Unit Slot").GetChild(0).gameObject;

                    yield return new WaitForSeconds(1f);

                    // Start a battle with that unit, where the enemy starts first
                    yield return StartCoroutine(battleSystem.GetComponent<BattleTurnSystem>().Battle(node.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit, "Enemy"));

                    viewedUnit = null;

                    hasAttacked = true;
                    gridSystem.resetValidAttackNodes();

                    checkIfUnitDefeated(node.transform.Find("Unit Slot").GetChild(0).gameObject, gridSystem.selectedUnit);
                    //checkWinLoseCondition();

                    yield return new WaitForSeconds(1f);
                }

                if (hasAttacked)
                {
                    break;
                }
            }
        }

        yield return null;
    }

    IEnumerator calculateMoveCloserEnemyAI()
    {
        List<Vector3> playerUnitPos = new List<Vector3>();

        for (int i = 0; i < playersUnits.Count; i++)
        {
            playerUnitPos.Add(playersUnits[i].transform.position);
        }

        // Begin the calculation by using the enemy unit's current node to compare against other possible valid move nodes (The unit's current location is ALWAYS a valid move node)
        GameObject closestMoveNodetoPlayerUnit = gridSystem.selectedUnit.transform.parent.transform.parent.gameObject;

        // Create a placeholder value that will never be close enough to the units
        float smallestDistanceBetweenNodePos = 1000f;

        // Begin by checking for the enemy unit's current position
        for (int i = 0; i < playerUnitPos.Count; i++)
        {
            Debug.Log("Distance between: " + Vector3.Distance(playerUnitPos[i], gridSystem.selectedUnit.transform.position));

            if (Vector3.Distance(playerUnitPos[i], gridSystem.selectedUnit.transform.position) < smallestDistanceBetweenNodePos)
            {
                smallestDistanceBetweenNodePos = Vector3.Distance(playerUnitPos[i], gridSystem.selectedUnit.transform.position);
            }
        }

        // For each valid move node, check to see which valid move gets the enemy unit the closest to a player unit
        for (int i = 0; i < gridSystem.validMoveNodes.Count; i++)
        {
            for (int j = 0; j < playerUnitPos.Count; j++)
            {
                Debug.Log("Distance between: " + Vector3.Distance(playerUnitPos[j], gridSystem.selectedUnit.transform.position));

                if (Vector3.Distance(playerUnitPos[j], gridSystem.validMoveNodes[i].transform.position) < smallestDistanceBetweenNodePos)
                {
                    smallestDistanceBetweenNodePos = Vector3.Distance(playerUnitPos[j], gridSystem.validMoveNodes[i].transform.position);
                    closestMoveNodetoPlayerUnit = gridSystem.validMoveNodes[i];
                }
            }
        }

        gridSystem.moveSelectedUnit(closestMoveNodetoPlayerUnit);
        gridSystem.resetValidMoveNodes();

        yield return new WaitForSeconds(1f);

        // Find all of the valid attack nodes for the enemy unit and select one
        gridSystem.validAttackNodes = gridSystem.selectedUnit.GetComponent<Unit>().showValidAttacks(gridSystem.grid, "PlayerUnit");

        yield return new WaitForSeconds(1f);

        // The attack action is skipped because the only reason the unit would use this AI is becuase it can't reach a unit on this turn
        gridSystem.resetValidAttackNodes();

        // Disable the attack indicator for the unit
        gridSystem.selectedUnit.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        // Show that the unit cannot be moved the rest of this turn
        gridSystem.selectedUnit.GetComponent<Unit>().hasMoved = true;
        gridSystem.selectedUnit.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

        enemysUnitsMoved++;

        gridSystem.selectedUnit = null;

        yield return null;
    }
}