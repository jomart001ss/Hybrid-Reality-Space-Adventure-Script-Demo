using UnityEngine;
using System.Collections;

public class Damager : MonoBehaviour 
{
	public float damage;
	public string shooter;
	public float knockback;
	public GameObject impactParticles;
    [HideInInspector] public float strength;
    public AttackManager owner;

	public virtual Vector3 GetKnockback (out float magnitude)
	{
        //placeholders
		magnitude = 0;
		return new Vector3 (0,0,0); 
	}
}
