using UnityEngine;
using System.Collections;

public class MachineGun : Weapon
{
    public Transform leftOrigin;
    public Transform rightOrigin;
	private GameObject leftObject;
	private Transform leftTransform;
	private GameObject rightObject;
	void Start ()
    {
        //SpawnParticles();
        //ToggleMuzzleEffect(false);
    }

    public GameObject muzzleEffect;
    private ParticleSystem leftMuzzleEffect;
    private ParticleSystem rightMuzzleEffect;
	public GameObject bulletShells;


    void SpawnParticles ()
    {
        leftObject = Instantiate(muzzleEffect) as GameObject;
        leftTransform = leftObject.transform;
        leftTransform.SetParent(leftOrigin);
        leftTransform.localPosition = Vector3.zero;
        leftTransform.localRotation = Quaternion.identity;
        leftMuzzleEffect = leftObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        rightObject = Instantiate(muzzleEffect) as GameObject;
        rightMuzzleEffect = rightObject.transform.GetChild(0).GetComponent<ParticleSystem>();

    }

	IEnumerator SpawnBulletShellsLeft ()
	{
	 
		Vector3 theDirection;
		Vector3 theDirectionSide;
		theDirection = transform.TransformDirection(0, 0, 150);
		theDirectionSide = transform.TransformDirection(-45, 0, 0);
		Debug.Log("Spawned"); 
		GameObject bulletShellInstance;
		bulletShellInstance = Instantiate(bulletShells, leftOrigin.position, leftOrigin.rotation) as GameObject;
		bulletShellInstance.AddComponent<Rigidbody>();
		bulletShellInstance.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
		bulletShellInstance.GetComponent<Rigidbody>().AddForce(theDirection);
		bulletShellInstance.GetComponent<Rigidbody>().AddForce(theDirectionSide);




		yield return new WaitForSeconds(600);

		Destroy(bulletShellInstance);
	}

	IEnumerator SpawnBulletShellsRight ()
	{
		
		Vector3 theDirection;
		Vector3 theDirectionSide;
		theDirection = transform.TransformDirection(0, 0, 150);
		theDirectionSide = transform.TransformDirection(45, 0, 0);
		Debug.Log("Spawned2"); 
		GameObject bulletShellInstanceTwo;
		bulletShellInstanceTwo = Instantiate(bulletShells, rightOrigin.position, rightOrigin.rotation) as GameObject;
		bulletShellInstanceTwo.AddComponent<Rigidbody>();
		bulletShellInstanceTwo.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
		bulletShellInstanceTwo.GetComponent<Rigidbody>().AddForce(theDirection);
		bulletShellInstanceTwo.GetComponent<Rigidbody>().AddForce(theDirectionSide);

		yield return new WaitForSeconds(600);

		Destroy(bulletShellInstanceTwo);
	}

    void ToggleMuzzleEffect (bool on)
    {
        leftMuzzleEffect.enableEmission = on;
        rightMuzzleEffect.enableEmission = on;
    }

    private float delayTimeLeft = 0;

    void Update ()
    {
        delayTimeLeft -= Time.deltaTime;
        if (triggerAxis > 0.5f ||
            Input.GetMouseButton(0) ||
            Input.GetKey(KeyCode.Space))
        {
            AttemptToShoot();
			StartCoroutine(SpawnBulletShellsLeft());
			StartCoroutine(SpawnBulletShellsRight());
        }
        else
        {
            //ToggleMuzzleEffect(false);
        }
    }

    private float delay = 0.1f;
    
    void AttemptToShoot ()
    {
        if (delayTimeLeft <= 0)
        {
            delayTimeLeft = delay;
            Shoot();
        }
    }

    public GameObject projectile;

    void Shoot ()
    {
        Shoot(leftOrigin);
        Shoot(rightOrigin);
        //ToggleMuzzleEffect(true);
    }

    void Shoot (Transform forward)
    {
        GameObject projectileInst = PoolManager.instance.ReuseFreeObject(projectile, forward.position, GetRotationToCrosshair(forward));
        if (projectileInst != null)
        {
            ForwardProjectileMovement projectileComp = projectileInst.GetComponent<ForwardProjectileMovement>();
            projectileComp.target = crosshair.hit.transform;
            projectileComp.targetPosition = crosshair.targetPos;
            projectileComp.targetNormal = crosshair.targetNormal;
            projectileComp.targetTag = crosshair.targetTag;
            projectileComp.initPosition = forward.position;
            projectileComp.Init();
        }
    }

    float triggerAxis
    {
        get
        {
            if (VRManager.primaryController == PrimaryController.Left)
            {
                return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch);
            }
            else
            {
                return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch);
            }
        }
    }
}
