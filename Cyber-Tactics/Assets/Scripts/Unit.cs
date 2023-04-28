using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;             // The name of the unit

    public int maxHP;                   // Maximum HP for the unit
    public int currentHP;               // Current HP for the unit
    public int basePhysicalAttack;      // The base attack value that is added to ATK modifier cards
    public int basePhysicalDefense;     // The base defense value that is added to DEF modifier cards
    public int baseMagicalAttack;       // The base magic value that is added to MAG modifier cards
    public int baseMagicalDefense;      // The base magic defense value that is added to MAG DEF modifier cards
    public List<GameObject> cards;      // The cards that this unit will choose from
    public string unitMoveID;           // The type of chess piece this unit moves as (Determines which moveset they use)
    public string unitAttackID;         // The type of chess piece this unit attack as (Determines which attackset they use)

    public bool hasMoved;               // Whether the unit has moved or not



    // Start is called before the first frame update
    void Start()
    {
        hasMoved = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> showValidAttacks(GameObject[,] grid, string opposingUnitTag)
    {
        // All of the possible nodes that the unit can attack are calculated
        List<List<Vector2>> unitAttackSet = selectAttackSet(grid);

        // The valid attacks that the unit can take are stored in this List
        List<GameObject> validAttackNodes = new List<GameObject>();

        // For each attack in the attack set...
        for (int i = 0; i < unitAttackSet.Count; i++)
        {
            for (int j = 0; j < unitAttackSet[i].Count; j++)
            {
                // Get the parent (Node) of the parent (Unit Slot) for the unit, because each unit is stored as a child within each node
                GameObject unitsNode = transform.parent.transform.parent.gameObject;

                // The position of the unit on the grid, according to the node that it is on
                Vector2 gridPos = unitsNode.GetComponent<GridNode>().nodeGridPos;

                // Check to see if the attack is outside of the grid
                if (unitAttackSet[i][j].x + gridPos.x < grid.GetLength(0) && unitAttackSet[i][j].x + gridPos.x >= 0
                    && unitAttackSet[i][j].y + gridPos.y < grid.GetLength(1) && unitAttackSet[i][j].y + gridPos.y >= 0)
                {
                    // Obtain the node that the unit wants to try to attack
                    GameObject attackNode = grid[(int)(unitAttackSet[i][j].x + gridPos.x), (int)(unitAttackSet[i][j].y + gridPos.y)];

                    // The GameObject within a node whose child is a unit on the node, if a unit exists on that node
                    GameObject unitSlot = attackNode.transform.Find("Unit Slot").gameObject;

                    // Determine if there is a unit on the node (A Unit Slot with no children does not have a unit in it)
                    if (unitSlot.transform.childCount != 0)
                    {
                        // Check to see if the unit is an opposing unit
                        if (unitSlot.transform.GetChild(0).tag == opposingUnitTag)
                        {
                            // Change the color of the node to show that it is a valid attack
                            attackNode.transform.Find("Indicators").Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = true;

                            // This is used later to ensure that you cannot select a node outside of the attackset
                            attackNode.GetComponent<GridNode>().validAttack = true;

                            // Add it to the valid attack nodes list
                            validAttackNodes.Add(attackNode);

                            // If the unit attackset does not allow the unit to attack over units, prevent it from calculating more attacks past this node
                            if (unitAttackSet.Count > 1)
                            {
                                break;
                            }
                        }
                        else if (unitSlot.transform.GetChild(0).gameObject == transform.gameObject)
                        {
                            // Change the color of the node to show that it is a valid attack
                            attackNode.transform.Find("Indicators").Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = true;

                            // This is used later to ensure that you cannot select a node outside of the moveset
                            attackNode.GetComponent<GridNode>().validAttack = true;

                            // Add it to the valid attack nodes list
                            validAttackNodes.Add(attackNode);
                        }
                    }
                }
            }
        }

        Debug.Log("Valid attacks have been calculated.");

        return validAttackNodes;
    }

    public List<GameObject> showValidMoves(GameObject[,] grid)
    {
        // All of the possible nodes that the unit can move to are calculated
        List<List<Vector2>> unitMoveset = selectMoveSet(grid);

        // The valid moves that the unit can take are stored in this List
        List<GameObject> validMoveNodes = new List<GameObject>();

        /* 
         * In order to prevent units from passing through units/obstacles to mimic how the Bishop and Rook chess pieces work,
         * we need to stop the valid move calculator from attempting to check squares that we KNOW won't work. Thus, we BREAK
         * the nested for loop to stop this calculation early, once a unit/obstacle is detected to be in the way.
         * 
         * This occurs because those special movesets are broken up into multiple "mini" movesets.
         * FOR EXAMPLE: The rook can move on either axis from its current location. The positive vertical route that it can move
         *              on is one "moveset" while the negative vertical route that it can move on is another "moveset." This allows
         *              us to stop the calculator for a specific route, if something is block the unit from those other squares.
         */

        // For each possible move in the moveset, check to see if it is a valid move
        for (int i = 0; i < unitMoveset.Count; i++)
        {
            for (int j = 0; j < unitMoveset[i].Count; j++)
            {
                // Get the parent (Node) of the parent (Unit Slot) for the unit, because each unit is stored as a child within each node
                GameObject unitsNode = transform.parent.gameObject.transform.parent.gameObject;

                // The position of the unit on the grid, according to the node that it is on
                Vector2 gridPos = unitsNode.GetComponent<GridNode>().nodeGridPos;

                // Check to see if the move would take you off the grid
                if (unitMoveset[i][j].x + gridPos.x < grid.GetLength(0) && unitMoveset[i][j].x + gridPos.x >= 0
                    && unitMoveset[i][j].y + gridPos.y < grid.GetLength(1) && unitMoveset[i][j].y + gridPos.y >= 0)
                {
                    // Obtain the node that the unit wants to try to move to
                    GameObject moveNode = grid[(int)(unitMoveset[i][j].x + unitsNode.GetComponent<GridNode>().nodeGridPos.x), (int)(unitMoveset[i][j].y + unitsNode.GetComponent<GridNode>().nodeGridPos.y)];

                    // The GameObject within a node whose child is a unit on the node, if a unit exists on that node
                    GameObject unitSlot = moveNode.transform.Find("Unit Slot").gameObject;

                    // Check to see if a unit/obstacle GameObject currently occupies that space
                    if (unitSlot.transform.childCount == 0)
                    {
                        // Change the color of the node to show that it is a valid move
                        moveNode.transform.Find("Indicators").Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = true;

                        // This is used later to ensure that you cannot select a node outside of the moveset
                        moveNode.GetComponent<GridNode>().validMove = true;

                        // Add it to the valid move nodes list
                        validMoveNodes.Add(moveNode);
                    }
                    else if (unitSlot.transform.GetChild(0).gameObject == transform.gameObject)
                    {
                        // Change the color of the node to show that it is a valid move
                        moveNode.transform.Find("Indicators").Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = true;

                        // This is used later to ensure that you cannot select a node outside of the moveset
                        moveNode.GetComponent<GridNode>().validMove = true;

                        // Add it to the valid move nodes list
                        validMoveNodes.Add(moveNode);
                    }
                    else
                    {
                        Debug.Log("Invalid move: Unit/Obstacle in the way!");

                        // If the unit moveset does not allow the unit to move over obstacles/units, prevent it from calculating more moves in the move pattern
                        if (unitMoveset.Count > 1)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("Invalid move: Outside of grid!");
                }
            }
            
        }

        Debug.Log("Valid moves have been calculated.");

        return validMoveNodes;
    }

    /*
     * The following three calculation functions are not necessarily required because their movement variations perform the same calculation,
     * but with different variable names.
     */

    public List<List<Vector2>> calculateAdjacentSquareAttackSet()
    {
        // Calculate a unit's attackset such that it can attack a unit at an adjacent square as long as it's not at a diagonal
        List<List<Vector2>> unitAttackSet = new List<List<Vector2>>();
        List<Vector2> attacks = new List<Vector2>();

        attacks.Add(new Vector2(0, 0));
        attacks.Add(new Vector2(1, 0));
        attacks.Add(new Vector2(0, 1));
        attacks.Add(new Vector2(0, -1));
        attacks.Add(new Vector2(-1, 0));

        unitAttackSet.Add(attacks);

        return unitAttackSet;
    }

    public List<List<Vector2>> calculateKingAttackSet()
    {
        // Calculate a unit's attack set such that it can attack like a King would in Chess
        List<List<Vector2>> unitAttackSet = new List<List<Vector2>>();
        List<Vector2> attacks = new List<Vector2>();

        attacks.Add(new Vector2(0, 0));
        attacks.Add(new Vector2(1, 0));
        attacks.Add(new Vector2(1, -1));
        attacks.Add(new Vector2(0, -1));
        attacks.Add(new Vector2(-1, -1));
        attacks.Add(new Vector2(-1, 0));
        attacks.Add(new Vector2(-1, 1));
        attacks.Add(new Vector2(0, 1));
        attacks.Add(new Vector2(1, 1));

        unitAttackSet.Add(attacks);

        return unitAttackSet;
    }

    public List<List<Vector2>> calculateKnightAttackSet()
    {
        // Calculate a unit's attack set such that it can attack like a Knight would in Chess
        List<List<Vector2>> unitAttackSet = new List<List<Vector2>>();
        List<Vector2> attacks = new List<Vector2>();

        attacks.Add(new Vector2(0, 0));
        attacks.Add(new Vector2(1, 2));
        attacks.Add(new Vector2(2, 1));
        attacks.Add(new Vector2(2, -1));
        attacks.Add(new Vector2(1, -2));
        attacks.Add(new Vector2(-1, -2));
        attacks.Add(new Vector2(-2, -1));
        attacks.Add(new Vector2(-2, 1));
        attacks.Add(new Vector2(-1, 2));

        unitAttackSet.Add(attacks);

        return unitAttackSet;
    }

    public List<List<Vector2>> calculatePseudoBishopAttackSet(int range)
    {
        // Calculate a unit's attack set such that it can attack like a Bishop would in Chess, but it can attack over units/obstacles
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> moves = new List<Vector2>();
        moves.Add(new Vector2(0, 0));

        for (int i = 1; i <= range; i++)
        {
            moves.Add(new Vector2(i, i));
            moves.Add(new Vector2(i, -i));
            moves.Add(new Vector2(-i, -i));
            moves.Add(new Vector2(-i, i));
        }

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateAdjacentSquareMoveset()
    {
        // Calculate a unit's moveset such that it can move to an adjacent square as long as it's not at a diagonal
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();
        List<Vector2> moves = new List<Vector2>();

        moves.Add(new Vector2(0, 0));
        moves.Add(new Vector2(1, 0));
        moves.Add(new Vector2(0, 1));
        moves.Add(new Vector2(0, -1));
        moves.Add(new Vector2(-1, 0));

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateKnightMoveset()
    {
        // Calculate a unit's moveset such that it can move like a Knight would in Chess
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();
        List<Vector2> moves = new List<Vector2>();

        moves.Add(new Vector2(0, 0));
        moves.Add(new Vector2(1, 2));
        moves.Add(new Vector2(2, 1));
        moves.Add(new Vector2(2, -1));
        moves.Add(new Vector2(1, -2));
        moves.Add(new Vector2(-1, -2));
        moves.Add(new Vector2(-2, -1));
        moves.Add(new Vector2(-2, 1));
        moves.Add(new Vector2(-1, 2));

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateKingMoveset()
    {
        // Calculate a unit's moveset such that it can move like a King would in Chess
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();
        List<Vector2> moves = new List<Vector2>();

        moves.Add(new Vector2(0, 0));
        moves.Add(new Vector2(1, 0));
        moves.Add(new Vector2(1, -1));
        moves.Add(new Vector2(0, -1));
        moves.Add(new Vector2(-1, -1));
        moves.Add(new Vector2(-1, 0));
        moves.Add(new Vector2(-1, 1));
        moves.Add(new Vector2(0, 1));
        moves.Add(new Vector2(1, 1));

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculatePseudoBishopMoveset(int range)
    {
        // Calculate a unit's moveset such that it can move like a Bishop would in Chess, but it can go through units/obstacles
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> moves = new List<Vector2>();
        moves.Add(new Vector2(0, 0));

        for (int i = 1; i <= range; i++)
        {
            moves.Add(new Vector2(i, i));
            moves.Add(new Vector2(i, -i));
            moves.Add(new Vector2(-i, -i));
            moves.Add(new Vector2(-i, i));
        }

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculatePseudoRookMoveset(int range)
    {
        // Calculate a unit's moveset such that it can move like a Rook would in Chess, but it can go through units/obstacles
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> moves = new List<Vector2>();
        moves.Add(new Vector2(0, 0));

        for (int i = 1; i <= range; i++)
        {
            moves.Add(new Vector2(i, 0));
            moves.Add(new Vector2(0, -i));
            moves.Add(new Vector2(-i, 0));
            moves.Add(new Vector2(0, i));
        }

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateTrueRookMoveset(int range)
    {
        // Calculate a unit's moveset such that it can move like a Rook would in Chess
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> staticMove = new List<Vector2>();
        staticMove.Add(new Vector2(0, 0));
        unitMoveset.Add(staticMove);

        List<Vector2> posiHoriMoves = new List<Vector2>();

        // Generate positive horizontal moves
        for (int i = 0; i < range; i++)
        {
            posiHoriMoves.Add(new Vector2(i + 1, 0));
        }
        unitMoveset.Add(posiHoriMoves);

        List<Vector2> negaVertMoves = new List<Vector2>();

        // Generate negative vertical moves
        for (int i = 0; i < range; i++)
        {
            negaVertMoves.Add(new Vector2(0, -(i + 1)));
        }
        unitMoveset.Add(negaVertMoves);

        List<Vector2> negaHoriMoves = new List<Vector2>();

        // Generate negative horizontal moves
        for (int i = 0; i < range; i++)
        {
            negaHoriMoves.Add(new Vector2(-(i + 1), 0));
        }
        unitMoveset.Add(negaHoriMoves);

        List<Vector2> posiVertMoves = new List<Vector2>();

        // Generate positive vertical moves
        for (int i = 0; i < range; i++)
        {
            posiVertMoves.Add(new Vector2(0, i + 1));
        }
        unitMoveset.Add(posiVertMoves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateTrueBishopMoveset(int range)
    {
        // Calculate a unit's moveset such that it can move like a Rook would in Chess
        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> staticMove = new List<Vector2>();
        staticMove.Add(new Vector2(0, 0));
        unitMoveset.Add(staticMove);

        List<Vector2> topRightDiagMoves = new List<Vector2>();

        // Generate positive horizontal moves
        for (int i = 0; i < range; i++)
        {
            topRightDiagMoves.Add(new Vector2(i + 1, i + 1));
        }
        unitMoveset.Add(topRightDiagMoves);

        List<Vector2> botRightDiagMoves = new List<Vector2>();

        // Generate negative vertical moves
        for (int i = 0; i < range; i++)
        {
            botRightDiagMoves.Add(new Vector2(i + 1, -(i + 1)));
        }
        unitMoveset.Add(botRightDiagMoves);

        List<Vector2> botLeftDiagMoves = new List<Vector2>();

        // Generate negative horizontal moves
        for (int i = 0; i < range; i++)
        {
            botLeftDiagMoves.Add(new Vector2(-(i + 1), -(i + 1)));
        }
        unitMoveset.Add(botLeftDiagMoves);

        List<Vector2> topLeftDiagMoves = new List<Vector2>();

        // Generate positive vertical moves
        for (int i = 0; i < range; i++)
        {
            topLeftDiagMoves.Add(new Vector2(-(i + 1), i + 1));
        }
        unitMoveset.Add(topLeftDiagMoves);

        return unitMoveset;
    }

    public List<List<Vector2>> selectAttackSet(GameObject[,] grid)
    {
        List<List<Vector2>> unitAttackSet;

        // THIS FUNCTION WILL CHANGE WHEN UNITS NEED A SHORTER RANGE THAN THE GRID WIDTH/HEIGHT
        // That will require the units to have a "range" member variable that is put as an argument for this 

        // Select an attack set based on the unit's attack ID
        if (unitAttackID == "King")
        {
            unitAttackSet = calculateKingMoveset();
        }
        else if (unitAttackID == "Knight")
        {
            unitAttackSet = calculateKnightMoveset();
        }
        else if (unitAttackID == "Pseudo Bishop")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitAttackSet = calculatePseudoBishopMoveset(range);
        }
        else if (unitAttackID == "Pseudo Rook")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitAttackSet = calculatePseudoRookMoveset(range);
        }
        else if (unitAttackID == "Bishop")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitAttackSet = calculateTrueBishopMoveset(range);
        }
        else if (unitAttackID == "Rook")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitAttackSet = calculateTrueRookMoveset(range);
        }
        else
        {
            // This is the default attack set if no attack id is specified
            unitAttackSet = calculateAdjacentSquareMoveset();
        }

        return unitAttackSet;
    }

    public List<List<Vector2>> selectMoveSet(GameObject[,] grid)
    {
        List<List<Vector2>> unitMoveset;
        
        // THIS FUNCTION WILL CHANGE WHEN UNITS NEED A SHORTER RANGE THAN THE GRID WIDTH/HEIGHT

        // Select a move set based on the unit's move ID
        if (unitMoveID == "King")
        {
            unitMoveset = calculateKingMoveset();
        }
        else if (unitMoveID == "Knight")
        {
            unitMoveset = calculateKnightMoveset();
        }
        else if (unitMoveID == "Pseudo Bishop")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitMoveset = calculatePseudoBishopMoveset(range);
        }
        else if (unitMoveID == "Pseudo Rook")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitMoveset = calculatePseudoRookMoveset(range);
        }
        else if (unitMoveID == "Bishop")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitMoveset = calculateTrueBishopMoveset(range);
        }
        else if (unitMoveID == "Rook")
        {
            int range = (grid.GetLength(0) < grid.GetLength(1)) ? grid.GetLength(0) : grid.GetLength(1);
            unitMoveset = calculateTrueRookMoveset(range);
        }
        else
        {
            // This is the default attack set if no move id is specified
            unitMoveset = calculateAdjacentSquareMoveset();
        }

        return unitMoveset;
    }
}