using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public GameObject unit;
    public Vector3 nodeWorldPos;
    public Vector2 nodeGridPos;

    // Start is called before the first frame update
    void Start()
    {
        nodeWorldPos = transform.position;
    }
}
