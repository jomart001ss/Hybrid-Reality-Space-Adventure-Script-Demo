using UnityEngine;
using System.Collections;

public class ButtonMovement : MonoBehaviour
{
    public int buttonIndex;
    public Transform pushedTransform;
    private Vector3 pushedPosition;
    private Quaternion pushedRotation;
    private Transform newTransform;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    void Start()
    {
        pushedPosition = pushedTransform.localPosition;
        pushedRotation = pushedTransform.localRotation;
        newTransform = GetComponent<Transform>();
        defaultPosition = newTransform.localPosition;
        defaultRotation = newTransform.localRotation;
    }

    void Update()
    {
        if (ArduinoManager.instance.GetButtonState(buttonIndex))
        {
            newTransform.localPosition = pushedPosition;
            newTransform.localRotation = pushedRotation;
        }
        else
        {
            newTransform.localPosition = defaultPosition;
            newTransform.localRotation = defaultRotation;
        }
    }
}
