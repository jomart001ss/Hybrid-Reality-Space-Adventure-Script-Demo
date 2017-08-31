using UnityEngine;
using System.Collections;

public class WhiteCubeResetter : MonoBehaviour
{
    private Transform newTransform;
    private Rigidbody newBody;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake ()
    {
        newTransform = GetComponent<Transform>();
        newBody = GetComponent<Rigidbody>();
        originalPosition = newTransform.position;
        originalRotation = newTransform.rotation;
        ForceManager.instance.OnReset += Reset;
    }

    void Reset ()
    {
        newBody.velocity = new Vector3(0, 0, 0);
        newTransform.position = originalPosition;
        newTransform.rotation = originalRotation;
    }

    void OnDisable ()
    {
        if (ForceManager.instance != null)
        {
            ForceManager.instance.OnReset -= Reset;
        }
    }
}
