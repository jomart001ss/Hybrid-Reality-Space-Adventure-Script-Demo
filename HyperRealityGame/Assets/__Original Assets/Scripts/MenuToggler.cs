using UnityEngine;
using System.Collections;

public class MenuToggler : MonoBehaviour
{
    public GameObject menu;

    void Start ()
    {
        menu.SetActive(false);
    }

    public KeyCode desktopInput;
    public int buttonIndex;
    
    void Update()
    {
        if(Input.GetKeyDown(desktopInput) ||
            ArduinoManager.instance.GetButtonDownState(buttonIndex))
        {
            menu.SetActive(!menu.activeSelf);
        }
    }
}
