using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyAttacks : MonoBehaviour
{
    //*LEER IMPORTANTE*
    //Al agregar o quitar un ataque es importante que los metodos se llamen
    //exactamente igual al nombre del enemigo + "Attack" + numero del ataque
    //Tambien hay que editar en el objeto en Unity la variable attackQuant
    //con la cantidad de ataques que tenga el enemigo.

    public int attackQuant = 1;

    private GameManager gm;
    private TextMeshProUGUI prompt;
    private ChangeController changeCon;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        prompt = GameObject.Find("DialogText").GetComponent<TextMeshProUGUI>();
        changeCon = GameObject.Find("ChangePJ").GetComponent<ChangeController>();
    }

    IEnumerator DamageAbilities(List<GameObject> toDamage, int dmg)
    {
        Animator anim = null;
        foreach (GameObject go in toDamage)
        {
            int HP = go.GetComponent<AbilityController>().currentHP;
            go.GetComponent<AbilityController>().currentHP -= HP < dmg ? HP : dmg;
            anim = go.GetComponent<Animator>();
            anim.SetTrigger("Shake");
            if(go.GetComponent<AbilityController>().currentHP <= 0)
            {
                if (GameObject.FindGameObjectsWithTag("Ability").Length > 1)
                {
                    changeCon.ChangeButton();
                    go.GetComponent<RawImage>().enabled = false;
                    yield return new WaitUntil(() => !changeCon.selecting);
                }
                go.SetActive(false);
            }
        }
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    IEnumerator SmileyAttack1()
    {
        Debug.Log("Setting attack prompt");
        List<GameObject> toDamage = new List<GameObject>();
        toDamage.Add(gm.activeAbility);
        prompt.text = "ataque de " + name + "!";
        prompt.color = Color.red;
        yield return StartCoroutine(DamageAbilities(toDamage, 5));
        prompt.SetText("");
        prompt.color = Color.white;
        Debug.Log("End attack");
    }

    IEnumerator WitchAttack1()
    {
        Debug.Log("Setting attack prompt");
        List<GameObject> toDamage = new List<GameObject>();
        toDamage.Add(gm.activeAbility);
        prompt.text = "ataque de " + name + "!";
        prompt.color = Color.red;
        yield return StartCoroutine(DamageAbilities(toDamage, 3));
        prompt.SetText("");
        prompt.color = Color.white;
        Debug.Log("End attack");
    }

    IEnumerator WitchMinionAttack1()
    {
        Debug.Log("Setting attack prompt");
        List<GameObject> toDamage = new List<GameObject>();
        toDamage.Add(gm.activeAbility);
        prompt.text = "ataque de " + name + "!";
        prompt.color = Color.red;
        yield return StartCoroutine(DamageAbilities(toDamage, 1));
        prompt.SetText("");
        prompt.color = Color.white;
        Debug.Log("End attack");
    }

    IEnumerator NecroTerliumAttack1()
    {
        Debug.Log("Setting attack prompt");
        List<GameObject> toDamage = new List<GameObject>();
        toDamage.Add(gm.activeAbility);
        prompt.text = "ataque de " + name + "!";
        prompt.color = Color.red;
        yield return StartCoroutine(DamageAbilities(toDamage, 3));
        prompt.SetText("");
        prompt.color = Color.white;
        Debug.Log("End attack");
    }
}
