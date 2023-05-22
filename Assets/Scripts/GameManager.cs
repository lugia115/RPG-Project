using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemiesSelected = new List<GameObject>();
    public string currentRoom = "";
    public int turn;
    public bool playersTurn;
    public bool roomClear;
    public bool selectingEnem;
    public GameObject fightCanvas;
    public GameObject defaultCanvas;
    public GameObject activeAbility;
    public GameObject itemDropCanvas;
    public GameObject gameoverObj;
    public Sprite spriteForSelection;
    public GameObject mapObj;
    public List<GameObject> heroItems;
    public List<GameObject> abilitySlots = new List<GameObject>();

    private List<GameObject> enemiesInRoom = new List<GameObject>();

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        turn = 0;
        playersTurn = true; 
        selectingEnem = false;
        fightCanvas = GameObject.Find("FightCanvas");
        defaultCanvas = GameObject.Find("DefaultCanvas");
        itemDropCanvas = GameObject.Find("ItemDropCanvas");
        itemDropCanvas.transform.Find("Arrow").GetComponent<RawImage>().enabled = false;
        fightCanvas.SetActive(false);
        defaultCanvas.SetActive(false);
        itemDropCanvas.SetActive(false);
        roomClear = true;
        gameoverObj = GameObject.Find("GameOverCanvas");
        gameoverObj.SetActive(false);

        //StartCoroutine(DropItem());
    }

    public IEnumerator DropItem()
    {
        //GenerateItem
        GameObject itemGenerated = GenerateDroppedItem();
        ItemController itemCon = itemGenerated.GetComponent<ItemController>();
        itemCon.GetParentAbility();
        //Set Parent
        GameObject itemEquipedParent = itemDropCanvas.transform.Find("Item1Parent").gameObject;
        GameObject itemEquipedClone = null;
        GameObject itemParent;
        if(itemCon.abilityFound != null)
        {
            if (itemCon.abilityFound.transform.Find("ItemSlot").childCount > 0)
            {
                itemDropCanvas.transform.Find("Arrow").GetComponent<RawImage>().enabled = true;
                GameObject itemEquiped = itemCon.abilityFound.transform.Find("ItemSlot").GetChild(0).gameObject;
                itemEquipedClone = Instantiate(itemEquiped, itemEquipedParent.transform);
                TextMeshProUGUI desc = itemEquipedParent.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
                desc.SetText(itemEquiped.GetComponent<ItemController>().statDesc);
                itemParent = itemDropCanvas.transform.Find("Item2Parent").gameObject;
            }
            else
            {
                itemParent = itemDropCanvas.transform.Find("ItemSoloParent").gameObject;
            }
        } else
        {
            itemParent = itemDropCanvas.transform.Find("ItemSoloParent").gameObject;
        }
        itemGenerated.transform.SetParent(itemParent.transform, false);
        itemGenerated.transform.localPosition = Vector3.zero;
        itemGenerated.transform.localScale = Vector3.one;
        //Set Description
        TextMeshProUGUI itemDesc = itemParent.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        itemDesc.SetText(itemCon.statDesc);
        itemDropCanvas.SetActive(true);
        //Set buttons
        Button takeItem = itemDropCanvas.transform.Find("TakeItem").GetComponent<Button>();
        if(itemCon.abilityFound != null)
        {
            takeItem.onClick.AddListener(itemCon.SetAsActiveItem);
        }
        else
        {
            takeItem.gameObject.SetActive(false);
        }
        Button leaveItem = itemDropCanvas.transform.Find("LeaveItem").GetComponent<Button>();
        leaveItem.onClick.AddListener(itemCon.LeaveItem);
        yield return new WaitUntil(() => itemDropCanvas.activeSelf == false);
        //Reset Parents
        for(int i = 0; i < itemParent.transform.childCount; i++)
        {
            if(itemParent.transform.GetChild(i).tag == "Item")
            {
                Destroy(itemParent.transform.GetChild(i).gameObject);
            }
            itemDesc.SetText("");
        }
        itemDropCanvas.transform.Find("Arrow").GetComponent<RawImage>().enabled = false;
        if (itemEquipedClone != null)
        {
            TextMeshProUGUI desc = itemEquipedClone.transform.parent.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
            desc.SetText("");
            Destroy(itemEquipedClone);
        }
        takeItem.onClick.RemoveAllListeners();
        takeItem.gameObject.SetActive(true);
        leaveItem.onClick.RemoveAllListeners();
    }

    GameObject GenerateDroppedItem()
    {
        GameObject itemGenerated = Instantiate(heroItems[Random.Range(0, heroItems.Count)]);
        ItemController itemCon = itemGenerated.GetComponent<ItemController>();
        int randomRarity = Random.Range(0, 4);
        int i;
        int j;
        string stat;
        string stat2;
        switch (randomRarity)
        {
            case 0:
                itemGenerated.GetComponent<RawImage>().color = Color.white;
                i = Random.Range(0, itemCon.stats.Count);
                stat = itemCon.stats[i];
                itemCon.SetStatByRarity(stat, 0);
                break;
            case 1:
                itemGenerated.GetComponent<RawImage>().color = Color.blue;
                i = Random.Range(0, itemCon.stats.Count);
                stat = itemCon.stats[i];
                itemCon.SetStatByRarity(stat, 1);
                break;
            case 2:
                itemGenerated.GetComponent<RawImage>().color = new Color(154, 0, 203);
                i = Random.Range(0, itemCon.stats.Count);
                stat = itemCon.stats[i];
                do {
                    j = Random.Range(0, itemCon.stats.Count);
                } while (i == j);
                stat2 = itemCon.stats[j];
                itemCon.SetStatByRarity(stat, 1);
                itemCon.SetStatByRarity(stat2, 0);
                break;
            case 3:
                itemGenerated.GetComponent<RawImage>().color = Color.yellow;
                i = Random.Range(0, itemCon.stats.Count);
                stat = itemCon.stats[i];
                do
                {
                    j = Random.Range(0, itemCon.stats.Count);
                } while (i == j);
                stat2 = itemCon.stats[j];
                itemCon.SetStatByRarity(stat, 2);
                itemCon.SetStatByRarity(stat2, 2);
                break;
        }
        return itemGenerated;
    }

    public void GetAllEnemies()
    {
        enemiesInRoom.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    public void ToggleMap()
    {
        GameObject[] HUDs = GameObject.FindGameObjectsWithTag("HUD");
        foreach(GameObject hud in HUDs)
        {
            if (roomClear)
            {
                hud.SetActive(false);
            }  
            
        }
        mapObj.SetActive(!mapObj.activeSelf);
    }

    public IEnumerator EnemyTurn()
    {
        playersTurn = false;
        List<GameObject> toRemove = new List<GameObject>();
        Coroutine lastCoroutine = null;
        foreach(GameObject go in enemiesInRoom)
        {
            EnemyController ec = go.GetComponent<EnemyController>();
            if (ec.lifePoints <= 0)
            {
                lastCoroutine = StartCoroutine(ec.WaitForDeathAnimation());
                toRemove.Add(go);
            }
        }
        if(lastCoroutine != null)
        {
            yield return lastCoroutine;
        }
        foreach(GameObject go in toRemove)
        {
            enemiesInRoom.Remove(go);
        }
        if (enemiesInRoom.Count == 0)
        {
            yield return StartCoroutine(DropItem());
            NextRoom();
        }
        else
        {
            Debug.Log(enemiesInRoom.Count);
            foreach(GameObject go in enemiesInRoom)
            {
                EnemyAttacks ea = go.GetComponent<EnemyAttacks>();
                int selectedAtt = Random.Range(0, ea.attackQuant) + 1;
                yield return ea.StartCoroutine(go.name + "Attack" + selectedAtt);
                if (GameObject.FindGameObjectsWithTag("Ability").Length == 0)
                {
                    GameOver();
                }
            }
        }
        playersTurn = true;
    }

    void GameOver()
    {
        gameoverObj.SetActive(true);
    }

    public void NextRoom()
    {
        playersTurn = true;
        turn = 0;
        roomClear = true;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ability"))
        {
            go.GetComponent<WaitCooldown>().remainingCD = 0;
        }
        ToggleMap();
        Destroy(GameObject.FindGameObjectWithTag("Background"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
