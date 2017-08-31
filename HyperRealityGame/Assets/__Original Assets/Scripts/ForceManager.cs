using UnityEngine;
using System.Collections;

public class ForceManager : Singleton<ForceManager>
{
    private bool pushing;
    private bool pulling;

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.B) || 
            ArduinoManager.instance.GetButtonDownState(1))
        {
            if (pulling)
            {
                StopPull();
            }
            else
            {
                StopPush();
                TriggerPull(1.5f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.N) ||
            ArduinoManager.instance.GetButtonDownState(5))
        {
            if (pushing)
            {
                StopPull();
                StopPush();
            }
            else
            {
                TriggerPush(1.5f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.R) ||
            ArduinoManager.instance.GetButtonDownState(2))
        {
            TriggerReset();
        }
    }

    public delegate void PushStopHandler();
    public event PushStopHandler OnPushStop;

    void StopPush ()
    {
        if (OnPushStop != null)
        {
            pushing = false;
            OnPushStop();
        }
    }

    public delegate void PushHandler(float pushTime);
    public event PushHandler OnPush;

    void TriggerPush (float pushTime)
    {
        if (OnPush != null)
        {
            pushing = true;
            OnPush(pushTime);
        }
    }

    public delegate void PullStopHandler();
    public event PullStopHandler OnPullStop;

    void StopPull ()
    {
        if (OnPullStop != null)
        {
            pulling = false;
            OnPullStop();
        }
    }

    public delegate void PullHandler(float pullTime);
    public event PullHandler OnPull;

    void TriggerPull (float pullTime)
    {
        if (OnPull != null)
        {
            pulling = true;
            OnPull(pullTime);
        }
    }

    public delegate void ResetHandler();
    public event ResetHandler OnReset;

    void TriggerReset ()
    {
        StopPull();
        StopPush();
        if (OnReset != null)
        {
            OnReset();
        }
    }
}
