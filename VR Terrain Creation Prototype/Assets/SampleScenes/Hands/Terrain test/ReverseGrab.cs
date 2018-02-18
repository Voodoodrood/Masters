using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseGrab : OVRGrabber
{
    public Transform cam;
    public Transform [] staticComp;
    Vector3 camPos, camAngle;
    Vector3 [] staticPos, staticAngle;
    public GameObject handLeft, handRight, projector;
    private bool grabbed;
    //public Transform temp;
    protected override void OnUpdatedAnchors()
    {
        if (m_grabbedObj != null && m_grabbedObj.tag == "terrain")
        {
            cam.localPosition = camPos;
            cam.localEulerAngles = camAngle;
            for (int i = 0; i < staticComp.Length; i++)
            {
                staticComp[i].localPosition = staticPos[i];
                staticComp[i].localEulerAngles = staticAngle[i];
            }
        }
        base.OnUpdatedAnchors();
    }
    override protected void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
      {
            //pos is destination position in world space
           
            if (m_grabbedObj == null)
            {
                return;
            }

            if (m_grabbedObj.tag == "terrain")
            {
                Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
                Vector3 grabbablePosition = pos; // sets target position to move object too, m_grabbedObjectPosOff is simply the objects offset to the controller when originally grabbed
                Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;
                if (forceTeleport)
                {
                    grabbedRigidbody.transform.position = grabbablePosition;
                    grabbedRigidbody.transform.rotation = grabbableRotation;
                }
                else
                {
                    cam.localPosition += (m_grabbedObj.transform.position - grabbablePosition);
                    cam.RotateAround(m_grabbedObj.transform.position, new Vector3(-1, 0, 0), grabbableRotation.eulerAngles.x);
                    cam.RotateAround(m_grabbedObj.transform.position, new Vector3(0, -1, 0), grabbableRotation.eulerAngles.y);
                    cam.RotateAround(m_grabbedObj.transform.position, new Vector3(0, 0, -1), grabbableRotation.eulerAngles.z);
                    for (int i = 0; i < staticComp.Length; i++)
                    {
                        staticComp[i].localPosition += (m_grabbedObj.transform.position - grabbablePosition);
                        staticComp[i].RotateAround(m_grabbedObj.transform.position, new Vector3(-1, 0, 0), grabbableRotation.eulerAngles.x);
                        staticComp[i].RotateAround(m_grabbedObj.transform.position, new Vector3(0, -1, 0), grabbableRotation.eulerAngles.y);
                        staticComp[i].RotateAround(m_grabbedObj.transform.position, new Vector3(0, 0, -1), grabbableRotation.eulerAngles.z);
                    }
                }
            }
            else
            {
                
                base.MoveGrabbedObject(pos, rot,forceTeleport);
                if (m_grabbedObj != null && m_grabbedObj.tag == "hillWidgetHeight")
                {
                m_grabbedObj.GetComponentInParent<HillWidget>().SetPosition(true);
                }
        }
           
      }
    protected override void GrabBegin()
    {

        
        
        camPos = new Vector3(cam.localPosition.x, cam.localPosition.y, cam.localPosition.z);
        //Debug.Log("X: " + cam.localPosition.x + " Y: "+ cam.localPosition.y+ " Z: "+ cam.localPosition.z);
        camAngle = new Vector3(cam.localEulerAngles.x, cam.localEulerAngles.y, cam.localEulerAngles.z);
        staticPos = new Vector3[staticComp.Length];
        staticAngle = new Vector3[staticComp.Length];
        for (int i = 0; i < staticComp.Length; i++)
        {
            staticPos[i] = new Vector3(staticComp[i].localPosition.x, staticComp[i].localPosition.y, staticComp[i].localPosition.z);
            staticAngle[i] = new Vector3(staticComp[i].localEulerAngles.x, staticComp[i].localEulerAngles.y, staticComp[i].localEulerAngles.z);
        }
        base.GrabBegin();
        if (m_grabbedObj != null && m_grabbedObj.tag == "PaintBrush")
        {
            projector.SetActive(true);
            //Debug.Log("Snapps1");
        }
    }

    protected override void GrabEnd()
    {
        //Debug.Log("Snapps");
        if (m_grabbedObj != null && m_grabbedObj.tag == "PaintBrush")
        {
            projector.SetActive(false);
            //Debug.Log("Snapps2");
        }
        if (m_grabbedObj != null && m_grabbedObj.tag == "terrain")
        {
            m_grabbedObj.transform.position = new Vector3(0, 0.29f, 0);
        }
        
            
        base.GrabEnd();
        grabbed = false;
        
    }

    public string getGrabbed()
    {
        if (m_grabbedObj != null)
        {
            return m_grabbedObj.tag;
        }
        else
        {
            return null;
        }
    }
}
