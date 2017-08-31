using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public Vector3 target;
    public GameObject laser;
    public float attackInterval;
    float attCounter;

    public delegate void stateHandler();
    public stateHandler state;
    public EnemyHealth enemyHealth;
	public GameObject self;
	public ConstantForce gravity;
	public Rigidbody rb;
	private SphereCollider myCollider;

    void Start()
    {
        attCounter = attackInterval;
        state = idle;
        enemyHealth.OnDeath += RemoveSelf;
		myCollider = self.GetComponent<SphereCollider>();
    }

    void Update()
    {
        state();
    }

    public void idle()
    {

    }

    public void attacking()
    {
        attCounter -= Time.deltaTime;
        if (attCounter <= 0)
        {
            attCounter = attackInterval;
            GameObject laserInstance = Instantiate(laser, transform.position, transform.rotation) as GameObject;
            ProjectileMovement projectileComp = laserInstance.GetComponent<ProjectileMovement>();
            projectileComp.target = target;
        }
    }

    void RemoveSelf (EnemyHealth enemyHealth)
    {
        enemyHealth.OnDeath -= RemoveSelf;


		StartCoroutine(PermaCorpse());
    }

	IEnumerator PermaCorpse()
	{  
		myCollider.radius = 0.05f;
		rb = self.AddComponent<Rigidbody>();
		gravity = self.AddComponent<ConstantForce>();
		gravity.force = new Vector3(0.0f, -9.81f, 0.0f);
		rb.freezeRotation = true;
		this.GetComponent<EnemyMovement>().enabled = false; 
		this.GetComponent<EnemyAttack>().enabled = false; 
		Debug.Log("Perma");
		yield return new WaitForSeconds(1200.0f);
		Destroy(gameObject);
		Debug.Log("Nent"); 

	}
}
