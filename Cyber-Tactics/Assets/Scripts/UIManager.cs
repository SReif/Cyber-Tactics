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
    [System.NonSerialized] public GameObject turnPane, selectedUnitPane, winPane, losePane, viewedUnitPane;
    [System.NonSerialized] public GameObject playerStat, playerHealth, enemyStat, enemyHealth;

    //Elements of the Selected Unit Stat Panel
    private string unitName, unitType;
    private int unitHp, unitMaxHp, unitPRes, unitMRes, unitPDmg, unitMDmg, highestStat;
    private List<int> valueList = new List<int>();

    //Elements of the Viewed Unit Stat Panel;
    private string vUnitName, vUnitType;
    private int vUnitHp, vUnitMaxHp, vUnitPRes, vUnitMRes, vUnitPDmg, vUnitMDmg;

    //Elements of the Player in Battle
    private string playerClassName;
    private int playerHp, playerMaxHp, playerPRes, playerMRes, playerPDmg, playerMDmg;
    private int playerPResMod, playerMResMod, playerPDmgMod, playerMDmgMod;

    //Elements of the Enemy in Battle
    private string enemyClassName;
    private int enemyHp, enemyMaxHp,enemyPRes, enemyMRes, enemyPDmg, enemyMDmg;
    private int enemyPResMod, enemyMResMod, enemyPDmgMod, enemyMDmgMod;

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
            if(GameObject.Find("Grid View Camera").activeSelf)
            {
                GameObject.Find("Grid View Camera").GetComponent<CameraPan>().enabled = true;
                turnPane.SetActive(true);
                levelUI.SetActive(true);
            }

            if (!GameObject.Find("Grid View Camera").activeSelf)
            {
                battleUI.SetActive(true);
            }

            gridSystem.GetComponent<GridSystem>().enabled = true;
            turnSystem.enabled = true;
            battleTurnSystem.enabled = true;
            pausePane.SetActive(false);
            paused = false;
        }

        else
        {
            if (GameObject.Find("Grid View Camera").activeSelf)
            {
                GameObject.Find("Grid View Camera").GetComponent<CameraPan>().enabled = false;
                turnPane.SetActive(false);
                levelUI.SetActive(false);
            }

            if(!GameObject.Find("Grid View Camera").activeSelf)
            {
                battleUI.SetActive(false);
            }

            gridSystem.GetComponent<GridSystem>().enabled = false;
            turnSystem.enabled = false;
            battleTurnSystem.enabled = false;

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
            OpenPanel(selectedUnitPane);

            GameObject selectedUnitNameStats = selectedUnitPane.transform.Find("StatsTemplate").gameObject;
            GameObject selectedUnitNumStats = selectedUnitPane.transform.Find("StatsNum").gameObject;

            selectedUnitNameStats.transform.Find("UnitName").GetComponent<TextMeshProUGUI>().text = gridSystem.selectedUnit.GetComponent<Unit>().unitName;
            selectedUnitNameStats.transform.Find("ClassName").GetComponent<TextMeshProUGUI>().text = gridSystem.selectedUnit.GetComponent<Unit>().className;

            selectedUnitNumStats.transform.Find("Health_Num").GetComponent<TextMeshProUGUI>().text = gridSystem.selectedUnit.GetComponent<Unit>().currentHP  + "/" + gridSystem.selectedUnit.GetComponent<Unit>().maxHP;
            selectedUnitNumStats.transform.Find("PhysDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalDefense;
            selectedUnitNumStats.transform.Find("MagDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalDefense;
            selectedUnitNumStats.transform.Find("PhysATK_Num").GetComponent<TextMeshProUGUI>().text = "" + gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalAttack;
            selectedUnitNumStats.transform.Find("MagATK_Num").GetComponent<TextMeshProUGUI>().text = "" + gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalAttack;

            //Set color of highest value stat for the player
            GetHighValue(gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalDefense,
                gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalDefense,
                gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalAttack,
                gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalAttack,
                selectedUnitNumStats.transform.Find("PhysDEF_Num").GetComponent<TextMeshProUGUI>(),
                selectedUnitNumStats.transform.Find("MagDEF_Num").GetComponent<TextMeshProUGUI>(),
                selectedUnitNumStats.transform.Find("PhysATK_Num").GetComponent<TextMeshProUGUI>(),
                selectedUnitNumStats.transform.Find("MagATK_Num").GetComponent<TextMeshProUGUI>());

            //Set health color
            SetHealthColor(gridSystem.selectedUnit.GetComponent<Unit>().currentHP,
                gridSystem.selectedUnit.GetComponent<Unit>().maxHP, 
                selectedUnitNumStats.transform.Find("Health_Num").GetComponent<TextMeshProUGUI>());
        }

        if (gridSystem.selectedUnit == null)
        {
            ClosePanel(selectedUnitPane);
        }

        if (turnSystem.viewedUnit != null)
        {
            OpenPanel(viewedUnitPane);

            GameObject viewedUnitNameStats = viewedUnitPane.transform.Find("StatsTemplate").gameObject;
            GameObject viewedUnitNumStats = viewedUnitPane.transform.Find("StatsNum").gameObject;

            viewedUnitNameStats.transform.Find("UnitName").GetComponent<TextMeshProUGUI>().text = turnSystem.viewedUnit.GetComponent<Unit>().unitName;
            viewedUnitNameStats.transform.Find("ClassName").GetComponent<TextMeshProUGUI>().text = turnSystem.viewedUnit.GetComponent<Unit>().className;

            viewedUnitNumStats.transform.Find("Health_Num").GetComponent<TextMeshProUGUI>().text = turnSystem.viewedUnit.GetComponent<Unit>().currentHP  + "/" + turnSystem.viewedUnit.GetComponent<Unit>().maxHP;
            viewedUnitNumStats.transform.Find("PhysDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + turnSystem.viewedUnit.GetComponent<Unit>().basePhysicalDefense;
            viewedUnitNumStats.transform.Find("MagDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + turnSystem.viewedUnit.GetComponent<Unit>().baseMagicalDefense;
            viewedUnitNumStats.transform.Find("PhysATK_Num").GetComponent<TextMeshProUGUI>().text = "" + turnSystem.viewedUnit.GetComponent<Unit>().basePhysicalAttack;
            viewedUnitNumStats.transform.Find("MagATK_Num").GetComponent<TextMeshProUGUI>().text = "" + turnSystem.viewedUnit.GetComponent<Unit>().baseMagicalAttack;

            //Set color of highest value stat for the player
            GetHighValue(turnSystem.viewedUnit.GetComponent<Unit>().basePhysicalDefense,
                turnSystem.viewedUnit.GetComponent<Unit>().baseMagicalDefense,
                turnSystem.viewedUnit.GetComponent<Unit>().basePhysicalAttack,
                turnSystem.viewedUnit.GetComponent<Unit>().baseMagicalAttack,
                viewedUnitNumStats.transform.Find("PhysDEF_Num").GetComponent<TextMeshProUGUI>(),
                viewedUnitNumStats.transform.Find("MagDEF_Num").GetComponent<TextMeshProUGUI>(),
                viewedUnitNumStats.transform.Find("PhysATK_Num").GetComponent<TextMeshProUGUI>(),
                viewedUnitNumStats.transform.Find("MagATK_Num").GetComponent<TextMeshProUGUI>());

            //Set health color
            SetHealthColor(turnSystem.viewedUnit.GetComponent<Unit>().currentHP,
                turnSystem.viewedUnit.GetComponent<Unit>().maxHP, 
                viewedUnitNumStats.transform.Find("Health_Num").GetComponent<TextMeshProUGUI>());
        }

        if (turnSystem.viewedUnit == null)
        {
            ClosePanel(viewedUnitPane);
        }
    }

    //Executes player turn within battle scene
    public void BattleEndTurn()
    {
        battleTurnSystem = GameObject.Find("Battle Turn System").GetComponent<BattleTurnSystem>();

        battleTurnSystem.takingTurn = false;
        battleTurnSystem.playerUnitClone.transform.Find("Selected Unit Indicator").gameObject.SetActive(false);
    }

    //Sets the stats of both the player and enemy unit that is currently in combat
    private void SetBattleStats()
    {
        //Player Stat Pane within Battle Scene
        GameObject player = battleTurnSystem.playerUnitClone;
        playerStat = GameObject.Find("PlayerBattleStats");
        playerHealth = GameObject.Find("PlayerBattleHealth");

        playerClassName = player.GetComponent<Unit>().className;
        playerHp = player.GetComponent<Unit>().currentHP;
        playerMaxHp = player.GetComponent<Unit>().maxHP;
        playerPDmg = player.GetComponent<Unit>().basePhysicalAttack;
        playerMDmg = player.GetComponent<Unit>().baseMagicalAttack;
        playerPRes = player.GetComponent<Unit>().basePhysicalDefense;
        playerMRes = player.GetComponent<Unit>().baseMagicalDefense;

        playerPResMod = battleTurnSystem.totalPlayerPHYSDEFModifier;
        playerMResMod = battleTurnSystem.totalPlayerMAGDEFModifier;
        playerPDmgMod = battleTurnSystem.totalPlayerPHYSATKModifier;
        playerMDmgMod = battleTurnSystem.totalPlayerMAGATKModifier;

        playerHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerHp + "/" + playerMaxHp;
        playerStat.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + playerClassName;
        playerStat.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + playerPDmg;
        playerStat.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + playerPRes;
        playerStat.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + playerMDmg;
        playerStat.transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "" + playerMRes;

        playerStat.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + playerPDmgMod;
        playerStat.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text = "+" + playerPResMod;
        playerStat.transform.GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>().text = "+" + playerMDmgMod;
        playerStat.transform.GetChild(2).GetChild(7).GetComponent<TextMeshProUGUI>().text = "+" + playerMResMod;

        SetHealthColor(playerHp, playerMaxHp, playerHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>());

        //Enemy Stat Pane within Battle Scene
        GameObject enemy = battleTurnSystem.enemyUnitClone;
        enemyStat = GameObject.Find("EnemyBattleStats");
        enemyHealth = GameObject.Find("EnemyBattleHealth");

        enemyClassName = enemy.GetComponent<Unit>().className;
        enemyHp = enemy.GetComponent<Unit>().currentHP;
        enemyMaxHp = enemy.GetComponent<Unit>().maxHP;
        enemyPDmg = enemy.GetComponent<Unit>().basePhysicalAttack;
        enemyMDmg = enemy.GetComponent<Unit>().baseMagicalAttack;
        enemyPRes = enemy.GetComponent<Unit>().basePhysicalDefense;
        enemyMRes = enemy.GetComponent<Unit>().baseMagicalDefense;

        enemyPResMod = battleTurnSystem.totalEnemyPHYSDEFModifier;
        enemyMResMod = battleTurnSystem.totalEnemyMAGDEFModifier;
        enemyPDmgMod = battleTurnSystem.totalEnemyPHYSATKModifier;
        enemyMDmgMod = battleTurnSystem.totalEnemyMAGATKModifier;

        enemyHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = enemyHp + "/" + enemyMaxHp;
        enemyStat.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + enemyClassName;
        enemyStat.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + enemyPDmg;
        enemyStat.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + enemyPRes;
        enemyStat.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + enemyMDmg;
        enemyStat.transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "" + enemyMRes;

        enemyStat.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + enemyPDmgMod;
        enemyStat.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text = "+" + enemyPResMod;
        enemyStat.transform.GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>().text = "+" + enemyMDmgMod;
        enemyStat.transform.GetChild(2).GetChild(7).GetComponent<TextMeshProUGUI>().text = "+" + enemyMResMod;

        SetHealthColor(enemyHp, enemyMaxHp, enemyHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>());
    }

    //Changes current turn text at top of the game screen to either player/enemy
    private void NextTurn()
    {

        if (turnSystem.state == TurnSystem.State.PlayerTurn)
        {
            turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Player Phase";
            turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(44, 177, 133, 255);
            //levelUI.transform.GetChild(5).gameObject.SetActive(true); //End Turn Button
        }

        else
        {
            turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Phase";
            turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(140, 45, 168, 255);
            //levelUI.transform.GetChild(5).gameObject.SetActive(false); //End Turn Button
        }
    }

    private void SwapCam()
    {
        if (gridViewCam.activeSelf == true)
        {
            levelUI.SetActive(true);
            battleUI.SetActive(false);
            turnPane = levelUI.transform.GetChild(0).gameObject; //Turn Pane
            selectedUnitPane = levelUI.transform.GetChild(1).gameObject; //Stats Pane
            winPane = levelUI.transform.GetChild(2).gameObject; //Victory Pane
            losePane = levelUI.transform.GetChild(3).gameObject; //Defeat Pane
            viewedUnitPane = levelUI.transform.GetChild(4).gameObject; //Viewed Unit Pane
        }

        else if(battleViewCam.activeSelf == true)
        {
            levelUI.SetActive(false);
            battleUI.SetActive(true);
            playerStat = battleUI.transform.GetChild(0).gameObject;  //Plyaer Battle Stats
            playerHealth = battleUI.transform.GetChild(1).gameObject; //Player Battle Health
            enemyStat = battleUI.transform.GetChild(2).gameObject; //Enemy Battle Stats
            enemyHealth = battleUI.transform.GetChild(3).gameObject; //Enemy Battle Health
            SetBattleStats();
        }
    }

    //Set color of the health stat based on the percent left
    private void SetHealthColor(int currentHP, int maxHP, TextMeshProUGUI text)
    {
        if (currentHP >= (maxHP * 0.75))
        {
            text.color = new Color32(6, 182, 99, 255);
        }

        if (currentHP < (maxHP * 0.75) && currentHP > (maxHP * 0.25))
        {
            text.color = new Color32(165, 162, 6, 255);
        }

        if (currentHP <= (maxHP * 0.25))
        {
            text.color = new Color32(169, 36, 33, 255);
        }
    }

    //Determines the highest stat value and changes its color
    private void GetHighValue(int pR, int mR, int pD, int mD, TextMeshProUGUI pRText, TextMeshProUGUI mRText, TextMeshProUGUI pDText, TextMeshProUGUI mDText)
    {
        valueList.Clear();

        valueList.Add(pR);
        valueList.Add(mR);
        valueList.Add(pD);
        valueList.Add(mD);

        highestStat = Mathf.Max(valueList.ToArray());
        //Debug.Log("Highest Stat " + highestStat);

        //Determines if Physical Resistance Stat is highest or not
        if (pR == highestStat)
        {
            pRText.color = new Color32(44, 177, 133, 255);
        }

        else
        {
            pRText.color = new Color32(255, 255, 255, 255);
        }

        //Determines if MAgical Resistance Stat is highest or not
        if (mR == highestStat)
        {
            mRText.color = new Color32(44, 177, 133, 255);
        }

        else
        {
            mRText.color = new Color32(255, 255, 255, 255);
        }

        //Determines if Physical Damage Stat is highest or not
        if (pD == highestStat)
        {
            pDText.color = new Color32(44, 177, 133, 255);
        }

        else
        {
            pDText.color = new Color32(255, 255, 255, 255);
        }

        //Determines if Magical Damage Stat is highest or not
        if (mD == highestStat)
        {
            mDText.color = new Color32(44, 177, 133, 255);
        }

        else
        {
            mDText.color = new Color32(255, 255, 255, 255);
        }
    }
}
