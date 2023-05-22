using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeController : MonoBehaviour
{
    public List<GameObject> containers = new List<GameObject>();
    public bool selecting;

    private GameManager gm;
    private TextMeshProUGUI prompt;
    // Start is called before the first frame update
    void Start()
    {
        prompt = GameObject.Find("DialogText").GetComponent<TextMeshProUGUI>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        foreach(GameObject go in containers)
        {
            go.transform.Find("SelectingFrame").GetComponent<RawImage>().enabled = false;
        }
    }

    public void ChangeButton()
    {
        if (!selecting)
        {
            selecting = true;
            foreach (GameObject go in containers)
            {
                for(int i = 0; i < go.transform.childCount; i++)
                {
                    GameObject goChild = go.transform.GetChild(i).gameObject;
                    if (goChild.CompareTag("Ability") && goChild.activeSelf)
                    {
                        go.transform.Find("SelectingFrame").GetComponent<RawImage>().enabled = true;
                        break;
                    }
                }
            }
            prompt.SetText("Select 1 hero to replace the current active one");
        }
        else
        {
            EndSelect();
        }
    }

    public void SelectAbility(GameObject abilityContainer)
    {
        Debug.Log("selected " + abilityContainer.name);
        GameObject activeAbParent = gm.activeAbility.transform.parent.gameObject;
        int indexOfActiveAb = gm.activeAbility.transform.GetSiblingIndex();
        for(int i = 0; i < abilityContainer.transform.childCount; i++)
        {
            GameObject goChild = abilityContainer.transform.GetChild(i).gameObject;
            if (goChild.CompareTag("Ability"))
            {
                gm.activeAbility.transform.SetParent(goChild.transform.parent, false);
                gm.activeAbility.transform.SetSiblingIndex(goChild.transform.GetSiblingIndex());
                goChild.transform.SetParent(activeAbParent.transform, false);
                goChild.transform.SetSiblingIndex(indexOfActiveAb);
                gm.activeAbility = goChild;
                break;
            }
        }
        EndSelect();
    }

    void EndSelect()
    {
        selecting = false;
        foreach (GameObject go in containers)
        {
            go.transform.Find("SelectingFrame").GetComponent<RawImage>().enabled = false;
        }
        prompt.SetText("");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
