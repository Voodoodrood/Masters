/********************************************************************************//**
\file      ColorGrabbable.cs
\brief     Simple component that changes color based on grab state.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ColorGrabbable : OVRGrabbable
    {
        public bool inLeftHand, inRightHand;
        GameObject laserLine;
        public GameObject playerBase;
        private Vector3 playerBasePosition;
        RaycastHit hit;
        List<Vector3> points;
        //private float timer, lineTimer;
        public GameObject ball, landmark;
        GameObject hillWidge;
        public GameObject FPV;
        public CircularMenu myWidgetMenu;

        public void Update()
        {
            myWidgetMenu.GetCurrentMenuItem();
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && FPV.activeSelf)
            {
                FPV.SetActive(false);
            }
            if (inLeftHand || inRightHand)
            {
                DrawLaser(this.transform.position, this.transform.position + this.transform.forward * -50, Color.red);
                Physics.Raycast(this.transform.position, -this.transform.forward, out hit);

                if (inLeftHand && myWidgetMenu.selectedItem == 0)
                {
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        
                        points.Clear();
                        createBall(); 
                    }
                    //Draws line if trigger is down
                    if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && points.Count>0 && Vector3.Distance(hit.point, points[points.Count - 1]) > 0.05)
                    {
                        points.Add(hit.point + new Vector3(0, 0.01f, 0));
                        hillWidge.GetComponentInChildren<HillWidget>().AddPoint(hit.point + new Vector3(0, 0.01f, 0));
                        hillWidge.GetComponentInChildren<HillWidget>().DrawLine();
                    }

                    if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) && points.Count > 1 && Physics.Raycast(this.transform.position, -this.transform.forward, out hit))
                    {
                            GameObject temp = Instantiate(ball, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                            hillWidge.GetComponentInChildren<HillWidget>().addPair(temp.GetComponentInChildren<HillWidget>());
                            temp.GetComponentInChildren<HillWidget>().addPair(hillWidge.GetComponentInChildren<HillWidget>());
                            temp.GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(hit.point, Quaternion.Euler(0, 0, 0));
                            temp.GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(hit.point + new Vector3(0, 0, -0.1f), Quaternion.Euler(0, 0, 0));
                    }
                }
                else if ((inLeftHand && myWidgetMenu.selectedItem == 1))
                {
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        createLandmark();
                    }
                }
                else if ((inLeftHand && myWidgetMenu.selectedItem == 2))
                {
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)&&!FPV.activeSelf)
                    {
                        FPV.transform.position = (hit.point + new Vector3(0, 0.5f, 0));
                        FPV.SetActive(true);
                    }
                }
                else if (myWidgetMenu.selectedItem == 0)
                {
                    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        points.Clear();
                        createBall();
                    }
                    //Draws line if trigger is down
                    if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && points.Count > 0 && Vector3.Distance(hit.point, points[points.Count - 1]) > 0.05)
                    {
                        points.Add(hit.point + new Vector3(0, 0.01f, 0));
                        hillWidge.GetComponentInChildren<HillWidget>().AddPoint(hit.point + new Vector3(0, 0.01f, 0));
                        hillWidge.GetComponentInChildren<HillWidget>().DrawLine();
                    }

                    if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) && points.Count > 1 && Physics.Raycast(this.transform.position, -this.transform.forward, out hit))
                    {
                        GameObject temp = Instantiate(ball, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                        hillWidge.GetComponentInChildren<HillWidget>().addPair(temp.GetComponentInChildren<HillWidget>());
                        temp.GetComponentInChildren<HillWidget>().addPair(hillWidge.GetComponentInChildren<HillWidget>());
                        temp.GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(hit.point, Quaternion.Euler(0, 0, 0));
                        temp.GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(hit.point + new Vector3(0, 0, -0.1f), Quaternion.Euler(0, 0, 0));
                    }
                }
                else if ((inRightHand && myWidgetMenu.selectedItem == 1))
                {
                    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        createLandmark();
                    }
                }
                else if ((inRightHand && myWidgetMenu.selectedItem == 2))
                {
                    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && !FPV.activeSelf)
                    {
                        FPV.transform.position = (hit.point + new Vector3(0, 0.5f, 0));
                        FPV.SetActive(true);
                    }
                }
            }
            else
            {
                this.GetComponentInParent<Transform>().position = this.GetComponentInParent<Transform>().position + (playerBase.transform.position- playerBasePosition);
            }

            playerBasePosition = playerBase.transform.position;

        }

        public void resetPosition()
        {
            this.GetComponentInParent<Transform>().position = new Vector3(0.05f, -0.15f, 0.3f) + (playerBase.transform.position);
            this.GetComponentInParent<Transform>().localRotation = Quaternion.Euler(-125, 5, 18);
        }

        public void createBall()
        {
            if (Physics.Raycast(this.transform.position, -this.transform.forward, out hit) && CheckforWidgets(hit.point))
            {
                hillWidge = Instantiate(ball, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                hillWidge.GetComponentsInChildren<Transform>()[1].SetPositionAndRotation(hit.point + new Vector3(0, 0.01f, 0), Quaternion.Euler(0, 0, 0));
                hillWidge.GetComponentsInChildren<Transform>()[2].SetPositionAndRotation(hit.point + new Vector3(0, 0.02f, -0.1f), Quaternion.Euler(-90, 0, 0));
                hillWidge.GetComponentsInChildren<Transform>()[3].SetPositionAndRotation(hit.point + new Vector3(0, 0.01f, -0.053f), Quaternion.Euler(-90, 0, 0));
                points.Add(hit.point + new Vector3(0, 0.01f, 0));
                hillWidge.GetComponentInChildren<HillWidget>().AddPoint(hit.point + new Vector3(0, 0.01f, 0));
                Debug.Log(hit.point + new Vector3(0, 0.01f, 0));
                //hillWidge.GetComponentInChildren<HillWidget>().SetPosition(false);
            }
        }
        public void createLandmark()
        {
            if (Physics.Raycast(this.transform.position, -this.transform.forward, out hit) && CheckforWidgets(hit.point))
            {
                hillWidge = Instantiate(landmark, hit.point + new Vector3(0, 0.01f, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
            }
        }

        public bool CheckforWidgets(Vector3 location)
        {
            bool check = true;
            GameObject[] hillWidgets = GameObject.FindGameObjectsWithTag("hillWidgetHeight");
            foreach (GameObject hillWidget in hillWidgets)
            {
                if (Vector3.Distance(hillWidget.transform.position, location) < 0.08)
                {
                    check = false;
                }
            }
            return check;
        }
        

        void DrawLaser(Vector3 start, Vector3 end, Color color, float duration = 0.02f)
        {
            laserLine = new GameObject();
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

        override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            if (hand.tag == "RightHand")
                inRightHand = true;
            else
                inLeftHand = true;
            base.GrabBegin(hand, grabPoint);
        }

        override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            if (inRightHand)
                inRightHand = false;
            else
                inLeftHand = false;
        }

        protected override void Awake()
        {
            base.Awake();
            playerBasePosition = new Vector3(playerBase.transform.position.x, playerBase.transform.position.y, playerBase.transform.position.z);
            inLeftHand = false;
            inRightHand = false;
            points = new List<Vector3>();
        }
    }
