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

	// Use this for initialization
	void Awake()
    {
        position = new Vector3(0,0,0);
        width = 0.1f;
        handler = GameObject.Find("InputHandler").GetComponent<InputHandler>();
        
    }

    /*override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        handler.changeTerrain(this.GetComponent<Collider>(), 0);
    }*/

    // Update is called once per frame
    void Update () {
        //sets minimum widget width
        if (GetComponentsInChildren<Transform>()[1].localPosition.z - GetComponentsInChildren<Transform>()[2].localPosition.z < .1f)
            //GetComponentsInChildren<Transform>()[2].localPosition = (GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, 0, (GetComponentsInChildren<Transform>()[1].localPosition.z - 0.1f) - GetComponentsInChildren<Transform>()[2].localPosition.z));

        GetComponentsInChildren<Transform>()[3].localScale = new Vector3(0.02f, (-Vector3.Distance(GetComponentsInChildren<Transform>()[1].localPosition, GetComponentsInChildren<Transform>()[2].localPosition)) *.5f + 0.02f, 0.02f);
        GetComponentsInChildren<Transform>()[3].localPosition = (GetComponentsInChildren<Transform>()[2].localPosition + GetComponentsInChildren<Transform>()[1].localPosition)/2 + new Vector3(0,0,-.003f);
        GetComponentsInChildren<Transform>()[3].localEulerAngles = new Vector3(-90,0,Vector3.Angle(GetComponentsInChildren<Transform>()[1].localPosition, GetComponentsInChildren<Transform>()[2].localPosition));
    }

    public void addPair(HillWidget other)
    {
        widgetPair = other;
    }

    public Vector3 GetPosition()
    {
        return position + new Vector3(0,-0.02f,0);
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

    public void SetPosition(bool height)
    {
        /*Scale here*/
        position = GetComponentsInChildren<Transform>()[1].localPosition;
        if (height)
            GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, GetComponentsInChildren<Transform>()[1].localPosition.y - GetComponentsInChildren<Transform>()[2].localPosition.y, 0), GetComponentsInChildren<Transform>()[2].rotation);
        position.y = position.y -0.28f;
    }

    public void SetWidth()
    {
        width = -Vector3.Distance(GetComponentsInChildren<Transform>()[1].localPosition,GetComponentsInChildren<Transform>()[2].localPosition);
    }

    public void AddPoint (Vector3 point)
    {
        if (attachedPoints == null)
            attachedPoints = new List<Vector3>();
        attachedPoints.Add(point);
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
}
