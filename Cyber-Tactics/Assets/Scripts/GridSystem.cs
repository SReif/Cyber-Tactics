using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int width = 0;                       // Width of the grid via the z-axis
    public int height = 0;                      // Height of the grid via the x-axis
    public Vector3 origin;                      // The original world position the grid's position is based on
    public GameObject selectedUnit;             // The unit that is currently selected
    public GameObject[,] grid;                  // A 2D array that holds the nodes that make up the grid system
    public List<GameObject> validMoveNodes;     // The nodes that the current selected unit can move to
    public List<GameObject> validAttackNodes;   // The nodes that the current selected unit can attack

    // Start is called before the first frame update
    void Start()
    {
        validMoveNodes = new List<GameObject>();
        validAttackNodes = new List<GameObject>();

        // Instantiate a 2D array with the given width and height parameters
        grid = new GameObject[height, width];
        int childIndex = 0;

        // Iterate through grid cells and place each children node one by one in the grid.
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject node = transform.GetChild(childIndex++).gameObject;
                node.transform.Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = false;
                node.transform.Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = false;

                // Offset the node world position by the origin vector, and inform the node of its position on the grid
                node.GetComponent<GridNode>().nodeWorldPos = new Vector3(x, 0, y) + origin;
                node.GetComponent<GridNode>().nodeGridPos = new Vector2(x, y);
                grid[x, y] = node;

                // Inform the node's current unit of its position on the grid
                if (node.GetComponent<GridNode>().currentUnit != null)
                {
                    if (node.GetComponent<GridNode>().currentUnit.transform.tag == "PlayerUnit" ||
                        node.GetComponent<GridNode>().currentUnit.transform.tag == "EnemyUnit")
                    {
                        node.GetComponent<GridNode>().currentUnit.GetComponent<Unit>().unitGridPos = new Vector2(x, y);
                    }
                    else if (node.GetComponent<GridNode>().currentUnit.transform.tag == "Obstacle")
                    {
                        node.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }

        Debug.Log("Grid created.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveSelectedUnit(GameObject newNode)
    {
        GameObject previousNode = selectedUnit.GetComponent<Unit>().currentNode;

        if (previousNode != newNode)
        {
            Vector3 nodeGridPos = previousNode.GetComponent<GridNode>().nodeGridPos;

            Vector3 unitOffset = new Vector3(0, 0.75f, 0);

            // Move the GameObject to the new node location, based off its position transform
            selectedUnit.transform.position = newNode.GetComponent<GridNode>().nodeWorldPos + unitOffset;

            // Transfer the GameObject to the new node
            newNode.GetComponent<GridNode>().currentUnit = selectedUnit;

            // Remove the GameObject from the previous node
            previousNode.GetComponent<GridNode>().currentUnit = null;

            // Make the new node location the selectedUnitsNode
            selectedUnit.GetComponent<Unit>().currentNode = newNode;
            selectedUnit.GetComponent<Unit>().unitGridPos = newNode.GetComponent<GridNode>().nodeGridPos;
        }        

        // Stop showing the valid moves for the unit's previous location
        resetValidMoveNodes();

        Debug.Log("Unit moved.");
    }

    public void resetValidMoveNodes()
    {
        // Revert the node's settings to their default
        for (int i = 0; i < validMoveNodes.Count; i++)
        {
            validMoveNodes[i].transform.Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = false;
            validMoveNodes[i].GetComponent<GridNode>().validMove = false;

        }

        validMoveNodes.Clear();

        Debug.Log("Valid moves for unit reset.");

    }

    public void resetValidAttackNodes()
    {
        // Revert the node's settings to their default
        for (int i = 0; i < validAttackNodes.Count; i++)
        {
            validAttackNodes[i].transform.Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = false;
            validAttackNodes[i].GetComponent<GridNode>().validAttack = false;
        }

        validAttackNodes.Clear();

        Debug.Log("Valid attacks for unit reset.");
    }
}
