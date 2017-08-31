using UnityEngine;
using System.Collections;

public class GravityForcePush : MonoBehaviour
{
	private float forceFactor = 25;
    private Rigidbody newBody;
    private Transform newTransform;
    private Transform playerTransform;

    void Awake()
    {
        newBody = GetComponent<Rigidbody>();
        newTransform = GetComponent<Transform>();
        playerTransform = Player.instance.positionTransform;
    }
    
    void OnEnable ()
    {
        ForceManager.instance.OnPush += Push;
        ForceManager.instance.OnPushStop += StopPush;
    }

    public void Push (float pushTime)
    {
        StopAllCoroutines();
        StartCoroutine(PushRoutine(pushTime));
    }

	IEnumerator PushRoutine(float time)
    {
        float distance = Vector3.Distance(newTransform.position, playerTransform.position);
        float timeLeft = time;
        while (distance <= 25 && distance >= 1 && timeLeft > 0)
        {
            distance = Vector3.Distance(newTransform.position, playerTransform.position);
            timeLeft -= Time.deltaTime;
            float forceMagnitude = Time.smoothDeltaTime * 500f;
			newBody.AddForce((newTransform.position - playerTransform.position) * forceMagnitude);
            yield return null;
		}
	}

    void StopPush ()
    {
        StopAllCoroutines();
    }

    void OnDisable ()
    {
        if (ForceManager.instance != null)
        {
            ForceManager.instance.OnPush -= Push;
            ForceManager.instance.OnPushStop -= StopPush;
        }
    }
}



