using UnityEngine;
using System.Collections;

public class PlayerKnockback : MonoBehaviour 
{
    private Rigidbody newRigidBody;

    void Start ()
    {
        newRigidBody = GetComponent<Rigidbody>();
    }

	//Divded with the magnitude of the knockback force to get the length
	public float timeDivider;

	public void KnockBack (Vector3 velocity, float magnitude, bool spawnParticles = true) 
	{
		float time = magnitude / timeDivider;
		StartCoroutine(KnockBackRoutine(velocity, time, spawnParticles));
	}

	public float vibrationMultiplier;
    public GameObject trail;
    public Transform leftTrailCoordinates;
    public Transform rightTrailCoordinates;
    public float knockBackMultiplier;

    IEnumerator KnockBackRoutine (Vector3 velocity, float time, bool spawnParticles) 
	{
        float rumble = time * vibrationMultiplier;
		Controller.instance.Rumble (time, rumble, rumble);
        if (spawnParticles)
        {
            SpawnKnockbackParticles(time);
        }
        while (time > 0)
        {
            if (!PauseManager.instance.paused)
            {
                newRigidBody.AddForce(velocity);
            }
            time -= Time.deltaTime;
            yield return null;
        }
    }

    public void SpawnKnockbackParticles (float lifeTime)
    {
        //StartCoroutine(SpawnParticlesRoutine(lifeTime));
    }

    IEnumerator SpawnParticlesRoutine (float lifeTime)
    {
        GameObject leftTrail = (GameObject)Instantiate(trail, leftTrailCoordinates.transform.position, leftTrailCoordinates.transform.rotation);
        GameObject rightTrail = (GameObject)Instantiate(trail, rightTrailCoordinates.transform.position, rightTrailCoordinates.transform.rotation);
        leftTrail.transform.parent = leftTrailCoordinates.transform;
        rightTrail.transform.parent = rightTrailCoordinates.transform;
        yield return new WaitForSeconds(lifeTime);
        ParticleManager.instance.RemoveParticles(leftTrail, false);
        ParticleManager.instance.RemoveParticles(rightTrail, false);
    }
}
