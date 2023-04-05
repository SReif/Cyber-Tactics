using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject currentNode;      // The node this unit is occupying
    public Vector2 unitGridPos;         // The grid position of the unit (Ex. [0, 1] or Vector2(0, 1))

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> showValidMoves(GameObject[,] grid)
    {
        // Define the current moveset that the unit has (Currently, units can only move to an adjacent square that is not at a diagonal)
        Vector2[] unitMoveset = { new Vector2(1, 0), new Vector2(0, 1), 
            new Vector2(0, -1), new Vector2(-1, 0) };

        // The valid moves that the unit can take are stored in this List
        List<GameObject> validMoveNodes = new List<GameObject>();

        // For each possible move in the moveset, check to see if it is a valid move
        for (int i = 0; i < unitMoveset.Length; i++)
        {

            // Check to see if the move would take you off the grid
            if (unitMoveset[i].x + unitGridPos.x < grid.GetLength(0) && unitMoveset[i].x + unitGridPos.x >= 0
                && unitMoveset[i].y + unitGridPos.y < grid.GetLength(1) && unitMoveset[i].y + unitGridPos.y >= 0)
            {
                // Obtain the node that the unit wants to try to move to
                GameObject node = grid[(int)(unitMoveset[i].x + unitGridPos.x), (int)(unitMoveset[i].y + unitGridPos.y)];

                // Check to see if a unit/obstacle GameObject currently occupies that space
                if (node.GetComponent<GridNode>().currentUnit == null)
                {
                    // Change the color of the node to show that it is a valid move
                    node.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Node Color", typeof(Material)) as Material;
                    
                    // This is used later to ensure that you cannot select a node outside of the moveset
                    node.GetComponent<GridNode>().validMove = true;

                    validMoveNodes.Add(node);
                }
                else
                {
                    Debug.Log("Invalid move: Unit/Obstacle in the way!");
                }
            }
            else
            {
                Debug.Log("Invalid move: Outside of grid!");
            }
        }

        return validMoveNodes;
    }
}
