using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnlockableMenu : MonoBehaviour
{
    void OnEnable ()
    {
        InitializeVariables();
        UpdateState();
        ResourceManager.instance.OnPointUpdate += UpdateState;
    }

    private Transform newTransform;
    private Image background;
    private Text pointText;
    private Color availableBG;
    private Color unavailableBG;
    private Color unlockedBG;

    void InitializeVariables ()
    {
        newTransform = GetComponent<Transform>();
        background = newTransform.parent.GetChild(0).GetComponent<Image>();
        pointText = background.GetComponent<Transform>().GetChild(1).GetComponent<Text>();
        availableBG = ColorManager.instance.unlockableMenuAvailableBG;
        unavailableBG = ColorManager.instance.unlockableMenuUnavailableBG;
        unlockedBG = ColorManager.instance.unloclableMenuUnlockedBG;
    }

    public enum State
    {
        Available,
        Unavailable,
        Unlocked
    }

    private State state;

    void UpdateState ()
    {
        int index;
        UnlockableInfo unlockableInfo = ResourceManager.instance.GetUnlockableInfo(unlockable, out index);
        if (!unlockableInfo.locked)
        {
            state = State.Unlocked;
            SetUnlockedSettings();
        }
        else if (unlockableInfo.pointsNeeded <= ResourceManager.instance.currentPoints)
        {
            state = State.Available;
            SetAvailableSettings();
        }
        else
        {
            state = State.Unavailable;
            SetUnavailableSettings();
        }
    }

    void SetUnlockedSettings ()
    {
        background.color = unlockedBG;
        pointText.text = "UNLOCKED";
    }

    void SetAvailableSettings ()
    {
        background.color = availableBG;
    }

    void SetUnavailableSettings ()
    {
        background.color = unavailableBG;
    }

    public Unlockable unlockable;

    void OnTriggerEnter (Collider otherCollider)
    {
        if (state == State.Unlocked)
        {
            PlayAlreadyUnlockedEffect();
        }
        else if (state == State.Unavailable)
        {
            PlayErrorEffect();
        }
        else if (state == State.Available)
        {
            ResourceManager.instance.TryToUnlock(unlockable);
            UpdateState();
            PlayUnlockEffect();
        }
    }

    void PlayAlreadyUnlockedEffect ()
    {

    }

    void PlayErrorEffect ()
    {

    }

    void PlayUnlockEffect ()
    {

    }

    void OnDisable ()
    {
        ResourceManager.instance.OnPointUpdate -= UpdateState;
    }
}
