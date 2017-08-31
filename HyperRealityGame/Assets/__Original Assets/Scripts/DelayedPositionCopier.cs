using UnityEngine;
using System.Collections;

public class DelayedPositionCopier : MonoBehaviour 
{
	public GameObject toCopy;

	void OnEnable ()
	{
		if (toCopy != null)
		{
			transform.position = toCopy.transform.position;
		}
	}

	void FixedUpdate ()
	{
		Move ();
	}

	public float speed;
	public float decelerateDist = 0.01f;

	void Move ()
	{
        //if null destroy self
		Vector3 targetPos = toCopy.transform.position;
		Vector3 currentPos = transform.position;
		float distance = Vector3.Distance (currentPos, targetPos);
		float multiplier = Mathf.Clamp01(Mathf.InverseLerp (0, decelerateDist, distance));
		float step = speed * multiplier;
		currentPos = Vector3.MoveTowards (currentPos, targetPos, step);
		transform.position = currentPos;
	}

	public void SnapPosition () 
	{
		transform.position = toCopy.transform.position;
	}
}
