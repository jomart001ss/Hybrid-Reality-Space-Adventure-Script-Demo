using UnityEngine;
using System.Collections;

public class ControllerUIReactor : MonoBehaviour
{
	void Start ()
    {
        ui.SetActive(false);
        VRManager.instance.OnNewPrimaryHand += UpdateUIState;
	}

    public enum ControllerUIType { Primary, Secondary }
    public ControllerUIType controllerUIType;
    public PrimaryController controllerSide;
    public GameObject ui;
    
    void UpdateUIState (PrimaryController primaryController)
    {
        if (primaryController == controllerSide)
        {
            if (controllerUIType == ControllerUIType.Primary)
            {
                ui.SetActive(true);
            }
            else
            {
                ui.SetActive(false);
            }
        }
        else
        {
            if (controllerUIType == ControllerUIType.Secondary)
            {
                ui.SetActive(true);
            }
            else
            {
                ui.SetActive(false);
            }
        }
    }
}
