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
        Debug.Log(GetComponentsInChildren<Transform>()[1].localPosition);
        line = new GameObject();
        
    }

    /*override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        handler.changeTerrain(this.GetComponent<Collider>(), 0);
    }*/

    // Update is called once per frame
    void Update () {
        //sets minimum widget width
        //if (GetComponentsInChildren<Transform>()[1].localPosition.z - GetComponentsInChildren<Transform>()[2].localPosition.z < .1f)
        //GetComponentsInChildren<Transform>()[2].localPosition = (GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, 0, (GetComponentsInChildren<Transform>()[1].localPosition.z - 0.1f) - GetComponentsInChildren<Transform>()[2].localPosition.z));
        //DrawLine();
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
        /*Scale here*/
       
        float verticalChange = GetComponentsInChildren<Transform>()[1].localPosition.y - position.y - 0.28f;
        position.x = GetComponentsInChildren<Transform>()[1].localPosition.x;
        position.z = GetComponentsInChildren<Transform>()[1].localPosition.z;
        position.y += verticalChange;
        if (move)
            GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, verticalChange, 0), GetComponentsInChildren<Transform>()[2].rotation);
        
    }

    public void setHeight(float height)
    {
        
        float vertChange = GetComponentsInChildren<Transform>()[2].localPosition.y - GetComponentsInChildren<Transform>()[1].localPosition.y;
        if (!GetComponentsInChildren<OVRGrabbable>()[0].isGrabbed && !GetComponentsInChildren<OVRGrabbable>()[1].isGrabbed)
        {
            GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(new Vector3(GetComponentsInChildren<Transform>()[1].localPosition.x, height + 0.28f, GetComponentsInChildren<Transform>()[1].localPosition.z), GetComponentsInChildren<Transform>()[2].rotation);
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
        attachedPoints.Add(point);
    }

    public void ChangePoint(float height, int point)
    {
        attachedPoints[point] = new Vector3(attachedPoints[point].x, height + 0.29f, attachedPoints[point].z);
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
            lr.SetPositions(attachedPoints.ToArray());
        } 
    }
}
