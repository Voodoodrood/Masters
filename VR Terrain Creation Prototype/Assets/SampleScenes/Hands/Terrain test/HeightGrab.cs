using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OVRTouchSample
{
    public class HeightGrab : OVRGrabbable
    {
        public InputHandler inputty;
        
        override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            base.GrabBegin(hand, grabPoint);
        }

        override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            //inputty.changeTerrain(this.GetComponent<Collider>(), 0);            
        }

        void Awake()
        {
            if (m_grabPoints.Length == 0)
            {
                // Get the collider from the grabbable
                Collider collider = this.GetComponent<Collider>();
                if (collider == null)
                {
                    throw new System.ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
                }
                // Create a default grab point
                m_grabPoints = new Collider[1] { collider };
            }
        }
    }
}
