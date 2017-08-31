using UnityEngine;
using System.Collections;

public class ShootingSound : MonoBehaviour {
	public AudioClip shootingSound; 
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.S)){
			PlaySound();
		}
	}

	void PlaySound () {
		AudioManager.instance.Play(shootingSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);
	}
}
