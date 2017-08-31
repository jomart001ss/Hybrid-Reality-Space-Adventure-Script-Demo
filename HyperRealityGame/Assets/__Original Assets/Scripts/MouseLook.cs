using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour 
{
    private MonoBehaviour blurEffect;
    private Transform newTransform;

    void Start ()
	{
        newTransform = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Keep the rigid body from changing the rotation
        if (GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
        blurEffect = VRManager.instance.currentCamera.GetComponent("Blur") as MonoBehaviour;
    }

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
    private float speedMultiplier;
    public bool cutscene;
	
	void Update ()
	{
        if (cutscene && VRManager.VRMode == VRMode.Oculus)
        {
            return;
        }
        speedMultiplier = Mathf.Lerp(0.3f, 2.5f, DataManager.instance.settings.rotationSpeed);
        if (axes == RotationAxes.MouseX)
		{
			XMovement ();
		}
		else if (axes == RotationAxes.MouseY)
		{
			YMovement ();
		}
		else
		{
			XAndYMovement ();
		}
	}

	public float sensitivityX = 15F;

	void XMovement ()
	{
		newTransform.Rotate(0, Input.GetAxis("Mouse X") * Time.unscaledDeltaTime * sensitivityX * speedMultiplier, 0);
        ControllerXMovement(); 
	}

    public float controllerSensitivityX = 15f;

    void ControllerXMovement ()
    {
        float oculusRightAxisX = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch).x;
        float rightAnalogX = (Controller.instance.player.GetAxis("Rotate Horizontal") /*+
            viveRightTouchPadAxisX + oculusRightAxisX*/) * 0.5f * Time.unscaledDeltaTime;
        bool moved = false;
        newTransform.Rotate(0, rightAnalogX * controllerSensitivityX * speedMultiplier, 0);
        moved = true;
    }

	private float rotationY = 0F;
	public float controllerError;
	public float sensitivityY = 15F;
    public float controllerSensitivityY = 15f;
	public float minimumY = -60F;
	public float maximumY = 60F;

	void YMovement ()
	{
        float multiplier = speedMultiplier;
		rotationY += (Input.GetAxis("Mouse Y") * multiplier) * Time.unscaledDeltaTime * sensitivityY;
        float rightAnalogY = (Controller.instance.player.GetAxis("Rotate Vertical") * multiplier) * 0.5f * Time.unscaledDeltaTime;
        rotationY += rightAnalogY * controllerSensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
		newTransform.localEulerAngles = new Vector3(-rotationY, newTransform.localEulerAngles.y, 0);
	}

	void XAndYMovement () 
	{
        float multiplier = speedMultiplier;
        float rotationX = newTransform.localEulerAngles.y + (Input.GetAxis("Mouse X") * sensitivityX * multiplier);
		rotationY += (Input.GetAxis("Mouse Y") * sensitivityY * speedMultiplier);
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
		newTransform.localEulerAngles = new Vector3(-rotationY , rotationX, 0);
	}
}