using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardType;     // The type of card (EX: PHYS ATK, PHYS DEF, HEAL, MAG ATK, MAG DEF, BUFF)
    public int modifier;        // The modifier that will be used based on the card type (EX: 1, 2, 3) (NOTE: Buff cards will have 0 for their modifier.)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
