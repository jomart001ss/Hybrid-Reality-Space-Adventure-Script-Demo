using UnityEngine;
using System.Collections;

public class Disable : MonoBehaviour {
	public bool enabled;
	public bool disabled; 
	public GameObject shield;
	public GameObject cube; 
	// Use this for initialization
	void Start () {
		//shield.SetActive(false);
		//gravityForce = GetComponent<GravityForce>();
		//gravityForce.SetActive(false);
		cube.GetComponent<GravityForcePull>().enabled = false;
		cube.GetComponent<GravityForcePush>().enabled = false;
		enabled = false; 
		disabled = true;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.V)) {
			if(enabled){
			Debug.Log("Disabled");
				shield.SetActive(false);
			enabled = false; 
			disabled = true;
			}
		else if (disabled){
				Debug.Log("Enabled");
				//shield.SetActive(true);
				enabled = true; 
				disabled = false;
			}

	}

		if (Input.GetKeyDown(KeyCode.B)) {
			
			if(enabled){
				Debug.Log("Disabled");
				cube.GetComponent<GravityForcePull>().enabled = false;
				enabled = false; 
				disabled = true;


			}
			else if (disabled){


				StartCoroutine(TimedEnable());
			}
		}

		if (Input.GetKeyDown(KeyCode.N)) {

			if(enabled){
				Debug.Log("Disabled");
				cube.GetComponent<GravityForcePush>().enabled = false;
				enabled = false; 
				disabled = true;


			}
			else if (disabled){


				StartCoroutine(TimedEnableTwo());
			}
		}
			
}

	IEnumerator TimedEnable(){
		Debug.Log("Enabled");
		cube.GetComponent<GravityForcePull>().enabled = true;
		enabled = true; 
		disabled = false;

		yield return new WaitForSeconds(2.5f);

		Debug.Log("Disabled");
		cube.GetComponent<GravityForcePull>().enabled = false;
		enabled = false; 
		disabled = true;

	}

	IEnumerator TimedEnableTwo(){
		Debug.Log("Enabled");
		cube.GetComponent<GravityForcePush>().enabled = true;
		enabled = true; 
		disabled = false;

		yield return new WaitForSeconds(2.5f);

		Debug.Log("Disabled");
		cube.GetComponent<GravityForcePush>().enabled = false;
		enabled = false; 
		disabled = true;

	}


}