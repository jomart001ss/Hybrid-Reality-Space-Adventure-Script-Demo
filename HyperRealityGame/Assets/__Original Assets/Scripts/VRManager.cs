using UnityEngine;
using UnityEngine.VR;
using UnityEngine.SceneManagement;
using Valve.VR;
using System.Collections;
using System.Collections.Generic;

public enum VRMode { Notset, Oculus, Desktop }
public enum ControlScheme { Notset, Gamepad, MouseKeyboard, MotionController }
public enum PrimaryController { Notset, Left, Right }
public enum HeadTracking { Sitting, Standing }

public class VRManager : Singleton<VRManager>
{
    void Awake()
    {
        ResetTimeScale();
        SetVRMode();
        MoveUICamera();
        SetCurrentCamera();
        SetHeadTracker();
        SetCurrentControllers();
        ParentCameras();
    }

    //Used to prevent the game freezing due to random pauses during scene transitions
    void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    public VRMode VRModeEditor;
    public static VRMode VRMode = VRMode.Notset;

    void SetVRMode()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];
            if (argument == "desktop")
            {
                VRMode = VRMode.Desktop;
            }
            else if (argument == "oculus")
            {
                VRMode = VRMode.Oculus;
            }
        }
        if (VRMode == VRMode.Notset)
        {
            VRMode = VRModeEditor;
        }
    }
    public List<Transform> layerCameras;

    void MoveUICamera()
    {
        if (VRMode == VRMode.Desktop)
        {
            foreach (Transform layerCamera in layerCameras)
            {
                layerCamera.position = desktopCamera.transform.position;
            }
        }
        else if (VRMode == VRMode.Oculus)
        {
            foreach (Transform layerCamera in layerCameras)
            {
                layerCamera.position = oculusCamera.transform.position;
            }
        }
    }

    public GameObject desktopCameraRig;
    public GameObject oculusCameraRig;
    public Camera desktopCamera;
    public Camera oculusCamera;
    public OVRManager OVRManager;
    [HideInInspector] public Camera currentCamera;
    public MouseLook desktopMouseLook;
    public MouseLook oculusMouseLook;
    [HideInInspector] public MouseLook currentMouseLook;
    public HapticHelper hapticHelper;

    void SetCurrentCamera()
    {
        if (VRMode == VRMode.Desktop)
        {
            currentCamera = desktopCamera;
            currentMouseLook = desktopMouseLook;
            desktopCameraRig.SetActive(true);
            VRSettings.enabled = false;
        }
        else if (VRMode == VRMode.Oculus)
        {
            currentCamera = oculusCamera;
            currentMouseLook = oculusMouseLook;
            oculusCameraRig.SetActive(true);
            OVRManager.enabled = true;
            //hapticHelper.enabled = true;
        }
    }

    [HideInInspector] public Transform headTracker;
    public Transform oculusHeadTracker;

    void SetHeadTracker()
    {
        if (VRMode == VRMode.Oculus)
        {
            headTracker = oculusHeadTracker;
        }
    }

    public Transform oculusLeftController, oculusRightController;
    [HideInInspector] public Transform leftController, rightController;

    void SetCurrentControllers()
    {
        //set to something to avoid null errors
        leftController = oculusLeftController;
        rightController = oculusRightController;
        if (VRMode == VRMode.Oculus)
        {
            leftController = oculusLeftController;
            rightController = oculusRightController;
        }
    }

    public Transform oculusTrackingSpace;
    public static Transform trackingSpace;

    void ParentCameras()
    {
        if (VRMode == VRMode.Desktop || Application.isWebPlayer)
        {
            foreach (Transform layerCamera in layerCameras)
            {
                layerCamera.SetParent(currentCamera.transform);
            }
        }
        else if (VRMode == VRMode.Oculus)
        {
            trackingSpace = oculusTrackingSpace;
            foreach (Transform layerCamera in layerCameras)
            {
                layerCamera.SetParent(oculusTrackingSpace);
            }
        }
        foreach (Transform layerCamera in layerCameras)
        {
            layerCamera.localPosition = Vector3.zero;
            layerCamera.localRotation = Quaternion.identity;
        }
    }

    void Start()
    {
        VRSettings.renderScale = DataManager.instance.settings.superSampling;
        SetPrimaryHand(primaryController);
        DataManager.instance.RecordHeadTrackingSettings(HeadTracking.Sitting);
        SetHeadTracking();
        ShiftTrackingSpaceVertically((float)DataManager.instance.settings.headOffset);
    }

    public PrimaryController primaryControllerEditor;
    public static PrimaryController primaryController = PrimaryController.Notset;
    [HideInInspector] public Transform primaryHand;
    [HideInInspector] public Transform secondaryHand;
    
    public void SetPrimaryHand(PrimaryController primaryController)
    {
        VRManager.primaryController = primaryController;
        if (primaryController == PrimaryController.Notset)
        {
            primaryController = primaryControllerEditor;
            primaryHand = oculusLeftController;
            secondaryHand = oculusRightController;
            DataManager.instance.RecordMotionControllerHandSettings(primaryController);
        }
        if (primaryController == PrimaryController.Left)
        {
            primaryHand = oculusLeftController;
            secondaryHand = oculusRightController;
            SetShiftToHand();
        }
        else if (primaryController == PrimaryController.Right)
        {
            primaryHand = oculusRightController;
            secondaryHand = oculusLeftController;
            SetShiftToHand();
        }
        NotifyNewPrimaryHand(primaryController);
    }

    [HideInInspector] public Vector3 shiftToHandOrigin;

    void SetShiftToHand ()
    {
        shiftToHandOrigin = primaryHand.position - currentCamera.transform.position;
        shiftToHandOrigin.x = 0;
        shiftToHandOrigin.z = 0;
    }

    public delegate void NewPrimaryHandNotifier (PrimaryController primaryController);
    public NewPrimaryHandNotifier OnNewPrimaryHand;


    void NotifyNewPrimaryHand (PrimaryController primaryController)
    {
        if (OnNewPrimaryHand != null)
        {
            OnNewPrimaryHand(primaryController);
        }
    }

    public static HeadTracking headTracking;

    void SetHeadTracking()
    {
        headTracking = DataManager.instance.GetHeadTrackingSetting();
        //Update once to switch to sitting space
        UpdateHeadTracking(headTracking);
        //Update again to apply the proper offset
        UpdateHeadTracking(headTracking);
        //NotifyNewHeadTracking(headTracking);
    }

    private float originalHeadOffset;
    public float standingOffet = -2.19f;

    void UpdateHeadTracking(HeadTracking headTracking)
    {
        if (headTracking == HeadTracking.Sitting)
        {
            if (VRMode == VRMode.Oculus)
            {
                UpdateCameraRigHeight(oculusCameraRig.transform, 0f);
                originalHeadOffset = 0;
                OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
            }
        }
        else if (headTracking == HeadTracking.Standing)
        {
            if (VRMode == VRMode.Oculus)
            {
                UpdateCameraRigHeight(oculusCameraRig.transform, standingOffet);
                originalHeadOffset = standingOffet;
                OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
            }
        }
    }

    float GetHeightOffset()
    {
        string currentScene = SceneManager.GetActiveScene().name.ToLower();
        if (trackingSpace == null ||
            currentScene == "cutscene" ||
            currentScene == "upperscene" ||
            currentScene == "lift scene")
        {
            return 0;
        }
        else
        {
            return (float)DataManager.instance.settings.headOffset;
        }
    }

    void UpdateCameraRigHeight(Transform rig, float newHeight)
    {
        Vector3 newPosition = rig.localPosition;
        newPosition.y = newHeight;
        rig.localPosition = newPosition;
    }

    public enum PlaySpace { withCockpit, withoutCockpit }
    public PlaySpace playSpace = PlaySpace.withoutCockpit; 

    void Update()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        if (playSpace == PlaySpace.withoutCockpit)
        {
            CheckForRecenter();
        }
        CheckForArmLengthUpdate();
    }

    void CheckForRecenter()
    {
        if (Controller.instance.player.GetButtonDown("Recenter") ||
            Input.GetKeyDown(KeyCode.F12) ||
            Input.GetKeyDown(KeyCode.R) ||
            OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Remote) ||
            OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Touch))
        {
            Recenter();
            NotifyRecenter();
        }
    }

    void CheckForArmLengthUpdate()
    {
        if (Controller.instance.player.GetButtonDown("Recenter") ||
            Input.GetKeyDown(KeyCode.F12) ||
            Input.GetKeyDown(KeyCode.R) ||
            OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Remote) ||
            OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Touch))
        {
            GetArmLength();
        }
    }

    [HideInInspector] public float armLength;
    [HideInInspector] public float armLengthSqr;

    void GetArmLength()
    {
        Vector2 cameraPosition = Vector3ToVector2(currentCamera.transform.position);
        Vector2 leftHandPosition = Vector3ToVector2(oculusLeftController.position);
        Vector2 rightHandPosition = Vector3ToVector2(oculusRightController.position);
        float leftDistance = Vector2.Distance(cameraPosition, leftHandPosition);
        float rightDistance = Vector2.Distance(cameraPosition, rightHandPosition);
        if (leftDistance > rightDistance)
        {
            SetPrimaryHand(PrimaryController.Left);
            armLength = leftDistance;
            armLengthSqr = leftDistance * leftDistance;
        }
        else
        {
            SetPrimaryHand(PrimaryController.Right);
            armLength = rightDistance;
            armLengthSqr = rightDistance * rightDistance;
        }
    }

    Vector2 Vector3ToVector2 (Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public delegate void RecenterNotifier();
    public event RecenterNotifier OnRecenter;

    void NotifyRecenter ()
    {
        if (OnRecenter != null)
        {
            OnRecenter();
        }
    }

    public void Recenter ()
    {
        InputTracking.Recenter();
        DataManager.instance.RecordHeadTrackingSettings(headTracking);
    }

    public void ShiftTrackingSpaceVertically (float offset)
    {
        string currentScene = SceneManager.GetActiveScene().name.ToLower();
        if (trackingSpace == null ||
            currentScene == "cutscene" ||
            currentScene == "upperscene" ||
            currentScene == "lift scene")
        {
            return;
        }
        Vector3 newPosition = trackingSpace.localPosition;
        newPosition.y = offset;
        trackingSpace.localPosition = newPosition;
    }

    public delegate void HeadTrackingNotifier(HeadTracking headTracking);
    public event HeadTrackingNotifier OnNewHeadTracking;

    public void NotifyNewHeadTracking (HeadTracking newHeadTracking)
    {
        headTracking = newHeadTracking;
        //Update once to switch to sitting space
        UpdateHeadTracking(newHeadTracking);
        //Update again to apply the proper offset
        UpdateHeadTracking(newHeadTracking);
        if (OnNewHeadTracking != null)
        {
            OnNewHeadTracking(newHeadTracking);
        }
    } 
}
