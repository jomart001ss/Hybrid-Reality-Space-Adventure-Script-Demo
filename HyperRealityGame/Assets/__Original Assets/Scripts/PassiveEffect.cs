using UnityEngine;
using System.Collections;

public class PassiveEffect : MonoBehaviour
{
    public Unlockable effect;
    private delegate void StateHandler();
    private StateHandler State;

    void Start()
    {
        State = Idle;
        timeLeft = activationTime;
    }

    void Update ()
    {
        State();
    }

    void Idle () {}

    public GameObject effectObject;

    public void Activate ()
    {
        State = Activated;
        effectObject.SetActive(true);
    }

    public float activationTime;
    [HideInInspector]
    public float timeLeft;

    void Activated ()
    {
        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Clamp(timeLeft, 0f, activationTime);
        if (timeLeft == 0)
        {
            Deactivate(true);
        }
    }

    public PassiveEffectMenu menu;

    public void Deactivate (bool outOfTime = false)
    {
        State = Deactivated;
        if (outOfTime)
        {
            menu.SetDeactivatedSettings();
        }
        effectObject.SetActive(false);
    }

    void Deactivated ()
    {
        timeLeft += Time.deltaTime;
        timeLeft = Mathf.Clamp(timeLeft, 0f, activationTime);
        if (timeLeft == activationTime)
        {
            State = Idle;
        }
    }

    public bool active
    {
        get
        {
            return State == Activated;
        }
    }
}
