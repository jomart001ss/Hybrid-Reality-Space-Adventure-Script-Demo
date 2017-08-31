using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour 
{
	void Start () 
	{
		ActivateCrosshair ();
    }

    public Transform crosshairOrigin;

    void ActivateCrosshair ()
	{
        ResetCrosshairs();
        InitiateValues();
        crossHairMask = WeaponManager.instance.crosshairMask;
        VRManager.instance.OnNewPrimaryHand += SetAimingDevice;
        if (VRManager.VRMode == VRMode.Desktop)
        {
            aimingDevice = VRManager.instance.currentCamera.GetComponent<Transform>();
        }
	}

    void SetAimingDevice (PrimaryController primaryController)
    {
        aimingDevice = VRManager.instance.primaryHand;
    }
	
	//Crosshairs specific to the weapon. Currently not used
	public GameObject specialDefaultCrosshair, specialEnemyTargetCrosshair;
	private Transform defaultCrosshair, enemyTargetCrosshair;
	private Transform currentCrosshair;
    private Transform newTransform;

	void InstantiateCrosshairs ()
	{
        newTransform = GetComponent<Transform>();
		GameObject toSpawn;
		toSpawn = specialDefaultCrosshair != null ? specialEnemyTargetCrosshair : WeaponManager.instance.defaultCrosshair;
		defaultCrosshair = ((GameObject)Instantiate (toSpawn, crosshairOrigin.position, crosshairOrigin.rotation)).GetComponent<Transform>();
        defaultCrosshair.SetParent(crosshairOrigin.parent);
        defaultCrosshair.localPosition = crosshairOrigin.localPosition;
        defaultCrosshair.localRotation = crosshairOrigin.localRotation;
		toSpawn = WeaponManager.instance.enemyTargetCrosshair;
		enemyTargetCrosshair = ((GameObject)Instantiate (toSpawn, crosshairOrigin.position, crosshairOrigin.rotation)).GetComponent<Transform>();
        enemyTargetCrosshair.SetParent(crosshairOrigin.parent);
        enemyTargetCrosshair.localPosition = crosshairOrigin.localPosition;
		currentCrosshair = defaultCrosshair;
        SetCrosshairType();
	}

    private int crosshairType;

    void SetCrosshairType()
    {
        string crosshairName = crosshairOrigin.name;
        string[] nameSplit = crosshairName.Split(' ');
        string type = nameSplit[0];
        if (type == "Single")
        {
            crosshairType = 1;
        }
        else if (type == "Double")
        {
            crosshairType = 2;
        }
        else if (type == "Triple")
        {
            crosshairType = 3;
        }
    }

    private Camera mainCamera;
    private Transform aimingDevice;
    private LayerMask crossHairMask;
    private LayerMask floorMask;
    private Material enemyTargetMat;
    private Material defaultTargetMat;

    void InitiateValues ()
    {
        mainCamera = VRManager.instance.currentCamera;
        enemyTargetMat = enemyTargetCrosshair.GetComponent<Renderer>().material;
        enemyTargetMat.renderQueue = 50000;
        defaultTargetMat = defaultCrosshair.GetComponent<Renderer>().material;
        defaultTargetMat.renderQueue = 50000;
    }

    void Update()
    {
        if (aimingDevice == null)
        {
            enemyTargetCrosshair.gameObject.SetActive(false);
            defaultCrosshair.gameObject.SetActive(false);
            return;
        }
        UpdateCrosshair();
    }

    [HideInInspector] public Vector3 crosshairPos;
    [HideInInspector] public Quaternion crosshairRotation;
    [HideInInspector] public Vector3 crosshairForward;
	
	void UpdateCrosshair () 
	{
        UpdateTransformations(VRManager.instance.primaryHand.forward);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(crosshairPos);
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, crossHairMask)) 
		{
            ApplyTargetSettings (hit);
		}
		else
		{
			ApplySkySettings ();
		}
        currentCrosshair.position = crosshairPos; //update new switched in crosshair
    }

    void UpdateTransformations(Vector3 clampedForward)
    {
        Vector3 newPosition = crosshairOrigin.localPosition;
        newPosition.z = 25;
        currentCrosshair.localPosition = newPosition;
        //currentCrosshair.transform.position = crosshairOrigin.position + clampedForward * 25;
        crosshairPos = currentCrosshair.position;
        crosshairRotation = currentCrosshair.rotation;
        UpdateScale();
    }

    [HideInInspector]
    public float scaleDistance;
    public float scaleDivider = 4f;

    /// <summary>
    /// Scale crosshair to make it seem the same size
    /// regardless of how far it is from the camera
    /// </summary>
    void UpdateScale()
    {
        float defaultScale = 2.5f / scaleDivider;
        float enemyScale = 2.5f / scaleDivider;
        float scale = _state == "Enemy" ? enemyScale : defaultScale;
        Plane plane = new Plane(aimingDevice.forward, aimingDevice.position);
        scaleDistance = plane.GetDistanceToPoint(crosshairPos);
        float newScale = scale * scaleDistance;
        currentCrosshair.localScale = Vector3.one * newScale / scaleDivider;
    }

    private string _state, prevState;
	public AudioClip enemyTargetedSound;
	[HideInInspector] public Vector3 targetPos;
    [HideInInspector] public Vector3 targetNormal;
    [HideInInspector] public float distance;
    [HideInInspector] public RaycastHit hit;
    [HideInInspector] public string targetTag;

	/// <summary>
	/// Settings when a target is found infront of gun
	/// </summary>
	void ApplyTargetSettings (RaycastHit hit)
	{
        targetTag = hit.collider.tag;
        if (targetTag != "Projectile")
		{
			SetTraits(targetTag, hit.collider);
            if (prevState != _state && 
                (_state == "Enemy" || _state == "Armor") &&
                enemyTargetedSound != null) 
			{
				AudioManager.instance.Play(enemyTargetedSound, newTransform.position, VolumeGroup.SoundEffect, 0.01f, 0.75f, 0, false);
			}
		}
        this.hit = hit;
		targetPos = hit.point;
        targetNormal = hit.normal;
        distance = hit.distance;
	}

	void SetTraits (string tag, Collider colliderHit)
	{
		prevState = _state;
		_state = tag;
        //enemy check is first because miss marks may be found even if there aren't any enemies
        if (tag == "Enemy") 
		{
            enemyTargetCrosshair.gameObject.SetActive(true);
			currentCrosshair = enemyTargetCrosshair;
            NotifyEnemySpotted();
		}
        else
        {
            enemyTargetCrosshair.gameObject.SetActive(false);
            currentCrosshair = defaultCrosshair;
        } 
	}

    public delegate void EnemySpottedHandler();
    public static event EnemySpottedHandler OnEnemySpotted;

    void NotifyEnemySpotted ()
    {
        if (OnEnemySpotted != null)
        {
            OnEnemySpotted();
        }
    }

	/// <summary>
	/// Settings when the gun is targeted at the sky
	/// </summary>
	void ApplySkySettings ()
	{
        targetTag = "Other";
		SetTraits(targetTag, null);//safe to shoot but not the enemy
		targetPos = currentCrosshair.position;
        distance = 1000;
	}

    public void ResetCrosshairs ()
    {
        if (defaultCrosshair != null)
        {
            Destroy(defaultCrosshair.gameObject);
            Destroy(enemyTargetCrosshair.gameObject);
        }
        InstantiateCrosshairs();
    }
}
