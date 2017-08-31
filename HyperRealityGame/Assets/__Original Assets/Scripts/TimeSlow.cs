using UnityEngine;
using System.Collections;

public class TimeSlow : MonoBehaviour
{
	void Update() {
		 
		if (Input.GetKeyDown(KeyCode.X)) {
			Debug.Log("Time");
			if (Time.timeScale == 1.0F)
				Time.timeScale = 0.1F;
			else
				Time.timeScale = 1.0F;
			Time.fixedDeltaTime = 0.01F * Time.timeScale;
		}
	}
}