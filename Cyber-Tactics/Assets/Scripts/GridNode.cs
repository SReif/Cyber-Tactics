using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public GameObject currentUnit;      // The unit occupying this node, if any
    public Vector3 nodeWorldPos;        // The world space position of the node (Ex. Vector3(0, 1, 2)
    public Vector2 nodeGridPos;         // The grid position of the node (Ex. [0, 1] or Vector2(0, 1))
    public bool validMove;              

    // Start is called before the first frame update
    void Start()
    {
        nodeWorldPos = transform.position;
        validMove = false;
    }
}
