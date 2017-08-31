using UnityEngine;
using System.Collections;

public class GravityForcePull : MonoBehaviour
{
    private Rigidbody newBody;
    private Transform newTransform;
    private Transform playerTransform;

    void Awake()
    {
        newBody = GetComponent<Rigidbody>();
        newTransform = GetComponent<Transform>();
        playerTransform = Player.instance.positionTransform;
    }

    void OnEnable()
    {
        ForceManager.instance.OnPull += Pull;
        ForceManager.instance.OnPullStop += StopPull;
    }

    public void Pull (float pullTime)
    {
        StopAllCoroutines();
        StartCoroutine(PullRoutine(pullTime));
    }

	IEnumerator PullRoutine(float time)
    {
        float distance = Vector3.Distance(newTransform.position, playerTransform.position);
        float timeLeft = time;
        while (distance >= 1 && timeLeft > 0)
        {
            distance = Vector3.Distance(newTransform.position, playerTransform.position);
            timeLeft -= Time.deltaTime;
            float forceMagnitude =  Time.smoothDeltaTime * 2500f;
            newBody.AddForce((playerTransform.position - newTransform.position) * forceMagnitude);
            yield return null;
		}
	}

    void StopPull ()
    {
        StopAllCoroutines();
    }

    void OnDisable()
    {
        if (ForceManager.instance != null)
        {
            ForceManager.instance.OnPull -= Pull;
            ForceManager.instance.OnPullStop -= StopPull;
        }
    }
}





