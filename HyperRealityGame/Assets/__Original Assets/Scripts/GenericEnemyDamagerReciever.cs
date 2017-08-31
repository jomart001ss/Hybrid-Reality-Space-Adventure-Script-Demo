using UnityEngine;
using System.Collections;

public class GenericEnemyDamagerReciever : EnemyDamageReciever
{
    public EnemyHealth enemyHealth;

    public override void RecieveDamage(Damager damager)
    {
        enemyHealth.RecieveDamage(damager);
    }
}
