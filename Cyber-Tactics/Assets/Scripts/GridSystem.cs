using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int width = 0;
    public int height = 0;
    public Vector3 origin;

    public GameObject selectedUnit;
    //public GameObject selectedUnitsNode;

    private GameObject[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        // Create one 2d array that stores all of the grid nodes for board position and valid moves.
        // Create another 2d array that stores all of the gameobjects on the board (if there is no gameobject on that tile, it is empty.)


        // Best Option: 
        // You use one 2D array that stores grid nodes that hold their world psoition and whatever gameobejct is on them.
        // GridNode(Vector3(int x, int y, int z), GameObject gameObject, ...)
        // Node is a script that holds the GameObject and world position, rather than a class so it would have MonoBehaviour

        // The nodes and gameobjects are already in the scene, so you have to find each one in order to place it on the grid.
        // The GridNodes and GameObjects are already in the scene, so you have to find each node to put in the 2darray Grid<GridNode>
        // You can place them in the array based on their name, which would just be a number. The GridSystem would be the parent class so you just have to iterate through the children

        grid = new GameObject[height, width];
        int childIndex = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject node = transform.GetChild(childIndex++).gameObject;
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Move up!");
            moveSelectedUnit(new Vector3(-1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Move left!");
            moveSelectedUnit(new Vector3(0, 0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Move down!");
            moveSelectedUnit(new Vector3(1, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Move right!");
            moveSelectedUnit(new Vector3(0, 0, 1));
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Node")
            {
                Debug.Log("Node selected.");
                moveSelectedUnitMouseClick(hit.transform.gameObject);
            }
            else if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Unit")
            {
                Debug.Log("Unit selected.");
                selectedUnit = hit.transform.gameObject;
            }
        }
    }

    public void moveSelectedUnit(Vector3 direction)
    {
        GameObject oldNode = selectedUnit.GetComponent<Unit>().currentNode;
        Vector3 nodeGridPos = selectedUnit.GetComponent<Unit>().currentNode.GetComponent<GridNode>().nodeGridPos;
        Vector3 unitOffset = new Vector3(0, selectedUnit.transform.localScale.y, 0);

        // Check to see if the move would take you off the grid
        if (direction.x + nodeGridPos.x < height && direction.x + nodeGridPos.x >= 0 
            && direction.z + nodeGridPos.y < width && direction.z + nodeGridPos.y >= 0)
        {
            
            GameObject newNode = grid[(int)(direction.x + nodeGridPos.x), (int)(direction.z + nodeGridPos.y)];

            if (newNode.GetComponent<GridNode>().unit == null)
            {
                Debug.Log("Valid move!");

                // Move the gamobject to the new node location, based off its position transform
                selectedUnit.transform.position = newNode.GetComponent<GridNode>().nodeWorldPos + unitOffset;

                // Transfer the gameobject to the new node
                newNode.GetComponent<GridNode>().unit = selectedUnit;

                // Remove the gameobject from the previous node
                oldNode.GetComponent<GridNode>().unit = null;

                // Make the new node location the selectedUnitsNode
                //selectedUnitsNode = newNode;
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
        GameObject oldNode = selectedUnit.GetComponent<Unit>().currentNode;
        Vector3 nodeGridPos = oldNode.GetComponent<GridNode>().nodeGridPos;
        Vector3 unitOffset = new Vector3(0, selectedUnit.transform.localScale.y, 0);

        if (newNode.GetComponent<GridNode>().unit == null)
        {
            Debug.Log("Valid move!");

            // Move the gamobject to the new node location, based off its position transform
            selectedUnit.transform.position = newNode.GetComponent<GridNode>().nodeWorldPos + unitOffset;

            // Transfer the gameobject to the new node
            newNode.GetComponent<GridNode>().unit = selectedUnit;

            // Remove the gameobject from the previous node
            oldNode.GetComponent<GridNode>().unit = null;

            // Make the new node location the selectedUnitsNode
            //selectedUnitsNode = newNode;
            selectedUnit.GetComponent<Unit>().currentNode = newNode;
        }
        else
        {
            Debug.Log("Invalid move: Unit/Obstacle in the way!");
        }
    }
}
