using UnityEngine;
using System.Collections;

public class PlayerDamager : Damager
{
    public override Vector3 GetKnockback(out float magnitude)
    {
        magnitude = knockback;
        Vector3 forward = transform.forward;
        Vector3 outputKnockback = forward * knockback;
        outputKnockback.y = Mathf.Clamp(outputKnockback.y, outputKnockback.y, 30f);
        return outputKnockback;
    }

    public AudioClip meshImpactSound;
    public AudioClip rockImpactSound;
    public float volume = 1, range = 500;

    protected void OnCollisionEnter(Collision collision)
    {
        string _tag = collision.collider.tag;
        Vector3 collisionPoint = collision.contacts[0].point;
        if (_tag == "Default")
        {
            if (meshImpactSound != null)
            {
                //PlayImpactSound(collisionPoint, strength, _tag);
            }
            if (impactParticles != null)
            {
                GameObject _particles = Instantiate(impactParticles, collisionPoint, transform.rotation) as GameObject;
                UpdateProjectile(_particles, strength);
            }
        }
        else if (_tag == "Player")
        {
            if (impactParticles != null)
            {
                GameObject _particles = Instantiate(impactParticles, collisionPoint, transform.rotation) as GameObject;
                UpdateProjectile(_particles, strength);
            }
        }
        else
        {
            if (impactParticles != null)
            {
                GameObject _particles = Instantiate(impactParticles, collisionPoint, transform.rotation) as GameObject;
                UpdateProjectile(_particles, strength);
            }
        }
        if (_tag != "Player")
        {
            if (name == "Quick Enemy Projectile(Clone)")
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void PlayImpactSound(Vector3 collisionPoint, float strength, string hitTag)
    {
        if (hitTag == "Mesh" ||
            hitTag == "Door")
        {
            AudioManager.instance.Play(meshImpactSound, collisionPoint, VolumeGroup.SoundEffect, volume, 1, range, true, 256);
        }
        else
        {
            AudioManager.instance.Play(rockImpactSound, collisionPoint, VolumeGroup.SoundEffect, volume, 1, range, true, 256);
        }
    }

    protected virtual void UpdateProjectile(GameObject _particles, float strength) { }
}
