using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The GameObject this script is attached to needs to be placed as a child of the GameObject with the Unit script
public class SetHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject hpBar;
    private Unit unit;

    private Vector3 originalScale;
    private int battleHP;


    // Start is called before the first frame update
    void Start()
    {
        unit = transform.parent.GetComponent<Unit>();
        originalScale = hpBar.transform.localScale; 
        battleHP = unit.currentHP;

        if (transform.parent.tag == "PlayerUnit")
        {
            SetHealthColor(unit.currentHP, unit.maxHP, hpBar);
        }

        if (transform.parent.tag == "EnemyUnit")
        {
            hpBar.GetComponent<SpriteRenderer>().color = new Color32(134, 13, 200, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(battleHP != unit.currentHP)
        {
            if(transform.parent.tag == "PlayerUnit")
            {
                SetHealthColor(unit.currentHP, unit.maxHP, hpBar);
            }
                    
            SetHealthSize(unit.currentHP, unit.maxHP, hpBar);
        }

        battleHP = unit.currentHP;
    }

    //Changes the color of the health bar
    private void SetHealthColor(int currentHP, int maxHP, GameObject obj)
    {
        if (currentHP >= (maxHP * 0.75))
        {
            obj.GetComponent<SpriteRenderer>().color = new Color32(6, 182, 99, 255);
        }

        if (currentHP < (maxHP * 0.75) && currentHP > (maxHP * 0.25))
        {
            obj.GetComponent<SpriteRenderer>().color = new Color32(165, 162, 6, 255);
        }

        if (currentHP <= (maxHP * 0.25))
        {
            obj.GetComponent<SpriteRenderer>().color = new Color32(169, 36, 33, 255);
        }
    }

    //Changes the scale of the health bar
    private void SetHealthSize(float currentHP, float maxHP, GameObject obj)
    {
        Vector3 temp = originalScale;
        temp.x *= (currentHP / maxHP);
        obj.transform.localScale = temp;
    }
}
