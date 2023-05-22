using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private GameManager gm;
    private bool opened;
    // Start is called before the first frame update
    void Start()
    {
        opened = false;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnMouseDown()
    {
        if (!opened)
        {
            Debug.Log("click on chest");
            StartCoroutine(OpenChest());
        }
    }

    IEnumerator OpenChest()
    {
        opened = true;
        yield return StartCoroutine(gm.DropItem());
        gm.roomClear = true;
        Destroy(GameObject.FindGameObjectWithTag("Background"));
        Destroy(gameObject);
        gm.ToggleMap();
    }
}
