using UnityEngine;
using System.Collections;

public enum UIStates
{
    Disabled = 0,
    Default = 1,
    BurstInstruction = 2,
    ChargeInstruction = 3,
    Transitioning = 4,
    Initializing = 5,
    AimInstruction = 6,
    DashInstruction = 7,
    WeaponSwitchInstruction = 8,
    MissInstruction = 9,
    WaitInstruction = 11,
    MissMarkDestruction = 10
}

public class UIStateManager : Singleton<UIStateManager> 
{
	private Quaternion notificationRotation;
    private Transform newTransform;
	
	void Awake () 
	{
        newTransform = transform;
	}

    private bool cinematicMode;

    void Update ()
    {
        if (DebugManager.instance.debugMode)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                cinematicMode = !cinematicMode;
                NotifyCinematicModeSwitch();
            }
        } 
    }

    public delegate void CinematicModeHandler(bool cinematicMode);
    public event CinematicModeHandler OnCinematicModeSwitch;

    void NotifyCinematicModeSwitch ()
    {
        if (OnCinematicModeSwitch != null)
        {
            OnCinematicModeSwitch(cinematicMode);
        }
    }

	public AudioClip notificationSound;
    private Notification lastNotification;

	public void NotifyPlayer (GameObject notificationPlate, float seconds, bool playSound = false) 
	{
        if (!cinematicMode)
        {
            if (playSound)
            {
                AudioManager.instance.Play(notificationSound, Vector3.zero, VolumeGroup.SoundEffect, 1f, 1f, 1f, false);
            }
            if (lastNotification != null)
            {
                lastNotification.dontDelay = true;
            }
            lastNotification = gameObject.AddComponent<Notification>();
            lastNotification.Init(notificationPlate, seconds, newTransform);
        }
	}

	public delegate void stringNotifier (UIStates lastState, UIStates newState);
	public event stringNotifier OnStateChange;

    private UIStates _state;
    public UIStates state
    {
        get { return _state; }
    }

	public void TriggerStateChange (UIStates newState, UIStates? completedState = null, bool playSound = false) 
	{
        if (completedState != null)
        {
            DataManager.instance.RecordCompletedState((UIStates)completedState);
        }
        if (playSound)
        {
            StartCoroutine(PlayNotificationSound(0.5f));
        }
        UIStates lastState = _state;
        _state = newState;
        DataManager.instance.RecordLastestState(_state);
        if (OnStateChange != null) 
		{
			OnStateChange(lastState, newState);
		}
	}

    public AudioClip stateChangeSound;

    IEnumerator PlayNotificationSound (float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.Play(stateChangeSound, Vector3.zero, VolumeGroup.SoundEffect, 0.15f, 1f, 1f, false);
    }

    public Color defaultButtonColor;
    public float defaultButtonEmission;
    public AnimationCurve buttonPulseCurve;
}
