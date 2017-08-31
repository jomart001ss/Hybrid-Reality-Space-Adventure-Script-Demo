using UnityEngine;
using System.Collections;

public class ParasiteImpactSound : MonoBehaviour {
	public AudioClip parasiteImpactSound;
	// Use this for initialization
	void Start () {

	}

	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.tag == "Parasite")
		{
			AudioManager.instance.Play(parasiteImpactSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
