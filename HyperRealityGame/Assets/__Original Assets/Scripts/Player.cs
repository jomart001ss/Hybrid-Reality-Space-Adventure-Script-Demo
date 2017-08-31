using UnityEngine;
using System.Collections;

public class Player : Singleton<Player> 
{
	[HideInInspector] public int score;
	[HideInInspector] public Quaternion rotation;
	[HideInInspector] public Vector3 position;
	public GameObject positionController;
    [HideInInspector] public Transform positionTransform, rotationTransform;
    public CollisionDetection collisionDetection;

	void Awake () 
	{
        positionTransform = positionController.GetComponent<Transform>();
		position = positionTransform.position;
		//rotation = rotationTransform.rotation;
	}
	
	void Update () 
	{
        position = positionTransform.position;
        //rotation = rotationTransform.rotation;//rotation will be off due to camera
	}
}
