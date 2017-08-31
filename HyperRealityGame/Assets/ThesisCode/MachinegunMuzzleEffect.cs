using UnityEngine;
using System.Collections;

public class MachinegunMuzzleEffect : MonoBehaviour {
	public AudioClip machinegunMuzzleEffectSound; 
	public GameObject machinegunMuzzleEffect; 
	public GameObject machinegunMuzzleEffectSpot;  
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.G)){
			
			StartCoroutine(StartEffect()); 
			PlaySound(); 
		}
	}

	IEnumerator StartEffect() {
		GameObject mME = (GameObject)Instantiate(machinegunMuzzleEffect,machinegunMuzzleEffectSpot.transform.position,machinegunMuzzleEffectSpot.transform.rotation);
		yield return new WaitForSeconds(0.1f);
		//Destroy(weaponUnlock); 
		Debug.Log("Destroy Particles"); 
		ParticleManager.instance.RemoveParticles(mME, false); 
	}

	void PlaySound () {
		AudioManager.instance.Play(machinegunMuzzleEffectSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);


	}
}
