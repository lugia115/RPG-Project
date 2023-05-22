using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Test : MonoBehaviour
{
    public GameObject firstPoint;
    public GameObject secondPoint;
    public Material lineMaterial;

    private GameObject lineObject;
    private LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        //Declare string for alphabet
        string strAlpha = "";
        //Loop through the ASCII characters 65 to 90
        for (int i = 65; i <= 90; i++) //

        {

            // Convert the int to a char to get the actual character behind the ASCII code

            strAlpha += ((char)i).ToString() + " ";

        }
        Debug.Log(strAlpha);

        /*lineObject = new GameObject();
        lineObject.AddComponent<LineRenderer>();
        line = lineObject.GetComponent<LineRenderer>();
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.material = lineMaterial;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*if (line.GetPosition(0) != firstPoint.transform.position)
        {
            Vector3[] linePoints = { firstPoint.transform.position, secondPoint.transform.position };
            lineObject.GetComponent<LineRenderer>().SetPositions(linePoints);
            Debug.Log("Line: " + lineObject.GetComponent<LineRenderer>().GetPosition(0));
            Debug.Log("Line: " + lineObject.GetComponent<LineRenderer>().GetPosition(1));
        }*/
    }
}
