/********************************************************************************//**
\file      ColorGrabbable.cs
\brief     Simple component that changes color based on grab state.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OVRTouchSample
{
    public class PainterGrab : OVRGrabbable
    {
        public static readonly Color COLOR_GRAB = new Color(1.0f, 0.5f, 0.0f, 1.0f);
        public static readonly Color COLOR_HIGHLIGHT = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        private Color m_color = Color.black;
        private MeshRenderer[] m_meshRenderers = null;
        private bool m_highlight;
        private bool inHand;
        GameObject laserLine;
        RaycastHit hit;
        public TerrainData terrData;
        public Terrain terr;

        public void Update()
        {
            
            if (inHand)
            {
                DrawLaser(this.transform.position, this.transform.position + this.transform.forward * -50, Color.red);
                Physics.Raycast(this.transform.position, -this.transform.forward, out hit);
            }

            if (inHand && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                UpdateTerrainTexture(terrData, 0, 1, hit.point.x, hit.point.z);
            }
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

        private void UpdateTerrainTexture(TerrainData terrainData, int textureNumberFrom, int textureNumberTo, float x, float y)
        {
            
            x = (x + 0.5f) * 512;
            y = (y + 0.5f) * 512;
            //get current paint mask
            float[,,] alphas = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            // make sure every grid on the terrain is modified
            for (int i = 0; i < terrainData.alphamapWidth; i++)
            {
                for (int j = 0; j < terrainData.alphamapHeight; j++)
                {
                    //for each point of mask do:
                    //paint all from old texture to new texture (saving already painted in new texture)
                    if (Mathf.Pow(j - x, 2) + Mathf.Pow(i - y, 2) < Mathf.Pow(20, 2))
                    {
                        //Debug.Log();
                        alphas[i, j, textureNumberTo] = Mathf.Max(alphas[i, j, textureNumberFrom], alphas[i, j, textureNumberTo]);
                        //set old texture mask to zero
                        alphas[i, j, textureNumberFrom] = 0f;
                    }

                }
            }
            // apply the new alpha
            terrainData.SetAlphamaps(0, 0, alphas);
        }

        public bool Highlight
        {
            get { return m_highlight; }
            set
            {
                m_highlight = value;
                UpdateColor();
            }
        }

        protected void UpdateColor()
        {
            if (isGrabbed) SetColor(COLOR_GRAB);
            else if (Highlight) SetColor(COLOR_HIGHLIGHT);
            else SetColor(m_color);

        }

        override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            inHand = true;
            base.GrabBegin(hand, grabPoint);
            UpdateColor();
            
        }

        override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            base.GrabEnd(linearVelocity, angularVelocity);
            UpdateColor();
            inHand = false;
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

                // Grab points are doing double-duty as a way to identify submeshes that should be colored.
                // If unspecified, just color self.
                m_meshRenderers = new MeshRenderer[1];
                m_meshRenderers[0] = this.GetComponent<MeshRenderer>();
            }
            else
            {
                m_meshRenderers = this.GetComponentsInChildren<MeshRenderer>();
            }
            m_color = new Color(
                Random.Range(0.1f, 0.95f),
                Random.Range(0.1f, 0.95f),
                Random.Range(0.1f, 0.95f),
                1.0f
            );
            SetColor(m_color);
            inHand = false;
            terrData = terr.terrainData;
        }

        private void SetColor(Color color)
        {
            for (int i = 0; i < m_meshRenderers.Length; ++i)
            {
                MeshRenderer meshRenderer = m_meshRenderers[i];
                for (int j = 0; j < meshRenderer.materials.Length; ++j)
                {
                    Material meshMaterial = meshRenderer.materials[j];
                    meshMaterial.color = color;
                }
            }
        }
    }
}
