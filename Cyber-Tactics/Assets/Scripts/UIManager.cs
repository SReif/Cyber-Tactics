using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//This scripts gets attached to the Canvas game object as it makes calls to the child objects
public class UIManager : MonoBehaviour
{
    private bool paused;
    [SerializeField] private GameObject gridViewCam;
    [SerializeField] private GameObject battleViewCam;
    [SerializeField] private GameObject levelUI;
    [SerializeField] private GameObject battleUI;

    //UI Elements
    public GameObject pausePane;
    [System.NonSerialized] public GameObject turnPane, unitPane, winPane, losePane;
    [System.NonSerialized] public GameObject playerStat, playerHealth, enemyStat, enemyHealth;

    //Elements of the Stat Panel
    private string unitName, unitType;
    private int unitHp, unitPRes, unitMRes, unitPDmg, unitMDmg;

    //Elements of the Player in Battle
    private string playerName;
    private int playerHp, playerMaxHp, playerPRes, playerMRes, playerPDmg, playerMDmg;

    //Elements of the Enemy in Battle
    private string enemyName;
    private int enemyHp, enemyMaxHp,enemyPRes, enemyMRes, enemyPDmg, enemyMDmg;

    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private BattleTurnSystem battleTurnSystem;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiating variables if necessary
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Grid System") != null && !paused)
        {
            SwapCam();

            if (gridViewCam.activeSelf == true)
            {
                ToggleStatsPane();
                NextTurn();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    
    //Opens specified panel
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
    
    //Closes specified panel
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    //Pauses game
    public void Pause()
    {
        if(paused)
        {
            Time.timeScale = 1;
            if(GameObject.Find("Grid System") != null)
            {
                turnPane.SetActive(true);
            }
            
            pausePane.SetActive(false);
            paused = false;
        }

        else
        {
            if (GameObject.Find("Grid System") != null)
            {
                turnPane.SetActive(false);
            }
            
            pausePane.SetActive(true);
            paused = true;
            Time.timeScale = 0;
        }
    }

    //Toggles the Stats pane on/off depending if a unit is selected or not
    //Stats pane is filled with seleced unit's attributes
    private void ToggleStatsPane()
    {
        if (gridSystem.selectedUnit != null)
        {
            OpenPanel(unitPane);
            unitName = gridSystem.selectedUnit.GetComponent<Unit>().unitName;
            unitType = gridSystem.selectedUnit.GetComponent<Unit>().unitMoveID;
            unitHp = gridSystem.selectedUnit.GetComponent<Unit>().currentHP;
            unitPRes = gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalDefense;
            unitMRes = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalDefense;
            unitPDmg = gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalAttack;
            unitMDmg = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalAttack;

            GameObject.Find("UnitName").GetComponent<TextMeshProUGUI>().text = unitName;
            GameObject.Find("ClassName").GetComponent<TextMeshProUGUI>().text = unitType;
            GameObject.Find("Health_Num").GetComponent<TextMeshProUGUI>().text = "" + unitHp;
            GameObject.Find("PhysDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + unitPRes;
            GameObject.Find("MagDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + unitMRes;
            GameObject.Find("PhysATK_Num").GetComponent<TextMeshProUGUI>().text = "" + unitPDmg;
            GameObject.Find("MagATK_Num").GetComponent<TextMeshProUGUI>().text = "" + unitMDmg;
        }

        if (gridSystem.selectedUnit == null)
        {
            ClosePanel(unitPane);
        }
    }

    //Sets the stats of both the player and enemy unit that is currently in combat
    private void SetBattleStats()
    {
        //Player Stat Pane within Battle Scene
        GameObject player = battleTurnSystem.playerUnitClone;
        playerStat = GameObject.Find("PlayerBattleStats");
        playerHealth = GameObject.Find("PlayerBattleHealth");

        playerName = player.GetComponent<Unit>().unitName;
        playerHp = player.GetComponent<Unit>().currentHP;
        playerMaxHp = player.GetComponent<Unit>().maxHP;
        playerPDmg = player.GetComponent<Unit>().basePhysicalAttack;
        playerMDmg = player.GetComponent<Unit>().baseMagicalAttack;
        playerPRes = player.GetComponent<Unit>().basePhysicalDefense;
        playerMRes = player.GetComponent<Unit>().baseMagicalDefense;

        playerHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + playerHp;
        playerHealth.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "/" + playerMaxHp;
        playerStat.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + playerName;
        playerStat.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + playerPDmg;
        playerStat.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + playerPRes;
        playerStat.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + playerMDmg;
        playerStat.transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "" + playerMRes;

        //Enemy Stat Pane within Battle Scene
        GameObject enemy = battleTurnSystem.enemyUnitClone;
        enemyStat = GameObject.Find("EnemyBattleStats");
        enemyHealth = GameObject.Find("EnemyBattleHealth");

        enemyName = enemy.GetComponent<Unit>().unitName;
        enemyHp = enemy.GetComponent<Unit>().currentHP;
        enemyMaxHp = enemy.GetComponent<Unit>().maxHP;
        enemyPDmg = enemy.GetComponent<Unit>().basePhysicalAttack;
        enemyMDmg = enemy.GetComponent<Unit>().baseMagicalAttack;
        enemyPRes = enemy.GetComponent<Unit>().basePhysicalDefense;
        enemyMRes = enemy.GetComponent<Unit>().baseMagicalDefense;

        enemyHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + enemyHp;
        enemyHealth.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "/" + enemyMaxHp;
        enemyStat.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + enemyName;
        enemyStat.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + enemyPDmg;
        enemyStat.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + enemyPRes;
        enemyStat.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + enemyMDmg;
        enemyStat.transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "" + enemyMRes;
    }

    //Changes current turn text at top of the game screen to either player/enemy
    private void NextTurn()
    {

        if (turnSystem.state == TurnSystem.State.PlayerTurn)
        {
            turnPane.transform.GetChild(0).gameObject.SetActive(false);
            turnPane.transform.GetChild(1).gameObject.SetActive(true);
            levelUI.transform.GetChild(4).gameObject.SetActive(true);
        }

        else
        {
            turnPane.transform.GetChild(0).gameObject.SetActive(true);
            turnPane.transform.GetChild(1).gameObject.SetActive(false);
            levelUI.transform.GetChild(4).gameObject.SetActive(false);
        }
    }

    private void SwapCam()
    {
        if (gridViewCam.activeSelf == true)
        {
            levelUI.SetActive(true);
            battleUI.SetActive(false);
            turnPane = levelUI.transform.GetChild(0).gameObject;
            unitPane = levelUI.transform.GetChild(1).gameObject;
            winPane = levelUI.transform.GetChild(2).gameObject;
            losePane = levelUI.transform.GetChild(3).gameObject;
        }

        else if(battleViewCam.activeSelf == true)
        {
            levelUI.SetActive(false);
            battleUI.SetActive(true);
            playerStat = battleUI.transform.GetChild(0).gameObject;
            playerHealth = battleUI.transform.GetChild(1).gameObject;
            enemyStat = battleUI.transform.GetChild(2).gameObject;
            enemyHealth = battleUI.transform.GetChild(3).gameObject;
            SetBattleStats();
        }
    }
}
