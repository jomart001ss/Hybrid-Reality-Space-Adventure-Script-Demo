using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForwardProjectileMovement : Projectile
{
    [HideInInspector]
    public Vector3 targetPosition;
    [HideInInspector]
    public Vector3 targetNormal;
    [HideInInspector]
    public string targetTag;
    [HideInInspector]
    public Vector3 initPosition;
    public GameObject particles; //possibily not needed
    private Transform newTransform;

    public void Init()
    {
        StopAllCoroutines();
        ResetParticles();
        if (particles == null)
        {
            particles = WeaponManager.instance.hitParticles;
        }
        newTransform = GetComponent<Transform>();
        if (targetTag == "Enemy")
        {
            canBeReused = false;
            StartCoroutine(CreateTrail());
        }
        else
        {
            canBeReused = true;
            StartCoroutine(CreateLongTrail());
        }
    }

    public ParticleSystem[] particleSystems;

    void ResetParticles()
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Clear();
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = true;
        }
    }

    public float longTrailMovementTime = 3f;

    /// <summary>
    /// Moves past the targetPosition and towards the sky.
    /// The projectile is destroyed afterwards without even attempting to deal damage.
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateLongTrail()
    {
        yield return null;
        newTransform.position = initPosition;
        targetPosition = (targetPosition - initPosition).normalized * 1000f + initPosition;
        float timeLeft = longTrailMovementTime;
        do
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(longTrailMovementTime, 0, timeLeft);
            newTransform.position = Vector3.Lerp(initPosition, targetPosition, progress);
            yield return null;
        }
        while (timeLeft > 0);
        //Destroy(gameObject);
        StartCoroutine(RemoveSelf());
    }

    public float movementTime = 0.3f;
    public bool destroyRightAway = false;

    IEnumerator CreateTrail()
    {
        yield return null;
        newTransform.position = initPosition;
        float timeLeft = movementTime;
        do
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(movementTime, 0, timeLeft);
            newTransform.position = Vector3.Lerp(initPosition, targetPosition, progress);
            yield return null;
        }
        while (timeLeft > 0);
        if (target != null) // in case the target is destroyed by this point
        {
            ApplyDamage();
            if (destroyRightAway)
            {
                Destroy();
            }
            else
            {
                StartCoroutine(RemoveSelf());
            }
        }
        else //get rid of the projectile if it's too late
        {
            //Destroy(gameObject); 
            Destroy();
        }
    }

    IEnumerator RemoveSelf()
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = false;
        }
        int particleCount;
        do
        {
            particleCount = 0;
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleCount += particleSystem.particleCount;
            }
            yield return null;
        }
        while (particleCount > 0);
        Destroy();
    }

    void ApplyDamage()
    {
        EnemyDamager enemyDamager = GetComponent<EnemyDamager>();
        //target.tag is used instead of targetTag because the tag of the object may change while shooting
        enemyDamager.ApplyDamage(target, targetPosition, targetNormal, target.tag);
    }
}
