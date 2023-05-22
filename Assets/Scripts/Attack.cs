using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject confOrCancel;
    public GameObject damageUI;

    //Dmg base de los ataques
    private int basicDmg = 5;

    private GameManager gm;
    private GameObject objClicked;
    private TextMeshProUGUI prompt;
    private ChangeController abilChange;
    public int enemQuant;
    private int damage;
    private float critRate;
    // Start is called before the first frame update
    void Start()
    {
        abilChange = GameObject.Find("ChangePJ").GetComponent<ChangeController>();
        prompt = GameObject.Find("DialogText").GetComponent<TextMeshProUGUI>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (gm.enemiesSelected.Count > 0)
        {
            if (!confOrCancel.activeSelf)
            {
                confOrCancel.SetActive(true);
            }
        } else
        {
            confOrCancel.SetActive(false);
        }*/
    }

    public void BasickAttack(GameObject oc)
    {
        WaitCooldown wc = oc.GetComponent<WaitCooldown>();
        if (gm.playersTurn && wc.remainingCD == 0 && !abilChange.selecting)
        {
            Debug.Log("attack");
            GameObject enemy = GameObject.FindWithTag("Enemy");
            if (enemy != null)
            {
                SelectAttack(oc, basicDmg, 1, 0);
            }
            //codigo placeholder para testear pasar de rooms
            else
            {
                gm.roomClear = true;
                gm.ToggleMap();
                Destroy(GameObject.FindGameObjectWithTag("Background"));
            }
        }
    }

    public void SelectAttack(GameObject go, int pow, int enemQ, float crit)
    {
        objClicked = go;
        damage = pow;
        enemQuant = enemQ;
        critRate = crit;
        if (enemQuant > 1)
        {
            prompt.SetText("Select up to " + enemQuant + " enemies");
        } else prompt.SetText("Select " + enemQuant + " enemy");

        gm.selectingEnem = true;
    }

    public void ConfirmStartCoroutine()
    {
        StartCoroutine(ConfirmAttack());
    }

    IEnumerator ConfirmAttack()
    {
        GameObject canvas = GameObject.Find("FightCanvas");
        clearUI();
        Coroutine lastCoroutine = null;
        foreach (GameObject enemy in gm.enemiesSelected)
        {
            string endDamage = CalculateDamage();
            int dmgNum;
            GameObject dmgUIparent = Instantiate(damageUI, canvas.transform);
            dmgUIparent.transform.position = enemy.transform.position;
            GameObject dmgUI = dmgUIparent.transform.GetChild(0).gameObject;
            TextMeshProUGUI dmgText = dmgUI.GetComponent<TextMeshProUGUI>();
            if (endDamage.Contains("C"))
            {
                dmgNum = int.Parse(endDamage.Split("C")[0]);
                dmgText.fontSize += 5;
                dmgText.color = Color.red;
            }
            else dmgNum = int.Parse(endDamage);
            enemy.GetComponent<EnemyController>().lifePoints -= dmgNum;
            Animator anim = enemy.GetComponent<Animator>();
            anim.SetTrigger("Shake");
            dmgText.SetText(dmgNum.ToString());
            lastCoroutine = StartCoroutine(WaitForAnimation(dmgUI));
        }
        yield return lastCoroutine;
        WaitCooldown cd = objClicked.GetComponent<WaitCooldown>();
        if (cd != null) cd.PutOnCooldown();
        gm.turn++;
        ResetVars();
        StartCoroutine(gm.EnemyTurn());
    }
    public void CancelAttack()
    {
        clearUI();
        ResetVars();
    }
    void clearUI()
    {
        foreach (GameObject enemy in gm.enemiesSelected)
        {
            enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;
        }
        prompt.SetText("");
        prompt.gameObject.SetActive(true);
        confOrCancel.SetActive(false);
    }
    void ResetVars()
    {
        gm.selectingEnem = false;
        gm.enemiesSelected.Clear();
        enemQuant = 0;
        damage = 0;
    }

    IEnumerator WaitForAnimation(GameObject dmgObj)
    {
        Animator anim = dmgObj.GetComponent<Animator>();
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        Destroy(dmgObj.transform.parent.gameObject);
    }

    string CalculateDamage()
    {
        float randomNum = Random.Range(0f, 100.0f);
        if (randomNum < critRate)
        {
            return damage * 2 + "C";
        }
        else return damage + "";
    }
    private void OnEnable()
    {
        Debug.Log("attack enabled");
        confOrCancel.SetActive(false);
    }
}
