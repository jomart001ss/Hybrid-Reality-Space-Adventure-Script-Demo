using UnityEngine;
using System.Collections;

public class Billboarder : MonoBehaviour
{
    public bool onlyY = true;
    private Transform newTransform;
    private Transform cameraTransform;

    void Start ()
    {
        newTransform = GetComponent<Transform>();
        cameraTransform = VRManager.instance.currentCamera.GetComponent<Transform>();
    }

	void Update () 
	{
		if (onlyY) 
		{
			Vector3 rotation = newTransform.eulerAngles;
			rotation.y = Quaternion.LookRotation(cameraTransform.position - newTransform.position).eulerAngles.y;
			newTransform.rotation = Quaternion.Euler(rotation);
			//Camera.main.transform.rotation;
		} 
		else 
		{
			newTransform.LookAt(cameraTransform.position);
		}
	}
}
