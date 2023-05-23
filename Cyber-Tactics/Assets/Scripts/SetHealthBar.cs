using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHealthBar : MonoBehaviour
{
    [SerializeField] private Sprite divSpritePrefab;
    [SerializeField] private GameObject hpBar;
    private int battleHP;
    private bool resize = false;

    private Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        unit = transform.parent.GetComponent<Unit>();
        battleHP = unit.currentHP;
        SetHealthColor(unit.currentHP, unit.maxHP, hpBar);
    }

    // Update is called once per frame
    void Update()
    {
        if(battleHP > unit.currentHP)
        {
            SetHealthColor(unit.currentHP, unit.maxHP, hpBar);
            SetHealthLower(unit.currentHP, unit.maxHP, hpBar);
            Debug.Log(battleHP + " / " + unit.currentHP);
        }

        if(battleHP < unit.currentHP)
        {
            SetHealthColor(unit.currentHP, unit.maxHP, hpBar);
            SetHealthHigher(unit.currentHP, unit.maxHP, hpBar);
            Debug.Log(battleHP + " / " + unit.currentHP);
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
    private void SetHealthLower(float currentHP, float maxHP, GameObject obj)
    {
        Debug.Log("Setting Health Lower");
        Vector3 temp = obj.transform.localScale;
        temp.x *= (currentHP / maxHP);
        obj.transform.localScale = temp;
    }

    private void SetHealthHigher(float currentHP, float maxHP, GameObject obj)
    {
        Debug.Log("Setting Healther Higher");
        Vector3 temp = obj.transform.localScale;
        temp.x *= (maxHP / currentHP);
        obj.transform.localScale = temp;
    }
}
