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

        
		
	}

    public void addPair(HillWidget other)
    {
        widgetPair = other;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
    public float GetWidth()
    {
        return width;
    }

    public void SetPosition()
    {
        position = GetComponentsInChildren<Transform>()[1].localPosition;
        GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(GetComponentsInChildren<Transform>()[2].localPosition + new Vector3(0, GetComponentsInChildren<Transform>()[1].localPosition.y - GetComponentsInChildren<Transform>()[2].localPosition.y, 0), GetComponentsInChildren<Transform>()[2].rotation);
        position.x = (position.x + 0.5f) * 513;
        position.z = (position.z + 0.5f) * 513;
        position.y = position.y -0.28f;
    }

    public void SetWidth()
    {
        width = -GetComponentsInChildren<Transform>()[1].localPosition.z+GetComponentsInChildren<Transform>()[2].localPosition.z ;
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
}
