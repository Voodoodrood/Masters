using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseGrab : OVRGrabber
{
    public Transform cam, staticComp;
    Vector3 camPos, staticPos, camAngle, staticAngle;
    public GameObject handLeft, handRight;
    //public Transform temp;
    protected override void OnUpdatedAnchors()
    {
        if (m_grabbedObj != null && m_grabbedObj.tag == "terrain")
        {
            cam.localPosition = camPos;
            cam.localEulerAngles = camAngle;
            base.OnUpdatedAnchors();
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
                Debug.Log("terrain found");
                if (forceTeleport)
                {
                    grabbedRigidbody.transform.position = grabbablePosition;
                    grabbedRigidbody.transform.rotation = grabbableRotation;
                }
                else
                {
                    
                    //staticComp.localPosition = staticPos;
                    //staticComp.localEulerAngles = staticAngle;
                    cam.localPosition += (m_grabbedObj.transform.position - grabbablePosition);
                    cam.RotateAround(m_grabbedObj.transform.position, new Vector3(-1, 0, 0), grabbableRotation.eulerAngles.x);
                    cam.RotateAround(m_grabbedObj.transform.position, new Vector3(0, -1, 0), grabbableRotation.eulerAngles.y);
                    cam.RotateAround(m_grabbedObj.transform.position, new Vector3(0, 0, -1), grabbableRotation.eulerAngles.z);
                    /*staticComp.localPosition += (m_grabbedObj.transform.position - grabbablePosition);
                    staticComp.RotateAround(m_grabbedObj.transform.position, new Vector3(-1, 0, 0), grabbableRotation.eulerAngles.x);
                    staticComp.RotateAround(m_grabbedObj.transform.position, new Vector3(0, -1, 0), grabbableRotation.eulerAngles.y);
                    staticComp.RotateAround(m_grabbedObj.transform.position, new Vector3(0, 0, -1), grabbableRotation.eulerAngles.z);*/

                    //grabbedRigidbody.MoveRotation(grabbableRotation);
                }
            }
            else
            {
                
                base.MoveGrabbedObject(pos, rot,forceTeleport);
            }
           
      }
    protected override void GrabBegin()
    {
        base.GrabBegin();
        camPos = new Vector3(cam.localPosition.x, cam.localPosition.y, cam.localPosition.z);
        camAngle = new Vector3(cam.localEulerAngles.x, cam.localEulerAngles.y, cam.localEulerAngles.z);
        staticPos = new Vector3(staticComp.localPosition.x, staticComp.localPosition.y, staticComp.localPosition.z);
        staticAngle = new Vector3(staticComp.localEulerAngles.x, staticComp.localEulerAngles.y, staticComp.localEulerAngles.z);
        handRight.SetActive(false);
        handLeft.SetActive(false);

    }

    protected override void GrabEnd()
    {
        base.GrabEnd();
        handRight.SetActive(true);
        handLeft.SetActive(true);
    }
}
