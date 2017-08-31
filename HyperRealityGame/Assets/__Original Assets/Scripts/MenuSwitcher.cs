using UnityEngine;
using System.Collections;

public class MenuSwitcher : MenuAction
{
    public GameObject disableByDefault;

    protected void OnEnable ()
    {
        if (disableByDefault != null)
        {
            disableByDefault.SetActive(false);
        }
    }

    public GameObject previousMenu;
    public CanvasGroup previousInstructions;

    protected void Update ()
    {
        if (previousMenu != null)
        {
            if (Controller.instance.player.GetButtonDown("Back") || 
                Input.GetKeyDown(KeyCode.Escape) ||
                Input.GetKeyDown(KeyCode.Backspace) /*||
                OVRInput.GetDown(OVRInput.RawButton.B, OVRInput.Controller.Touch)*/)
            {
                SwitchMenuActivation(previousMenu, true);
                previousMenu.SetActive(true);
                if (currentMenu != null)
                {
                    currentMenu.SetActive(false);
                }
            }
        }
    }

    public enum CurrentMenuAction
    {
        Hide,
        Disable
    }
    public CurrentMenuAction currentMenuAction = CurrentMenuAction.Hide;
    public GameObject currentMenu;
    public CanvasGroup currentInstructions;
    public GameObject nextMenu;
    public CanvasGroup nextInstructions;

    public override void Activate()
    {
        if (nextMenu != null)
        {
            SwitchMenuActivation(nextMenu, true);
            nextMenu.SetActive(true);
        }
        if (currentMenu != null)
        {
            if (currentMenuAction == CurrentMenuAction.Hide)
            {
                currentMenu.SetActive(false);
            }
            else if (currentMenuAction == CurrentMenuAction.Disable)
            {
                SwitchMenuActivation(currentMenu, false);
            }
        }
    }

    protected void SwitchMenuActivation (GameObject menuGO, bool activated)
    {
        MenuLook[] menuLooks = menuGO.GetComponentsInChildren<MenuLook>();
        foreach (MenuLook menuLook in menuLooks)
        {
            menuLook.enabled = activated;
        }
        MenuSwitcher[] menuSwitchers = menuGO.GetComponentsInChildren<MenuSwitcher>();
        foreach (MenuSwitcher menuSwitcher in menuSwitchers)
        {
            menuSwitcher.enabled = activated;
        }
    }
}
