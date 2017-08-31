using UnityEngine;
using System.Collections;

public class TouchStickMovement : MonoBehaviour
{
    private Transform newTransform;

    void Start ()
    {
        newTransform = GetComponent<Transform>();
    }

	void Update ()
    {
        MoveStick();
    }

    public enum ControllerSide { Left, Right }
    public ControllerSide hand;
    public Vector3 topRotation;
    public Vector3 bottomRotation;
    public Vector3 leftRotation;
    public Vector3 rightRotation;

    void MoveStick ()
    {
        Vector2 coords;
        if (hand == ControllerSide.Left)
        {
            coords = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        }
        else
        {
            coords = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        }
        float verticalAmount = Mathf.InverseLerp(-1, 1, coords.y);
        Vector3 verticalRotation = Vector3.Lerp(bottomRotation, topRotation, verticalAmount);
        float horizontalAmount = Mathf.InverseLerp(-1, 1, coords.x);
        Vector3 horizontalRotation = Vector3.Lerp(leftRotation, rightRotation, horizontalAmount);
        Vector3 newRotation = verticalRotation + horizontalRotation;
        newTransform.localEulerAngles = newRotation;
    }
}
