using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class Instructions : MonoBehaviour {

        Text instructions;
        public ColorGrabbable pencil;
        public PainterGrab brush;
        bool menu, grabPencil, createConstraint1, createConstraint2, manipConstraint1, manipConstraint2, colorMenu, menuNavigation, grabBrush1, grabBrush2, grabBrush3, grabTerrain, grabTerrain2, landmarkMenu,selectLandmark, makeLandmark, selectTP, TP, reverseTP;

        // Use this for initialization
        void Start() {
            instructions = GetComponentInParent<Text>();
            menu = false;
            grabPencil = false;
            createConstraint1 = false;
            createConstraint2 = false;
            manipConstraint1 = false;
            manipConstraint2 = false;
            colorMenu = false;
            menuNavigation = false;
            grabBrush1 = false;
            grabBrush2= false;
            grabBrush3 = false;
            grabTerrain = false;
            landmarkMenu = false;
            selectLandmark = false;
            makeLandmark = false;
            selectTP = false;
            TP = false;
            reverseTP = false;
            grabTerrain2 = false;
        }

        // Update is called once per frame
        void Update() {

            if (!menu)
            {
                if (OVRInput.GetDown(OVRInput.Button.Start))
                    menu = true;
            }
            else if (!grabPencil)
            {
                instructions.text = "To create a constraint first grab the pencil tool using either controller and the hand trigger.";
                if (pencil.inLeftHand || pencil.inRightHand)
                    grabPencil = true;

            }
            else if (!createConstraint1)
            {
                instructions.text = "Aim at the desired point on the terrain with the pencil and tap the index trigger to create the constraint.";
                if ((pencil.inLeftHand && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) || (pencil.inRightHand && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
                    createConstraint1 = true;

            }
            else if (!createConstraint2)
            {               
                instructions.text = "You can hold the index trigger down and move the pencil to create a line-type constraint";
                if ((pencil.inLeftHand && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) || (pencil.inRightHand && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))) //make sure a line is drawn?
                    createConstraint2 = true;


            }
            else if (!manipConstraint1)
            {
                instructions.text = "To manipulate a constraint grab it with either your left or right hand avatar. Grabbing the ball will allow you to change the height of the hill.";
                if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) //check ball is actually grabbed
                    manipConstraint1 = true;
            }
            //breaks here
            else if (!manipConstraint2)
            {
                instructions.text = "Grabbing the cube will let you adjust the width and the steepness of the hill";
                if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) //check cube is grabbed
                    manipConstraint2 = true;

            }
            else if (!colorMenu)//need a delay here
            {
                instructions.text = "To create a terrain type constraint first select a terrain type by using the selection menu accessed by pressing the X button near the top of the left hand controller.";
                if (OVRInput.GetDown(OVRInput.Button.Three))
                    colorMenu = true;
            }
            else if (!menuNavigation)
            {              
                instructions.text = "Use either joystick to navigate through the menu. Press the right index trigger to select the terrain type.";
                if ((OVRInput.GetDown(OVRInput.Button.Three) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
                    menuNavigation = true;

            }
            else if (!grabBrush1)
            {
                instructions.text = "Now pick up the paintbrush tool and aim it at the area you wish to add the constraint to.";
                if (brush.inLeftHand || brush.inRightHand)
                    grabBrush1 = true;


            }
            else if (!grabBrush2)
            {
                Debug.Log("Here");
                instructions.text = "To adjust the area to be painted use the joystick on the hand gripping the paintbrush. Pushing the joystick up increases the area and pushing down decreases it.";
                if ((brush.inLeftHand && Mathf.Abs(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y) > 0.2) || (brush.inRightHand && Mathf.Abs(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y) > 0.2))
                    grabBrush2 = true;
            }
            else if (!grabBrush3)
            {               
                instructions.text = "Press the index finger trigger on the hand holding the brush to paint the type constraint onto the map.";
                if ((brush.inLeftHand && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) || (brush.inRightHand &&OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
                    grabBrush3 = true;
            }
            else if (!grabTerrain)
            {
                instructions.text = "You can also grab onto the terrain base to move it to a comfortable position and angle for interaction.";
                    grabTerrain = true;
            }
            else if (!grabTerrain2)
            {
                instructions.text = "Grab with both hands and move them apart/together to scale the terrain.";
                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                    grabTerrain2 = true;
            }
            else if (!selectLandmark)
            {
                instructions.text = "To create a landmark select the cube icon in the selection menu accessed by pressing the Y button near the middle of the left hand controller.";
                if (OVRInput.GetDown(OVRInput.Button.Four))
                    selectLandmark = true;
            }
            else if (!makeLandmark)
            {
                instructions.text = "Now use the pencil tool to create a landmark in the same way you would create point constraint.";
                if ((pencil.inLeftHand && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) || (pencil.inRightHand && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
                    makeLandmark = true;
            }
            else if (!TP)
            {
                instructions.text = "To teleport into the terrain select the person icon in the selection menu and use the pencil to select a location to teleport to";
                if ((pencil.inLeftHand && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) || (pencil.inRightHand && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
                    TP = true;
            }
            else if (!reverseTP)
            {
                instructions.text = "To leave First person mode press an index trigger. To move use the left hand joystick. To rotate use the right hand joystick";
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                    reverseTP = true;
            }
            else
            {
                //this.GetComponentInParent<GameObject>().SetActive(false);
                this.transform.parent.gameObject.SetActive(false);
            }

        }
        public void resetBools()
        {
            menu = false;
            grabPencil = false;
            createConstraint1 = false;
            createConstraint2 = false;
            manipConstraint1 = false;
            manipConstraint2 = false;
            colorMenu = false;
            menuNavigation = false;
            grabBrush1 = false;
            grabBrush2 = false;
            grabBrush3 = false;
            grabTerrain = false;
            landmarkMenu = false;
            selectLandmark = false;
            makeLandmark = false;
            selectTP = false;
            TP = false;
            reverseTP = false;
            grabTerrain2 = false;
        }
    }
