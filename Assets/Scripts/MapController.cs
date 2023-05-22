using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private float startPosX;
    private float mouseDownPosX;
    private GameObject mapObj;
    private Map_Generator mapGen;
    // Start is called before the first frame update
    void Start()
    {
        mapObj = transform.GetChild(0).gameObject;
        mapGen = GameObject.Find("MapGenerator").GetComponent<Map_Generator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        mouseDownPosX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        startPosX = mapObj.transform.position.x;
    }

    void OnMouseDrag()
    {
        float offset = mouseDownPosX - Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mapObj.transform.position = new Vector3(startPosX - offset, transform.position.y);
        mapGen.moveLines();
    }
}
