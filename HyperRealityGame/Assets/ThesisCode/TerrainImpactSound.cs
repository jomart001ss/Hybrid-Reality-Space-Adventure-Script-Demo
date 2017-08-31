using UnityEngine;
using System.Collections;

public class TerrainImpactSound : MonoBehaviour {
	public AudioClip terrainImpactSound;
	// Use this for initialization
	void Start () {
	
	}

	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.tag == "Terrain")
		{
			AudioManager.instance.Play(terrainImpactSound, transform.position, VolumeGroup.SoundEffect, 1.0f, 1.0f, 25f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
