using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This script gets attached to any card in scene. It
public class EnableCardText : MonoBehaviour
{
    public BattleTurnSystem battleTurnSystem;
    private Card card;
    public TextMeshProUGUI cardText;
    public GameObject textPanel;

    private string cardType;
    private int cardModifier;

    private string fullText;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Initializing card components
        battleTurnSystem = GameObject.Find("Battle Turn System").GetComponent<BattleTurnSystem>();
        card = this.GetComponent<Card>();
        cardType = card.cardType;
        cardModifier = card.modifier;

        //Writing out the full length of the card type
        if (cardType == "MAG ATK")
        {
            fullText = "Magical Attack";
        }

        if (cardType == "PHYS ATK")
        {
            fullText = "Physical Attack";
        }

        if (cardType == "PHYS DEF")
        {
            fullText = "Physical Defense";
        }

        if (cardType == "MAG DEF")
        {
            fullText = "Magical Defense";
        }

        cardText.text = "+" + cardModifier + " " + fullText;

        if (cardType == "Heal")
        {
            cardText.text = "Heal for " + cardModifier;
        }

        //Setting color of card text based on if it's a positive or negative value
        /*if(cardModifier > 0)
        {
            cardText.color = Color.blue;
        }

        if(cardModifier < 0)
        {
            cardText.color = Color.red;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //Enabling and disabling the card text based on if the card is selected or not
        if (battleTurnSystem.playerSelectedCards.Contains(this.gameObject))
        {
            textPanel.gameObject.SetActive(true);
        }

        else
        {
            textPanel.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        
    }
}
