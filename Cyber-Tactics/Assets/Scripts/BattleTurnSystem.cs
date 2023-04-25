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
    public GameObject battleViewCamera;                 // The camera that is enabled/disabled for the battle 
    public GameObject gridViewCamera;                   // The camera that is used for the grid; the battle moves back to this camera at the end

    public int totalPlayerPHYSDEFModifier = 0;          // The total amount of PHYSICAL DEFENSE that is being applied to the player from cards
    public int totalPlayerPHYSATKModifier = 0;          // The total amount of PHYSICAL ATTACK that is being applied to the player from cards
    public int totalPlayerMAGATKModifier = 0;           // The total amount of MAGICAL ATTACK that is being applied to the player from cards
    public int totalPlayerMAGDEFModifier = 0;           // The total amount of MAGICAL DEFENSE that is being applied to the player from cards
    public int totalPlayerHEALModifier = 0;             // The total amount of HEALING that is being applied to the player from cards

    public int totalEnemyPHYSDEFModifier = 0;           // The total amount of PHYSICAL DEFENSE that is being applied to the enemy from cards
    public int totalEnemyPHYSATKModifier = 0;           // The total amount of PHYSICAL ATTACK that is being applied to the enemy from cards
    public int totalEnemyMAGATKModifier = 0;            // The total amount of MAGICAL ATTACK that is being applied to the enemy from cards
    public int totalEnemyMAGDEFModifier = 0;            // The total amount of MAGICAL DEFENSE that is being applied to the enemy from cards
    public int totalEnemyHEALModifier = 0;              // The total amount of HEALING that is being applied to the enemy from cards

    private List<GameObject> playerSelectedCards;       // The list of cards that the player selected for their turn
    private List<GameObject> enemySelectedCards;        // The list of cards that the enemy selected for their turn

    [System.NonSerialized] public GameObject playerUnitClone;                 // A clone of the player unit in the battle for visual reference
    [System.NonSerialized] public GameObject enemyUnitClone;                  // A clone of the enemy unit in the battle for visual reference

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
        playerUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        // Setup the battle view clone of the enemy unit
        enemyUnitClone = Instantiate(enemyUnit, enemyUnitLocation.transform);
        enemyUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        //Make a copy of the player unit's deck
        List<GameObject> playerDeck = new List<GameObject>(playerUnit.GetComponent<Unit>().cards);

        // Iterate through the unit's cards list and populate the card slots with them
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            // Pick a random card from the player unit's deck
            int index = Random.Range(0, playerDeck.Count);
            GameObject card = GameObject.Instantiate(playerDeck[index].gameObject, playerCardSlots.transform.GetChild(i).Find("Card").transform);
            card.SetActive(true);

            // Remove the card from the copy of the player unit's deck
            playerDeck.Remove(playerDeck[index]);
            playerDeck.TrimExcess();
        }

        // Change over to the Battle View Camera
        gridViewCamera.SetActive(false);
        battleViewCamera.SetActive(true);

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

        cleanUpBattleView();

        yield return null;
    }

    public void updateStatsFromCard(string cardType, int modifier, string currentTurn, string action)
    {
        if (currentTurn == "Player")
        {
            if (action == "Selecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalPlayerPHYSATKModifier += modifier;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalPlayerPHYSDEFModifier += modifier;
                }
                else if (cardType == "MAG ATK")
                {
                    totalPlayerMAGATKModifier += modifier;
                }
                else if (cardType == "MAG DEF")
                {
                    totalPlayerMAGDEFModifier += modifier;
                }
                else if (cardType == "HEAL")
                {
                    totalPlayerHEALModifier += modifier;
                }
            }
            else if (action == "Deselecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalPlayerPHYSATKModifier -= modifier;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalPlayerPHYSDEFModifier -= modifier;
                }
                else if (cardType == "MAG ATK")
                {
                    totalPlayerMAGATKModifier -= modifier;
                }
                else if (cardType == "MAG DEF")
                {
                    totalPlayerMAGDEFModifier -= modifier;
                }
                else if (cardType == "HEAL")
                {
                    totalPlayerHEALModifier -= modifier;
                }
            }

        }
        else if (currentTurn == "Enemy")
        {
            if (action == "Selecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalEnemyPHYSATKModifier += modifier;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalEnemyPHYSDEFModifier += modifier;
                }
                else if (cardType == "MAG ATK")
                {
                    totalEnemyMAGATKModifier += modifier;
                }
                else if (cardType == "MAG DEF")
                {
                    totalEnemyMAGDEFModifier += modifier;
                }
                else if (cardType == "HEAL")
                {
                    totalEnemyHEALModifier += modifier;
                }
            }
            else if (action == "Deselecting")
            {
                if (cardType == "PHYS ATK")
                {
                    totalEnemyPHYSATKModifier -= modifier;
                }
                else if (cardType == "PHYS DEF")
                {
                    totalEnemyPHYSDEFModifier -= modifier;
                }
                else if (cardType == "MAG ATK")
                {
                    totalEnemyMAGATKModifier -= modifier;
                }
                else if (cardType == "MAG DEF")
                {
                    totalEnemyMAGDEFModifier -= modifier;
                }
                else if (cardType == "HEAL")
                {
                    totalEnemyHEALModifier -= modifier;
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

                    // Check to see if the card has already been selected
                    // If index == -1, the card is not in the list
                    if (playerSelectedCards.IndexOf(hit.transform.gameObject) == -1)
                    {
                        // Check to see if the player is trying to heal themselves at full HP
                        if (cardType == "HEAL" && (playerUnitClone.GetComponent<Unit>().maxHP > playerUnitClone.GetComponent<Unit>().currentHP))
                        {
                            Debug.Log("Player is not at full health!");
                            Debug.Log("Adding card to selected pool!");

                            updateStatsFromCard(cardType, cardModifier, "Player", "Selecting");
                            playerSelectedCards.Add(hit.transform.gameObject);

                            // Find the Selected Card Indicator of the Card Slot and activate it
                            hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);
                        }
                        else if (cardType == "HEAL" && (playerUnitClone.GetComponent<Unit>().maxHP == playerUnitClone.GetComponent<Unit>().currentHP))
                        {
                            Debug.Log("Player is at full health!");
                        }
                        else if (hit.transform.gameObject.GetComponent<Card>().cardType != "HEAL")
                        {
                            Debug.Log("Adding card to selected pool!");

                            updateStatsFromCard(cardType, cardModifier, "Player", "Selecting");
                            playerSelectedCards.Add(hit.transform.gameObject);

                            // Find the Selected Card Indicator of the Card Slot and activate it
                            hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.Log("Deselecting card!");

                        updateStatsFromCard(cardType, cardModifier, "Player", "Deselecting");
                        playerSelectedCards.Remove(hit.transform.gameObject);

                        // Find the Selected Card Indicator of the Card Slot and activate it
                        hit.transform.parent.transform.parent.Find("Selected Card Indicator").gameObject.SetActive(false);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Shifting to enemy turn!");

                takingTurn = false;
                playerUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator BeginEnemysTurn(GameObject enemyUnit)
    {
        enemyUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(true);

        List<GameObject> enemyCards = enemyUnitClone.GetComponent<Unit>().cards;

        // Select one card at random from the enemy unit's deck to play
        int index = Random.Range(0, enemyCards.Count);

        while (enemyCards[index].GetComponent<Card>().cardType == "HEAL" && (enemyUnit.GetComponent<Unit>().currentHP == enemyUnit.GetComponent<Unit>().maxHP))
        {
            Debug.Log("Enemy unit already at full HP, find another card.");
            index = Random.Range(0, enemyCards.Count);
        }

        enemySelectedCards.Add(enemyCards[index]);

        string cardType = enemyCards[index].GetComponent<Card>().cardType;
        int cardModifier = enemyCards[index].GetComponent<Card>().modifier;

        updateStatsFromCard(cardType, cardModifier, "Enemy", "Selecting");

        yield return new WaitForSeconds(1);

        enemyUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);

        yield return null;
    }

    IEnumerator resolveCardModifiers(GameObject playerUnit, GameObject enemyUnit)
    {
        Debug.Log("Resolving cards!");

        int totalPlayerPhysicalDefense = playerUnit.GetComponent<Unit>().basePhysicalDefense + totalPlayerPHYSDEFModifier;
        int totalPlayerPhysicalAttack = playerUnit.GetComponent<Unit>().basePhysicalAttack + totalPlayerPHYSATKModifier;
        int totalPlayerMagicalAttack = playerUnit.GetComponent<Unit>().baseMagicalAttack + totalPlayerMAGATKModifier;
        int totalPlayerMagicalDefense = playerUnit.GetComponent<Unit>().baseMagicalDefense + totalEnemyMAGDEFModifier;

        int totalEnemyPhysicalDefense = enemyUnit.GetComponent<Unit>().basePhysicalDefense + totalEnemyPHYSDEFModifier;
        int totalEnemyPhysicalAttack = enemyUnit.GetComponent<Unit>().basePhysicalAttack + totalEnemyPHYSATKModifier;
        int totalEnemyMagicalAttack = enemyUnit.GetComponent<Unit>().baseMagicalAttack + totalEnemyMAGATKModifier;
        int totalEnemyMagicalDefense = enemyUnit.GetComponent<Unit>().baseMagicalDefense + totalEnemyMAGDEFModifier;

        // Calculate total damage taken by the player
        int totalPlayerDamageTaken = (totalEnemyPhysicalAttack - totalPlayerPhysicalDefense < 0) ? 0 : totalEnemyPhysicalAttack - totalPlayerPhysicalDefense;
        totalPlayerDamageTaken += (totalEnemyMagicalAttack - totalPlayerMagicalDefense < 0) ? 0 : totalEnemyMagicalAttack - totalPlayerMagicalDefense;

        // Calculate total damage taken by the enemy
        int totalEnemyDamageTaken = (totalPlayerPhysicalAttack - totalEnemyPhysicalDefense < 0) ? 0 : totalPlayerPhysicalAttack - totalEnemyPhysicalDefense;
        totalEnemyDamageTaken += (totalPlayerMagicalAttack - totalEnemyMagicalDefense < 0) ? 0 : totalPlayerMagicalAttack - totalEnemyMagicalDefense;

        // INSERT ATTACK ANIMATIONS COROUTINE HERE
        yield return StartCoroutine(attackAnimation());

        // Resolve the healing done and taken taken for the player unit
        playerUnit.GetComponent<Unit>().currentHP += totalPlayerHEALModifier;
        playerUnit.GetComponent<Unit>().currentHP -= totalPlayerDamageTaken;

        // Make sure the player unit cannot overheal themselves.
        playerUnit.GetComponent<Unit>().currentHP = (playerUnit.GetComponent<Unit>().currentHP > playerUnit.GetComponent<Unit>().maxHP) ? playerUnit.GetComponent<Unit>().maxHP : playerUnit.GetComponent<Unit>().currentHP;

        // Resolve the healing done and taken taken for the enemy unit
        enemyUnit.GetComponent<Unit>().currentHP += totalEnemyHEALModifier;
        enemyUnit.GetComponent<Unit>().currentHP -= totalEnemyDamageTaken;

        // Make sure the player unit cannot overheal themselves.
        enemyUnit.GetComponent<Unit>().currentHP = (enemyUnit.GetComponent<Unit>().currentHP > enemyUnit.GetComponent<Unit>().maxHP) ? enemyUnit.GetComponent<Unit>().maxHP : enemyUnit.GetComponent<Unit>().currentHP;

        yield return new WaitForSeconds(0.33f);

        Debug.Log("End battle!");

        //state = State.None;
    }

    public void cleanUpBattleView()
    {
        // Remove the players cards from the card slots
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            Destroy(playerCardSlots.transform.GetChild(i).Find("Card").GetChild(0).gameObject);

            playerCardSlots.transform.GetChild(i).Find("Selected Card Indicator").gameObject.SetActive(false);
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

        playerSelectedCards.Clear();
        enemySelectedCards.Clear();
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

            yield return null;
        }

        // Enemy unit moves away from player unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return null;
        }

        //Player unit moves away from enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return null;
        }

        //Player unit moves toward enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return null;
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

            yield return null;
        }

        //Player unit moves away from enemy unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            playerUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return null;
        }

        // Enemy unit moves away from player unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position += new Vector3(0f, 0f, increment);

            yield return null;
        }

        // Enemy unit moves toward player unit
        curDistance = 0f;
        while (curDistance < maxDistance)
        {
            curDistance += increment;
            enemyUnitClone.transform.position -= new Vector3(0f, 0f, increment);

            yield return null;
        }

        yield return null;
    }
}
