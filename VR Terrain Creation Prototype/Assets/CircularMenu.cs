using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularMenu : MonoBehaviour {

    public List<MenuButton> buttons = new List<MenuButton>();
    public int menuItems;
    public int curItem;
    public int oldItem;
    public int selectedItem;
    public Vector2 fromVector2M = new Vector2(0.5f, 1.0f);
    public Vector2 centercircle = new Vector2(0.5f, 0.5f);
    public Canvas myCanvas;

    // Use this for initialization
    void Start () {
        menuItems = buttons.Count;
        selectedItem = 0;
	}
	
	// Update is called once per frame
	void Update () {
        GetCurrentMenuItem();
        
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) && myCanvas.enabled)
        {
            myCanvas.enabled = false;
        }

    }

    public void GetCurrentMenuItem()
    {
        Vector2 stickPos = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (stickPos != new Vector2(0, 0))
        {
            //claculates the angle of the joystick
            float angle = (Mathf.Atan2(fromVector2M.y - centercircle.y, fromVector2M.x - centercircle.x) - Mathf.Atan2(stickPos.y - centercircle.y, stickPos.x - centercircle.x)) * Mathf.Rad2Deg;

            if (angle < 0)
                angle += 360;
            
            curItem = (int)(angle / (360 / menuItems));
            if (oldItem == -1)
            {
                foreach (MenuButton button in buttons)
                {
                    button.sceneImage.rectTransform.localPosition = new Vector3(0, 0, 0);
                }
            }
            if (curItem != oldItem && (Mathf.Abs(stickPos.x)>0.2 || Mathf.Abs(stickPos.y) > 0.2))
            {
                selectedItem = curItem;
                Debug.Log("X: " + stickPos.x + " Y: " + stickPos.y);
                if (oldItem != -1)
                    buttons[oldItem].sceneImage.rectTransform.localPosition = new Vector3(0,0,0);
                oldItem = curItem;
                buttons[curItem].sceneImage.rectTransform.localPosition = new Vector3(0, 0, -0.01f);
            }
        }
        else
        {
                oldItem = -1;
        }
    }
}

[System.Serializable]
public class MenuButton
{
    public string name;
    public Image sceneImage;
    public Color NormalColor = Color.red;
    public Color SelectColor = Color.grey;
}
