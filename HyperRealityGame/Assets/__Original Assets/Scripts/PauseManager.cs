using UnityEngine;
using UnityStandardAssets.ImageEffects;
using AmplifyColor;
using System.Collections;

public class PauseManager : Singleton<PauseManager> 
{
    void Start () 
	{
		Unpause ();
    }

	[HideInInspector] public bool paused;
	delegate void stateHandler();
	private stateHandler State;

	public void Unpause (bool godMode = false, bool playSound = false) 
	{
        State = Unpaused;
		Time.timeScale = 1;
        if (godMode)
        {
            //turn on god mode to avoid enemies killing the player after 
            //they decided to go back to the start menu or restart
            HealthManager.instance.SwitchGodMode(true, false);
        }
	}

    public AudioClip sound;

    void PlaySound()
    {
        if (sound != null)
        {
            AudioManager.instance.Play(sound, Vector3.zero, VolumeGroup.MenuSound, 0.07f, 1f, 0f, false, 256);
        }
    }

    public delegate void pauseNotifier ();
	public event pauseNotifier OnUnpause;

	void NotifyUnpause () 
	{
		if (OnUnpause!= null) 
		{
			OnUnpause();
		}
	}

    void Update () 
	{
        State();
    }

    public int framesSinceUnpause;

	void Unpaused ()
	{
        framesSinceUnpause++;
		GetPauseInput ();
	}

	void GetPauseInput ()
	{
		if (Input.GetKeyDown (KeyCode.Escape) ||
            Controller.instance.player.GetButtonDown("Pause") /*||
            OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Touch)*/)
		{
			Pause(true, true, false, true);
		}
	}

	public void Pause (bool lockMouse = true, bool addEffect = true, bool steamOverlayPause = false, bool playSound = false)
	{
        //Don't let players pause while fading, even if it's due to focus loss
        State = Paused;
        Time.timeScale = 0;
        paused = true;
		NotifyPause ();
	}

	public event pauseNotifier OnPause;
	
	void NotifyPause ()
	{
		if (OnPause != null) 
		{
			OnPause();
		}
	}

	void Paused ()
	{
		GetUnpauseInput ();
    }

	void GetUnpauseInput ()
	{
		if (Controller.instance.player.GetButtonDown("Pause") /*||
            OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Touch)*/)
		{
			Unpause(false, true);	
		}
	}
}
