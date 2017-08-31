using UnityEngine;
using System.Collections;

public class TouchPrimaryTriggerMovement : MonoBehaviour
{
    private Transform newTransform;

	void Start ()
    {
        newTransform = GetComponent<Transform>();
	}

    void Update()
    {
        MoveTrigger();
    }

    public enum ControllerSide { Left, Right }
    public ControllerSide hand;
    public Vector3 defaultRotation;
    public Vector3 pressedRotation;

    void MoveTrigger ()
    {
        float axis;
        if (hand == ControllerSide.Left)
        {
            axis = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
        }
        else
        {
            axis = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        }
        Vector3 newRotation = Vector3.Lerp(defaultRotation, pressedRotation, axis);
        newTransform.localEulerAngles = newRotation;
    }
}
