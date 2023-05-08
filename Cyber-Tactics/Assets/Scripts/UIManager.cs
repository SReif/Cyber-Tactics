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
    [System.NonSerialized] public GameObject turnPane, pUnitPane, eUnitPane, winPane, losePane;
    [System.NonSerialized] public GameObject playerStat, playerHealth, enemyStat, enemyHealth;

    //Elements of the Player Stat Panel
    private string unitName, unitType;
    private int unitHp, unitMaxHp, unitPRes, unitMRes, unitPDmg, unitMDmg, highestStat;
    private List<int> valueList = new List<int>();

    //Elements of the Enemy Stat Panel;
    private string eUnitName, eUnitType;
    private int eUnitHp, eUnitMaxHp, eUnitPRes, eUnitMRes, eUnitPDmg, eUnitMDmg;

    //Elements of the Player in Battle
    private string playerName;
    private int playerHp, playerMaxHp, playerPRes, playerMRes, playerPDmg, playerMDmg;
    private int playerPResMod, playerMResMod, playerPDmgMod, playerMDmgMod;

    //Elements of the Enemy in Battle
    private string enemyName;
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

    //Toggles between beeing pasued and not paused
    public void Pause()
    {
        //If the game is currently paused it will execute the below code
        if(paused)
        {
            Time.timeScale = 1;

            if(GameObject.Find("Grid System") != null)
            {
                turnPane.SetActive(true);
                GameObject.Find("Grid View Camera").GetComponent<CameraPan>().enabled = true;
            }
            pausePane.SetActive(false);

            for (int i = 0; i < turnSystem.playersUnits.Count; i++)
            {
                turnSystem.playersUnits[i].GetComponent<Unit>().enabled = true;
            }

            for (int i = 0; i < turnSystem.playersUnits.Count; i++)
            {
                turnSystem.playersUnits[i].GetComponent<Unit>().enabled = true;
            }

            turnPane.SetActive(true);
            turnSystem.GetComponent<TurnSystem>().enabled = true;
            battleTurnSystem.GetComponent<BattleTurnSystem>().enabled = true;
            gridSystem.GetComponent<GridSystem>().enabled = true;
            
            paused = false;
        }

        //If the game is not currently pasued it will execute the below code
        else
        {
            if (GameObject.Find("Grid System") != null)
            {
                turnPane.SetActive(false);
                GameObject.Find("Grid View Camera").GetComponent<CameraPan>().enabled = false;
            }

            if (!winPane.activeSelf && !losePane.activeSelf)
            {
                pausePane.SetActive(true);
            }
            
            paused = true;

            for (int i = 0; i < turnSystem.playersUnits.Count; i++)
            {
                turnSystem.playersUnits[i].GetComponent<Unit>().enabled = false;
            }

            for (int i = 0; i < turnSystem.playersUnits.Count; i++)
            {
                turnSystem.playersUnits[i].GetComponent<Unit>().enabled = false;
            }

            turnPane.SetActive(false);
            turnSystem.GetComponent<TurnSystem>().enabled = false;
            battleTurnSystem.GetComponent<BattleTurnSystem>().enabled = false;
            gridSystem.GetComponent<GridSystem>().enabled = false;
            Time.timeScale = 0;
        }
    }

    //Toggles the Stats pane on/off depending if a unit is selected or not
    //Stats pane is filled with seleced unit's attributes
    private void ToggleStatsPane()
    {
        if (gridSystem.selectedUnit != null && gridSystem.selectedUnit.tag == "PlayerUnit")
        {
            OpenPanel(pUnitPane);

            //Intializing the player unit stats for the selected player unit
            unitName = gridSystem.selectedUnit.GetComponent<Unit>().unitName;
            unitType = gridSystem.selectedUnit.GetComponent<Unit>().unitMoveID;
            unitHp = gridSystem.selectedUnit.GetComponent<Unit>().currentHP;
            unitMaxHp = gridSystem.selectedUnit.GetComponent<Unit>().maxHP;
            unitPRes = gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalDefense;
            unitMRes = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalDefense;
            unitPDmg = gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalAttack;
            unitMDmg = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalAttack;

            GameObject.Find("pUnitName").GetComponent<TextMeshProUGUI>().text = unitName;
            GameObject.Find("pClassName").GetComponent<TextMeshProUGUI>().text = unitType;
            GameObject.Find("pHealth_Num").GetComponent<TextMeshProUGUI>().text = unitHp + "/" + unitMaxHp;
            GameObject.Find("pPhysDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + unitPRes;
            GameObject.Find("pMagDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + unitMRes;
            GameObject.Find("pPhysATK_Num").GetComponent<TextMeshProUGUI>().text = "" + unitPDmg;
            GameObject.Find("pMagATK_Num").GetComponent<TextMeshProUGUI>().text = "" + unitMDmg;

            //Set color of highest value stat for the player
            GetHighValue(unitPRes, unitMRes, unitPDmg, unitMDmg, 
                GameObject.Find("pPhysDEF_Num").GetComponent<TextMeshProUGUI>(),
                GameObject.Find("pMagDEF_Num").GetComponent<TextMeshProUGUI>(),
                GameObject.Find("pPhysATK_Num").GetComponent<TextMeshProUGUI>(),
                GameObject.Find("pMagATK_Num").GetComponent<TextMeshProUGUI>());

            //Set health color
            SetHealthColor(unitHp, unitMaxHp, GameObject.Find("pHealth_Num").GetComponent<TextMeshProUGUI>()); 
        }

        if (gridSystem.selectedUnit != null && gridSystem.selectedUnit.tag == "EnemyUnit")
        {
            OpenPanel(eUnitPane);

            //Intializing the enemy unit stats for the selected enemy unit
            eUnitName = gridSystem.selectedUnit.GetComponent<Unit>().unitName;
            eUnitType = gridSystem.selectedUnit.GetComponent<Unit>().unitMoveID;
            eUnitHp = gridSystem.selectedUnit.GetComponent<Unit>().currentHP;
            eUnitMaxHp = gridSystem.selectedUnit.GetComponent<Unit>().maxHP;
            eUnitPRes = gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalDefense;
            eUnitMRes = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalDefense;
            eUnitPDmg = gridSystem.selectedUnit.GetComponent<Unit>().basePhysicalAttack;
            eUnitMDmg = gridSystem.selectedUnit.GetComponent<Unit>().baseMagicalAttack;

            GameObject.Find("eUnitName").GetComponent<TextMeshProUGUI>().text = eUnitName;
            GameObject.Find("eClassName").GetComponent<TextMeshProUGUI>().text = eUnitType;
            GameObject.Find("eHealth_Num").GetComponent<TextMeshProUGUI>().text = eUnitHp + "/" + eUnitMaxHp;
            GameObject.Find("ePhysDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + eUnitPRes;
            GameObject.Find("eMagDEF_Num").GetComponent<TextMeshProUGUI>().text = "" + eUnitMRes;
            GameObject.Find("ePhysATK_Num").GetComponent<TextMeshProUGUI>().text = "" + eUnitPDmg;
            GameObject.Find("eMagATK_Num").GetComponent<TextMeshProUGUI>().text = "" + eUnitMDmg;

            //Set color of highest value stat
            GetHighValue(eUnitPRes, eUnitMRes, eUnitPDmg, eUnitMDmg, 
                GameObject.Find("ePhysDEF_Num").GetComponent<TextMeshProUGUI>(), 
                GameObject.Find("eMagDEF_Num").GetComponent<TextMeshProUGUI>(),
                GameObject.Find("ePhysATK_Num").GetComponent<TextMeshProUGUI>(), 
                GameObject.Find("eMagATK_Num").GetComponent<TextMeshProUGUI>());

            //Set health color
            SetHealthColor(eUnitHp, eUnitMaxHp, GameObject.Find("eHealth_Num").GetComponent<TextMeshProUGUI>()); 
        }

        if (gridSystem.selectedUnit == null)
        {
            //ClosePanel(pUnitPane);
            //ClosePanel(eUnitPane);
        }
    }

    //Executes player turn within battle scene
    public void BattleEndTurn()
    {
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

        playerName = player.GetComponent<Unit>().unitName;
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
        SetHealthColor(playerHp, playerMaxHp, playerHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>());

        playerStat.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + playerName;
        playerStat.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + playerPDmg;
        playerStat.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + playerPRes;
        playerStat.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + playerMDmg;
        playerStat.transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "" + playerMRes;

        playerStat.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + playerPDmgMod;
        playerStat.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text = "+" + playerPResMod;
        playerStat.transform.GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>().text = "+" + playerMDmgMod;
        playerStat.transform.GetChild(2).GetChild(7).GetComponent<TextMeshProUGUI>().text = "+" + playerMResMod;

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

        enemyPResMod = battleTurnSystem.totalEnemyPHYSDEFModifier;
        enemyMResMod = battleTurnSystem.totalEnemyMAGDEFModifier;
        enemyPDmgMod = battleTurnSystem.totalEnemyPHYSATKModifier;
        enemyMDmgMod = battleTurnSystem.totalEnemyMAGATKModifier;

        enemyHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = enemyHp + "/" + enemyMaxHp;
        SetHealthColor(playerHp, playerMaxHp, enemyHealth.transform.GetChild(1).GetComponent<TextMeshProUGUI>());

        enemyStat.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + enemyName;
        enemyStat.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + enemyPDmg;
        enemyStat.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + enemyPRes;
        enemyStat.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + enemyMDmg;
        enemyStat.transform.GetChild(2).GetChild(6).GetComponent<TextMeshProUGUI>().text = "" + enemyMRes;

        enemyStat.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = "+" + enemyPDmgMod;
        enemyStat.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text = "+" + enemyPResMod;
        enemyStat.transform.GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>().text = "+" + enemyMDmgMod;
        enemyStat.transform.GetChild(2).GetChild(7).GetComponent<TextMeshProUGUI>().text = "+" + enemyMResMod;
    }

    //Changes current turn text at top of the game screen to either player/enemy phase
    private void NextTurn()
    {
        if(!paused)
        {
            if (turnSystem.state == TurnSystem.State.PlayerTurn)
            {
                turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Player Phase";
                turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(44, 177, 133, 255);
                //levelUI.transform.GetChild(4).gameObject.SetActive(true); //End Turn Button
            }

            else
            {
                turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Enemy Phase";
                turnPane.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(140, 45, 168, 255);
                //levelUI.transform.GetChild(4).gameObject.SetActive(false); //End Turn Button
            }
        }  
    }

    private void SwapCam()
    {
        if (gridViewCam.activeSelf == true)
        {
            levelUI.SetActive(true);
            battleUI.SetActive(false);
            turnPane = levelUI.transform.GetChild(0).gameObject;    //Turn Pane
            pUnitPane = levelUI.transform.GetChild(1).gameObject;   //Player Stats Pane
            eUnitPane = levelUI.transform.GetChild(2).gameObject;   //Enemy Stats Pane
            winPane = levelUI.transform.GetChild(3).gameObject;     //Victory Pane
            losePane = levelUI.transform.GetChild(4).gameObject;    //Defeat Pane
        }

        else if(battleViewCam.activeSelf == true)
        {
            levelUI.SetActive(false);
            battleUI.SetActive(true);
            playerStat = battleUI.transform.GetChild(0).gameObject;     //Plyaer Battle Stats
            playerHealth = battleUI.transform.GetChild(1).gameObject;   //Player Battle Health
            enemyStat = battleUI.transform.GetChild(2).gameObject;      //Enemy Battle Stats
            enemyHealth = battleUI.transform.GetChild(3).gameObject;    //Enemy Battle Health
            SetBattleStats();
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
        Debug.Log("Highest Stat " + highestStat);

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

    //Set color of the health stat based on the percent left
    private void SetHealthColor(int currentHP, int maxHP, TextMeshProUGUI text)
    {
        if(currentHP >= (maxHP * 0.75))
        {
            text.color = new Color32(6, 182, 99, 255);
        }

        if(currentHP < (maxHP * 0.75) && currentHP > (maxHP * 0.25))
        {
            text.color = new Color32(165, 162, 6, 255);
        }

        if (currentHP <= (maxHP * 0.25))
        {
            text.color = new Color32(169, 36, 33, 255);
        }
    }
}
