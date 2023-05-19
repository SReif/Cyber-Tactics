using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnSystem : MonoBehaviour
{
    //public State state;

    private AudioManager audioManager;

    public bool takingTurn;

    public GameObject playerUnitLocation;               // The GameObject that stores the location that the player unit appears in the battle
    public GameObject enemyUnitLocation;                // The GameObject that stores the location that the player unit appears in the battle
    public GameObject playerCardSlots;                  // The GameObject that stores the location of the player card slots
    public GameObject enemyCardSlots;                   // The GameObject that stores the location of the enemy card slots
    public GameObject battleViewCamera;                 // The camera that is enabled/disabled for the battle 
    public GameObject gridViewCamera;                   // The camera that is used for the grid; the battle moves back to this camera at the end

    [System.NonSerialized] public int totalPlayerPHYSDEFModifier = 0;          // The total amount of PHYSICAL DEFENSE that is being applied to the player from cards
    [System.NonSerialized] public int totalPlayerPHYSATKModifier = 0;          // The total amount of PHYSICAL ATTACK that is being applied to the player from cards
    [System.NonSerialized] public int totalPlayerMAGATKModifier = 0;           // The total amount of MAGICAL ATTACK that is being applied to the player from cards
    [System.NonSerialized] public int totalPlayerMAGDEFModifier = 0;           // The total amount of MAGICAL DEFENSE that is being applied to the player from cards
    [System.NonSerialized] public int totalPlayerHEALModifier = 0;             // The total amount of HEALING that is being applied to the player from cards

    [System.NonSerialized] public int totalEnemyPHYSDEFModifier = 0;           // The total amount of PHYSICAL DEFENSE that is being applied to the enemy from cards
    [System.NonSerialized] public int totalEnemyPHYSATKModifier = 0;           // The total amount of PHYSICAL ATTACK that is being applied to the enemy from cards
    [System.NonSerialized] public int totalEnemyMAGATKModifier = 0;            // The total amount of MAGICAL ATTACK that is being applied to the enemy from cards
    [System.NonSerialized] public int totalEnemyMAGDEFModifier = 0;            // The total amount of MAGICAL DEFENSE that is being applied to the enemy from cards
    [System.NonSerialized] public int totalEnemyHEALModifier = 0;              // The total amount of HEALING that is being applied to the enemy from cards

    [System.NonSerialized] public List<GameObject> playerSelectedCards;         // The list of cards that the player selected for their turn
    [System.NonSerialized] public List<GameObject> enemySelectedCards;          // The list of cards that the enemy selected for their turn

    [System.NonSerialized] public GameObject playerUnitClone;       // A clone of the player unit in the battle for visual reference
    [System.NonSerialized] public GameObject enemyUnitClone;        // A clone of the enemy unit in the battle for visual reference
    private List<GameObject> playerDeck;                            // A copy of the player's deck; cards are removed from here when they are placed in the player's hand
    private List<GameObject> enemyDeck;                             // A copy of the enemy's deck; cards are removed from here when they are placed in the enemy's hand

    //private List<GameObject> enemyHand;

    private int playerNumBuffCards = 0;
    private int playerNumDebuffCards = 0;
    private int enemyNumBuffCards = 0;
    private int enemyNumDebuffCards = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerSelectedCards = new List<GameObject>();
        enemySelectedCards = new List<GameObject>();

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Battle(GameObject playerUnit, GameObject enemyUnit, string battleInitiator)
    {
        // Setup the battle view clone of the player unit
        playerUnitClone = Instantiate(playerUnit, playerUnitLocation.transform);
        playerUnitClone.GetComponent<MeshRenderer>().enabled = false;
        playerUnitClone.transform.Find("Battle View Model").GetComponent<MeshRenderer>().enabled = true;
        playerUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        // Setup the battle view clone of the enemy unit
        enemyUnitClone = Instantiate(enemyUnit, enemyUnitLocation.transform);
        enemyUnitClone.GetComponent<MeshRenderer>().enabled = false;
        enemyUnitClone.transform.Find("Battle View Model").GetComponent<MeshRenderer>().enabled = true;
        enemyUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        //Make a copy of the player unit's deck
        playerDeck = new List<GameObject>(playerUnit.GetComponent<Unit>().cards);
        enemyDeck = new List<GameObject>(enemyUnit.GetComponent<Unit>().cards);

        //playerHand = new List<GameObject>();
        //enemyHand = new List<GameObject>();

        // Iterate through the player unit's cards list and populate the card slots with them
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            if (playerDeck.Count == 0)
            {
                break;
            }
            else
            {
                // Pick a random card from the player unit's deck
                int index = Random.Range(0, playerDeck.Count);
                GameObject card = GameObject.Instantiate(playerDeck[index].gameObject, playerCardSlots.transform.GetChild(i).Find("Card").transform);

                card.SetActive(true);

                // Remove the card from the copy of the player unit's deck
                playerDeck.Remove(playerDeck[index]);
                playerDeck.TrimExcess();
            }
        }

        // Iterate through the enemy unit's cards list and populate the card slots with them
        for (int i = 0; i < enemyCardSlots.transform.childCount; i++)
        {
            if (enemyDeck.Count == 0)
            {
                break;
            }
            else
            {
                // Pick a random card from the enemy unit's deck
                int index = Random.Range(0, enemyDeck.Count);
                GameObject card = GameObject.Instantiate(enemyDeck[index].gameObject, enemyCardSlots.transform.GetChild(i).Find("Card").transform);

                // DO NOT SET THE ENEMY CARDS ACTIVE UNTIL YOU RESOLVE THE CARDS
                card.SetActive(false);

                // Remove the card from the copy of the player unit's deck
                enemyDeck.Remove(enemyDeck[index]);
                enemyDeck.TrimExcess();
            }
        }

        /*
        // FOR DEBUG USE ONLY BECAUSE THERE IS NO VISUAL FOR ENEMY HAND
        Debug.Log("Enemy's hand includes: ");
        for (int i = 0; i < enemyHand.Count; i++)
        {
            Debug.Log(enemyHand[i].GetComponent<Card>().cardType + " " + enemyHand[i].GetComponent<Card>().modifier + " " + enemyHand[i].GetComponent<Card>().element);
        }
        */

        // Change over to the Battle View Camera
        gridViewCamera.SetActive(false);
        battleViewCamera.SetActive(true);

        checkForInitiatorBoost(battleInitiator);

        if (battleInitiator == "Player")
        {
            /*
             *  Beginning of Player's turn
             */
            //state = State.PlayerTurn;
            Debug.Log("Player's turn!");
            yield return StartCoroutine(BeginPlayersTurn(playerUnit));

            /*
             *  Beginning of Enemy's turn
             */
            //state = State.EnemyTurn;
            Debug.Log("Enemy's turn!");
            yield return StartCoroutine(BeginEnemysTurn(enemyUnit));

            /*
             *  Preparing card calculations
             */
            //state = State.ResolveCards;
            Debug.Log("Preparing calculations!");
            yield return StartCoroutine(resolveCardModifiers(playerUnit, enemyUnit));
        }
        else if (battleInitiator == "Enemy")
        {
            /*
             *  Beginning of Enemy's turn
             */
            //state = State.EnemyTurn;
            Debug.Log("Enemy's turn!");
            yield return StartCoroutine(BeginEnemysTurn(enemyUnit));

            /*
             *  Beginning of Player's turn
             */
            //state = State.PlayerTurn;
            Debug.Log("Player's turn!");


            yield return StartCoroutine(BeginPlayersTurn(playerUnit));

            /*
             *  Preparing card calculations
             */
            //state = State.ResolveCards;
            Debug.Log("Preparing calculations!");
            yield return StartCoroutine(resolveCardModifiers(playerUnit, enemyUnit));
        }

        // Change back to the Grid View Camera
        battleViewCamera.SetActive(false);
        gridViewCamera.SetActive(true);

        cleanUpBattleView(playerUnit, enemyUnit);

        yield return null;
    }

    public void updateStatsFromCard(string cardType, int modifier, string cardElement, string currentTurn, string action)
    {
        if (currentTurn == "Player")
        {
            string playerUnitElement = playerUnitClone.GetComponent<Unit>().element;

            if (action == "Selecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalPlayerPHYSATKModifier += modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerPHYSATKModifier++;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalPlayerPHYSDEFModifier += modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerPHYSDEFModifier++;
                }
                else if (cardType == "MAG ATK")
                {
                    totalPlayerMAGATKModifier += modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerMAGATKModifier++;
                }
                else if (cardType == "MAG DEF")
                {
                    totalPlayerMAGDEFModifier += modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerMAGDEFModifier++;
                }
                else if (cardType == "HEAL")
                {
                    totalPlayerHEALModifier += modifier;
                }
                else if (cardType == "BUFF")
                {
                    playerNumBuffCards++;
                }
                else if (cardType == "DEBUFF")
                {
                    playerNumDebuffCards++;
                }
            }
            else if (action == "Deselecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalPlayerPHYSATKModifier -= modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerPHYSATKModifier--;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalPlayerPHYSDEFModifier -= modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerPHYSDEFModifier--;
                }
                else if (cardType == "MAG ATK")
                {
                    totalPlayerMAGATKModifier -= modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerMAGATKModifier--;
                }
                else if (cardType == "MAG DEF")
                {
                    totalPlayerMAGDEFModifier -= modifier;

                    if (playerUnitElement == cardElement)
                        totalPlayerMAGDEFModifier--;
                }
                else if (cardType == "HEAL")
                {
                    totalPlayerHEALModifier -= modifier;
                }
                else if (cardType == "BUFF")
                {
                    playerNumBuffCards--;
                }
                else if (cardType == "DEBUFF")
                {
                    playerNumDebuffCards--;
                }
            }

        }
        else if (currentTurn == "Enemy")
        {
            string enemyUnitElement = enemyUnitClone.GetComponent<Unit>().element;

            if (action == "Selecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalEnemyPHYSATKModifier += modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyPHYSATKModifier++;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalEnemyPHYSDEFModifier += modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyPHYSDEFModifier++;
                }
                else if (cardType == "MAG ATK")
                {
                    totalEnemyMAGATKModifier += modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyMAGATKModifier++;
                }
                else if (cardType == "MAG DEF")
                {
                    totalEnemyMAGDEFModifier += modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyMAGDEFModifier++;
                }
                else if (cardType == "HEAL")
                {
                    totalEnemyHEALModifier += modifier;
                }
                else if (cardType == "BUFF")
                {
                    enemyNumBuffCards++;
                }
                else if (cardType == "DEBUFF")
                {
                    enemyNumDebuffCards++;
                }
            }
            else if (action == "Deselecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalEnemyPHYSATKModifier -= modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyPHYSATKModifier--;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalEnemyPHYSDEFModifier -= modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyPHYSDEFModifier--;
                }
                else if (cardType == "MAG ATK")
                {
                    totalEnemyMAGATKModifier -= modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyMAGATKModifier--;
                }
                else if (cardType == "MAG DEF")
                {
                    totalEnemyMAGDEFModifier -= modifier;

                    if (enemyUnitElement == cardElement)
                        totalEnemyMAGDEFModifier--;
                }
                else if (cardType == "HEAL")
                {
                    totalEnemyHEALModifier -= modifier;
                }
                else if (cardType == "BUFF")
                {
                    enemyNumBuffCards--;
                }
                else if (cardType == "DEBUFF")
                {
                    enemyNumDebuffCards--;
                }
            }
        }
    }

    public IEnumerator BeginPlayersTurn(GameObject playerUnit)
    {
        yield return new WaitForSeconds(0.33f);

        playerUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);

        takingTurn = true;

        while (takingTurn)
        {
            var ray = battleViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Card")
                {
                    string cardType = hit.transform.gameObject.GetComponent<Card>().cardType;
                    int cardModifier = hit.transform.gameObject.GetComponent<Card>().modifier;
                    string cardElement = hit.transform.gameObject.GetComponent<Card>().element;

                    // Check to see if the card has already been selected
                    // If index == -1, the card is not in the list
                    if (playerSelectedCards.IndexOf(hit.transform.gameObject) == -1)
                    {
                        // Check to see if the player is trying to heal themselves at full HP
                        if (cardType == "HEAL" && (playerUnitClone.GetComponent<Unit>().maxHP > playerUnitClone.GetComponent<Unit>().currentHP))
                        {
                            Debug.Log("Player is not at full health!");
                            Debug.Log("Adding card to selected pool!");

                            updateStatsFromCard(cardType, cardModifier, cardElement, "Player", "Selecting");
                            playerSelectedCards.Add(hit.transform.gameObject);

                            // Find the Selected Card Indicator of the Card Slot and activate it
                            hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);
                        }
                        else if (cardType == "HEAL" && (playerUnitClone.GetComponent<Unit>().maxHP == playerUnitClone.GetComponent<Unit>().currentHP))
                        {
                            Debug.Log("Player is at full health!");
                        }
                        else if (cardType == "BUFF")
                        {
                            Debug.Log("Player selected buff card. Adding it to selected pool.");

                            updateStatsFromCard(cardType, cardModifier, cardElement, "Player", "Selecting");
                            playerSelectedCards.Add(hit.transform.gameObject);

                            // Find the Selected Card Indicator of the Card Slot and activate it
                            hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);
                        }
                        else if (cardType == "DEBUFF")
                        {
                            Debug.Log("Player selected debuff card. Adding it to selected pool.");

                            updateStatsFromCard(cardType, cardModifier, cardElement, "Player", "Selecting");
                            playerSelectedCards.Add(hit.transform.gameObject);

                            // Find the Selected Card Indicator of the Card Slot and activate it
                            hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);
                        }
                        else if (hit.transform.gameObject.GetComponent<Card>().cardType != "HEAL")
                        {
                            Debug.Log("Adding card to selected pool!");

                            if (cardElement == playerUnitClone.GetComponent<Unit>().element)
                            {
                                toggleCardText(hit.transform.gameObject, "Player");
                            }

                            updateStatsFromCard(cardType, cardModifier, cardElement, "Player", "Selecting");
                            playerSelectedCards.Add(hit.transform.gameObject);

                            // Find the Selected Card Indicator of the Card Slot and activate it
                            hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.Log("Deselecting card!");

                        if (cardElement == playerUnitClone.GetComponent<Unit>().element)
                        {
                            toggleCardText(hit.transform.gameObject, "Player");
                        }

                        updateStatsFromCard(cardType, cardModifier, cardElement, "Player", "Deselecting");
                        playerSelectedCards.Remove(hit.transform.gameObject);

                        // Find the Selected Card Indicator of the Card Slot and activate it
                        hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(false);
                    }
                }
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator BeginEnemysTurn(GameObject enemyUnit)
    {
        enemyUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);

        bool selectedHEALCard = false;
        bool selectedATKCard = false;
        bool selectedDEFCard = false;

        // Select cards for the enemy to play
        for (int i = 0; i < enemyCardSlots.transform.childCount; i++)
        {

            if (enemyCardSlots.transform.GetChild(i).Find("Card").childCount != 0)
            {
                string cardType = enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).GetComponent<Card>().cardType;
                int cardModifier = enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).GetComponent<Card>().modifier;
                string cardElement = enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).GetComponent<Card>().element;

                // Select up to one ATK card
                if ((cardType == "PHYS ATK" || cardType == "MAG ATK") && !selectedATKCard)
                {
                    Debug.Log("Enemy selects an attack card.");

                    /*
                    if (cardElement == playerUnitClone.GetComponent<Unit>().element)
                    {
                        toggleCardText(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject, "Enemy");
                    }
                    */

                    enemySelectedCards.Add(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);
                    updateStatsFromCard(cardType, cardModifier, cardElement, "Enemy", "Selecting");
                    selectedATKCard = true;
                }

                // Select up to one DEF card
                if ((cardType == "PHYS DEF" || cardType == "MAG DEF") && !selectedDEFCard)
                {
                    Debug.Log("Enemy selects a defense card.");

                    /*
                    if (cardElement == playerUnitClone.GetComponent<Unit>().element)
                    {
                        toggleCardText(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject, "Enemy");
                    }
                    */

                    enemySelectedCards.Add(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);
                    updateStatsFromCard(cardType, cardModifier, cardElement, "Enemy", "Selecting");
                    selectedDEFCard = true;
                }

                // Select up to one HEAL card, if the enemy is missing health
                if (cardType == "HEAL" && !selectedHEALCard && (enemyUnitClone.GetComponent<Unit>().currentHP < enemyUnitClone.GetComponent<Unit>().maxHP))
                {
                    Debug.Log("Enemy selects a heal card.");

                    enemySelectedCards.Add(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);
                    updateStatsFromCard(cardType, cardModifier, cardElement, "Enemy", "Selecting");
                    selectedHEALCard = true;
                }

                // Select ALL BUFF cards in hand
                if (cardType == "BUFF")
                {
                    Debug.Log("Enemy selects a buff card.");

                    enemySelectedCards.Add(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);
                    updateStatsFromCard(cardType, cardModifier, cardElement, "Enemy", "Selecting");
                }

                // Select ALL DEBUFF cards in hand
                if (cardType == "DEBUFF")
                {
                    Debug.Log("Enemy selects a debuff card.");

                    enemySelectedCards.Add(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);
                    updateStatsFromCard(cardType, cardModifier, cardElement, "Enemy", "Selecting");
                }
            }
        }

        // FOR DEBUG USE ONLY BECAUSE THERE IS NO VISUAL FOR ENEMY SELECTED CARDS
        /*
        Debug.Log("Enemy's selected cards includes: " + enemySelectedCards.Count);
        for (int i = 0; i < enemySelectedCards.Count; i++)
        {
            Debug.Log(enemySelectedCards[i].GetComponent<Card>().cardType + " "
                + enemySelectedCards[i].GetComponent<Card>().modifier + " "
                + enemySelectedCards[i].GetComponent<Card>().element);
        }
        */

        yield return new WaitForSeconds(1);

        enemyUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        yield return null;
    }

    IEnumerator resolveCardModifiers(GameObject playerUnit, GameObject enemyUnit)
    {
        Debug.Log("Resolving cards!");

        // Set all cards in the enemy slots to active
        for (int i = 0; i < enemyCardSlots.transform.childCount; i++)
        {
            if (enemyCardSlots.transform.GetChild(i).Find("Card").childCount != 0)
            {
                enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject.SetActive(true);
            }
        }

        // Activate the selected card indicator for the cards that the enemy selected
        for (int i = 0; i < enemySelectedCards.Count; i++)
        {
            enemySelectedCards[i].transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);

            string cardType = enemySelectedCards[i].GetComponent<Card>().cardType;
            string cardElement = enemySelectedCards[i].GetComponent<Card>().element;

            Debug.Log("Enemy Card's Element: " + cardElement);
            Debug.Log("Enemy Unit's Element: " + enemyUnitClone.GetComponent<Unit>().element);

            if (cardElement == enemyUnitClone.GetComponent<Unit>().element)
            {
                toggleCardText(enemySelectedCards[i], "Enemy");
            }

            /*
            if (cardType == "MAG DEF" || cardType == "MAG ATK" || cardType == "PHYS DEF" || cardType == "PHYS ATK")
            {
                enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).transform.GetChild(0).Find("Card Text").gameObject.SetActive(true);
            }
            */
        }

        yield return new WaitForSeconds(.3f);

        print("Buff/Debuff Card Results:");
        print("playerNumBuffCards: " + playerNumBuffCards);
        print("playerNumDebuffCards: " + playerNumDebuffCards);
        print("enemyNumBuffCards: " + enemyNumBuffCards);
        print("enemyNumDebuffCards: " + enemyNumDebuffCards);

        // Check to see if the player has more buff cards than the enemy has debuff cards
        if (playerNumBuffCards > enemyNumDebuffCards)
        {
            Debug.Log("Player has more buff cards than the enemy has debuff cards!");

            int numActivatedBuffCards = playerNumBuffCards - enemyNumDebuffCards;

            for (int i = 0; i < playerSelectedCards.Count; i++)
            {
                if (numActivatedBuffCards == 0)
                {
                    break;
                }
                else
                {
                    string cardType = playerSelectedCards[i].GetComponent<Card>().cardType;

                    if (cardType == "BUFF")
                    {
                        // Player receives the BUFF
                        if (playerDeck.Count != 0)
                        {
                            Debug.Log("Deck is not empty!");

                            int index = Random.Range(0, playerDeck.Count);

                            // Add the new random card in the same location as the old buff card's location
                            GameObject card = GameObject.Instantiate(playerDeck[index].gameObject, playerSelectedCards[i].transform.parent.transform);

                            if (card.GetComponent<Card>().cardType != "BUFF")
                            {
                                playerNumBuffCards--;
                            }

                            card.SetActive(true);

                            updateStatsFromCard(card.GetComponent<Card>().cardType, card.GetComponent<Card>().modifier, card.GetComponent<Card>().element, "Player", "Selecting");

                            if (card.GetComponent<Card>().element == playerUnitClone.GetComponent<Unit>().element)
                            {
                                toggleCardText(card, "Player");
                            }

                            Debug.Log("Playing " + card.GetComponent<Card>().cardType + " " + card.GetComponent<Card>().modifier + " " + card.GetComponent<Card>().element);
                        }

                        numActivatedBuffCards--;
                    }
                }
            }
        }

        yield return new WaitForSeconds(.3f);

        // Check to see if the player has more debuff cards than the enemy has buff cards
        if (playerNumDebuffCards > enemyNumBuffCards)
        {
            Debug.Log("Player has more debuff cards than the enemy has buff cards!");

            int numActivatedDebuffCards = playerNumDebuffCards - enemyNumBuffCards;

            /*
            for (int i = 0; i < playerSelectedCards.Count; i++)
            {
                if (numActivatedDebuffCards == 0)
                {
                    break;
                }
                else
                {
                    string cardType = playerSelectedCards[i].GetComponent<Card>().cardType;

                    if (cardType == "DEBUFF")
                    {
                        // Player successfully debuffs the enemy

                        if (enemySelectedCards.Count > 0)
                        {
                            Debug.Log("Enemy must remove a card because of the debuff.");

                            // Pick a random card from the enemy's selected cards to remove from this battle. Update the stats as if the card was deselected, but don't remove the card from the scenario.
                            int index = Random.Range(0, enemySelectedCards.Count);
                            updateStatsFromCard(enemySelectedCards[index].GetComponent<Card>().cardType, enemySelectedCards[index].GetComponent<Card>().modifier,
                                enemySelectedCards[index].GetComponent<Card>().element, "Enemy", "Deselecting");

                            enemySelectedCards[index].transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(false);

                            enemySelectedCards.Remove(enemySelectedCards[index]);
                            enemySelectedCards.TrimExcess();
                        }

                        numActivatedDebuffCards--;
                    }
                }
            }
            */

            for (int i = 0; i < numActivatedDebuffCards; i++)
            {
                // Player successfully debuffs the enemy

                if (enemySelectedCards.Count > 0)
                {
                    Debug.Log("Enemy must remove a card because of the debuff.");

                    // Pick a random card from the enemy's selected cards to remove from this battle. Update the stats as if the card was deselected, but don't remove the card from the scenario.
                    int index = Random.Range(0, enemySelectedCards.Count);
                    updateStatsFromCard(enemySelectedCards[index].GetComponent<Card>().cardType, enemySelectedCards[index].GetComponent<Card>().modifier,
                        enemySelectedCards[index].GetComponent<Card>().element, "Enemy", "Deselecting");

                    enemySelectedCards[index].transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(false);

                    if (enemySelectedCards[index].GetComponent<Card>().element == enemyUnitClone.GetComponent<Unit>().element)
                    {
                        toggleCardText(enemySelectedCards[index], "Enemy");
                    }

                    enemySelectedCards.Remove(enemySelectedCards[index]);
                    enemySelectedCards.TrimExcess();
                }
            }
        }

        yield return new WaitForSeconds(.3f);

        // Check to see if the enemy has more buff cards than the player has debuff cards
        if (enemyNumBuffCards > playerNumDebuffCards)
        {
            Debug.Log("Enemy has more buff cards than the player has debuff cards!");

            int numActivatedBuffCards = enemyNumBuffCards - playerNumDebuffCards;

            for (int i = 0; i < enemySelectedCards.Count; i++)
            {
                if (numActivatedBuffCards == 0)
                {
                    break;
                }
                else
                {
                    string cardType = enemySelectedCards[i].GetComponent<Card>().cardType;

                    if (cardType == "BUFF")
                    {
                        // Enemy receives the BUFF

                        if (enemyDeck.Count != 0)
                        {
                            Debug.Log("Deck is not empty!");

                            int index = Random.Range(0, enemyDeck.Count);
                            GameObject card = GameObject.Instantiate(enemyDeck[index].gameObject, enemySelectedCards[i].transform.parent.transform);

                            if (card.GetComponent<Card>().cardType != "BUFF")
                            {
                                enemyNumBuffCards--;
                            }

                            card.SetActive(true);
                            updateStatsFromCard(card.GetComponent<Card>().cardType, card.GetComponent<Card>().modifier, card.GetComponent<Card>().element, "Enemy", "Selecting");

                            if (card.GetComponent<Card>().element == enemyUnitClone.GetComponent<Unit>().element)
                            {
                                toggleCardText(card, "Enemy");
                            }

                            Debug.Log("Playing " + card.GetComponent<Card>().cardType + " " + card.GetComponent<Card>().modifier + " " + card.GetComponent<Card>().element);
                        }

                        numActivatedBuffCards--;
                    }
                }
            }
        }

        yield return new WaitForSeconds(.3f);

        // Check to see if the enemy has more debuff cards than the playerhas buff cards
        if (enemyNumDebuffCards > playerNumBuffCards)
        {
            Debug.Log("Enemy has more debuff cards than the player has buff cards!");

            int numActivatedDebuffCards = enemyNumDebuffCards - playerNumBuffCards;

            for (int i = 0; i < numActivatedDebuffCards; i++)
            {
                // Enemy successfully debuffs the player

                if (playerSelectedCards.Count > 0)
                {
                    Debug.Log("Player must remove a card because of the debuff.");

                    // Pick a random card from the enemy's selected cards to remove from this battle. Update the stats as if the card was deselected, but don't remove the card from the scenario.
                    int index = Random.Range(0, playerSelectedCards.Count);
                    updateStatsFromCard(playerSelectedCards[index].GetComponent<Card>().cardType, playerSelectedCards[index].GetComponent<Card>().modifier,
                        playerSelectedCards[index].GetComponent<Card>().element, "Player", "Deselecting");

                    playerSelectedCards[index].transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(false);

                    if (playerSelectedCards[index].GetComponent<Card>().element == playerUnitClone.GetComponent<Unit>().element)
                    {
                        toggleCardText(playerSelectedCards[index], "Player");
                    }

                    playerSelectedCards.Remove(playerSelectedCards[index]);
                    playerSelectedCards.TrimExcess();
                }
            }

            /*
            for (int i = 0; i < enemySelectedCards.Count; i++)
            {
                if (numActivatedDebuffCards == 0)
                {
                    break;
                }
                else
                {
                    string cardType = enemySelectedCards[i].GetComponent<Card>().cardType;

                    if (cardType == "DEBUFF")
                    {
                        // Enemy successfully debuffs the player

                        if (playerSelectedCards.Count > 0)
                        {
                            Debug.Log("Player must remove a card because of the debuff.");

                            // Pick a random card from the enemy's selected cards to remove from this battle. Update the stats as if the card was deselected, but don't remove the card from the scenario.
                            int index = Random.Range(0, playerSelectedCards.Count);
                            updateStatsFromCard(playerSelectedCards[index].GetComponent<Card>().cardType, playerSelectedCards[index].GetComponent<Card>().modifier,
                                playerSelectedCards[index].GetComponent<Card>().element, "Player", "Deselecting");

                            playerSelectedCards[index].transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(false);

                            playerSelectedCards.Remove(playerSelectedCards[index]);
                            playerSelectedCards.TrimExcess();
                        }

                        numActivatedDebuffCards--;
                    }
                }
            }
            */
        }

        yield return new WaitForSeconds(.3f);

        if ((playerNumBuffCards == enemyNumDebuffCards) && playerNumBuffCards != 0)
        {
            // Do nothing because they cancelled each other out
            Debug.Log("Player BUFF and enemy DEBUFF cards cancelled out!");
        }

        if ((enemyNumBuffCards == playerNumDebuffCards) && enemyNumBuffCards != 0)
        {
            // Do nothing because they cancelled each other out
            Debug.Log("Enemy BUFF and player DEBUFF cards cancelled out!");
        }

        int totalPlayerPhysicalDefense = playerUnitClone.GetComponent<Unit>().basePhysicalDefense + totalPlayerPHYSDEFModifier;
        int totalPlayerPhysicalAttack = playerUnitClone.GetComponent<Unit>().basePhysicalAttack + totalPlayerPHYSATKModifier;
        int totalPlayerMagicalAttack = playerUnitClone.GetComponent<Unit>().baseMagicalAttack + totalPlayerMAGATKModifier;
        int totalPlayerMagicalDefense = playerUnitClone.GetComponent<Unit>().baseMagicalDefense + totalPlayerMAGDEFModifier;

        int totalEnemyPhysicalDefense = enemyUnitClone.GetComponent<Unit>().basePhysicalDefense + totalEnemyPHYSDEFModifier;
        int totalEnemyPhysicalAttack = enemyUnitClone.GetComponent<Unit>().basePhysicalAttack + totalEnemyPHYSATKModifier;
        int totalEnemyMagicalAttack = enemyUnitClone.GetComponent<Unit>().baseMagicalAttack + totalEnemyMAGATKModifier;
        int totalEnemyMagicalDefense = enemyUnitClone.GetComponent<Unit>().baseMagicalDefense + totalEnemyMAGDEFModifier;

        // Calculate total damage taken by the player
        int totalPlayerDamageTaken = (totalEnemyPhysicalAttack - totalPlayerPhysicalDefense < 0) ? 0 : totalEnemyPhysicalAttack - totalPlayerPhysicalDefense;
        totalPlayerDamageTaken += (totalEnemyMagicalAttack - totalPlayerMagicalDefense < 0) ? 0 : totalEnemyMagicalAttack - totalPlayerMagicalDefense;

        // Calculate total damage taken by the enemy
        int totalEnemyDamageTaken = (totalPlayerPhysicalAttack - totalEnemyPhysicalDefense < 0) ? 0 : totalPlayerPhysicalAttack - totalEnemyPhysicalDefense;
        totalEnemyDamageTaken += (totalPlayerMagicalAttack - totalEnemyMagicalDefense < 0) ? 0 : totalPlayerMagicalAttack - totalEnemyMagicalDefense;

        // INSERT ATTACK ANIMATIONS COROUTINE HERE
        yield return StartCoroutine(attackAnimation());

        // Resolve the healing done and make sure the player unit cannot overheal themselves.
        playerUnit.GetComponent<Unit>().currentHP += totalPlayerHEALModifier;
        playerUnit.GetComponent<Unit>().currentHP = (playerUnit.GetComponent<Unit>().currentHP > playerUnit.GetComponent<Unit>().maxHP) ? playerUnit.GetComponent<Unit>().maxHP : playerUnit.GetComponent<Unit>().currentHP;

        // Resolve the damage taken for the player unit
        playerUnit.GetComponent<Unit>().currentHP -= totalPlayerDamageTaken;

        // Resolve the healing done and make sure the enemy unit cannot overheal themselves.
        enemyUnit.GetComponent<Unit>().currentHP += totalEnemyHEALModifier;
        enemyUnit.GetComponent<Unit>().currentHP = (enemyUnit.GetComponent<Unit>().currentHP > enemyUnit.GetComponent<Unit>().maxHP) ? enemyUnit.GetComponent<Unit>().maxHP : enemyUnit.GetComponent<Unit>().currentHP;

        // Resolve the damage taken for the enemy unit
        enemyUnit.GetComponent<Unit>().currentHP -= totalEnemyDamageTaken;

        yield return new WaitForSeconds(0.33f);

        Debug.Log("End battle!");

        //state = State.None;
    }

    public void cleanUpBattleView(GameObject playerUnit, GameObject enemyUnit)
    {
        // Remove the players cards from the card slots
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            if (playerCardSlots.transform.GetChild(i).Find("Card").transform.childCount > 0)
            {
                Destroy(playerCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);

                playerCardSlots.transform.GetChild(i).Find("Selected Card Indicator").gameObject.SetActive(false);
            }
        }

        // Remove the enemys cards from the card slots
        for (int i = 0; i < enemyCardSlots.transform.childCount; i++)
        {
            if (enemyCardSlots.transform.GetChild(i).Find("Card").transform.childCount > 0)
            {
                Destroy(enemyCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);

                enemyCardSlots.transform.GetChild(i).Find("Selected Card Indicator").gameObject.SetActive(false);
            }
        }

        // Remove the player unit clone
        Destroy(playerUnitClone);

        // Remove the enemy unit clone
        Destroy(enemyUnitClone);

        // Reset player stat modifier values
        totalPlayerPHYSDEFModifier = 0;
        totalPlayerPHYSATKModifier = 0;
        totalPlayerMAGATKModifier = 0;
        totalPlayerMAGDEFModifier = 0;
        totalPlayerHEALModifier = 0;

        // Reset enemy stat modifier values
        totalEnemyPHYSDEFModifier = 0;
        totalEnemyPHYSATKModifier = 0;
        totalEnemyMAGATKModifier = 0;
        totalEnemyMAGDEFModifier = 0;
        totalEnemyHEALModifier = 0;

        // Reset buff/debuff card counters
        playerNumBuffCards = 0;
        playerNumDebuffCards = 0;
        enemyNumBuffCards = 0;
        enemyNumDebuffCards = 0;

        // Destroy player cards that were used in this battle
        for (int i = 0; i < playerSelectedCards.Count; i++)
        {
            for (int j = 0; j < playerUnit.GetComponent<Unit>().cards.Count; j++)
            {
                string selectedCardCardType = playerSelectedCards[i].GetComponent<Card>().cardType;
                int selectedCardModifier = playerSelectedCards[i].GetComponent<Card>().modifier;
                string selectedCardElement = playerSelectedCards[i].GetComponent<Card>().element;

                string cardInDeckCardType = playerUnit.GetComponent<Unit>().cards[j].GetComponent<Card>().cardType;
                int cardInDeckModifier = playerUnit.GetComponent<Unit>().cards[j].GetComponent<Card>().modifier;
                string cardInDeckElement = playerUnit.GetComponent<Unit>().cards[j].GetComponent<Card>().element;

                if (selectedCardCardType == cardInDeckCardType && selectedCardModifier == cardInDeckModifier && selectedCardElement == cardInDeckElement)
                {
                    Debug.Log("Found selected card in player deck! Removing it from scenario!");
                    playerUnit.GetComponent<Unit>().cards.Remove(playerUnit.GetComponent<Unit>().cards[j]);
                    break;
                }
            }
        }
        playerUnit.GetComponent<Unit>().cards.TrimExcess();

        // Destroy enemy cards that were used in this battle
        for (int i = 0; i < enemySelectedCards.Count; i++)
        {
            for (int j = 0; j < enemyUnit.GetComponent<Unit>().cards.Count; j++)
            {
                string selectedCardCardType = enemySelectedCards[i].GetComponent<Card>().cardType;
                int selectedCardModifier = enemySelectedCards[i].GetComponent<Card>().modifier;
                string selectedCardElement = enemySelectedCards[i].GetComponent<Card>().element;

                string cardInDeckCardType = enemyUnit.GetComponent<Unit>().cards[j].GetComponent<Card>().cardType;
                int cardInDeckModifier = enemyUnit.GetComponent<Unit>().cards[j].GetComponent<Card>().modifier;
                string cardInDeckElement = enemyUnit.GetComponent<Unit>().cards[j].GetComponent<Card>().element;

                if (selectedCardCardType == cardInDeckCardType && selectedCardModifier == cardInDeckModifier && selectedCardElement == cardInDeckElement)
                {
                    Debug.Log("Found selected card in enemy deck! Removing it from scenario!");
                    enemyUnit.GetComponent<Unit>().cards.Remove(enemyUnit.GetComponent<Unit>().cards[j]);
                    break;
                }
            }
        }
        enemyUnit.GetComponent<Unit>().cards.TrimExcess();

        // Clear out any cards that were created by a player's buff card
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            if (playerCardSlots.transform.GetChild(i).Find("Card").childCount != 0)
            {
                for (int j = 0; j < playerCardSlots.transform.GetChild(i).Find("Card").transform.childCount; j++)
                {
                    Destroy(playerCardSlots.transform.GetChild(i).Find("Card").transform.GetChild(j).gameObject);
                }
            }
        }

        // Clear out any cards that were created by a enemy's buff card
        for (int i = 0; i < enemyCardSlots.transform.childCount; i++)
        {
            if (enemyCardSlots.transform.GetChild(i).Find("Card").childCount != 0)
            {
                for (int j = 0; j < enemyCardSlots.transform.GetChild(i).Find("Card").transform.childCount; j++)
                {
                    Destroy(enemyCardSlots.transform.GetChild(i).Find("Card").transform.GetChild(j).gameObject);
                }
            }
        }


        playerSelectedCards.Clear();
        enemySelectedCards.Clear();

        playerDeck.Clear();
        enemyDeck.Clear();
    }

    IEnumerator attackAnimation()
    {
        float maxDistance = 1f;
        float curDistance = 0f;
        float increment = 0.05f;

        // First Part
        // Enemy unit attacks and player unit reacts

        // Perform hit sound; the hit sound and animations should line up
        audioManager.Play("Battle Attack Hit");

        // Enemy unit moves toward player unit
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        // Enemy unit moves away from player unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        //Player unit moves away from enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        //Player unit moves toward enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.25f);

        // Second Part
        // Player unit attacks and enemy unit reacts

        // Perform hit sound again; the hit sound and animations should line up
        audioManager.Play("Battle Attack Hit");

        //Player unit moves toward enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        //Player unit moves away from enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        // Enemy unit moves away from player unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        // Enemy unit moves toward player unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    public void toggleCardText(GameObject card, string currentSide)
    {
        string cardType = card.GetComponent<Card>().cardType;
        int cardModifier = card.GetComponent<Card>().modifier;

        Debug.Log(card.transform.GetChild(0).Find("Card Text").gameObject.activeSelf);

        /*
        if (currentSide == "Player")
        {
            if (!card.transform.GetChild(0).Find("Card Text").gameObject.activeSelf)
            {
                card.transform.GetChild(0).Find("Card Text").Find("Card Type Text").GetComponent<TMPro.TMP_Text>().text = "+1 " + cardType;
                card.transform.GetChild(0).Find("Card Text").gameObject.SetActive(true);
            }
            else
            {
                card.transform.GetChild(0).Find("Card Text").gameObject.SetActive(false);
            }

        }
        else if (currentSide == "Enemy")
        {
            if (!card.transform.GetChild(0).Find("Card Text").gameObject.activeSelf)
            {
                card.transform.GetChild(0).Find("Card Text").Find("Card Type Text").GetComponent<TMPro.TMP_Text>().text = "+1 " + cardType;
                card.transform.GetChild(0).Find("Card Text").gameObject.SetActive(true);
            }
            else
            {
                card.transform.GetChild(0).Find("Card Text").gameObject.SetActive(false);
            }
        }
        */

        if (!card.transform.GetChild(0).Find("Card Text").gameObject.activeSelf)
        {
            card.transform.GetChild(0).Find("Card Text").Find("Card Type Text").GetComponent<TMPro.TMP_Text>().text = "+1 " + cardType;
            card.transform.GetChild(0).Find("Card Text").gameObject.SetActive(true);
        }
        else
        {
            card.transform.GetChild(0).Find("Card Text").gameObject.SetActive(false);
        }
    }

    public void checkForInitiatorBoost(string battleInitiator)
    {
        string playerElement = playerUnitClone.GetComponent<Unit>().element;
        string enemyElement = enemyUnitClone.GetComponent<Unit>().element;

        if (playerElement == "Electricity" && enemyElement == "Electricity")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Electricity" && enemyElement == "Water")
        {
            if (battleInitiator == "Player")
            {
                totalPlayerPHYSATKModifier++;
                totalPlayerPHYSDEFModifier++;
                totalPlayerMAGATKModifier++;
                totalPlayerMAGDEFModifier++;
            }
            else
            {
                totalEnemyPHYSATKModifier++;
                totalEnemyPHYSDEFModifier++;
                totalEnemyMAGATKModifier++;
                totalEnemyMAGDEFModifier++;
            }
        }
        else if (playerElement == "Electricity" && enemyElement == "Sand")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Electricity" && enemyElement == "Earth")
        {
            if (battleInitiator == "Player")
            {
                totalPlayerPHYSATKModifier += 2;
                totalPlayerPHYSDEFModifier += 2;
                totalPlayerMAGATKModifier += 2;
                totalPlayerMAGDEFModifier += 2;
            }
            else
            {
                /*
                enemyUnitClone.GetComponent<Unit>().basePhysicalAttack = (enemyUnitClone.GetComponent<Unit>().basePhysicalAttack == 0) 
                    ? enemyUnitClone.GetComponent<Unit>().basePhysicalAttack : enemyUnitClone.GetComponent<Unit>().basePhysicalAttack--;
                enemyUnitClone.GetComponent<Unit>().basePhysicalDefense = (enemyUnitClone.GetComponent<Unit>().basePhysicalDefense == 0)
                    ? enemyUnitClone.GetComponent<Unit>().basePhysicalDefense : enemyUnitClone.GetComponent<Unit>().basePhysicalDefense--;
                enemyUnitClone.GetComponent<Unit>().baseMagicalAttack = (enemyUnitClone.GetComponent<Unit>().baseMagicalAttack == 0)
                    ? enemyUnitClone.GetComponent<Unit>().baseMagicalAttack : enemyUnitClone.GetComponent<Unit>().baseMagicalAttack--;
                enemyUnitClone.GetComponent<Unit>().baseMagicalDefense = (enemyUnitClone.GetComponent<Unit>().baseMagicalDefense == 0)
                    ? enemyUnitClone.GetComponent<Unit>().baseMagicalDefense : enemyUnitClone.GetComponent<Unit>().baseMagicalDefense--;
                */
                /*
                totalEnemyPHYSATKModifier--;
                totalEnemyPHYSDEFModifier--;
                totalEnemyMAGATKModifier--;
                totalEnemyMAGDEFModifier--;
                */

                // Nothing happens
            }
        }
        else if (playerElement == "Water" && enemyElement == "Electricity")
        {
            if (battleInitiator == "Player")
            {
                totalPlayerPHYSATKModifier++;
                totalPlayerPHYSDEFModifier++;
                totalPlayerMAGATKModifier++;
                totalPlayerMAGDEFModifier++;
            }
            else
            {
                totalEnemyPHYSATKModifier++;
                totalEnemyPHYSDEFModifier++;
                totalEnemyMAGATKModifier++;
                totalEnemyMAGDEFModifier++;
            }
        }
        else if (playerElement == "Water" && enemyElement == "Water")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Water" && enemyElement == "Sand")
        {
            if (battleInitiator == "Player")
            {
                totalPlayerPHYSATKModifier += 2;
                totalPlayerPHYSDEFModifier += 2;
                totalPlayerMAGATKModifier += 2;
                totalPlayerMAGDEFModifier += 2;
            }
            else
            {
                /*
                enemyUnitClone.GetComponent<Unit>().basePhysicalAttack = (enemyUnitClone.GetComponent<Unit>().basePhysicalAttack == 0)
                    ? enemyUnitClone.GetComponent<Unit>().basePhysicalAttack : enemyUnitClone.GetComponent<Unit>().basePhysicalAttack--;
                enemyUnitClone.GetComponent<Unit>().basePhysicalDefense = (enemyUnitClone.GetComponent<Unit>().basePhysicalDefense == 0)
                    ? enemyUnitClone.GetComponent<Unit>().basePhysicalDefense : enemyUnitClone.GetComponent<Unit>().basePhysicalDefense--;
                enemyUnitClone.GetComponent<Unit>().baseMagicalAttack = (enemyUnitClone.GetComponent<Unit>().baseMagicalAttack == 0)
                    ? enemyUnitClone.GetComponent<Unit>().baseMagicalAttack : enemyUnitClone.GetComponent<Unit>().baseMagicalAttack--;
                enemyUnitClone.GetComponent<Unit>().baseMagicalDefense = (enemyUnitClone.GetComponent<Unit>().baseMagicalDefense == 0)
                    ? enemyUnitClone.GetComponent<Unit>().baseMagicalDefense : enemyUnitClone.GetComponent<Unit>().baseMagicalDefense--;
                */
                /*
                totalEnemyPHYSATKModifier--;
                totalEnemyPHYSDEFModifier--;
                totalEnemyMAGATKModifier--;
                totalEnemyMAGDEFModifier--;
                */

                // Nothing happens
            }
        }
        else if (playerElement == "Water" && enemyElement == "Earth")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Sand" && enemyElement == "Electricity")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Sand" && enemyElement == "Water")
        {
            if (battleInitiator == "Player")
            {
                /*
                playerUnitClone.GetComponent<Unit>().basePhysicalAttack = (playerUnitClone.GetComponent<Unit>().basePhysicalAttack == 0)
                    ? playerUnitClone.GetComponent<Unit>().basePhysicalAttack : playerUnitClone.GetComponent<Unit>().basePhysicalAttack--;
                playerUnitClone.GetComponent<Unit>().basePhysicalDefense = (playerUnitClone.GetComponent<Unit>().basePhysicalDefense == 0)
                    ? playerUnitClone.GetComponent<Unit>().basePhysicalDefense : playerUnitClone.GetComponent<Unit>().basePhysicalDefense--;
                playerUnitClone.GetComponent<Unit>().baseMagicalAttack = (playerUnitClone.GetComponent<Unit>().baseMagicalAttack == 0)
                    ? playerUnitClone.GetComponent<Unit>().baseMagicalAttack : playerUnitClone.GetComponent<Unit>().baseMagicalAttack--;
                playerUnitClone.GetComponent<Unit>().baseMagicalDefense = (playerUnitClone.GetComponent<Unit>().baseMagicalDefense == 0)
                    ? playerUnitClone.GetComponent<Unit>().baseMagicalDefense : playerUnitClone.GetComponent<Unit>().baseMagicalDefense--;
                */
                
                /*
                totalPlayerPHYSATKModifier--;
                totalPlayerPHYSDEFModifier--;
                totalPlayerMAGATKModifier--;
                totalPlayerMAGDEFModifier--;
                */

                // Nothing happens
            }
            else
            {
                totalEnemyPHYSATKModifier += 2;
                totalEnemyPHYSDEFModifier += 2;
                totalEnemyMAGATKModifier += 2;
                totalEnemyMAGDEFModifier += 2;
            }
        }
        else if (playerElement == "Sand" && enemyElement == "Sand")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Sand" && enemyElement == "Earth")
        {
            if (battleInitiator == "Player")
            {
                totalPlayerPHYSATKModifier++;
                totalPlayerPHYSDEFModifier++;
                totalPlayerMAGATKModifier++;
                totalPlayerMAGDEFModifier++;
            }
            else
            {
                totalEnemyPHYSATKModifier++;
                totalEnemyPHYSDEFModifier++;
                totalEnemyMAGATKModifier++;
                totalEnemyMAGDEFModifier++;
            }
        }
        else if (playerElement == "Earth" && enemyElement == "Electricity")
        {
            if (battleInitiator == "Player")
            {
                /*
                playerUnitClone.GetComponent<Unit>().basePhysicalAttack = (playerUnitClone.GetComponent<Unit>().basePhysicalAttack == 0)
                    ? playerUnitClone.GetComponent<Unit>().basePhysicalAttack : playerUnitClone.GetComponent<Unit>().basePhysicalAttack--;
                playerUnitClone.GetComponent<Unit>().basePhysicalDefense = (playerUnitClone.GetComponent<Unit>().basePhysicalDefense == 0)
                    ? playerUnitClone.GetComponent<Unit>().basePhysicalDefense : playerUnitClone.GetComponent<Unit>().basePhysicalDefense--;
                playerUnitClone.GetComponent<Unit>().baseMagicalAttack = (playerUnitClone.GetComponent<Unit>().baseMagicalAttack == 0)
                    ? playerUnitClone.GetComponent<Unit>().baseMagicalAttack : playerUnitClone.GetComponent<Unit>().baseMagicalAttack--;
                playerUnitClone.GetComponent<Unit>().baseMagicalDefense = (playerUnitClone.GetComponent<Unit>().baseMagicalDefense == 0)
                    ? playerUnitClone.GetComponent<Unit>().baseMagicalDefense : playerUnitClone.GetComponent<Unit>().baseMagicalDefense--;
                */

                /*
                totalPlayerPHYSATKModifier--;
                totalPlayerPHYSDEFModifier--;
                totalPlayerMAGATKModifier--;
                totalPlayerMAGDEFModifier--;
                */

                // Nothing happens
            }
            else
            {
                totalEnemyPHYSATKModifier += 2;
                totalEnemyPHYSDEFModifier += 2;
                totalEnemyMAGATKModifier += 2;
                totalEnemyMAGDEFModifier += 2;
            }
        }
        else if (playerElement == "Earth" && enemyElement == "Water")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement == "Earth" && enemyElement == "Sand")
        {
            if (battleInitiator == "Player")
            {
                totalPlayerPHYSATKModifier++;
                totalPlayerPHYSDEFModifier++;
                totalPlayerMAGATKModifier++;
                totalPlayerMAGDEFModifier++;
            }
            else
            {
                totalEnemyPHYSATKModifier++;
                totalEnemyPHYSDEFModifier++;
                totalEnemyMAGATKModifier++;
                totalEnemyMAGDEFModifier++;
            }
        }
        else if (playerElement == "Earth" && enemyElement == "Earth")
        {
            if (battleInitiator == "Player")
            {
                // Nothing happens
            }
            else
            {
                // Nothing happens
            }
        }
        else if (playerElement != "Void" && enemyElement == "Void")
        {
            // CHECK TRUE MODIFIERS WITH TIEN
            totalEnemyPHYSATKModifier += 2;
            totalEnemyPHYSDEFModifier += 2;
            totalEnemyMAGATKModifier += 2;
            totalEnemyMAGDEFModifier += 2;
        }
    }
}
