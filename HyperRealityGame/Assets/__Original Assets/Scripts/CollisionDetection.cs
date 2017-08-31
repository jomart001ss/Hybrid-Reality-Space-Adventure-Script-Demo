using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : Singleton<CollisionDetection> 
{
	public float fullHealth;
	public float health = 100f;
	[HideInInspector] public PlayerKnockback knockback;
    private Transform newTransform;
    private PlayerHealthEffect healthEffect;
    private PlayerHitEffect hitEffect;

	void Start () 
	{
        fullHealth = 100;
        health = 100;
		knockback = gameObject.GetComponent<PlayerKnockback> ();
        newTransform = GetComponent<Transform>();
        healthEffect = GetComponent<PlayerHealthEffect>();
        healthEffect.Init();
        //hitEffect = GetComponent<PlayerHitEffect>();
        //hitEffect.Init();
        healthEffect.UpdateEffect(health, fullHealth);
        SwitchInvincibility (false);
	}

	private bool invincible;
	public List<int> threatLayers;

	public void SwitchInvincibility (bool invincible) 
	{
		this.invincible = invincible;
		foreach (int threatLayer in threatLayers) 
		{
			Physics.IgnoreLayerCollision (gameObject.layer, threatLayer, invincible);
		}
	}

	void OnTriggerEnter (Collider _collider) 
	{
		Damager damager = _collider.gameObject.GetComponent<Damager> ();
		if (damager != null) 
		{
			if (damager.shooter != gameObject.tag) 
			{
				if (!invincible)
				{
					RecieveDamage (damager);
				}
                if (_collider.name == "Quick Enemy Projectile(Clone)")
                {
                    _collider.gameObject.SetActive(false);
                }
                else if (_collider.tag == "Projectile") 
				{
					Destroy(_collider.gameObject);
				}
			}
		}
	}

	void RecieveDamage (Damager damager) 
	{
		bool dead = UpdateHealth(-damager.damage);
        if (!dead)
        {
            //hitEffect.TriggerEffect();
        }
        else
        {
            //EnemyManager.instance.enemyThatKilledPlayer = damager.owner;
            //damager.owner.EnableHighlight();
        }
		KnockBack (damager);
		StartCoroutine (InvincibilityMode ());
        NotifyHit();
    }

	private bool notified;
    public AudioClip hitSound1;
    public AudioClip hitSound2;
	
	public bool UpdateHealth (float toAdd) 
	{
		health += toAdd;
		health = Mathf.Clamp (health, 0, fullHealth);
        healthEffect.UpdateEffect(health, fullHealth);
		if (hitSound1 != null)
		{
            //AudioManager.instance.Play(hitSound1, newTransform.position, VolumeGroup.SoundEffect, 0.8f, 1f, 1f);
            //AudioManager.instance.Play(hitSound2, newTransform.position, VolumeGroup.SoundEffect, 1f, 1f, 1f);
		}
		if (health <= 0 && !notified) 
		{
			notified = true;
			NotifyDeath(this);
            return true;
		}
        return false;
	}

    public void SetHealth(float newHealth)
    {
        health = newHealth;
        health = Mathf.Clamp(health, 0, fullHealth);
        healthEffect.UpdateEffect(health, fullHealth);
        if (health <= 0 && !notified)
        {
            notified = true;
            NotifyDeath(this);
        }
    }

    public delegate void notificationHandler ();
	public event notificationHandler OnHit;
	
	public void NotifyHit() 
	{
		if (OnHit != null) 
		{
			OnHit();
		}
	}

    public delegate void deathHandler(CollisionDetection collisionDetection);
    public event deathHandler OnDeath;
	
	public void NotifyDeath(CollisionDetection collisionDetection) 
	{
		if (OnDeath != null) 
		{
			OnDeath(collisionDetection);
		}
	}

	void KnockBack (Damager damager) 
	{
		float magnitude;
		Vector3 velocity = damager.GetKnockback(out magnitude);
		if (knockback != null)
		{
			knockback.KnockBack(velocity, magnitude);
		}
	}

	public float coolDown;
	
	IEnumerator InvincibilityMode () 
	{
		SwitchInvincibility (true);
		yield return new WaitForSeconds (coolDown);
		SwitchInvincibility (false);
	}

	void OnCollisionEnter (Collision collision) 
	{
		Damager damager = collision.gameObject.GetComponent<Damager> ();
        GameObject go = collision.gameObject;
		if (damager != null) 
		{
			if (damager.shooter != gameObject.tag) 
			{
				if (!invincible)
				{
					RecieveDamage (damager);
				}
                if (go.name == "Quick Enemy Projectile(Clone)")
                {
                    go.SetActive(false);
                }
                else if (go.tag == "Projectile")
                {
                    Destroy(go);
                }
			}
		}
	}

	public void Heal (float toAdd) 
	{
		health += toAdd;
		health = Mathf.Clamp (health, 0, fullHealth);
        healthEffect.UpdateEffect(health, fullHealth);
        NotifyHeal();
    }

    public delegate void HealHandler();
    public event HealHandler OnHeal;

    void NotifyHeal ()
    {
        if (OnHeal != null)
        {
            OnHeal();
        }
    }

    void Update ()
    {
        HealPlayerCheck();
        KillEnemyCheck();
        KillPlayerCheck();
    }

    void HealPlayerCheck ()
    {
        if (DebugManager.instance.debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Heal(100);
            }
        }
    }

    /// <summary>
    /// Test method that kills all enemies 
    /// </summary>
    void KillEnemyCheck ()
    {
        if (DebugManager.instance.debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (tag == "Enemy")
                {
                    UpdateHealth(-1000000);
                }
            }
        } 
    }

    void KillPlayerCheck ()
    {
        if (DebugManager.instance.debugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (tag == "Player")
                {
                    UpdateHealth(-1000000);
                }
            }
        }
    }

    public float healthAmount
    {
        get
        {
            return health / fullHealth;
        }
    }
}
