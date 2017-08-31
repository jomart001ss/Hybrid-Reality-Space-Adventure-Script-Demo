using UnityEngine;
using System.Collections;

public class MachineBulletEffect : MonoBehaviour {
	public AudioClip machineBulletEffectSound; 
	public GameObject machineBulletEffect; 
	public GameObject machineBulletEffectSpot; 
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.B)){
			
			StartCoroutine(StartEffect()); 
			PlaySound(); 
		}
	}

	IEnumerator StartEffect() {
		GameObject mBE = (GameObject)Instantiate(machineBulletEffect,machineBulletEffectSpot.transform.position,machineBulletEffectSpot.transform.rotation);
		yield return new WaitForSeconds(0.1f);
		//Destroy(weaponUnlock); 
		ParticleManager.instance.RemoveParticles(mBE, false); 
	}

	void PlaySound () {
		AudioManager.instance.Play(machineBulletEffectSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);
	}
}
