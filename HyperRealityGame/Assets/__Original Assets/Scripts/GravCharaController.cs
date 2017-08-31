using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GravCharaController : Singleton<GravCharaController>
{
    private Collider newCollider;
    private Transform newTransform;
    private Rigidbody newRigidbody;
    private float height;
    private CollisionDetection collisionDetection;
    private Camera mainCamera;
    private Transform headTracker;
    private bool inSpace = false;

    void Start()
    {
        newCollider = GetComponent<Collider>();
        newTransform = GetComponent<Transform>();
        newRigidbody = GetComponent<Rigidbody>();
        height = newCollider.bounds.extents.y;
        collisionDetection = GetComponent<CollisionDetection>();
        mainCamera = VRManager.instance.currentCamera;
        headTracker = VRManager.instance.headTracker;
        CheckEnvironmentSetting();
        InitiateStopRoutine();
    }

    void CheckEnvironmentSetting ()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        inSpace = sceneName == "Space";
        Debug.Log("inSpace: " + inSpace + ", sceneName: " + sceneName);
    }

    void OnEnable()
    {
        if (newCollider != null)
        {
            height = newCollider.bounds.extents.y;
        }
    }

    void Update()
    {
        if (!PauseManager.instance.paused && PauseManager.instance.framesSinceUnpause > 2)
        {
            ShiftBasedOnCamera();
            JumpCheck();
            NoClipCheck();
            ApplyMovement(); 
        }
    }

    public Transform cameraOffsetPivot;
    public Transform rotationController;

    void ShiftBasedOnCamera ()
    {
        if (VRManager.VRMode == VRMode.Oculus)
        {
            Vector3 currentPosition = cameraOffsetPivot.localPosition;
            Vector3 newPosition = currentPosition;
            Vector3 positionToCopy = rotationController.localRotation * headTracker.localPosition;
            if ((positionToCopy.x - currentPosition.x) != 0)
            {
                newPosition.x += (positionToCopy.x - currentPosition.x);
            }
            if ((positionToCopy.z - currentPosition.z) != 0)
            {
                newPosition.z += (positionToCopy.z - currentPosition.z);
            }
            cameraOffsetPivot.localPosition = newPosition;
        }
    }

    public float jumpSpeed;
    private int framesAfterJump;
    private float timeAfterJump;
    public AudioClip jumpSound;
    public AudioClip rockJumpSound;
    private bool jump;
    private bool jumping;
    private bool canDoubleJump;
    private bool canTrippleJump;

    void JumpCheck()
    {
        GroundedCheck();
        if (Input.GetKeyDown(KeyCode.Space) ||
            Controller.instance.player.GetButtonDown("Jump") /*||
            OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Touch)*/)
        {
            if (grounded) //normal jump
            {
                jump = true;
                jumping = true;
                framesAfterJump = 0;
                timeAfterJump = 0;
                AudioClip sound = groundedTag == "Mesh" ? jumpSound : rockJumpSound;
                AudioManager.instance.Play(sound, Player.instance.position, VolumeGroup.SoundEffect, 0.2f, 1, 20f);
                jumpSpeed = 11.6f;
            }
            else if (!jumping) //mid-air jump  
            {
                jump = true;
                jumping = true;
                framesAfterJump = 0;
                timeAfterJump = 0;
                float jumpPotential = Mathf.InverseLerp(0, maxJumpPotVelocity, newRigidbody.velocity.y);
                jumpSpeed = Mathf.Lerp(11.6f, 37f, jumpPotential);
            }
        }
    }

    public float maxJumpPotVelocity;

    void FixedUpdate()
    {
        if (!noClip && !inSpace)
        {
            ApplyGravity();
        }
    }

    private bool _grounded;
    public bool grounded { get { return _grounded; } }
    private bool movementKeyPressed;

    void ApplyMovement()
    {
        bool jumped;
        Vector3 velocity = GetInput(out jumped);
        ApplyForces(velocity);
        //Debug.Log("framesAfterJumped: " + framesAfterJump);
    }

    public float speed;
    public float noClipSpeed = 6000f;
    public bool noClip = false;

    Vector3 GetInput(out bool jumped)
    {
        GroundedCheck();
        float speed = noClip ? noClipSpeed : this.speed;
        jumped = JumpReactionCheck();
        movementKeyPressed = false;
        Vector3 movement = new Vector3();
        DirectionalMovementJoystick(ref movement);
        DirectionalMovementKeyboard(ref movement);
        if (!noClip)
        {
            DashCheck(ref speed, movement);
        }
        Vector3 velocity = movement * speed;
        return velocity;
    }

    void NoClipCheck()
    {
        if (DebugManager.instance.debugMode)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                SetNoClip(!noClip);
            }
            if (noClip)
            {
                if (Controller.instance.player.GetButton("Increase Speed"))
                {
                    noClipSpeed += 300 * Time.deltaTime;
                }
                else if (Controller.instance.player.GetButton("Decrease Speed"))
                {
                    noClipSpeed -= 300 * Time.deltaTime;
                }
                noClipSpeed = Mathf.Clamp(noClipSpeed, 0, noClipSpeed);
            }
        }
    }

    bool JumpReactionCheck()
    {
        if (jump)
        {
            newRigidbody.velocity += Vector3.up * jumpSpeed;
            jump = false;
            return true;
        }
        return false;
    }


    public LayerMask mask;
    public float jumpError;
    private Vector3 _lastGroundedPosition;
    public Vector3 lastGroundedPosition { get { return _lastGroundedPosition; } }
    private string groundedTag;

    void GroundedCheck()
    {
        bool wasGrounded = _grounded;
        timeAfterJump += Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(newTransform.position, -Vector3.up, out hit, 30f, mask.value))
        {
            framesAfterJump++;
            float _jumpError = framesAfterJump >= 3 ? jumpError : 0;
            _grounded = hit.distance <= height + _jumpError && framesAfterJump >= 6  && timeAfterJump >= 0.2f ? true : false;
            if (_grounded)
            {
                //Debug.Log("jumping = false;");
                //Debug.Log("timeAfterJump: " + timeAfterJump);
                jumping = false;
            }
        }
        else
        {
            _grounded = false;
        }
        if (_grounded)
        {
            _lastGroundedPosition = newTransform.position;
            groundedTag = hit.collider.tag;
        }
        if (!wasGrounded && _grounded)
        {
            NotifyGrounded();
        }
    }

    public delegate void GroundedNotifier();
    public event GroundedNotifier OnGrounded;

    public void NotifyGrounded()
    {
        if (OnGrounded != null)
        {
            OnGrounded();
        }
    }

    public float controllerError;
    public float controllerMultiplier = 1;
    public AnimationCurve viveTouchPadCurve;

    void DirectionalMovementJoystick(ref Vector3 velocity)
    {
        Vector3 movement;
        Vector2 oculusStickAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
        float leftAnalogY = Controller.instance.player.GetAxis("Move Vertical") /* + viveTouchPadY + oculusStickAxis.y*/;
        if (Mathf.Abs(leftAnalogY) >= controllerError)
        {
            movement = new Vector3(0, 0, leftAnalogY * controllerMultiplier);
            velocity += movement;
            movementKeyPressed = true;
        }
        float leftAnalogX = Controller.instance.player.GetAxis("Move Horizontal") /*+ viveTouchPadX + oculusStickAxis.x*/;
        if (Mathf.Abs(leftAnalogX) >= controllerError)
        {
            movement = new Vector3(leftAnalogX * controllerMultiplier, 0, 0);
            velocity += movement;
            movementKeyPressed = true;
        }
        if (noClip)
        {
            if (Controller.instance.player.GetButton("Jump"))
            {
                movement = mainCamera.transform.InverseTransformDirection(Vector3.up) * controllerMultiplier;
                velocity += movement;
                movementKeyPressed = true;
            }
            if (Controller.instance.player.GetButton("Interact"))
            {
                movement = mainCamera.transform.InverseTransformDirection(Vector3.down) * controllerMultiplier;
                velocity += movement;
                movementKeyPressed = true;
            }
        }
    }


    void DirectionalMovementKeyboard(ref Vector3 velocity)
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow)) 
		{
			movement = new Vector3(0,0,1);
			movementKeyPressed = true;
		}
		if (Input.GetKey (KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow))
		{
			movement = new Vector3(0,0,-1);
			movementKeyPressed = true;
		}
		if (Input.GetKey (KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow)) 
		{
			movement = new Vector3(-1,0,0);
			movementKeyPressed = true;
		}	
		if (Input.GetKey (KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow)) 
		{
			movement = new Vector3(1,0,0);
			movementKeyPressed = true;
		}
        velocity += movement.normalized;
	}

    [HideInInspector] public float dashDelayCounter;
    private float dashLengthCounter;
    public float dashDelay;
    public float dashLength;
    public AudioClip dashSound;
    public float dashMultiplier;
    public PlayerKnockback playerKnockback;
    private Coroutine turnOffInvincibility;
    private bool prevDashPress;

    void DashCheck(ref float speed, Vector3 movement)
    {
        dashDelayCounter -= Time.deltaTime;
        dashLengthCounter -= Time.deltaTime;
        dashDelayCounter = Mathf.Clamp(dashDelayCounter, 0, dashDelay);
        bool dashPress = Controller.instance.player.GetButton("Dash");
        if ((Input.GetKeyDown(KeyCode.LeftShift) ||
            (dashPress && !prevDashPress) ||
            Controller.instance.touchLeftTriggerDown) &&
            movement != Vector3.zero)
        {
            if (dashDelayCounter <= (dashDelay / 2f))
            {
                CheckIfDashInstructionCleared();
                dashDelayCounter += dashDelay/2f;
                dashLengthCounter = dashLength;
                playerKnockback.SpawnKnockbackParticles(dashLength);
                AudioManager.instance.Play(dashSound, newTransform.position, VolumeGroup.SoundEffect, 0.6f, 1f, 1, true, 127);
                collisionDetection.SwitchInvincibility(true);
                if (turnOffInvincibility != null)
                {
                    StopCoroutine(turnOffInvincibility);
                }
                turnOffInvincibility = StartCoroutine(TurnOffInvincibility());
            }
        }
        if (dashLengthCounter > 0)
        {
            speed *= dashMultiplier;
        }
        prevDashPress = dashPress;
    }

    IEnumerator TurnOffInvincibility()
    {
        yield return new WaitForSeconds(0.3f);
        collisionDetection.SwitchInvincibility(false);
    }

    void CheckIfDashInstructionCleared ()
    {
        if (UIStateManager.instance.state == UIStates.DashInstruction && movementKeyPressed)
        {
            UIStateManager.instance.TriggerStateChange(UIStates.Default, UIStates.DashInstruction);
        }
    }

    void ApplyForces (Vector3 velocity) 
	{
        Vector3 relativeVelocity;
        if (!noClip)
        {
            relativeVelocity = newTransform.TransformDirection(velocity);
        }
        else
        {
            relativeVelocity = mainCamera.transform.TransformDirection(velocity);
        }
        newRigidbody.AddForce(relativeVelocity);
        if (movementKeyPressed)
        {
            InitiateStopRoutine ();
        }
        Decelerate ();
	}

    public void InitiateStopRoutine ()
    {
        if (stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
        }
        stopRoutine = StartCoroutine(StopRoutine());
    }

	public float gravity;

	void ApplyGravity ()
	{
        Vector3 velocity = new Vector3(0, -gravity, 0);
        Vector3 relativeVelocity;
        if (!noClip)
        {
            relativeVelocity = newTransform.TransformDirection(velocity);
        }
        else
        {
            relativeVelocity = mainCamera.transform.TransformDirection(velocity);
        }
        newRigidbody.AddForce(relativeVelocity);
    }

	public float maxSpeed;
	public AnimationCurve decceleration;

	void Decelerate () 
	{
		Vector3 rigidVelocity = newRigidbody.velocity;
		float speedNormalized;
		speedNormalized = Mathf.Clamp01(Mathf.InverseLerp (0, maxSpeed, Mathf.Abs(rigidVelocity.x)));
		rigidVelocity.x *= decceleration.Evaluate (speedNormalized) * stopMultiplier;
        speedNormalized = Mathf.Clamp01(Mathf.InverseLerp (0, maxSpeed, Mathf.Abs(rigidVelocity.z)));
		rigidVelocity.z *= decceleration.Evaluate(speedNormalized) * stopMultiplier;
        if (noClip)
        {
            speedNormalized = Mathf.Clamp01(Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(rigidVelocity.y)));
            rigidVelocity.y *= decceleration.Evaluate(speedNormalized);
        }
        float minValue = -150f;
        float maxValue = 73.82f;
        rigidVelocity.x = Mathf.Clamp(rigidVelocity.x, minValue, maxValue);
        rigidVelocity.y = Mathf.Clamp(rigidVelocity.y, minValue, maxValue);
        rigidVelocity.z = Mathf.Clamp(rigidVelocity.z, minValue, maxValue);
        newRigidbody.velocity = rigidVelocity;
        if (noClip)
        {
            if (!movementKeyPressed)
            {
                newRigidbody.velocity = Vector3.zero;
            }
        }
    }

    private Coroutine stopRoutine;
    private float stopMultiplier = 1;

    IEnumerator StopRoutine ()
    {
        stopMultiplier = 1;
        yield return new WaitForSeconds(0.1f);
        float stopTime = 7f;
        float timeLeft = stopTime;
        do
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(stopTime, 0f, timeLeft);
            
            stopMultiplier = 1 - progress;
            yield return null;
        }
        while (timeLeft > 0f);
    }

	//might not be used
	void OnCollisionEnter (Collision collision) 
	{
        if (newTransform == null)
        {
            return;
        }
		float difference = newTransform.position.y - collision.contacts [0].point.y - height;
		float error = 0.1f;
		if (difference <= error && difference >= -error) 
		{
			_grounded = true;
		}
	}

    public GameObject weapons;

    public void SetNoClip(bool enabled)
    {
        noClip = enabled;
        if (noClip)
        {
            newCollider.enabled = false;
            weapons.SetActive(false);
            SetOcclusionCulling(false);
        }
        else
        {
            newCollider.enabled = true;
            weapons.SetActive(true);
            SetOcclusionCulling(true);
        }
    }

    public void SetOcclusionCulling(bool enabled)
    {
        mainCamera.useOcclusionCulling = enabled;
    }

    public GameObject godModeIndicator;
    public Text godModeText;
}
