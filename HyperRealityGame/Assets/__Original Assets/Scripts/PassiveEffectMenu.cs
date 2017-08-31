using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PassiveEffectMenu : MonoBehaviour
{
    private Image background;

    void OnEnable ()
    {
        if (background == null)
        {
            InitializeVariables();
        }
        UpdateState();
        ResourceManager.instance.OnUnlock += UpdateState;
    }

    private Transform newTransform;
    private Color lockedBG;
    private Color activatedBG;
    private Color deactivatedBG;
    private Text timeText;

    void InitializeVariables ()
    {
        newTransform = GetComponent<Transform>();
        background = newTransform.parent.GetChild(0).GetComponent<Image>();
        lockedBG = ColorManager.instance.passiveMenuLockedBG;
        activatedBG = ColorManager.instance.passiveMenuActivatedBG;
        deactivatedBG = ColorManager.instance.passiveMenuDeactivatedBG;
        timeText = background.GetComponent<Transform>().GetChild(1).GetComponent<Text>();
    }

    public enum State
    {
        Activated,
        Deactivated,
        Locked
    }

    private State state;
    public PassiveEffect passiveEffect;

    void UpdateState ()
    {
        if (ResourceManager.instance.IsLocked(passiveEffect.effect))
        {
            state = State.Locked;
            SetLockedSettings();
        }
        else if (passiveEffect.active)
        {
            state = State.Activated;
            SetActiveSettings();
        }
        else
        {
            state = State.Deactivated;
            SetDeactivatedSettings();
        }
    }

    void SetLockedSettings ()
    {
        background.color = lockedBG;
    }

    public void SetActiveSettings ()
    {
        background.color = activatedBG;
    }

    public void SetDeactivatedSettings()
    {
        background.color = deactivatedBG;
    }

    void UpdateState (Unlockable unlockable)
    {
        UpdateState();
    }

    void OnTriggerEnter (Collider otherCollider)
    {
        if (state == State.Locked)
        {
            PlayErrorEffect();
        }
        else if (state == State.Deactivated)
        {
            passiveEffect.Activate();
            UpdateState();
            PlayActivatedEffect();
        }
        else if (state == State.Activated)
        {
            passiveEffect.Deactivate();
            UpdateState();
            PlayDeactivatedEffect();
        }
    }

    void PlayErrorEffect ()
    {

    }

    void PlayActivatedEffect ()
    {

    }

    void PlayDeactivatedEffect ()
    {

    }

    void Update ()
    {
        float timeLeft = passiveEffect.timeLeft;
        //int timeLeftMilliSeconds = (int)(timeLeft * 1000);
        TimeSpan t = TimeSpan.FromSeconds(timeLeft);
        string text = string.Format("{0:D2}:{1:D2}:{2:D3}",
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
        timeText.text = text;
    }

    void OnDisable()
    {
        ResourceManager.instance.OnUnlock -= UpdateState;
    }
}
