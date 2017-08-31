using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Collider[] triggers;

    void Start ()
    {
        InitiateTriggers();
    }

    void InitiateTriggers ()
    {
        triggers = GetComponentsInChildren<Collider>();
        foreach (Collider trigger in triggers)
        {
            CheckpointTrigger checkpointTrigger = trigger.gameObject.AddComponent<CheckpointTrigger>();
            checkpointTrigger.checkpoint = this;
        }
    }

	public Transform checkpoint;
	public GameObject message;

	public void Trigger ()
    {
    }
}
