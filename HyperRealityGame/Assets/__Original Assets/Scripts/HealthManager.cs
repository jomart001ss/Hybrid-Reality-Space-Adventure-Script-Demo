using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthManager : Singleton<HealthManager>
{
	[HideInInspector] public CollisionDetection collisionDetection;
	private bool godMode;
	
	void Awake () 
	{
        Initialize();
	}

    void Initialize ()
    {
        collisionDetection = Player.instance.collisionDetection;
        //turned on and then off to make sure everything related to it is switched off
        SwitchGodMode(false, false);
    }

    private Coroutine godModeRoutine;
    private Coroutine displayGodModeIndicator;
    private bool firstSwitch = true;

    public void SwitchGodMode (bool on, bool showUI = true) 
	{
        godMode = on;
        if (displayGodModeIndicator != null)
        {
            StopCoroutine(displayGodModeIndicator);
        }
        if (godModeRoutine != null)
        {
            StopCoroutine(godModeRoutine);
        }
        if (godMode)
        {
            if (showUI)
            {
                GravCharaController.instance.godModeText.text = "God Mode On";
                displayGodModeIndicator = StartCoroutine(DisplayGodModeIndicator());
            }
            godModeRoutine = StartCoroutine(GodModeRoutine());
        }
        else
        {
            if (showUI)
            {
                GravCharaController.instance.godModeText.text = "God Mode Off";
                displayGodModeIndicator = StartCoroutine(DisplayGodModeIndicator());
            }    
            collisionDetection.SwitchInvincibility(false);
        }   
	}

    IEnumerator DisplayGodModeIndicator()
    {
        GravCharaController.instance.godModeIndicator.SetActive(true);
        float displayTime = 1f;
        yield return new WaitForSeconds(displayTime);
        GravCharaController.instance.godModeIndicator.SetActive(false);
    }

    IEnumerator GodModeRoutine ()
    {
        while (true)
        {
            collisionDetection.SwitchInvincibility(true);
            yield return null;
        }
    }
	
	private float delayCounter;
	public float delay;

	void Update () 
	{
		delayCounter -= Time.deltaTime;
		if (delayCounter > 0)
		{
			return;
		}
        if (DebugManager.instance.debugMode)
        {
            GodModeSwitchCheck();
        }
	}

	public int healthAmount;
	public AudioClip healSound;

	void GodModeSwitchCheck () 
	{
		if (Input.GetKeyDown(KeyCode.G)) 
		{
			SwitchGodMode(!godMode);
			delayCounter = delay;
		}
	}

    public AnimationCurve particleAnimtion;
}
