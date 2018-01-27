using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public Terrain terr;
    List<GameObject> lines;
    int xRes;
    int yRes;
    public float[,] heights;
    float[,,] element;
    int mapX, mapY;
    float[,,] map;
    ReverseGrab grabberRight, grabberLeft;
    public GameObject rightTracker, leftTracker, terrainGrip, terrainVisGrip;
    List<Vector3> initWidgets = new List<Vector3>();
    List<Vector3> initWidgetExtras = new List<Vector3>();
    float scale;
    float scaleChange = 0;

    // Use this for initialization
    void Start()
    {
        xRes = terr.terrainData.heightmapWidth;
        yRes = terr.terrainData.heightmapHeight;
        map = new float[terr.terrainData.alphamapWidth, terr.terrainData.alphamapHeight, terr.terrainData.alphamapLayers];
        element = new float[1, 1, terr.terrainData.alphamapLayers];
        myReset();
        terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
        grabberRight = GameObject.Find("AvatarGrabberRight").GetComponent<ReverseGrab>();
        grabberLeft = GameObject.Find("AvatarGrabberLeft").GetComponent<ReverseGrab>();
        InvokeRepeating("TerrainUpdate", 2.0f, 0.05f);
        scale = 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        // = GameObject.FindGameObjectsWithTag("hillWidgetHeight");

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)&& OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            scaleChange = Vector3.Distance(rightTracker.transform.position, leftTracker.transform.position);
            initWidgets.Clear();
            initWidgetExtras.Clear();
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("hillWidgetHeight"))
            {
                initWidgets.Add(new Vector3 (temp.transform.localPosition.x/(scale*2), temp.transform.localPosition.y, temp.transform.localPosition.z/(scale * 2)));
            }
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("hillWidget"))
            {
                initWidgetExtras.Add(new Vector3(temp.transform.localPosition.x / (scale * 2), temp.transform.localPosition.y, temp.transform.localPosition.z / (scale * 2)));
            }

        }
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            scale += (Vector3.Distance(rightTracker.transform.position, leftTracker.transform.position) - scaleChange);
            scaleChange = Vector3.Distance(rightTracker.transform.position, leftTracker.transform.position);
            terr.terrainData.size = new Vector3(scale*2, 1, scale*2);
            terr.transform.position = new Vector3(-scale,0.28f,-scale);
            terrainGrip.transform.localScale = new Vector3(scale * 2+0.05f, 0.001f, scale * 2 + 0.05f);
            terrainVisGrip.transform.localScale = new Vector3(scale * 2 + 0.05f, 0.001f, scale * 2 + 0.05f);
            GameObject[] currentWidgets = GameObject.FindGameObjectsWithTag("hillWidgetHeight");
            for (int i = 0; i < currentWidgets.Length; i++)
            {
                currentWidgets[i].transform.localPosition = new Vector3(initWidgets[i].x*(scale*2), initWidgets[i].y, initWidgets[i].z *(scale*2));
            }
            GameObject[] currentWidgetExtras = GameObject.FindGameObjectsWithTag("hillWidget");
            for (int i = 0; i < currentWidgets.Length; i++)
            {
                currentWidgetExtras[i].transform.localPosition = new Vector3(initWidgetExtras[i].x * (scale * 2), initWidgetExtras[i].y, initWidgetExtras[i].z * (scale * 2));
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

    //changes the terrain at the location of a collider
    public void changeTerrain()
    {
        resetTerrain();
        GameObject [] hillWidgets;
        hillWidgets = GameObject.FindGameObjectsWithTag("hillWidgetContainer");
        foreach (GameObject hillWidget in hillWidgets)
        {
            HillWidget current = hillWidget.GetComponent<HillWidget>();
            /*if (grabberRight.getGrabbed() != null && (grabberRight.getGrabbed() == "hillWidgetHeight"))
                current.SetPosition(true);
            else
                current.SetPosition(false);*/
            current.SetWidth();
            float xPos = current.GetPosition().x;
            float zPos = current.GetPosition().z;
            float yPos = current.GetPosition().y;
            float width = current.GetWidth();

            CreateHill(xPos, zPos, yPos, width, current.getOffset(), current.getSteepness());
            if (current.getPair() != null)
            {
                if (current.GetPoints() != null)
                {
                    for (int i =0; i<current.GetPoints().Count;i++)
                    {
                        CreateRidge(convert(current.GetPoints()[i].x), convert(current.GetPoints()[i].z), (current.GetPosition().y*(1-((float)i / current.GetPoints().Count)) + current.getPair().GetPosition().y * (((float)i / current.GetPoints().Count))), width);
                    }
                }
            }
            
        }
        foreach (GameObject hillWidget in hillWidgets)
        {
            HillWidget current = hillWidget.GetComponent<HillWidget>();
            current.setHeight(heights[(int)(((current.GetPosition().z / scale) + 1) * 256), (int)(((current.GetPosition().x / scale) + 1) * 256)]);
            if (current.getPair() != null)
            {
                if (current.GetPoints() != null)
                {
                    for (int i = 0; i < current.GetPoints().Count; i++)
                    {
                        current.ChangePoint(heights[(int)(((current.GetPoints()[i].z / scale) + 1) * 256), (int)(((current.GetPoints()[i].x / scale) + 1) * 256)], i);
                    }
                }
            }
            current.DrawLine();
        }
            //Smooth();
            terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
        //Smooth();
    }

    public float convert(float input)
    {
        return (input + (terr.terrainData.size.x/2.0f)) * (513/ terr.terrainData.size.x);
    }
    

    //creates a hill using
    public void CreateHill(float x, float z, float y, float area, Vector2 offset, float steepness)
    {
        //compute the line along which to change steepness
        float angle;
        if (Mathf.Abs(z + offset.y) > 0.001 && Mathf.Abs(x + offset.x) > 0.001)
            angle = -((x + offset.x) / (z + offset.y));
        else if (Mathf.Abs(z - offset.y) > 0.001)
            angle = 0;
        else if (Mathf.Abs(x - offset.x) > 0.001)
            angle = 1000;
        else
            angle = 0;
        angle = Mathf.Min(angle, 1000);    

        //Check if we are making the terrain above or below the line steeper
        bool above;
        float k = z - angle * x;
        if (offset.y < angle* offset.x + k)
            above = true;
        else
            above = false;

        x = convert(x) + offset.x;
        z = convert(z) + offset.y;

        k = z - angle * x;

        float point = 0;
        steepness += 2.1f;

        for (int a = (int)Mathf.Max(0,z-(area*(-600))); a < (int)Mathf.Min(513, z + (area * (-600))); a++)
        {
            for (int b = (int)Mathf.Max(0, x - (area * (-600))); b < (int)Mathf.Min(513, x + (area * (-600))); b++)
            {
                if (a > angle*b+k && above)
                {
                    point = (y+0.1f) * Mathf.Pow((float)Math.E, -(float)((1 / (double)(2 * Math.Pow((area * (200)), 2))) * (Mathf.Pow(Math.Abs((x) - b), steepness) + Mathf.Pow(Math.Abs((z) - a), steepness))));
                }                    
                else if (a < angle * b + k && !above)
                    point = (y + 0.1f) * Mathf.Pow((float)Math.E, -(float)((1 / (double)(2 * Math.Pow((area * (200)), 2))) * (Mathf.Pow(Math.Abs(x - b), steepness) + Mathf.Pow(Math.Abs(z - a), steepness))));
                else
                {
                    point = (y + 0.1f) * Mathf.Pow((float)Math.E, -(float)((1 / (double)(2 * Math.Pow((area * (200)), 2))) * (Mathf.Pow((x - b), 2) + Mathf.Pow((z - a), 2))));
                }

                if (point > 0.005f)
                    if (heights[a, b] > 0.01f)
                        heights[a, b] = Mathf.Max(heights[a, b] , point);
                    else
                        heights[a, b] = point;
            }
        }

    }
    public void CreateRidge(float x, float z, float y, float area)
    {
        float point;
        for (int a = (int)Mathf.Max(0, z - (area * (-600))); a < (int)Mathf.Min(513, z + (area * (-600))); a++)
        {
            for (int b = (int)Mathf.Max(0, x - (area * (-600))); b < (int)Mathf.Min(513, x + (area * (-600))); b++)
            {
                point = (y + 0.1f) * Mathf.Pow((float)Math.E, -(float)((1 / (double)(2 * Math.Pow((area * (200)), 2))) * (Mathf.Pow((x - b), 2) + Mathf.Pow((z - a), 2))));

                if (point > 0.005f) { }
                    if (heights[a, b] > 0.01f)
                        heights[a, b] = Mathf.Max(heights[a, b], point);
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
        //terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
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
        if ((grabberRight.getGrabbed() != null && (grabberRight.getGrabbed() == "hillWidget" || grabberRight.getGrabbed() == "hillWidgetHeight"))|| (grabberLeft.getGrabbed() != null && (grabberLeft.getGrabbed() == "hillWidget" || grabberLeft.getGrabbed() == "hillWidgetHeight")))
        {
            changeTerrain();
        }
    }

}
