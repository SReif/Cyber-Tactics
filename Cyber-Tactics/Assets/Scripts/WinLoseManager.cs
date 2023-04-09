using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    //This script gets attached to the SceneManager game object

    [System.NonSerialized] public GameObject[] enemies; //array of enemy units
    [System.NonSerialized] public GameObject[] allies; //array of player units

    public UIManager uiManager;

    // Start is called before the first frame update
    public void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        //Fill unit arrays
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        allies = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    public void Update()
    {
        //TEMPORARY CALLS FOR ENEMIES ARRAY AND ALLIES ARRAY. REMOVE EVERYTHING BETWEEN HERE AND THERE.
        //HERE
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        allies = GameObject.FindGameObjectsWithTag("Player");

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
    }

    //If lose conditions are met, this function will execute
    public void Lose()
    {
        uiManager.OpenPanel(uiManager.losePane);
    }


}
