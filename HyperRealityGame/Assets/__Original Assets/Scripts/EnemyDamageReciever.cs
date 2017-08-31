using UnityEngine;
using System.Collections;

public class EnemyDamageReciever : MonoBehaviour
{
    public void AttemptToDamage(Damager damager)
    {
        RecieveDamage(damager);
    }

    public virtual void RecieveDamage(Damager damager) { }
}
