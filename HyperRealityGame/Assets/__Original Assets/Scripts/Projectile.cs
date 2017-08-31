using UnityEngine;
using System.Collections;

public class Projectile : PoolObject 
{
	[HideInInspector] public Transform target;
    protected Vector3 direction;
	public float speed;

    public void SetDirection (Vector3 initPosition, Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - initPosition).normalized;
        this.direction = direction;
    }
}
