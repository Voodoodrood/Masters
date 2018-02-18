using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillWidget : MonoBehaviour
{

    static float baseHeight = 0.8f;
    float width;
    Vector3 position;
    List<Vector3> attachedPoints;
    InputHandler handler;
    HillWidget widgetPair;
    GameObject line;
    public Material matt;

    // Use this for initialization
    void Awake()
    {
        position = new Vector3(0,0,0);
        width = 0.1f;
        handler = GameObject.Find("InputHandler").GetComponent<InputHandler>();
        //Debug.Log(GetComponentsInChildren<Transform>()[1].localPosition);
        line = new GameObject();        
    }

    // Update is called once per frame
    void Update () {
        //sets minimum widget width
        //if (GetComponentsInChildren<Transform>()[1].localPosition.z - GetComponentsInChildren<Transform>()[2].localPosition.z < .1f)
        //GetComponentsInChildren<Transform>()[2].localPosition = (GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, 0, (GetComponentsInChildren<Transform>()[1].localPosition.z - 0.1f) - GetComponentsInChildren<Transform>()[2].localPosition.z));
        DrawLine();
        
        if (!GetComponentsInChildren<OVRGrabbable>()[0].isGrabbed && !GetComponentsInChildren<OVRGrabbable>()[1].isGrabbed)
        {
            //SetPosition(false);
        }
        GetComponentsInChildren<Transform>()[3].localScale = new Vector3(0.02f, (-Vector3.Distance(GetComponentsInChildren<Transform>()[1].localPosition, GetComponentsInChildren<Transform>()[2].localPosition)) *.5f + 0.02f, 0.02f);
        GetComponentsInChildren<Transform>()[3].localPosition = (GetComponentsInChildren<Transform>()[2].localPosition + GetComponentsInChildren<Transform>()[1].localPosition)/2 + new Vector3(0,0,-.003f);
        GetComponentsInChildren<Transform>()[3].LookAt(GetComponentsInChildren<Transform>()[2].localPosition);
        GetComponentsInChildren<Transform>()[3].localEulerAngles = new Vector3(GetComponentsInChildren<Transform>()[3].localEulerAngles.x- 90, GetComponentsInChildren<Transform>()[3].localEulerAngles.y, GetComponentsInChildren<Transform>()[3].localEulerAngles.z);
    }

    public void addPair(HillWidget other)
    {
        widgetPair = other;
    }

    public Vector3 GetPosition()
    {
        return position + new Vector3(0,-0.1f,0);
    }

    public Vector2 getOffset()
    {
        Vector2 temp = new Vector2(GetComponentsInChildren<Transform>()[1].localPosition.x - GetComponentsInChildren<Transform>()[2].localPosition.x, GetComponentsInChildren<Transform>()[1].localPosition.z - GetComponentsInChildren<Transform>()[2].localPosition.z);
        temp.Normalize();
        temp = temp * 10f;
        return temp;
    }

    public float GetWidth()
    {
        return width;
    }

    public void SetPosition(bool move)
    {
        if (move)
        {
            float verticalChange = GetComponentsInChildren<Transform>()[1].localPosition.y - position.y - 0.29f;
            position.x = GetComponentsInChildren<Transform>()[1].localPosition.x;
            position.z = GetComponentsInChildren<Transform>()[1].localPosition.z;
            position.y += verticalChange;
            GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, verticalChange, 0), GetComponentsInChildren<Transform>()[2].rotation);
        }
        else
        {
            position.x = GetComponentsInChildren<Transform>()[1].localPosition.x;
            position.z = GetComponentsInChildren<Transform>()[1].localPosition.z;
            if (Mathf.Abs((GetComponentsInChildren<Transform>()[1].localPosition.y - 0.29f)-position.y)>0.01)
                position.y = GetComponentsInChildren<Transform>()[1].localPosition.y - 0.29f;
        }

    }

    public void setHeight(float height)//sets the height of constraints to, at minimum, the terrain height
    {
        
        float vertChange = GetComponentsInChildren<Transform>()[2].localPosition.y - GetComponentsInChildren<Transform>()[1].localPosition.y;
        if (!GetComponentsInChildren<OVRGrabbable>()[0].isGrabbed && !GetComponentsInChildren<OVRGrabbable>()[1].isGrabbed)
        {
            GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(new Vector3(GetComponentsInChildren<Transform>()[1].localPosition.x, height + 0.29f, GetComponentsInChildren<Transform>()[1].localPosition.z), GetComponentsInChildren<Transform>()[2].rotation);
            GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(new Vector3(GetComponentsInChildren<Transform>()[2].localPosition.x, GetComponentsInChildren<Transform>()[1].localPosition.y + vertChange, GetComponentsInChildren<Transform>()[2].localPosition.z), GetComponentsInChildren<Transform>()[2].rotation);
        }
    }

    public void SetWidth()
    {
        width = -Mathf.Pow((Mathf.Pow(GetComponentsInChildren<Transform>()[1].position.x-GetComponentsInChildren<Transform>()[2].position.x,2)+ Mathf.Pow(GetComponentsInChildren<Transform>()[1].position.z - GetComponentsInChildren<Transform>()[2].position.z, 2)),0.5f);
    }

    public void AddPoint (Vector3 point)
    {
        if (attachedPoints == null)
            attachedPoints = new List<Vector3>();
        attachedPoints.Add(new Vector3(point.x/(handler.scale), point.y, point.z/(handler.scale)));
    }

    public void ChangePoint()
    {
        RaycastHit hit;
        //DrawLaser(new Vector3(attachedPoints[point].x* (handler.scale), 3, attachedPoints[point].z*(handler.scale)), new Vector3(attachedPoints[point].x * (handler.scale), 0, attachedPoints[point].z * (handler.scale)), Color.red);
        for (int i = 0; i < attachedPoints.Count; i++)
        {
            Physics.Raycast(new Vector3(attachedPoints[i].x * (handler.scale), 4, attachedPoints[i].z * (handler.scale)), Vector3.down, out hit);
            attachedPoints[i] = new Vector3(attachedPoints[i].x, hit.point.y, attachedPoints[i].z) + new Vector3(0, 0.005f * handler.scale, 0);
        }
    }
    void DrawLaser(Vector3 start, Vector3 end, Color color, float duration = 5f)
    {
        GameObject laserLine = new GameObject();
        laserLine.transform.position = start;
        laserLine.AddComponent<LineRenderer>();
        LineRenderer lr = laserLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = (color);
        lr.endColor = (color);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(laserLine, duration);
    }

    public List< Vector3> GetPoints()
    {
        if (attachedPoints != null)
            return attachedPoints;
        else
            return null;
    }

    public HillWidget getPair()
    {
        if (widgetPair != null)
            return widgetPair;
        else
            return null;
    }

    public float getSteepness()
    {
        return (GetComponentsInChildren<Transform>()[1].localPosition.y - GetComponentsInChildren<Transform>()[2].localPosition.y);
    }

    public void DrawLine()
    {
        if (attachedPoints != null && attachedPoints.Count>1)
        {
            if (line.GetComponent<LineRenderer>() == null)
            {
                line.AddComponent<LineRenderer>();
            }
            line.transform.position = GetComponentsInChildren<Transform>()[1].localPosition;
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.material = matt;
            lr.startColor = Color.green;
            lr.endColor = Color.green;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.positionCount = attachedPoints.Count;
            Vector3[] temp = attachedPoints.ToArray();
            for (int i = 0;i<temp.Length;i++)
            {
                temp[i] = new Vector3(temp[i].x * handler.scale, temp[i].y, temp[i].z * handler.scale);
            }
            lr.SetPositions(temp);
        } 
    }
    public void convertAttachedPoints(float scale)
    {
        for (int i =0; i < attachedPoints.Count; i++)
        {
            //Debug.Log(i);
            attachedPoints[i] = new Vector3(attachedPoints[i].x*scale, attachedPoints[i].y*scale, attachedPoints[i].z*scale);
        }
    }
    public void saveAttachedPoints(float scale)
    {
        for (int i = 0; i < attachedPoints.Count; i++)
        {
            attachedPoints[i] = new Vector3(attachedPoints[i].x / scale, attachedPoints[i].y / scale, attachedPoints[i].z / scale);
        }
    }
}
