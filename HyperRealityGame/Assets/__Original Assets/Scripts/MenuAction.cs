using UnityEngine;
using System.Collections;

public abstract class MenuAction : MonoBehaviour
{
    public MenuAction nextAction;

    public void ActivateActions()
    {
        Activate();
        if (nextAction != null)
        {
            nextAction.Activate();
        }
    }

    public abstract void Activate();
}
