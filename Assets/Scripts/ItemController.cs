using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemController : MonoBehaviour
{
    public List<string> stats = new List<string> { "crit", "power", "HP", "cdr" };
    public string statDesc;
    public float crit = 0;
    public int power = 0;
    public int HP = 0;
    public float cdr = 0;
    public GameObject parentAbility;
    public GameObject abilityFound = null;
    public GameObject DescObj;

    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        DescObj.SetActive(false);
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        DescObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(statDesc);
        //GetParentAbility();
    }

    void Update()
    {
        if (DescObj.activeSelf)
        {
            DescObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DescObj.transform.position = new Vector3(DescObj.transform.position.x, DescObj.transform.position.y, 0);
        }
    }
    public void SetStatByRarity(string stat, int gain)
    {
        switch (stat)
        {
            case "crit":
                float critBonus = Random.Range(5f, 10f);
                critBonus += critBonus * Mathf.Pow(gain, 2);
                crit = critBonus;
                statDesc += "Critical chance +%" + string.Format("{0:0.##}", crit ) + "\n";
                break;
            case "power":
                int powerBonus = Random.Range(1, 4);
                powerBonus += powerBonus * (int) Mathf.Pow(gain, 2);
                power = powerBonus;
                statDesc += "Damage +" + power + "\n";
                break;
            case "HP":
                int HPBonus = Random.Range(1, 4);
                HPBonus += HPBonus * (int)Mathf.Pow(gain, 2);
                HP = HPBonus;
                statDesc += "HP Bonus +" + HP + "\n";
                break;
            case "cdr":
                float cdrBonus = Random.Range(0.1f, 0.21f);
                cdrBonus += cdrBonus * (int)Mathf.Pow(gain, 2);
                cdr = cdrBonus;
                statDesc += "Cooldown reduction +%" + string.Format("{0:0.##}", cdr * 100) + "\n";
                break;
            default:
                Debug.Log("No se esta pasando ningun parametro en stat");
                break;
        }
    }

    public void GetParentAbility()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        foreach (GameObject ab in gm.abilitySlots)
        {
            for (int i = 0; i < ab.transform.childCount; i++)
            {
                GameObject childAbility = ab.transform.GetChild(i).gameObject;
                Debug.Log(childAbility.name);
                if (childAbility.name.Contains(parentAbility.name) &&
                    childAbility.CompareTag("Ability") &&
                    childAbility.activeSelf)
                {
                    abilityFound = childAbility;
                }
            }
        }
    }

    public void SetAsActiveItem()
    {
        if(abilityFound != null)
        {
            GameObject itemSlot = abilityFound.transform.Find("ItemSlot").gameObject;
            if (itemSlot.transform.childCount > 0)
            {
                Destroy(itemSlot.transform.GetChild(0).gameObject);
            }
            transform.SetParent(itemSlot.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            abilityFound.GetComponent<AbilityController>().SetItemStats(gameObject);
        }
        gm.itemDropCanvas.SetActive(false);
    }

    public void LeaveItem()
    {
        gm.itemDropCanvas.SetActive(false);
    }

    public void ShowItemDesc()
    {
        Debug.Log("show desc");
        if(transform.parent.name == "ItemSlot")
        {
            DescObj.SetActive(true);
        }
    }

    public void HideItemDesc()
    {
        Debug.Log("hide desc");
        DescObj.SetActive(false);
    }
}
