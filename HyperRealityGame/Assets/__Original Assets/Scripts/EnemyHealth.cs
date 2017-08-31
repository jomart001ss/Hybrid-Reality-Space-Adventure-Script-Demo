using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    public float fullHealth;
    public float health = 100f;

    public void Init()
    {
        fullHealth = fullHealth < health ? health : fullHealth;
    }

    public void RecieveDamage(Damager damager, float multiplier = 1f)
    {
        UpdateHealth(-damager.damage * multiplier);
    }

    private bool notified;

    public void UpdateHealth(float toAdd, bool notifyChanges = true)
    {
        health += toAdd;
        health = Mathf.Clamp(health, 0, fullHealth);
        if (notifyChanges)
        {
            NotifyHit();
            if (health <= 0 && !notified)
            {
                notified = true;
                EnemySpawner.instance.UpdateEnemyCount(-1);
                NotifyDeath(this);
            }
        }
    }

    public delegate void notificationHandler();
    public event notificationHandler OnHit;

    public void NotifyHit()
    {
        if (OnHit != null)
        {
            OnHit();
        }
    }

    public delegate void deathHandler(EnemyHealth enemyHealth);
    public event deathHandler OnDeath;

    public void NotifyDeath(EnemyHealth enemyHealth)
    {
        if (OnDeath != null)
        {
            OnDeath(enemyHealth);
        }
    }

    public delegate void HealHandler();
    public event HealHandler OnHeal;

    public void Heal(float toAdd)
    {
        if (health != fullHealth)
        {
            health += toAdd;
            health = Mathf.Clamp(health, 0, fullHealth);
            OnHeal();
        }
    }

    private void NotifyHeal ()
    {
        if (OnHeal!= null)
        {
            OnHeal();
        }
    }

    void Update()
    {
        KillEnemyCheck();
    }

    /// <summary>
    /// Test method that kills all enemies 
    /// </summary>
    void KillEnemyCheck()
    {
        if (DebugManager.instance.debugMode)
        {
			if (Input.GetKeyDown(KeyCode.T))
            {
                UpdateHealth(-1000000);        
            }
        }
    }
}
