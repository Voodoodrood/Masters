using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public Transform pointer;
    public GameObject ball;
    public Terrain terr;
    public Material matt;
    List<GameObject> lines;
    int xRes;
    int yRes;
    int lineTracker = -1;
    public float[,] heights;
    Vector3 lastHit;
    RaycastHit hit;
    List<Vector3> points;
    private float timer;
    float[,,] element;
    int mapX, mapY;
    float[,,] map;
    public Canvas myColorMenu;
    ReverseGrab grabberRight;
    GameObject hillWidge;

    // Use this for initialization
    void Start()
    {
        points = new List<Vector3>();
        lines = new List<GameObject>();
        xRes = terr.terrainData.heightmapWidth;
        yRes = terr.terrainData.heightmapHeight;
        map = new float[terr.terrainData.alphamapWidth, terr.terrainData.alphamapHeight, terr.terrainData.alphamapLayers];
        element = new float[1, 1, terr.terrainData.alphamapLayers];
        myReset();
        terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
        grabberRight = GameObject.Find("AvatarGrabberRight").GetComponent<ReverseGrab>();
        InvokeRepeating("TerrainUpdate", 2.0f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(pointer.position, -pointer.forward, out hit);

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (myColorMenu.enabled)
                myColorMenu.enabled = false;
            else
                myColorMenu.enabled = true;

        }

        //creates orb at location of pointer target when trigger pulled
        if (grabberRight.getGrabbed() != null && grabberRight.getGrabbed() == "bullet")
        {
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            {
                lineTracker++;
                lines.Add(new GameObject());
                lines[lineTracker].AddComponent<LineRenderer>();
                points.Clear();
                createBall();
                timer = Time.time;
            }
            //Draws line if trigger is down
            if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                points.Add(hit.point + new Vector3(0, 0.01f, 0));
                DrawLine(points);
                hillWidge.GetComponentInChildren<HillWidget>().AddPoint(hit.point + new Vector3(0, 0.01f, 0));
            }

            if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
            {
                if (Time.time - timer > 0.1)
                {
                    if (Physics.Raycast(pointer.position, -pointer.forward, out hit))
                    {
                        GameObject temp = Instantiate(ball, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                        hillWidge.GetComponentInChildren<HillWidget>().addPair(temp.GetComponentInChildren<HillWidget>());
                        temp.GetComponentInChildren<HillWidget>().addPair(hillWidge.GetComponentInChildren<HillWidget>());
                        temp.GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(hit.point, Quaternion.Euler(0, 0, 0));
                        temp.GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(hit.point + new Vector3(0, 0, -0.1f), Quaternion.Euler(0, 0, 0));
                    }
                }
            }
        }
        
    }
    //resets the terrain at the beginning of each session
    private void resetTerrain()
    {
        heights = terr.terrainData.GetHeights(0, 0, xRes, yRes);
        for (int y = 0; y < yRes; y++)
        {
            for (int x = 0; x < xRes; x++)
            {
                heights[x, y] = 0.0f;

            }
        }
    }
    //creates an orb where the laser is pointing
    public void createBall()
    {
        if (Physics.Raycast(pointer.position, -pointer.forward, out hit) && CheckforWidgets(hit.point))
        {
            hillWidge = Instantiate(ball, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0)) as GameObject;
            hillWidge.GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(hit.point, Quaternion.Euler(0, 0, 0));
            hillWidge.GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(hit.point +new Vector3(0,0,-0.1f), Quaternion.Euler(0, 0, 0));
        }
    }

    //changes the terrain at the location of a collider
    public void changeTerrain()
    {
        resetTerrain();
        GameObject [] hillWidgets;
        hillWidgets = GameObject.FindGameObjectsWithTag("hillWidgetContainer");
        foreach (GameObject hillWidget in hillWidgets)
        {
            HillWidget current = hillWidget.GetComponent<HillWidget>();
            current.SetPosition();
            current.SetWidth();
            float xPos = current.GetPosition().x;
            float zPos = current.GetPosition().z;
            float yPos = current.GetPosition().y;
            float width = current.GetWidth();

            CreateHill(xPos, zPos, yPos, width);
            if (current.getPair() != null)
            {
                if (current.GetPoints() != null)
                {
                    for (int i =0; i<current.GetPoints().Count;i++)
                    {
                        CreateHill(convert(current.GetPoints()[i].x), convert(current.GetPoints()[i].z), (current.GetPosition().y*(1-((float)i / current.GetPoints().Count)) + current.getPair().GetPosition().y * (((float)i / current.GetPoints().Count))), width);
                        //Debug.Log("I: " +i + " count: " + current.GetPoints().Count + " weighting: " + (1 - ((float)i / current.GetPoints().Count)) + " total: " + current.GetPosition().y * (1 - (i / current.GetPoints().Count)));
                    }
                }
            }            
        }
        terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
        //Smooth();
    }

    public float convert(float input)
    {
        return (input + 0.5f) * 513;
    }

    // Used to draw lines on the map between points
    void DrawLine(List<Vector3> pointList)
    {
        lines[lineTracker].transform.position = pointList[0];
        LineRenderer lr = lines[lineTracker].GetComponent<LineRenderer>();
        lr.material = matt;
        lr.startColor = Color.green;
        lr.endColor = Color.green;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.positionCount = pointList.Count;
        lr.SetPositions(pointList.ToArray());
    }
    //Used to draw the laser out of the pencil TODO: move to own script held by pencil object
    

    //creates a hill using
    public void CreateHill(float x, float z, float y, float area)
    {
        float point = 0;
        for (int a = (int)Mathf.Max(0,z-(area*(-600))); a < (int)Mathf.Min(513, z + (area * (-600))); a++)
        {
            for (int b = (int)Mathf.Max(0, x - (area * (-600))); b < (int)Mathf.Min(513, x + (area * (-600))); b++)
            {
                point = y*Mathf.Pow((float)Math.E, -(float)((1 / (double)(2 * Math.Pow((area*(-200)), 2))) * (Mathf.Pow((x - b), 2) + Mathf.Pow((z - a), 2))));
                if (point > 0.005f)
                    if (heights[a, b] > 0.01f)
                        heights[a, b] = Mathf.Max(heights[a, b] , point);
                    else
                        heights[a, b] = point;
            }
        }

    }

    //smooths the map and updates it
    private void Smooth()
    {

        float k = 0.5f;
        /* Rows, left to right */
        for (int j = 0; j < 5; j++)
        {
            for (int x = 1; x < terr.terrainData.heightmapWidth; x++)
                for (int z = 0; z < terr.terrainData.heightmapHeight; z++)
                    heights[x, z] = heights[x - 1, z] * (1 - k) +
                              heights[x, z] * k;

            /* Rows, right to left*/
            for (int x = terr.terrainData.heightmapWidth - 2; x < -1; x--)
                for (int z = 0; z < terr.terrainData.heightmapHeight; z++)
                    heights[x, z] = heights[x + 1, z] * (1 - k) +
                              heights[x, z] * k;

            /* Columns, bottom to top */
            for (int x = 0; x < terr.terrainData.heightmapWidth; x++)
                for (int z = 1; z < terr.terrainData.heightmapHeight; z++)
                    heights[x, z] = heights[x, z - 1] * (1 - k) +
                              heights[x, z] * k;

            /* Columns, top to bottom */
            for (int x = 0; x < terr.terrainData.heightmapWidth; x++)
                for (int z = terr.terrainData.heightmapHeight; z < -1; z--)
                    heights[x, z] = heights[x, z + 1] * (1 - k) +
                              heights[x, z] * k;
        }
        terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
    }

    private void myReset()
    {
        resetTerrain();
        float[,,] alphas = terr.terrainData.GetAlphamaps(0, 0, terr.terrainData.alphamapWidth, terr.terrainData.alphamapHeight);
        for (int i = 0; i < terr.terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terr.terrainData.alphamapHeight; j++)
            {                
                alphas[i, j, 0] = 1;
                //set old texture mask to zero
                for (int k = 1; k < 10; k++)
                {
                    alphas[i, j, k] = 0f;
                }

            }
        }
        terr.terrainData.SetAlphamaps(0, 0, alphas);
    }

    public void TerrainUpdate()
    {
        if (grabberRight.getGrabbed() != null && (grabberRight.getGrabbed() == "hillWidget" || grabberRight.getGrabbed() == "hillWidgetHeight"))
        {
            changeTerrain();
        }
    }

    public bool CheckforWidgets(Vector3 location)
    {
        bool check = true;
        GameObject [] hillWidgets  = GameObject.FindGameObjectsWithTag("hillWidgetHeight");
        foreach (GameObject hillWidget in hillWidgets)
        {
            if (Vector3.Distance(hillWidget.transform.position, location) < 0.08)
            {
                check = false;
            }
        }
        return check;
    }

}
