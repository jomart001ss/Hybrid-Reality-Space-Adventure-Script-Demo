using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public float minPosition;
    public float maxPosition;
    private CollisionDetection collisionDetection;
    private Transform newTransform;

	void Start ()
    {
        collisionDetection = CollisionDetection.instance;
        newTransform = GetComponent<Transform>();
    }

    public float animationSpeed;
	
	void Update ()
    {
        float healthAmount = collisionDetection.healthAmount;
        Vector3 currentPosition = newTransform.localPosition;
        Vector3 newPosition = currentPosition;
        newPosition.y = Mathf.Lerp(minPosition, maxPosition, healthAmount);
        newTransform.localPosition = Vector3.MoveTowards(currentPosition, newPosition, animationSpeed * Time.deltaTime);
	}
}
