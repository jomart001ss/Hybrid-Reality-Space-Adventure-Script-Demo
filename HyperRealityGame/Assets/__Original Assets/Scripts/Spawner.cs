using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Transform[] respawnLocations;
	public GameObject[] whatToSpawnPrefab;
	public GameObject[] whatToSpawnClone;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Respawn(){
		Debug.Log("Respawn"); 
		whatToSpawnClone[0] = Instantiate(whatToSpawnPrefab[0], respawnLocations[0].transform.position, whatToSpawnPrefab[0].transform.rotation) as GameObject;
		whatToSpawnClone[0].name = "FPSController"; 
	}
}
