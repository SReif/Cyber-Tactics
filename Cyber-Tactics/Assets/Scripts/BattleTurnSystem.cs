using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnSystem : MonoBehaviour
{
    //public State state;

    public GameObject playerUnitLocation;       // The GameObject that stores the location that the player unit appears in the battle
    public GameObject enemyUnitLocation;        // The GameObject that stores the location that the player unit appears in the battle
    public GameObject playerCardSlots;          // The GameObject that stores the location of the player card slots
    public GameObject battleViewCamera;         // The camera that is enabled/disabled for the battle 
    public GameObject gridViewCamera;           // The camera that is used for the grid; the battle moves back to this camera at the end

    private List<GameObject> playerSelectedCards;       // The list of cards that the player selected for their turn
    private List<GameObject> enemySelectedCards;        // The list of cards that the enemy selected for their turn

    /*
    public enum State
    {
        PlayerTurn,
        EnemyTurn,
        ResolveCards,
        None,
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        playerSelectedCards = new List<GameObject>();
        enemySelectedCards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Battle(GameObject playerUnit, GameObject enemyUnit, string battleInitiator)
    {

        // Setup the sprite representations for the player unit
        playerUnitLocation.GetComponent<SpriteRenderer>().sprite = playerUnit.GetComponent<SpriteRenderer>().sprite;
        playerUnitLocation.GetComponent<SpriteRenderer>().color = playerUnit.GetComponent<SpriteRenderer>().color;
        playerUnitLocation.GetComponent<SpriteRenderer>().flipX = playerUnit.GetComponent<SpriteRenderer>().flipX;

        // Setup the sprite representations for the enemy unit
        enemyUnitLocation.GetComponent<SpriteRenderer>().sprite = enemyUnit.GetComponent<SpriteRenderer>().sprite;
        enemyUnitLocation.GetComponent<SpriteRenderer>().color = enemyUnit.GetComponent<SpriteRenderer>().color;
        enemyUnitLocation.GetComponent<SpriteRenderer>().flipX = enemyUnit.GetComponent<SpriteRenderer>().flipX;

        //Fill the player card slots from the unit's deck
        List<GameObject> playerCards = playerUnit.GetComponent<Unit>().cards;

        // Iterate through the unit's cards list and populate the card slots with them
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            // WITH THIS IMPLEMENTATION, IT WILL ONLY PULL FROM THE FIRST FIVE CARDS
            // IT ALSO REQUIRES THAT THERE ARE AT LEAST 5 CARDS IN THE UNIT'S DECK
            // CHANGE THIS TO PULL RANDOMLY FROM THE UNIT'S DECK
            GameObject card = GameObject.Instantiate(playerCards[i].gameObject, playerCardSlots.transform.GetChild(i).transform);
            card.SetActive(true);
        }

        // Change over to the Battle View Camera
        gridViewCamera.SetActive(false);
        battleViewCamera.SetActive(true);

        if (battleInitiator == "Player")
        {
            // Player's turn
            //state = State.PlayerTurn;
            print("Player's turn!");

            playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(true);

            bool takingTurn = true;

            while (takingTurn)
            {
                var ray = battleViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Card")
                    {
                        //Debug.Log("Card detected!");

                        // Check to see if the card has already been selected
                        // If index == -1, the card is not in the list
                        if (playerSelectedCards.IndexOf(hit.transform.gameObject) == -1)
                        {
                            //Debug.Log("Selecting Card!");

                            // Check to see if the player is trying to heal themselves at full HP
                            if (hit.transform.gameObject.GetComponent<Card>().cardType == "HEAL" &&
                                (playerUnit.GetComponent<Unit>().maxHP > playerUnit.GetComponent<Unit>().currentHP))
                            {
                                Debug.Log("Player is not at full health!");
                                Debug.Log("Adding card to selected pool!");

                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
                                playerSelectedCards.Add(hit.transform.gameObject);
                            }
                            else if (hit.transform.gameObject.GetComponent<Card>().cardType == "HEAL" &&
                                (playerUnit.GetComponent<Unit>().maxHP == playerUnit.GetComponent<Unit>().currentHP))
                            {
                                Debug.Log("Player is at full health!");
                            }
                            else if (hit.transform.gameObject.GetComponent<Card>().cardType != "HEAL")
                            {
                                Debug.Log("Adding card to selected pool!");

                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
                                playerSelectedCards.Add(hit.transform.gameObject);
                            }
                        }
                        else
                        {
                            Debug.Log("Deselecting card!");

                            hit.transform.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Node Color", typeof(Material)) as Material;
                            playerSelectedCards.Remove(hit.transform.gameObject);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Shifting to enemy turn!");

                    takingTurn = false;
                    playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);
                    //state = State.EnemyTurn;
                }

                yield return null;
            }

            // Enemy's turn
            print("Enemy's turn!");
            
            enemyUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(true);

            List<GameObject> enemyCards = enemyUnit.GetComponent<Unit>().cards;

            // Select one card at random from the enemy unit's deck to play
            int index = Random.Range(0, enemyCards.Count);
            enemySelectedCards.Add(enemyCards[index]);

            yield return new WaitForSeconds(1);

            enemyUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);

            Debug.Log("Preparing calculations!");
            //state = State.ResolveCards;

            resolveCards(playerUnit, enemyUnit);
        }
        else if (battleInitiator == "Enemy")
        {
            // Enemy's turn
            print("Enemy's turn!");
            enemyUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(true);

            List<GameObject> enemyCards = enemyUnit.GetComponent<Unit>().cards;

            // Select one card at random from the enemy unit's deck to play
            int index = Random.Range(0, enemyCards.Count);
            enemySelectedCards.Add(enemyCards[index]);

            enemyUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);

            // Player's turn
            //state = State.PlayerTurn;
            print("Player's turn!");

            playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(true);

            bool takingTurn = true;

            while (takingTurn)
            {
                var ray = battleViewCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetMouseButtonDown(0) && hit.transform.tag == "Card")
                    {
                        Debug.Log("Card detected!");

                        // Check to see if the card has already been selected
                        // If index == -1, the card is not in the list
                        if (playerSelectedCards.IndexOf(hit.transform.gameObject) == -1)
                        {
                            Debug.Log("Selecting Card!");

                            // Check to see if the player is trying to heal themselves at full HP
                            if (hit.transform.gameObject.GetComponent<Card>().cardType == "HEAL" &&
                                (playerUnit.GetComponent<Unit>().maxHP > playerUnit.GetComponent<Unit>().currentHP))
                            {
                                Debug.Log("Player is not at full health!");
                                Debug.Log("Adding card to selected pool!");

                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
                                playerSelectedCards.Add(hit.transform.gameObject);
                            }
                            else if (hit.transform.gameObject.GetComponent<Card>().cardType == "HEAL" &&
                                (playerUnit.GetComponent<Unit>().maxHP == playerUnit.GetComponent<Unit>().currentHP))
                            {
                                Debug.Log("Player is at full health!");
                            }
                            else if (hit.transform.gameObject.GetComponent<Card>().cardType != "HEAL")
                            {
                                Debug.Log("Adding card to selected pool!");

                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Valid Move Node Color", typeof(Material)) as Material;
                                playerSelectedCards.Add(hit.transform.gameObject);
                            }
                        }
                        else
                        {
                            Debug.Log("Deselecting card!");

                            hit.transform.gameObject.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Node Color", typeof(Material)) as Material;
                            playerSelectedCards.Remove(hit.transform.gameObject);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Shifting to enemy turn!");

                    takingTurn = false;
                    playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);
                    //state = State.EnemyTurn;
                }

                yield return null;
            }

            Debug.Log("Preparing calculations!");
            //state = State.ResolveCards;

            resolveCards(playerUnit, enemyUnit);
        }

        battleViewCamera.SetActive(false);
        gridViewCamera.SetActive(true);

        yield return null;
    }

    public void resolveCards(GameObject playerUnit, GameObject enemyUnit)
    {
        print("Resolving cards!");

        int totalPlayerPhysicalDefense = playerUnit.GetComponent<Unit>().basePhysicalDefense;
        int totalPlayerPhysicalAttack = playerUnit.GetComponent<Unit>().basePhysicalAttack;
        int totalPlayerMagicalAttack = playerUnit.GetComponent<Unit>().baseMagicalAttack;
        int totalPlayerMagicalDefense = playerUnit.GetComponent<Unit>().baseMagicalDefense;
        int totalPlayerHealing = 0;

        int totalEnemyPhysicalDefense = enemyUnit.GetComponent<Unit>().basePhysicalDefense;
        int totalEnemyPhysicalAttack = enemyUnit.GetComponent<Unit>().basePhysicalAttack;
        int totalEnemyMagicalAttack = enemyUnit.GetComponent<Unit>().baseMagicalAttack;
        int totalEnemyMagicalDefense = enemyUnit.GetComponent<Unit>().baseMagicalDefense;
        int totalEnemyHealing = 0;

        // Resolve all player cards
        for (int i = 0; i < playerSelectedCards.Count; i++)
        {
            if (playerSelectedCards[i].GetComponent<Card>().cardType == "PHYS ATK")
            {
                totalPlayerPhysicalAttack += playerSelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (playerSelectedCards[i].GetComponent<Card>().cardType == "PHYS DEF")
            {
                totalPlayerPhysicalDefense += playerSelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (playerSelectedCards[i].GetComponent<Card>().cardType == "MAG ATK")
            {
                totalPlayerMagicalAttack += playerSelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (playerSelectedCards[i].GetComponent<Card>().cardType == "MAG DEF")
            {
                totalPlayerMagicalDefense += playerSelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (playerSelectedCards[i].GetComponent<Card>().cardType == "HEAL")
            {
                totalPlayerHealing += playerSelectedCards[i].GetComponent<Card>().modifier;
            }
        }

        // Resolve all enemy cards
        for (int i = 0; i < enemySelectedCards.Count; i++)
        {
            if (enemySelectedCards[i].GetComponent<Card>().cardType == "PHYS ATK")
            {
                totalEnemyPhysicalAttack += enemySelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (enemySelectedCards[i].GetComponent<Card>().cardType == "PHYS DEF")
            {
                totalEnemyPhysicalDefense += enemySelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (enemySelectedCards[i].GetComponent<Card>().cardType == "MAG ATK")
            {
                totalEnemyMagicalDefense += enemySelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (enemySelectedCards[i].GetComponent<Card>().cardType == "MAG DEF")
            {
                totalEnemyMagicalDefense += enemySelectedCards[i].GetComponent<Card>().modifier;
            }
            else if (enemySelectedCards[i].GetComponent<Card>().cardType == "HEAL")
            {
                totalEnemyHealing += enemySelectedCards[i].GetComponent<Card>().modifier;
            }
        }

        // Calculate total damage taken by the player
        int totalPlayerDamageTaken = (totalEnemyPhysicalAttack - totalPlayerPhysicalDefense < 0) ? 0 : totalEnemyPhysicalAttack - totalPlayerPhysicalDefense;
        totalPlayerDamageTaken += (totalEnemyMagicalAttack - totalPlayerMagicalDefense < 0) ? 0 : totalEnemyMagicalAttack - totalPlayerMagicalDefense;

        // Calculate total damage taken by the enemy
        int totalEnemyDamageTaken = (totalPlayerPhysicalAttack - totalEnemyPhysicalDefense < 0) ? 0 : totalPlayerPhysicalAttack - totalEnemyPhysicalDefense;
        totalEnemyDamageTaken += (totalPlayerMagicalAttack - totalEnemyMagicalDefense < 0) ? 0 : totalPlayerMagicalAttack - totalEnemyMagicalDefense;

        // Resolve the healing done and taken taken for the player unit
        playerUnit.GetComponent<Unit>().currentHP += totalPlayerHealing;
        playerUnit.GetComponent<Unit>().currentHP -= totalPlayerDamageTaken;

        // Resolve the healing done and taken taken for the enemy unit
        enemyUnit.GetComponent<Unit>().currentHP += totalEnemyHealing;
        enemyUnit.GetComponent<Unit>().currentHP -= totalEnemyDamageTaken;

        cleanUpBattleView();

        print("End battle!");

        //state = State.None;
    }

    public void cleanUpBattleView()
    {
        // Remove the players cards from the card slots
        for (int i = 0; i < playerCardSlots.transform.childCount; i++)
        {
            Destroy(playerCardSlots.transform.GetChild(i).GetChild(0).gameObject);
        }

        // Clean up the SpriteRenderer for the player unit
        playerUnitLocation.GetComponent<SpriteRenderer>().sprite = null;
        playerUnitLocation.GetComponent<SpriteRenderer>().color = Color.white;
        playerUnitLocation.GetComponent<SpriteRenderer>().flipX = false;

        // Clean up the SpriteRenderer for the enemy unit
        enemyUnitLocation.GetComponent<SpriteRenderer>().sprite = null;
        enemyUnitLocation.GetComponent<SpriteRenderer>().color = Color.white;
        enemyUnitLocation.GetComponent<SpriteRenderer>().flipX = false;

        playerSelectedCards.Clear();
        enemySelectedCards.Clear();
    }
}
