using UnityEngine;
using System.Collections;

public class EnemyDamager : Damager 
{
    void Start () 
	{
		if (impactParticles == null)
		{
			impactParticles = WeaponManager.instance.hitParticles;
		}
	}

    public void ApplyDamage (Transform colliderTransform, Vector3 collisionPoint, Vector3 collisionNormal, string tag)
    {
        if (tag != shooter && tag != "Projectile")
        {
            Damage(colliderTransform, collisionPoint, collisionNormal, tag);
        }
    }

	public AudioClip enemyImpactSound;
    public float emission = 1;

	public void Damage (Transform colliderTransform, Vector3 collisionPoint, Vector3 collisionNormal, string tag)
	{
        if (tag == "Enemy") 
		{
            DamageEnemy(colliderTransform, collisionPoint);
        }
		else
		{
            DamageOther(colliderTransform, collisionPoint);
		}
	}

    protected virtual void DamageEnemy (Transform colliderTransform, Vector3 collisionPoint)
    {
        PlayEnemySound(colliderTransform, collisionPoint);
        //SpawnEnemyParticles(collisionPoint);
        //Rumble();
    }

    protected virtual void PlayEnemySound (Transform colliderTransform, Vector3 collisionPoint)
    {
        EnemyDamageReciever enemyDamageReciever = colliderTransform.GetComponent<EnemyDamageReciever>();
        enemyDamageReciever.AttemptToDamage(this);
        float range = 15.14868f;
        //AudioManager.instance.Play(enemyImpactSound, collisionPoint, VolumeGroup.SoundEffect, 1.0f, 1.0f, range, true, 127);
    }

    protected virtual void SpawnEnemyParticles (Vector3 collisionPoint)
    {
        GameObject _particles = Instantiate(impactParticles, collisionPoint, transform.rotation) as GameObject;
    }


    protected virtual void DamageOther (Transform colliderTransform, Vector3 collisionPoint)
    {
        Instantiate(impactParticles, collisionPoint, transform.rotation);
    }

    /*
    protected virtual void Rumble ()
    {
        Controller.instance.Rumble(3, 0, 1, WeaponManager.instance.burstVibrationCurve);
    }
    */
}
