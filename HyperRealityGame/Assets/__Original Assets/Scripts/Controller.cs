using UnityEngine;
using System;
using System.Collections;
using Rewired;
using VRTK;

public class Controller : Singleton<Controller>
{
    public CollisionDetection collisionDetection;
    public Rewired.Player player;
    public bool rightTriggerPressed;
    public bool leftTriggerPressed;

    void Awake ()
    {
        if (collisionDetection != null)
        {//only add in gameplay scenes
            collisionDetection.OnDeath += StopRumble;
        }
        if (PauseManager.instance != null)
        {
            PauseManager.instance.OnPause += TurnOffRumble;
        }
        player = ReInput.players.GetPlayer(0);
    }

    void TurnOffRumble ()
    {
        foreach (Joystick j in player.controllers.Joysticks)
        {
            if (!j.supportsVibration)
            {
                continue;
            }
            else
            {
                j.StopVibration();
            }
        }
    }

    public bool XBoxOneController;
    private bool touchSupport = false;

    void Update()
    {
        leftStrength = -1;
        rightStrength = -1;
        string thisJoyStick = "";
        XBoxOneController = false;
        foreach (string joyStickName in Input.GetJoystickNames())
        {
            if (joyStickName != "")
            {
                thisJoyStick = joyStickName;
            }
            if (thisJoyStick == "Controller (Xbox One For Windows)")
            {
                XBoxOneController = true;
            }
        }
        UpdateGamepadControls();
        if (touchSupport)
        {
            UpdateTouchControls();
        } 
    }

    void UpdateGamepadControls ()
    {
        UpdateStickFlicks();
    }

    private float stickDeadZone = 0.35f;
    public bool leftStickLeftFlick;
    public bool leftStickRightFlick;
    public bool leftStickDownFlick;
    public bool leftStickUpFlick;
    public bool rightStickLeftFlick;
    public bool rightStickRightFlick;

    void UpdateStickFlicks ()
    {
        leftStickLeftFlick = false;
        leftStickRightFlick = false;
        leftStickDownFlick = false;
        leftStickUpFlick = false;
        float horizontalMovement = player.GetAxis("Move Horizontal");
        float prevHoriztonalMovement = player.GetAxisPrev("Move Horizontal");
        float verticalMovement = player.GetAxis("Move Vertical");
        float prevVerticalMovement = player.GetAxisPrev("Move Vertical");
        if (horizontalMovement < -stickDeadZone && prevHoriztonalMovement > -stickDeadZone)
        {
            leftStickLeftFlick = true;
        }
        else if (horizontalMovement > stickDeadZone && prevHoriztonalMovement < stickDeadZone)
        {
            leftStickRightFlick = true;
        }
        else if (verticalMovement > stickDeadZone && prevVerticalMovement < stickDeadZone)
        {
            leftStickUpFlick = true;
        }
        else if (verticalMovement < -stickDeadZone && prevVerticalMovement > -stickDeadZone)
        {
            leftStickDownFlick = true;
        }
        rightStickLeftFlick = false;
        rightStickRightFlick = false;
        horizontalMovement = player.GetAxis("Rotate Horizontal");
        prevHoriztonalMovement = player.GetAxisPrev("Rotate Horizontal");
        if (horizontalMovement < -stickDeadZone && prevHoriztonalMovement > -stickDeadZone)
        {
            rightStickLeftFlick = true;
        }
        else if (horizontalMovement > stickDeadZone && prevHoriztonalMovement < stickDeadZone)
        {
            rightStickRightFlick = true;
        }
    }

    void UpdateTouchControls ()
    {
        SetTouchTriggerDown();
        SetTouchTriggerPressed();
        SetTouchStickAxis();
        UpdateTouchStickFlicks();
    }

    private bool touchRightTriggerWasDown;
    public bool touchRightTriggerDown;
    private bool touchLeftTriggerWasDown;
    public bool touchLeftTriggerDown;

    void SetTouchTriggerDown ()
    {
        bool leftTriggerPressed = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.Touch) > 0.5f;
        touchLeftTriggerDown = leftTriggerPressed != touchLeftTriggerWasDown && leftTriggerPressed;
        touchLeftTriggerWasDown = leftTriggerPressed;
        bool rightTriggerPressed = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.Touch) > 0.5f;
        touchRightTriggerDown = rightTriggerPressed != touchRightTriggerWasDown && rightTriggerPressed;
        touchRightTriggerWasDown = rightTriggerPressed;
    }

    public bool touchRightTriggerPressed;
    public bool touchLeftTriggerPressed;

    void SetTouchTriggerPressed()
    {
        touchLeftTriggerPressed = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.Touch) > 0.5f;
        touchRightTriggerPressed = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.Touch) > 0.5f;
    }

    public Vector2 touchRightTouchStickAxis;
    public Vector2 prevTouchRightStickAxis;
    public Vector2 touchLeftTouchStickAxis;
    public Vector2 prevTouchLeftStickAxis;

    void SetTouchStickAxis()
    {
        prevTouchLeftStickAxis = touchLeftTouchStickAxis;
        prevTouchRightStickAxis = touchRightTouchStickAxis;
        touchRightTouchStickAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
        touchLeftTouchStickAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
    }

    public bool touchLeftStickLeftFlick;
    public bool touchLeftStickRightFlick;
    public bool touchLeftStickDownFlick;
    public bool touchLeftStickUpFlick;

    void UpdateTouchStickFlicks()
    {
        touchLeftStickLeftFlick = false;
        touchLeftStickRightFlick = false;
        touchLeftStickDownFlick = false;
        touchLeftStickUpFlick = false;
        float horizontalMovement = touchLeftTouchStickAxis.x;
        float prevHoriztonalMovement = prevTouchLeftStickAxis.x;
        float verticalMovement = touchLeftTouchStickAxis.y;
        float prevVerticalMovement = prevTouchLeftStickAxis.y;
        if (horizontalMovement < -stickDeadZone && prevHoriztonalMovement > -stickDeadZone)
        {
            touchLeftStickLeftFlick = true;
        }
        else if (horizontalMovement > stickDeadZone && prevHoriztonalMovement < stickDeadZone)
        {
            touchLeftStickRightFlick = true;
        }
        else if (verticalMovement > stickDeadZone && prevVerticalMovement < stickDeadZone)
        {
            touchLeftStickUpFlick = true;
        }
        else if (verticalMovement < -stickDeadZone && prevVerticalMovement > -stickDeadZone)
        {
            touchLeftStickDownFlick = true;
        }
    }

    public void Rumble (float time, float left, float right, AnimationCurve _vibrationCurve = null)
    {
        //StartCoroutine(RumbleRoutine(time, left, right, _vibrationCurve));
    }

    private bool vibrationOn = true;
    private float leftStrength, rightStrength;
    public AnimationCurve vibrationCurve;

    IEnumerator RumbleRoutine(float time, float left, float right, AnimationCurve _vibrationCurve)
    {
        /*
        if (_vibrationCurve == null)
        {
            _vibrationCurve = vibrationCurve;
        }
        float progress = 0;
        float timeLeft = time;
        do
        {
            while (PauseManager.instance.paused)
            {
                yield return null;
            }
            timeLeft -= Time.deltaTime;
            progress = Mathf.InverseLerp(time, 0, timeLeft);
            float multiplier = _vibrationCurve.Evaluate(progress);
            float leftVibration = Mathf.Clamp01(multiplier * left);
            float rightVibration = Mathf.Clamp01(multiplier * right);
            if (leftVibration >= leftStrength)
            {
                leftStrength = leftVibration;
            }
            if (rightVibration >= rightStrength)
            {
                rightStrength = rightVibration;
            }
            Rumble(0, 0);
            //SetRewiredRumble(leftStrength, rightStrength);
            yield return null;

        }
        while (timeLeft > 0);
        Rumble(0, 0);
        //SetRewiredRumble(0, 0);
        */
        yield return null;
    }

    public void Rumble (float left, float right)
    { //make sure to turn off at one point
        /*
        if (!PauseManager.instance.paused)
        {
            if(left >= leftStrength)
            {
                leftStrength = left;
            }
            if (right >= rightStrength)
            {
                rightStrength = right;
            }
            float rumbleSetting = DataManager.instance.settings.rumble;
            float settingRightStrength = rightStrength * rumbleSetting;
            float settingLeftStrength = leftStrength * rumbleSetting;
            ControlScheme controlScheme = DataManager.instance.GetControlSchemeSettings();
            if (controlScheme == ControlScheme.MotionController)
            {
                if (VRManager.VRMode == VRMode.Oculus)
                {
                    SetOculusRumble(settingLeftStrength, settingRightStrength);
                }
            }
            else if (controlScheme == ControlScheme.Gamepad)
            {
                SetRewiredRumble(settingLeftStrength, settingRightStrength);
            }
        }
        */
    }

    void SetOculusRumble (float left, float right)
    {
        int leftIntensity = Mathf.RoundToInt(Mathf.Lerp(0, 255, left));
        HapticHelper.instance.ProceduralTone(true, leftIntensity);
        int rightIntensity = Mathf.RoundToInt(Mathf.Lerp(0, 255, right));
        HapticHelper.instance.ProceduralTone(false, rightIntensity);
    }

    public AnimationCurve XBoxOneRumble;

    void SetRewiredRumble(float left, float right)
    {
        foreach (Joystick j in player.controllers.Joysticks)
        {
            if (!j.supportsVibration)
            {
                continue;
            }
            else
            {
                if (XBoxOneController)
                {
                    left = XBoxOneRumble.Evaluate(left);
                    right = XBoxOneRumble.Evaluate(right);
                }
                j.SetVibration(left, right);
            }
        }
    }

	void OnDestroy ()
    {
		//StopRumble ();
    }

    void StopRumble (CollisionDetection collisionDetection = null)
    {
        //StopAllCoroutines();
        //TurnOffRumble();
        //vibrationOn = false;
    }
}
