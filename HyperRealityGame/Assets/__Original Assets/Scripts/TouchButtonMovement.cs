using UnityEngine;
using System.Collections;

public class TouchButtonMovement : MonoBehaviour
{
    private Transform newTransform;
    private Vector3 originalPosition;

	void Start ()
    {
        newTransform = GetComponent<Transform>();
        originalPosition = newTransform.localPosition;
	}
	
	void Update ()
    {
        MoveButton();
	}

    public float pressedPosition;
    public OVRInput.Button button;

    void MoveButton ()
    {
        bool pressed = OVRInput.Get(button, OVRInput.Controller.Touch);
        float position = pressed ? pressedPosition : originalPosition.y;
        Vector3 newPosition = originalPosition;
        newPosition.y = position;
        newTransform.localPosition = newPosition;
    }
}
