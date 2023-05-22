using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class RoomMap : MonoBehaviour
{
    public int roomType;
    public Sprite RoomImg;
    public GameObject[] enemies;
    public GameObject chest;

    private GameManager gm;
    private Map_Generator mapGen;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapGen = GameObject.Find("MapGenerator").GetComponent<Map_Generator>();
    }

    public void OnHover()
    {
        Debug.Log("trigger OnHover!");
    }

    public void OnClick()
    {
        Debug.Log("trigger OnClick!" + gameObject.name.Substring(0, 2));
        string rName = gameObject.name.Substring(0, 2);

        if (gm.currentRoom != "")
        {
            if (mapGen.roomMap.ContainsKey(gm.currentRoom))
            {
                if (mapGen.roomMap[gm.currentRoom].Contains(rName) && gm.roomClear)
                {
                    GameObject.Find(gm.currentRoom + "(Clone)").GetComponent<RawImage>().color = gameObject.GetComponent<RawImage>().color;
                    SetToCurrentRoom(rName);
                }
            }
        } else if (rName.Substring(0, 1) == "A")
        {
            SetToCurrentRoom(rName);
        }
        
    }

    void SetToCurrentRoom(string rName)
    {
        gameObject.GetComponent<RawImage>().color = Color.red;
        gm.currentRoom = rName;

        //experimentacion (parece que funciona :O)
        List<string> allConnRooms = new List<string>();
        List<string> roomsToSearch = new List<string>();
        allConnRooms.Add(rName);
        roomsToSearch.Add(rName);
        while(roomsToSearch.Count > 0)
        {
            List<string> roomsToAdd = new List<string>();
            foreach (string r in roomsToSearch)
            {
                if (mapGen.roomMap.ContainsKey(r))
                {
                    roomsToAdd.AddRange(mapGen.roomMap[r]);
                }
            }
            allConnRooms.AddRange(roomsToAdd);
            roomsToSearch.Clear();
            roomsToSearch.AddRange(roomsToAdd);
        }
        GameObject[] allIcons = GameObject.FindGameObjectsWithTag("MapIcons");
        foreach(GameObject go in allIcons)
        {
            if(!allConnRooms.Contains(go.name.Substring(0, 2)))
            {
                if(go.TryGetComponent<RawImage>(out RawImage img))
                {
                    img.color = Color.grey;
                }else if (go.TryGetComponent<LineRenderer>(out LineRenderer lr))
                {
                    lr.startColor = Color.gray;
                    lr.endColor = Color.gray;
                }
                
            }
        }
        GenerateRoom();
    }

    void GenerateRoom()
    {
        //crear y ajustar imagen de fondo
        GameObject roomObj = new GameObject();
        roomObj.name = "Background";
        roomObj.tag = "Background";
        roomObj.AddComponent<SpriteRenderer>();
        SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();
        sr.sprite = RoomImg;
        sr.sortingLayerName = "Background";
        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;
        float scAspectRatio = (float)Screen.width / (float)Screen.height;
        float imgAspectRatio = width / height;

        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight * scAspectRatio;

        Vector2 imgScale;
        if (imgAspectRatio > scAspectRatio)
        {
            imgScale = new Vector2(worldScreenHeight / height, worldScreenHeight / height);
        } else imgScale = new Vector2(worldScreenWidth / width, worldScreenWidth / width);
        roomObj.transform.localScale = imgScale;

        switch (roomType)
        {
            case 0:
                CreateEnemy();
                gm.fightCanvas.SetActive(true);
                break;
            case 1:
                CreateLoot();
                gm.defaultCanvas.SetActive(true);
                break;
            default:
                gm.defaultCanvas.SetActive(true);
                break;
        }

        gm.roomClear = false;
        gm.GetAllEnemies();
        gm.ToggleMap();
    }

    void CreateEnemy()
    {
        GameObject enemy = enemies[Random.Range(0, enemies.Length)];
        Instantiate(enemy);
    }

    void CreateLoot()
    {
        Instantiate(chest);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
