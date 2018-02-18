using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularMenu : MonoBehaviour {

    public List<MenuButton> buttons = new List<MenuButton>();
    public int menuItems;
    public int selectedItem;
    public Canvas myCanvas;
    private float timer;
    public bool color;

    // Use this for initialization
    void Start () {
        menuItems = buttons.Count;
        selectedItem = 0;
        timer = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        if(myCanvas.enabled)
            GetCurrentMenuItem();

        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            if (myCanvas.enabled && color)
                myCanvas.enabled = false;
            else if (color)
                myCanvas.enabled = true;
        }
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            if (myCanvas.enabled && !color)
                myCanvas.enabled = false;
            else if (!color)
                myCanvas.enabled = true;
        }
        if ((OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && myCanvas.enabled)|| (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && myCanvas.enabled))
        {
            myCanvas.enabled = false;
        }

    }

    public void GetCurrentMenuItem()
    {
        Vector2 stickPos;
        if (color)
            stickPos = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        else
            stickPos = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (stickPos.x > 0.2 && (Time.time-timer) > 0.1)
        {
            if (selectedItem < (menuItems-1))
                selectedItem++;
            else
                selectedItem = 0;
            myCanvas.transform.localRotation = Quaternion.Euler(myCanvas.transform.localRotation.eulerAngles + new Vector3(0,0,360/ menuItems));
            timer = Time.time;
        }
        else if (stickPos.x < -0.2 && (Time.time - timer) > 0.5)
        {
            if (selectedItem >= 1)
                selectedItem--;
            else
                selectedItem = (menuItems - 1);
            myCanvas.transform.localRotation = Quaternion.Euler(myCanvas.transform.localRotation.eulerAngles - new Vector3(0, 0, 360 / menuItems));
            timer = Time.time;
        }
        foreach (MenuButton button in buttons)
        {
            button.sceneImage.rectTransform.localPosition = new Vector3(0, 0, 0);
        }
        buttons[selectedItem].sceneImage.rectTransform.localPosition = new Vector3(0, 0, -0.01f);
    }
}

[System.Serializable]
public class MenuButton
{
    public string name;
    public Image sceneImage;
}
