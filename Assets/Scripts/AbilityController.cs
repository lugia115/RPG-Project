using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityController : MonoBehaviour
{
    public int power;
    public int enemQuant;
    public float critRate;
    public int maxHP;
    public int currentHP;
    public GameObject HPObj;

    private int basePower;
    private float baseCrit;
    private int baseHP;
    private Attack attScript;
    private GameManager gm;
    private Slider hpBar;
    private TextMeshProUGUI healthPoints;
    private ChangeController abilChange;
    public GameObject itemSlot;
    // Start is called before the first frame update
    void Start()
    {
        basePower = power;
        baseCrit = critRate;
        baseHP = maxHP;
        abilChange = GameObject.Find("ChangePJ").GetComponent<ChangeController>();
        attScript = GameObject.Find("Attack").GetComponent<Attack>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        currentHP = maxHP;
        hpBar = transform.GetComponentInChildren<Slider>();
        healthPoints = HPObj.GetComponentInChildren<TextMeshProUGUI>();
        itemSlot = transform.Find("ItemSlot").gameObject;
        if (itemSlot.transform.childCount == 1)
        {
            SetItemStats(itemSlot.transform.GetChild(0).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = (float) currentHP / (float) maxHP;
        healthPoints.SetText(currentHP + "/" + maxHP);
    }

    public void SelectAbility()
    {
        WaitCooldown wc = GetComponent<WaitCooldown>();
        if (gm.playersTurn && wc.remainingCD == 0 && !abilChange.selecting)
        {
            Debug.Log("attack");
            GameObject enemy = GameObject.FindWithTag("Enemy");
            if (enemy != null)
            {
                attScript.SelectAttack(gameObject, power, enemQuant, critRate);
            }
        }
    }

    public void HealAbility()
    {
        WaitCooldown wc = GetComponent<WaitCooldown>();
        if (gm.playersTurn && wc.remainingCD == 0 && !abilChange.selecting)
        {
            Debug.Log("heal");
            GameObject[] team = GameObject.FindGameObjectsWithTag("Ability");
            foreach(GameObject go in team)
            {
                AbilityController ac = go.GetComponent<AbilityController>();
                int missingHP = ac.maxHP - ac.currentHP;
                ac.currentHP += power < missingHP ? power : missingHP;
            }
            if (wc != null) wc.PutOnCooldown();
            gm.turn++;
            StartCoroutine(gm.EnemyTurn());
        }
    }

    public void SetItemStats(GameObject item)
    {
        Debug.Log(currentHP);
        Debug.Log(maxHP);
        float currentPercentage = (float) currentHP / (float) maxHP;
        Debug.Log(currentPercentage);
        SetBaseStats();
        ItemController equipedItem = item.GetComponent<ItemController>();
        maxHP += equipedItem.HP;
        currentHP = (int) (maxHP * currentPercentage);
        power += equipedItem.power;
        critRate += equipedItem.crit;
        int cooldown = GetComponent<WaitCooldown>().abilityCD;
        GetComponent<WaitCooldown>().abilityCD = (int) (cooldown * (1 - equipedItem.cdr));
    }

    void SetBaseStats()
    {
        GetComponent<WaitCooldown>().abilityCD = GetComponent<WaitCooldown>().baseCD;
        power = basePower;
        critRate = baseCrit;
        maxHP = baseHP;
    }
}
