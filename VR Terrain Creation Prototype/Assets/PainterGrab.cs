/********************************************************************************//**
\file      ColorGrabbable.cs
\brief     Simple component that changes color based on grab state.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;


    public class PainterGrab : OVRGrabbable
    {

        public bool inLeftHand, inRightHand;
        GameObject laserLine;
        RaycastHit hit;
        public TerrainData terrData;
        public Terrain terr;
        public CircularMenu myColorMenu;
        public GameObject projector;
        private float aoe, height;
        public GameObject playerBase;
        private Vector3 playerBasePosition;


        public void Update()
        {
            
            if (inLeftHand || inRightHand)
            {
                DrawLaser(this.transform.position, this.transform.position + this.transform.forward * -50, Color.red);
                Physics.Raycast(this.transform.position, -this.transform.forward, out hit);
                DrawPreview(hit.point);
            }
            else
            {
                this.GetComponentInParent<Transform>().position = this.GetComponentInParent<Transform>().position + (playerBase.transform.position - playerBasePosition);
            }

            if (inLeftHand)
            {
                Vector2 stick2Pos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                    UpdateTerrainTexture(terrData, hit.point.x, hit.point.z);
                if (stick2Pos.y > 0.5f)//Scale here
                {
                    aoe = Mathf.Min(aoe + 2, 150);
                    height = Mathf.Min(height + 0.3f, 22.5f);
                }
                else if ((stick2Pos.y < -0.5f))
                {
                    aoe = Mathf.Max(aoe - 2, 10);
                    height = Mathf.Max(height - 0.3f, 1.5f);
                }
            }
            else if (inRightHand)
            {
                Vector2 stick2Pos = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                    UpdateTerrainTexture(terrData, hit.point.x, hit.point.z);
                if (stick2Pos.y > 0.5f)
                {
                    aoe = Mathf.Min(aoe + 1, 150);
                    height = Mathf.Min(height + 0.15f, 22.5f);
                }
                else if ((stick2Pos.y < -0.5f))
                {
                    aoe = Mathf.Max(aoe - 1, 10);
                    height = Mathf.Max(height - 0.15f, 1.5f);
                }
            }
            playerBasePosition = playerBase.transform.position;
        }
        public void resetPosition()
        {
            this.GetComponentInParent<Transform>().position = new Vector3(0.13f, -0.15f, 0.3f) + (playerBase.transform.position);
            this.GetComponentInParent<Transform>().localRotation = Quaternion.Euler(-125,5,18);
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

        void DrawPreview(Vector3 point)
        {
            projector.transform.position = new Vector3(point.x, point.y+height, point.z);
        }

        private void UpdateTerrainTexture(TerrainData terrainData, float x, float y)
        {
            
            x = (x + (terrainData.size.x/2.0f)) * (512/ terrainData.size.x);
            y = (y + (terrainData.size.z/2.0f)) * (512/ terrainData.size.z);
            int textureTo = myColorMenu.selectedItem;

            //get current paint mask
            float[,,] alphas = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            
            // make sure every grid on the terrain is modified
            for (int i = Mathf.Max(0,(int)Mathf.Floor(y-aoe)); i < Mathf.Min(Mathf.Ceil(y + aoe), terrainData.alphamapWidth); i++)
            {
                for (int j = Mathf.Max(0, (int)Mathf.Floor(x - aoe)); j < Mathf.Min(Mathf.Ceil(x + aoe), terrainData.alphamapWidth); j++)
                {
                    if (Mathf.Pow(j - x, 2) + Mathf.Pow(i - y, 2) < Mathf.Pow((aoe/ terrainData.size.x), 2))
                    {
                        alphas[i, j, textureTo] = 1;
                        //set old texture masks to zero
                        for (int k = 0; k<10;k++)
                        {
                            if(k != textureTo)
                                alphas[i, j, k] = 0f;
                        }
                    }

                }
            }
            // apply the new alpha
            terrainData.SetAlphamaps(0, 0, alphas);
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

        override protected void Awake()
        {
            base.Awake();
            playerBasePosition = new Vector3(playerBase.transform.position.x, playerBase.transform.position.y, playerBase.transform.position.z);
            inLeftHand = false;
            inRightHand = false;
            terrData = terr.terrainData;
            aoe = 20;
            height = 3f;
        }
    }
