using UnityEngine;
using System.Collections;

public class ShipMovementStateSound : MonoBehaviour {
	private bool moving;
	public AudioClip movingSound;
	public AudioClip idleSound;
	// Use this for initialization
	void Start () {
		moving = false; 
	}
		
	// code to refer movement

	void MoveState (){
		if(moving == false){

		AudioManager.instance.Play(idleSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);
		}

		if(moving == true){

		AudioManager.instance.Play(movingSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);
		}
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
