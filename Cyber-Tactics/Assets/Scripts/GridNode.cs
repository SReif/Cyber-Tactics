using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public Vector3 nodeWorldPos;                // The world space position of the node (Ex. Vector3(0, 1, 2)
    public Vector2 nodeGridPos;                 // The grid position of the node (Ex. [0, 1] or Vector2(0, 1))
    public bool validMove;                      // Whether this node is a valid move for a unit's moveset
    public bool validAttack;                    // Whether this node is a valid attack for a unit's attackset

    private GameObject validMoveIndicator;
    private GameObject validAttackIndicator;

    // Start is called before the first frame update
    void Start()
    {
        nodeWorldPos = transform.position;
        validMove = false;
        validAttack = false;

        validMoveIndicator = transform.gameObject.transform.Find("Indicators").GetChild(0).gameObject;
        validAttackIndicator = transform.gameObject.transform.Find("Indicators").GetChild(1).gameObject;
    }

    private void OnMouseEnter()
    {
        if (validMove)
        {
            validMoveIndicator.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color (Hovered)", typeof(Material)) as Material;
        }

        if (validAttack)
        {
            validAttackIndicator.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Attack Node Color (Hovered)", typeof(Material)) as Material;
        }
    }

    private void OnMouseExit()
    {
        if (validMoveIndicator.activeSelf)
        {
            validMoveIndicator.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
        }

        if (validAttackIndicator.activeSelf)
        {
            validAttackIndicator.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Attack Node Color", typeof(Material)) as Material;
        }
    }
}
