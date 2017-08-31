using UnityEngine;
using System.Collections;

public class CheckpointTrigger : MonoBehaviour
{
    public Checkpoint checkpoint;

    void OnTriggerEnter(Collider _collider)
    {
        checkpoint.Trigger();
    }
}
