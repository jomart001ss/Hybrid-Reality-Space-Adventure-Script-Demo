using UnityEngine;
using System.Collections;

public class WeaponDelayedCameraRotCopier : MonoBehaviour
{
    void Start()
    {
        InitializeTransforms();
    }

    private GameObject toCopy;
    private Transform transformToCopy;
    private Transform newTransform;

    void InitializeTransforms()
    {
        transformToCopy = toCopy.GetComponent<Transform>();
        newTransform = GetComponent<Transform>();
        Transform child = newTransform.GetChild(0).GetChild(0).GetChild(0);
        crosshair = child.GetComponent<Crosshair>();
    }

    private Crosshair crosshair;

    void OnEnable()
    {
        if (toCopy != null)
        {
            newTransform.localRotation = transformToCopy.localRotation;
        }
    }

    void Update()
    {
        Rotate();
    }

    public float minSpeed;
    public float maxSpeed;
    public float maxDifference;
    public AnimationCurve speedAnimation;

    void Rotate()
    {
        Quaternion targetRot = transformToCopy.localRotation;
        Vector3 targetRotEuler = targetRot.eulerAngles;
        targetRotEuler.x = 0;
        targetRotEuler.z = 0;
        targetRot = Quaternion.Euler(targetRotEuler);
        Quaternion currentRot = newTransform.localRotation;
        float difference = Quaternion.Angle(targetRot, currentRot);
        float speedAmount = Mathf.InverseLerp(0, maxDifference, difference);
        speedAmount = speedAnimation.Evaluate(speedAmount);
        float speed = Mathf.Lerp(minSpeed, maxSpeed, speedAmount);
        float step = speed * Time.deltaTime;
        currentRot = Quaternion.RotateTowards(currentRot, targetRot, step);
        newTransform.localRotation = currentRot;
    }

    public void SnapRotation()
    {
        if (newTransform == null)
        {
            InitializeTransforms();
        }
        newTransform.localRotation = transformToCopy.localRotation;
    }
}
