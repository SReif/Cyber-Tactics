using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTurnSystem : MonoBehaviour
{
    public GameObject playerUnit;
    public GameObject enemyUnit;
    public State state;

    private string battleInitiator;
    private List<GameObject> playerSelectedCards = new List<GameObject>();
    private List<GameObject> enemySelectedCards = new List<GameObject>();

    public enum State
    {
        PlayerTurn,
        EnemyTurn,
        ResolveCards,
        None,
    }

    // Start is called before the first frame update
    void Start()
    {
        // SPAWN THE PLAYERS CARDS HERE

        battleInitiator = "Player";

        Debug.Log("Player's turn!");
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.PlayerTurn)
        {
            playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(true);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

            if (Input.GetKeyDown(KeyCode.E) && battleInitiator == "Player")
            {
                Debug.Log("Shifting to enemy turn!");

                playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);
                state = State.EnemyTurn;
            }
            else if (Input.GetKeyDown(KeyCode.E) && battleInitiator == "Enemy")
            {
                Debug.Log("Preparing calculations!");

                playerUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);
                state = State.ResolveCards;
            }
        }
        else if (state == State.EnemyTurn)
        {
            Debug.Log("Enemy's turn!");

            enemyUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(true);

            // INSERT FUNCTIONALITY FOR ENEMY CHOOSING CARDS
            List<GameObject> enemyCards = enemyUnit.GetComponent<Unit>().cards;

            // Randomly choose a card to play for the enemy
            int index = Random.Range(0, enemyCards.Count);

            enemySelectedCards.Add(enemyCards[index]);

            enemyUnit.transform.Find("Selected Unit Indicator").transform.gameObject.SetActive(false);

            if (battleInitiator == "Enemy")
            {
                Debug.Log("Shifting to player's turn!");

                state = State.PlayerTurn;
            }
            else if (battleInitiator == "Player")
            {
                Debug.Log("preparing calculations!");

                state = State.ResolveCards;
            }
        }
        else if (state == State.ResolveCards)
        {
            int totalPlayerDefense = playerUnit.GetComponent<Unit>().baseDefense;
            int totalPlayerAttack = playerUnit.GetComponent<Unit>().baseAttack;
            int totalPlayerMagic = playerUnit.GetComponent<Unit>().baseMagic;
            int totalPlayerMagicDefense = playerUnit.GetComponent<Unit>().baseMagicDefense;
            int totalPlayerHealing = 0;

            int totalEnemyDefense = enemyUnit.GetComponent<Unit>().baseDefense;
            int totalEnemyAttack = enemyUnit.GetComponent<Unit>().baseAttack;
            int totalEnemyMagic = enemyUnit.GetComponent<Unit>().baseMagic;
            int totalEnemyMagicDefense = enemyUnit.GetComponent<Unit>().baseMagicDefense;
            int totalEnemyHealing = 0;

            // Resolve all player cards
            for (int i = 0; i < playerSelectedCards.Count; i++)
            {
                if (playerSelectedCards[i].GetComponent<Card>().cardType == "ATK")
                {
                    totalPlayerAttack += playerSelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (playerSelectedCards[i].GetComponent<Card>().cardType == "DEF")
                {
                    totalPlayerDefense += playerSelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (playerSelectedCards[i].GetComponent<Card>().cardType == "MAG")
                {
                    totalPlayerMagic += playerSelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (playerSelectedCards[i].GetComponent<Card>().cardType == "MAG DEF")
                {
                    totalPlayerMagicDefense += playerSelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (playerSelectedCards[i].GetComponent<Card>().cardType == "HEAL")
                {
                    totalPlayerHealing += playerSelectedCards[i].GetComponent<Card>().modifier;
                }
            }

            // Resolve all enemy cards
            for (int i = 0; i < enemySelectedCards.Count; i++)
            {
                if (enemySelectedCards[i].GetComponent<Card>().cardType == "ATK")
                {
                    totalEnemyAttack += enemySelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (enemySelectedCards[i].GetComponent<Card>().cardType == "DEF")
                {
                    totalEnemyDefense += enemySelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (enemySelectedCards[i].GetComponent<Card>().cardType == "MAG")
                {
                    totalEnemyMagic += enemySelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (enemySelectedCards[i].GetComponent<Card>().cardType == "MAG DEF")
                {
                    totalEnemyMagicDefense += enemySelectedCards[i].GetComponent<Card>().modifier;
                }
                else if (enemySelectedCards[i].GetComponent<Card>().cardType == "HEAL")
                {
                    totalEnemyHealing += enemySelectedCards[i].GetComponent<Card>().modifier;
                }
            }

            // Calculate total damage taken by the player
            int totalPlayerDamageTaken = (totalEnemyAttack - totalPlayerDefense < 0) ? 0 : totalEnemyAttack - totalPlayerDefense;
            totalPlayerDamageTaken += (totalEnemyMagic - totalPlayerMagicDefense < 0) ? 0 : totalEnemyMagic - totalPlayerMagicDefense;

            // Calculate total damage taken by the enemy
            int totalEnemyDamageTaken = (totalPlayerAttack - totalEnemyDefense < 0) ? 0 : totalPlayerAttack - totalEnemyDefense;
            totalEnemyDamageTaken += (totalPlayerMagic - totalEnemyMagicDefense < 0) ? 0 : totalPlayerMagic - totalEnemyMagicDefense;

            // Resolve the healing done and taken taken for the player unit
            playerUnit.GetComponent<Unit>().currentHP += totalPlayerHealing;
            playerUnit.GetComponent<Unit>().currentHP -= totalPlayerDamageTaken;

            // Resolve the healing done and taken taken for the enemy unit
            enemyUnit.GetComponent<Unit>().currentHP += totalEnemyHealing;
            enemyUnit.GetComponent<Unit>().currentHP -= totalEnemyDamageTaken;

            if (playerUnit.GetComponent<Unit>().currentHP < 0 && enemyUnit.GetComponent<Unit>().currentHP < 0)
            {
                // INSERT FUCNTIONALITY FOR BOTH UNITS DYING HERE

                Debug.Log("Both units are defeated!");
            }
            else if (enemyUnit.GetComponent<Unit>().currentHP <= 0)
            {
                // INSERT FUCNTIONALITY FOR PLAYER VICTORY HERE
                Debug.Log("Enemy is defeated!");
            }
            else if (playerUnit.GetComponent<Unit>().currentHP <= 0)
            {
                // INSERT FUCNTIONALITY FOR ENEMY VICTORY HERE
                Debug.Log("Player is defeated!");
            }

            Debug.Log("End combat!");

            state = State.None;
        }
    }
}
