using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Map_Generator : MonoBehaviour
{
    public GameObject roomType0;
    public GameObject roomType1;
    public GameObject roomType2;
    public GameObject mapObj;
    public Material lineMaterial;

    public int levelQuant = 5;
    private int counter = 0;
    
    private Dictionary<int, List<int>> roomsByLvl = new Dictionary<int, List<int>>();
    public Dictionary<string, List<string>> roomMap = new Dictionary<string, List<string>>(); //aca se guarda a donde puede ir cada room
    private Dictionary<GameObject, List<GameObject>> linesByRooms = new Dictionary<GameObject, List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < levelQuant; i++)
        {
            CreateCanvas(i);
            int level = i;
            /**/
            for(int j = 0; j < Random.Range(2, 5); j++)
            {
                int randomRoom = Random.Range(0, 3); //aca va el tipo de room
                if (roomsByLvl.ContainsKey(level))
                {
                    roomsByLvl[level].Add(randomRoom);
                }
                else roomsByLvl.Add(level, new List<int>(){ randomRoom });
            }
        }
        CalculateRoomProgresion();
        CreateRooms();
    }

    void CalculateRoomProgresion()
    {
        for(int i = 0; i < roomsByLvl.Keys.Count; i++)
        {
            int nextRoom = 0; //ultimo room conectado por la ultima iteracion
            for (int j = 0; j < roomsByLvl[i].Count; j++)
            {
                string Lvl;
                string nextLvl = "";
                bool isLast = j == roomsByLvl[i].Count - 1;
                Debug.Log(isLast);
                Lvl = NumberToLetter(i);
                nextLvl = NumberToLetter(i + 1);
                if (roomsByLvl.ContainsKey(i+1))
                {
                    if(j != 0 && nextRoom < roomsByLvl[i + 1].Count - 1)
                    {
                        //chance binaria que el proximo room sea el mismo que el ultimo o el siguiente
                        nextRoom += Random.Range(0, 2);
                    }
                    int connectedRooms;
                    if (!isLast)
                    {
                        int max = nextRoom + 2 > roomsByLvl[i + 1].Count ? roomsByLvl[i + 1].Count : nextRoom + 2;
                        connectedRooms = Random.Range(nextRoom, max);
                    }
                    else connectedRooms = roomsByLvl[i + 1].Count - 1;
                    List<string> conRoomList = new List<string>();
                    do
                    {
                        conRoomList.Add(nextLvl + nextRoom);
                        nextRoom++;
                    } while (nextRoom <= connectedRooms);
                    nextRoom--;
                    Debug.Log("Intentando agregar: " + Lvl + j);
                    roomMap.Add(Lvl + j, conRoomList);
                }
            }
        }

        foreach (List<string> r in roomMap.Values)
        {
            Debug.Log(string.Join(", ", r));
        }
    }

    void CreateCanvas(int num)
    {
        GameObject newCanvas = new GameObject();
        newCanvas.transform.SetParent(mapObj.transform);
        Camera camera = Camera.main;
        float cameraH = camera.orthographicSize * camera.aspect * 2;
        float cameraV = camera.orthographicSize * 2;
        float canvWidth = cameraH / 5;
        newCanvas.name = "Lv" + num + "_Canvas";
        newCanvas.AddComponent<Canvas>();
        newCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        newCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        newCanvas.GetComponent<Canvas>().sortingLayerName = "Menu";
        newCanvas.GetComponent<Canvas>().sortingOrder = 2;
        newCanvas.AddComponent<CanvasScaler>();
        newCanvas.AddComponent<GraphicRaycaster>();
        newCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(canvWidth, cameraV);
        newCanvas.layer = 5;
        newCanvas.transform.position = new Vector3(-(cameraH/2)+(canvWidth /2) + canvWidth * num, 0, 0);
        newCanvas.AddComponent<VerticalLayoutGroup>();
        newCanvas.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        newCanvas.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = true;
        newCanvas.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
        newCanvas.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
        newCanvas.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
    }

    void CreateRooms()
    {
        List<string> createdRooms = new List<string>();
        for(int i = 0; i < roomsByLvl.Keys.Count; i++)
        {
            GameObject lvlCanvas = GameObject.Find("Lv" + i + "_Canvas");
            int j = 0;
            foreach(int r in roomsByLvl[i])
            {
                GameObject roomToAdd;
                switch (r)
                {
                    case 0:
                        roomToAdd = roomType0;
                        break;
                    case 1:
                        roomToAdd = roomType1;
                        break;
                    default:
                        roomToAdd = roomType2;
                        break;
                }
                string name = NumberToLetter(i) + j;
                roomToAdd.name = name;
                roomToAdd.GetComponent<RoomMap>().roomType = r;
                Instantiate(roomToAdd, lvlCanvas.transform);
                createdRooms.Add(roomToAdd.name.Substring(0, 2));
                j++;
            }
        }
        Debug.Log(string.Join(", ", createdRooms));
        ConnectRooms(createdRooms);
    }

    void ConnectRooms(List<string> roomsToConnect)
    {
        foreach(string rtc in roomsToConnect)
        {
            if(!rtc.StartsWith(NumberToLetter(levelQuant - 1))){
                Debug.Log("room: " + rtc);
                GameObject roomObject1 = GameObject.Find(rtc + "(Clone)");
                foreach (string r in roomMap[rtc])
                {
                    GameObject roomObject2 = GameObject.Find(r + "(Clone)");
                    GenerateLine(roomObject1, roomObject2);
                }
            }
        }
    }

    void GenerateLine(GameObject obj1, GameObject obj2)
    {
        GameObject lineObject;
        LineRenderer line;
        lineObject = new GameObject();
        lineObject.transform.SetParent(mapObj.transform);
        lineObject.name = obj1.name.Substring(0, 2) + ":" + obj2.name.Substring(0, 2);
        lineObject.tag = "MapIcons";
        lineObject.AddComponent<LineRenderer>();
        line = lineObject.GetComponent<LineRenderer>();
        line.sortingLayerName = "Menu";
        line.sortingOrder = 1;
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = lineMaterial;

        linesByRooms.Add(lineObject, new List<GameObject> {obj1, obj2});
    }

    string NumberToLetter(int num)
    {
        num += 65;
        string letter = ((char)num).ToString() + "";
        return letter;
    }
    
    public void moveLines()
    {
        foreach (GameObject go in linesByRooms.Keys)
        {
            List<GameObject> rooms = linesByRooms[go];
            Vector3[] linePoints = { rooms[0].transform.position, rooms[1].transform.position };
            go.GetComponent<LineRenderer>().SetPositions(linePoints);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if(counter < 3)
        {
            moveLines();
            counter++;
        }
        
    }
}
