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
    }

    // Update is called once per frame
    void Update()
    {

        
        Physics.Raycast(pointer.position, -pointer.forward, out hit);

        //creates orb at location of pointer target when trigger pulled
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
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            if (Time.time - timer > 0.1)
            {
                createBall();
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
        if (Physics.Raycast(pointer.position, -pointer.forward, out hit))
        {
            Instantiate(ball, hit.point, Quaternion.Euler(0, 0, 0));
        }
    }

    //changes the terrain at the location of a collider
    public void changeTerrain(Collider col, int type)
    {
        if (type == 0)
        {
            float yPos = (float)(col.transform.position.x + 0.5) * 513;
            float xPos = (float)(col.transform.position.z + 0.5) * 513;
            float height = col.transform.position.y + 0.8f;

            heights = terr.terrainData.GetHeights(0, 0, xRes, yRes);
            CreateHill(xPos, yPos, height - 1.08f, 3,0);
            Smooth();
        }
        else
        {
            float width = col.transform.localPosition.z;
            float yPos = (float)(col.transform.position.x + 0.5) * 513;
            float xPos = (float)(col.transform.position.z - col.transform.localPosition.z + 0.5) * 513;
            float height = col.transform.position.y + 0.8f;

            heights = terr.terrainData.GetHeights(0, 0, xRes, yRes);
            CreateHill(xPos, yPos, height - 1.08f, 3, width);
            Smooth();
        }
        
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
    public void CreateHill(float x, float y, float height, float pointyness, float area)
    {
        Debug.Log(area);
        float point = 0;
        for (int a = 0; a < 513; a++)
        {
            for (int b = 0; b < 513; b++)
            {
                point = height*Mathf.Pow((float)Math.E, -(float)((1 / (double)(2 * Math.Pow((area*(-200)), 2))) * (Mathf.Pow((y - b), 2) + Mathf.Pow((x - a), 2))));
                if (point > 0.005f)
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
                alphas[i, j, 2] = 0f;
                alphas[i, j, 1] = 0f;

            }
        }
        terr.terrainData.SetAlphamaps(0, 0, alphas);
    }

}
