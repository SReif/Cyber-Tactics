using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject currentNode;      // The node this unit is occupying
    public Vector2 unitGridPos;         // The grid position of the unit (Ex. [0, 1] or Vector2(0, 1))
    public string unitID;               // The type of unit this unit is (Determines which moveset they use)

    public bool hasMoved;               // Whether the unit has moved or not

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> showPlayerValidMoves(GameObject[,] grid)
    {
        List<List<Vector2>> unitMoveset;
        
        // Set the unit's movset based on its unit ID (Might change this later if separate Unit scripts are required)
        if (unitID == "King")
        {
            unitMoveset = calculateKingMoveset();
        }
        else if (unitID == "Knight")
        {
            unitMoveset = calculateKnightMoveset();
        }
        else if (unitID == "Pseudo Bishop")
        {
            unitMoveset = calculatePseudoBishopMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else if (unitID == "Pseudo Rook")
        {
            unitMoveset = calculatePseudoRookMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else if (unitID == "Bishop")
        {
            unitMoveset = calculateTrueBishopMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else if (unitID == "Rook")
        {
            unitMoveset = calculateTrueRookMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else
        {
            unitMoveset = calculateAdjacentSquareMoveset();
        }

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
                // Check to see if the move would take you off the grid
                if (unitMoveset[i][j].x + unitGridPos.x < grid.GetLength(0) && unitMoveset[i][j].x + unitGridPos.x >= 0
                    && unitMoveset[i][j].y + unitGridPos.y < grid.GetLength(1) && unitMoveset[i][j].y + unitGridPos.y >= 0)
                {
                    // Obtain the node that the unit wants to try to move to
                    GameObject node = grid[(int)(unitMoveset[i][j].x + unitGridPos.x), (int)(unitMoveset[i][j].y + unitGridPos.y)];

                    // Check to see if a unit/obstacle GameObject currently occupies that space
                    if (node.GetComponent<GridNode>().currentUnit == null)
                    {
                        // Change the color of the node to show that it is a valid move
                        //node.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
                        node.transform.Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = true;

                        // This is used later to ensure that you cannot select a node outside of the moveset
                        node.GetComponent<GridNode>().validMove = true;

                        validMoveNodes.Add(node);
                    }
                    else if (node.GetComponent<GridNode>().currentUnit.tag == "EnemyUnit")
                    {
                        // Change the color of the node to show that it is a valid move
                        //node.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Attack Node Color", typeof(Material)) as Material;
                        node.transform.Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = true;

                        // This is used later to ensure that you cannot select a node outside of the moveset
                        node.GetComponent<GridNode>().validMove = true;

                        validMoveNodes.Add(node);

                        // If the unit moveset does not allow the unit to move over its own units, prevent it from calculating more moves in the move pattern
                        if (unitMoveset.Count > 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("Invalid move: Obstacle in the way!");

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

        return validMoveNodes;
    }

    public List<GameObject> showEnemyValidMoves(GameObject[,] grid)
    {
        List<List<Vector2>> unitMoveset;

        // Set the unit's movset based on its unit ID (Might change this later if separate Unit scripts are required)
        if (unitID == "King")
        {
            unitMoveset = calculateKingMoveset();
        }
        else if (unitID == "Knight")
        {
            unitMoveset = calculateKnightMoveset();
        }
        else if (unitID == "Pseudo Bishop")
        {
            unitMoveset = calculatePseudoBishopMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else if (unitID == "Pseudo Rook")
        {
            unitMoveset = calculatePseudoRookMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else if (unitID == "Bishop")
        {
            unitMoveset = calculateTrueBishopMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else if (unitID == "Rook")
        {
            unitMoveset = calculateTrueRookMoveset(grid.GetLength(0), grid.GetLength(1));
        }
        else
        {
            unitMoveset = calculateAdjacentSquareMoveset();
        }

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
                // Check to see if the move would take you off the grid
                if (unitMoveset[i][j].x + unitGridPos.x < grid.GetLength(0) && unitMoveset[i][j].x + unitGridPos.x >= 0
                    && unitMoveset[i][j].y + unitGridPos.y < grid.GetLength(1) && unitMoveset[i][j].y + unitGridPos.y >= 0)
                {
                    // Obtain the node that the unit wants to try to move to
                    GameObject node = grid[(int)(unitMoveset[i][j].x + unitGridPos.x), (int)(unitMoveset[i][j].y + unitGridPos.y)];

                    // Check to see if a unit/obstacle GameObject currently occupies that space
                    if (node.GetComponent<GridNode>().currentUnit == null)
                    {
                        // Change the color of the node to show that it is a valid move
                        //node.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
                        node.transform.Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = true;

                        // This is used later to ensure that you cannot select a node outside of the moveset
                        node.GetComponent<GridNode>().validMove = true;

                        validMoveNodes.Add(node);
                    }
                    else if (node.GetComponent<GridNode>().currentUnit.tag == "PlayerUnit")
                    {
                        // Change the color of the node to show that it is a valid move
                        //node.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Attack Node Color", typeof(Material)) as Material;
                        node.transform.Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = true;

                        // This is used later to ensure that you cannot select a node outside of the moveset
                        node.GetComponent<GridNode>().validMove = true;

                        validMoveNodes.Add(node);

                        // If the unit moveset does not allow the unit to move over its own units, prevent it from calculating more moves in the move pattern
                        if (unitMoveset.Count > 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Debug.Log("Invalid move: Obstacle in the way!");

                        // If the unit moveset does not allow the unit to move over obstacles, prevent it from calculating more moves in the move pattern
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

        return validMoveNodes;
    }

    public List<List<Vector2>> calculateAdjacentSquareMoveset()
    {
        // Calculate a unit's moveset such that it can move to an adjacent square as long as it's not at a diagonal

        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();
        List<Vector2> moves = new List<Vector2>();

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

    public List<List<Vector2>> calculatePseudoBishopMoveset(int gridWidth, int gridHeight)
    {
        // Calculate a unit's moveset such that it can move like a Bishop would in Chess, but it can go through units/obstacles

        int maxDistance = (gridWidth < gridHeight) ? gridWidth : gridHeight;

        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();
        List<Vector2> moves = new List<Vector2>();

        for (int i = 1; i <= maxDistance; i++)
        {
            moves.Add(new Vector2(i, i));
            moves.Add(new Vector2(i, -i));
            moves.Add(new Vector2(-i, -i));
            moves.Add(new Vector2(-i, i));
        }

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculatePseudoRookMoveset(int gridWidth, int gridHeight)
    {
        // Calculate a unit's moveset such that it can move like a Rook would in Chess, but it can go through units/obstacles

        int maxDistance = (gridWidth > gridHeight) ? gridWidth : gridHeight;

        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();
        List<Vector2> moves = new List<Vector2>();

        for (int i = 1; i <= maxDistance; i++)
        {
            moves.Add(new Vector2(i, 0));
            moves.Add(new Vector2(0, -i));
            moves.Add(new Vector2(-i, 0));
            moves.Add(new Vector2(0, i));
        }

        unitMoveset.Add(moves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateTrueRookMoveset(int gridWidth, int gridHeight)
    {
        // Calculate a unit's moveset such that it can move like a Rook would in Chess

        int maxDistance = (gridWidth > gridHeight) ? gridWidth : gridHeight;

        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> posiHoriMoves = new List<Vector2>();

        // Generate positive horizontal moves
        for (int i = 0; i < maxDistance; i++)
        {
            posiHoriMoves.Add(new Vector2(i + 1, 0));
        }
        unitMoveset.Add(posiHoriMoves);

        List<Vector2> negaVertMoves = new List<Vector2>();

        // Generate negative vertical moves
        for (int i = 0; i < maxDistance; i++)
        {
            negaVertMoves.Add(new Vector2(0, -(i + 1)));
        }
        unitMoveset.Add(negaVertMoves);

        List<Vector2> negaHoriMoves = new List<Vector2>();

        // Generate negative horizontal moves
        for (int i = 0; i < maxDistance; i++)
        {
            negaHoriMoves.Add(new Vector2(-(i + 1), 0));
        }
        unitMoveset.Add(negaHoriMoves);

        List<Vector2> posiVertMoves = new List<Vector2>();

        // Generate positive vertical moves
        for (int i = 0; i < maxDistance; i++)
        {
            posiVertMoves.Add(new Vector2(0, i + 1));
        }
        unitMoveset.Add(posiVertMoves);

        return unitMoveset;
    }

    public List<List<Vector2>> calculateTrueBishopMoveset(int gridWidth, int gridHeight)
    {
        // Calculate a unit's moveset such that it can move like a Rook would in Chess

        int maxDistance = (gridWidth < gridHeight) ? gridWidth : gridHeight;

        List<List<Vector2>> unitMoveset = new List<List<Vector2>>();

        List<Vector2> topRightDiagMoves = new List<Vector2>();

        // Generate positive horizontal moves
        for (int i = 0; i < maxDistance; i++)
        {
            topRightDiagMoves.Add(new Vector2(i + 1, i + 1));
        }
        unitMoveset.Add(topRightDiagMoves);

        List<Vector2> botRightDiagMoves = new List<Vector2>();

        // Generate negative vertical moves
        for (int i = 0; i < maxDistance; i++)
        {
            botRightDiagMoves.Add(new Vector2(i + 1, -(i + 1)));
        }
        unitMoveset.Add(botRightDiagMoves);

        List<Vector2> botLeftDiagMoves = new List<Vector2>();

        // Generate negative horizontal moves
        for (int i = 0; i < maxDistance; i++)
        {
            botLeftDiagMoves.Add(new Vector2(-(i + 1), -(i + 1)));
        }
        unitMoveset.Add(botLeftDiagMoves);

        List<Vector2> topLeftDiagMoves = new List<Vector2>();

        // Generate positive vertical moves
        for (int i = 0; i < maxDistance; i++)
        {
            topLeftDiagMoves.Add(new Vector2(-(i + 1), i + 1));
        }
        unitMoveset.Add(topLeftDiagMoves);

        return unitMoveset;
    }
}