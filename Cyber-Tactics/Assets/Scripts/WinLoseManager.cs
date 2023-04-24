using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the SceneManager game object
public class WinLoseManager : MonoBehaviour
{
    [System.NonSerialized] public GameObject[] enemies; //array of enemy units
    [System.NonSerialized] public GameObject[] allies; //array of player units

    public UIManager uiManager;

    // Start is called before the first frame update
    public void Start()
    {
        //Instantiating variables
        if(GameObject.Find("UIManager") != null)
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }

        //Fill unit arrays
        enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        allies = GameObject.FindGameObjectsWithTag("PlayerUnit");
    }

    // Update is called once per frame
    public void Update()
    {
        //TEMPORARY CALLS FOR ENEMIES ARRAY AND ALLIES ARRAY. REMOVE EVERYTHING BETWEEN HERE AND THERE.
        //HERE
        if(uiManager == null && GameObject.Find("LevelUI") != null)
        {
            uiManager = GameObject.Find("LevelUI").GetComponent<UIManager>();
        }

        enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        allies = GameObject.FindGameObjectsWithTag("PlayerUnit");

        if (enemies.Length == 0)
        {
            Win();
        }

        if(allies.Length == 0)
        {
            Lose();
        }
        //THERE
    }

    //If win conditions are met, this function will execute
    public void Win()
    {
        uiManager.OpenPanel(uiManager.winPane);
        uiManager.ClosePanel(uiManager.turnPane);
        uiManager.ClosePanel(uiManager.unitPane);
        uiManager.Pause();
    }

    //If lose conditions are met, this function will execute
    public void Lose()
    {
        uiManager.OpenPanel(uiManager.losePane);
        uiManager.ClosePanel(uiManager.turnPane);
        uiManager.ClosePanel(uiManager.unitPane);
        uiManager.Pause();
    }


}
