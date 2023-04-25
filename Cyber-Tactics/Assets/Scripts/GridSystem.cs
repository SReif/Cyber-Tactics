using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int width = 0;                       // Width of the grid via the z-axis
    public int height = 0;                      // Height of the grid via the x-axis
    public Vector3 firstNodeOrigin;             // The world position of the grid based on the first node's world position
    public GameObject selectedUnit;             // The unit that is currently selected
    public Vector3 unitOffset;                  // The height offset for the units on the grid
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
            for (int y = 0; y < grid.GetLength(1); y++) // Width
            {
                GameObject node = transform.GetChild(childIndex++).gameObject;
                node.transform.Find("Indicators").Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = false;
                node.transform.Find("Indicators").Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = false;

                // Offset the node world position by the firstNodeOrigin vector, and inform the node of its position on the grid
                node.GetComponent<GridNode>().nodeWorldPos = new Vector3(x, 0, y) + firstNodeOrigin;
                node.GetComponent<GridNode>().nodeGridPos = new Vector2(x, y);
                grid[x, y] = node;

                // Disable the SpriteRenderer for nodes that contain an Obstacle
                if (node.transform.Find("Unit Slot").childCount != 0)
                {
                    if (node.transform.Find("Unit Slot").GetChild(0).tag == "Obstacle")
                    {
                        node.transform.Find("Grid Sprite").GetComponent<SpriteRenderer>().enabled = false;
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
        // Get the parent (Node) of the parent (Unit Slot)
        GameObject previousNode = selectedUnit.transform.parent.transform.parent.gameObject;

        if (previousNode != newNode)
        {
            Vector3 nodeGridPos = previousNode.GetComponent<GridNode>().nodeGridPos;

            // Move the GameObject to the new node location, based off its position transform
            selectedUnit.transform.position = newNode.GetComponent<GridNode>().nodeWorldPos + unitOffset;

            // Transfer the GameObject to the new node
            selectedUnit.transform.SetParent(newNode.transform.Find("Unit Slot"));
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
            validMoveNodes[i].transform.Find("Indicators").Find("Valid Move Indicator").GetComponent<MeshRenderer>().enabled = false;
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
            validAttackNodes[i].transform.Find("Indicators").Find("Valid Attack Indicator").GetComponent<MeshRenderer>().enabled = false;
            validAttackNodes[i].GetComponent<GridNode>().validAttack = false;
        }

        validAttackNodes.Clear();

        Debug.Log("Valid attacks for unit reset.");
    }
}
