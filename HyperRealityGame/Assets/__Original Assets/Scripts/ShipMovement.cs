using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShipMovement : MonoBehaviour
{
    private Transform drivingHand;
    public Rigidbody newRigidbody;
    private Transform newTransform;
    private Transform cameraTransform;
    private bool inSpace = false;
	public GameObject groundTrail;
	public GameObject groundSpot;
	private GameObject rockTrailing;

    void Start ()
    {
        UpdateDrivingHand(VRManager.primaryController);
        newTransform = GetComponent<Transform>();
        cameraTransform = VRManager.instance.currentCamera.GetComponent<Transform>();
        VRManager.instance.OnNewPrimaryHand += UpdateDrivingHand;
        CheckEnvironmentSetting();
    }

    void CheckEnvironmentSetting()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        inSpace = sceneName == "Space";
    }

    void UpdateDrivingHand (PrimaryController primaryController)
    {
        drivingHand = VRManager.instance.secondaryHand;
    }
	
	void Update ()
    {
        ApplyRotation();
        ApplyMovement();
	}

    void ApplyRotation ()
    {
        float rotation = GetRotationMovement();
        ApplyRotationalVelocity(rotation);
    }

    public float rotationSpeed;

    float GetRotationMovement ()
    {
        float rotation = 0;
        if (VRManager.VRMode == VRMode.Oculus)
        {
            GetVRRotationInput(ref rotation);
        }
        else if (VRManager.VRMode == VRMode.Desktop)
        {
            GetDesktopRotationInput(ref rotation);
        }
        rotation *= rotationSpeed * Time.deltaTime;
        return rotation;
    }

    public Transform shipTransform;

    void GetVRRotationInput (ref float rotation)
    {
        if (!movementButtonPressed)
        {
            return;
        }
        float newRotation = 0f;
        Vector3 direction3D = drivingHand.position - cameraTransform.position;
        Vector2 direction = Vector3ToVector2(direction3D).normalized;
        Vector2 shipDirection = Vector3ToVector2(shipTransform.right);
        newRotation = Vector2.Dot(shipDirection, direction);
        rotation += newRotation;
    }

    void GetDesktopRotationInput (ref float rotation)
    {
        float newRotation = 0f;
        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            newRotation = -1f;
            movementKeyPressed = true;
        }
        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            newRotation = 1f;
            movementKeyPressed = true;
        }
        rotation += newRotation;
    }

    void ApplyRotationalVelocity (float rotation)
    {
        newTransform.Rotate(0, rotation, 0);
    }

    void ApplyMovement ()
    {
        Vector3 velocity = GetHorizontalMovementInput();
        if (newTransform.position.y >= maxHeight && velocity.y > 0)
        {
            velocity.y = 0;
        }
        ApplyVelocity(velocity);
    }

    public float speed;

    Vector3 GetHorizontalMovementInput ()
    {
        Vector3 movement = Vector3.zero;
        if (VRManager.VRMode == VRMode.Oculus)
        {
            GetHoriztonalVRMovementInput(ref movement);
            GetVerticalVRMovementInput(ref movement);
        }
        else if (VRManager.VRMode == VRMode.Desktop)
        {
            GetHoriztonalDesktopMovementInput(ref movement);
        }
        Vector3 velocity = movement * speed * Time.deltaTime;
        return velocity;
    }

    private bool movementKeyPressed;
    public AnimationCurve movementCurve;

    void GetHoriztonalVRMovementInput (ref Vector3 movement)
    {
        if (!movementButtonPressed)
        {
            return;
        }
        Vector3 newMovement = Vector3.zero;
        Vector2 handPosition = Vector3ToVector2(drivingHand.position);
        Vector2 headPosition = Vector3ToVector2(cameraTransform.position);
        float currentArmDistSqr = Vector2.SqrMagnitude(handPosition - headPosition);
        float movementNormalized = Mathf.InverseLerp(0, VRManager.instance.armLengthSqr, currentArmDistSqr);
        float movementCurved = movementCurve.Evaluate(movementNormalized);
        float forwardMovemenet = Mathf.Lerp(-1f, 1f, movementCurved);
        newMovement.z = forwardMovemenet;
        movement += newMovement;
    }

    bool movementButtonPressed
    {
        get
        {
            bool pressed = false;
            if (VRManager.primaryController == PrimaryController.Left)
            {
                if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
                {
                    pressed = true;
                }
            }
            else if (VRManager.primaryController == PrimaryController.Right)
            {
                if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
                {
                    pressed = true;
                }
            }
            return pressed;
        }
    }

    public float verticalSpeed;
    public float maxHeight;

    void GetVerticalVRMovementInput(ref Vector3 movement)
    {
        if (!movementButtonPressed)
        {
            return;
        }
        Vector3 newMovement = Vector3.zero;
        Vector3 handOrigin = VRManager.instance.currentCamera.transform.position + VRManager.instance.shiftToHandOrigin;
        float currentArmDist = drivingHand.position.y - handOrigin.y;
        if (newTransform.position.y >= maxHeight && currentArmDist > 0)
        {
            currentArmDist = 0;
        }
        float currentArmDistSqr = Mathf.Sign(currentArmDist) * currentArmDist * currentArmDist;
        float movementNormalized = Mathf.InverseLerp(-VRManager.instance.armLengthSqr, VRManager.instance.armLengthSqr, currentArmDistSqr);
        //float movementCurved = movementCurve.Evaluate(movementNormalized);
        float upwardMovement = Mathf.Lerp(-1f, 1f, movementNormalized);
        newMovement.y = upwardMovement * verticalSpeed;
        movement += newMovement;
    }

    Vector2 Vector3ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    void GetHoriztonalDesktopMovementInput (ref Vector3 movement)
    {
        Vector3 newMovement = Vector3.zero;
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow))
        {
            newMovement = new Vector3(0, 0, 1);
            movementKeyPressed = true;

        }
        if (Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow))
        {
            newMovement = new Vector3(0, 0, -1);
            movementKeyPressed = true;
        }
        movement += newMovement.normalized;
    }

    void ApplyVelocity (Vector3 velocity)
    {
        Vector3 relativeVelocity;
        relativeVelocity = newTransform.TransformDirection(velocity);
        newRigidbody.AddForce(relativeVelocity);
        if (movementKeyPressed)
        {
			
            InitiateStopRoutine();
        }
        Decelerate();
    }

    public void InitiateStopRoutine()
    {
        if (stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
        }
        stopRoutine = StartCoroutine(StopRoutine());
    }

    private Coroutine stopRoutine;
    private float stopMultiplier = 1;

    IEnumerator StopRoutine()
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

    public float maxSpeed;
    public AnimationCurve decceleration;

    void Decelerate()
    {
        Vector3 rigidVelocity = newRigidbody.velocity;
        float speedNormalized;
        speedNormalized = Mathf.Clamp01(Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(rigidVelocity.x)));
        rigidVelocity.x *= decceleration.Evaluate(speedNormalized) * stopMultiplier;
        speedNormalized = Mathf.Clamp01(Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(rigidVelocity.z)));
        rigidVelocity.z *= decceleration.Evaluate(speedNormalized) * stopMultiplier;
        float minValue = -150f;
        float maxValue = 73.82f;
        rigidVelocity.x = Mathf.Clamp(rigidVelocity.x, minValue, maxValue);
        rigidVelocity.y = Mathf.Clamp(rigidVelocity.y, minValue, maxValue);
        rigidVelocity.z = Mathf.Clamp(rigidVelocity.z, minValue, maxValue);
        newRigidbody.velocity = rigidVelocity;
    }

    void FixedUpdate ()
    {
        if (!inSpace)
        {
            ApplyGravity();
        }
    }

    public float gravity;

    void ApplyGravity()
    {
        Vector3 velocity = new Vector3(0, -gravity, 0);
        Vector3 relativeVelocity;
        relativeVelocity = newTransform.TransformDirection(velocity);
        newRigidbody.AddForce(relativeVelocity);
    }
		
	void OnCollisionStay (Collision col)
	{
		if (Input.GetKey(KeyCode.W) ||
			Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S) ||
			Input.GetKey(KeyCode.DownArrow))
		{
		if(col.gameObject.tag == "Terrain")
		{
				Debug.Log("Rock");

			rockTrailing = Instantiate(groundTrail, groundSpot.transform.position, groundSpot.transform.rotation) as GameObject;
			rockTrailing.AddComponent<Rigidbody>();
			rockTrailing.GetComponent<Rigidbody>().AddForce(Vector3.up * 15);
				StartCoroutine(TrailingDelete());
		}
		}
	}

	IEnumerator TrailingDelete(){
		yield return new WaitForSeconds(1200.0f);
		Destroy(rockTrailing); 
	}

}
