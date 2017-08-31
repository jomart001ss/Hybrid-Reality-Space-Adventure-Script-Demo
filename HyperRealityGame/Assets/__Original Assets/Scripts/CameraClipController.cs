using UnityEngine;
using System.Collections;

public class CameraClipController : Singleton<CameraClipController>
{
	void OnTriggerEnter(Collider otherCollider)
    {
        NotifyClip(otherCollider);
    }

    public delegate void TriggerHandler(Collider otherCollider);
    public event TriggerHandler OnClip;

    void NotifyClip (Collider otherCollider)
    {
        if (OnClip != null)
        {
            OnClip(otherCollider);
        }
    }

    void OnTriggerExit (Collider otherCollider)
    {
        NotifyUnclip(otherCollider);
    }

    public event TriggerHandler OnUnclip;

    void NotifyUnclip (Collider otherCollider)
    {
        if (OnUnclip != null)
        {
            OnUnclip(otherCollider);
        }
    }
}
