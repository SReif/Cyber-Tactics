using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int width = 0;               // Width of the grid via the z-axis
    public int height = 0;              // Height of the grid via the x-axis
    public Vector3 origin;              // The original world position the grid's position is based on
    public GameObject selectedUnit;     // The unit that is currently selected

    private GameObject[,] grid;         // A 2D array that holds the nodes that make up the grid system

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate a 2D array with the given width and height parameters
        grid = new GameObject[height, width];
        int childIndex = 0;

        // Iterate through grid cells and place each children node one by one in the grid.
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject node = transform.GetChild(childIndex++).gameObject;

                // Offset the node world position by the origin vector, and inform the node of it's position on the grid
                node.GetComponent<GridNode>().nodeWorldPos = new Vector3(x, 0, y) + origin;
                node.GetComponent<GridNode>().nodeGridPos = new Vector2(x, y);
                grid[x, y] = node;
            }
        }

        // Debugging
        /*
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                print(grid[x, y].transform.name + ":  " + x + ", " + y);
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        // Input System for Key Presse (Moving on grid)
        // DISCLAIMER: The controls are based on the axis directions and not the camera perspective, so the controls will be off
        if (Input.GetKeyDown(KeyCode.W))
        {
            //Debug.Log("Move up!");
            moveSelectedUnit(new Vector3(-1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("Move left!");
            moveSelectedUnit(new Vector3(0, 0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("Move down!");
            moveSelectedUnit(new Vector3(1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            //Debug.Log("Move right!");
            moveSelectedUnit(new Vector3(0, 0, 1));
        }

        // Input System for Mouse Clicks (Selecting Units/Nodes and moving on grid)
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node")
            {
                //Debug.Log("Node selected.");
                moveSelectedUnitMouseClick(hit.transform.gameObject);
            }
            else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Unit")
            {
                //Debug.Log("Unit selected.");
                selectedUnit = hit.transform.gameObject;
            }
        }
    }

    public void moveSelectedUnit(Vector3 direction)
    {
        GameObject previousNode = selectedUnit.GetComponent<Unit>().currentNode;

        Vector3 nodeGridPos = selectedUnit.GetComponent<Unit>().currentNode.GetComponent<GridNode>().nodeGridPos;
        Vector3 unitOffset = new Vector3(0, selectedUnit.transform.localScale.y, 0);

        // Check to see if the move would take you off the grid
        if (direction.x + nodeGridPos.x < height && direction.x + nodeGridPos.x >= 0 
            && direction.z + nodeGridPos.y < width && direction.z + nodeGridPos.y >= 0)
        {
            
            GameObject newNode = grid[(int)(direction.x + nodeGridPos.x), (int)(direction.z + nodeGridPos.y)];

            // Check to see if a unit/obstacle GameObject currently occupies that space
            if (newNode.GetComponent<GridNode>().currentUnit == null)
            {
                Debug.Log("Valid move!");

                // Move the gamobject to the new node location, based off its position transform
                selectedUnit.transform.position = newNode.GetComponent<GridNode>().nodeWorldPos + unitOffset;

                // Transfer the gameobject to the new node
                newNode.GetComponent<GridNode>().currentUnit = selectedUnit;

                // Remove the gameobject from the previous node
                previousNode.GetComponent<GridNode>().currentUnit = null;

                // Make the new node location the selectedUnitsNode
                selectedUnit.GetComponent<Unit>().currentNode = newNode;
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

    public void moveSelectedUnitMouseClick(GameObject newNode)
    {
        GameObject previousNode = selectedUnit.GetComponent<Unit>().currentNode;
        Vector3 nodeGridPos = previousNode.GetComponent<GridNode>().nodeGridPos;
        Vector3 unitOffset = new Vector3(0, selectedUnit.transform.localScale.y, 0);

        // Check to see if a unit/obstacle GameObject currently occupies that space
        if (newNode.GetComponent<GridNode>().currentUnit == null)
        {
            Debug.Log("Valid move!");

            // Move the gamobject to the new node location, based off its position transform
            selectedUnit.transform.position = newNode.GetComponent<GridNode>().nodeWorldPos + unitOffset;

            // Transfer the gameobject to the new node
            newNode.GetComponent<GridNode>().currentUnit = selectedUnit;

            // Remove the gameobject from the previous node
            previousNode.GetComponent<GridNode>().currentUnit = null;

            // Make the new node location the selectedUnitsNode
            selectedUnit.GetComponent<Unit>().currentNode = newNode;
        }
        else
        {
            Debug.Log("Invalid move: Unit/Obstacle in the way!");
        }
    }
}
