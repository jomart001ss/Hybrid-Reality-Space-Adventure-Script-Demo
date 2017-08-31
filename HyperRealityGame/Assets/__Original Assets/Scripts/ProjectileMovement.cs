using UnityEngine;
using System.Collections;

public class ProjectileMovement : MonoBehaviour
{
    public Vector3 target;
    public float speed;
    Vector3 movement;
    Vector3 direction;
    public float lifeSpan;
    float lifeCounter;

    void Start()
    {
        direction = Vector3.Normalize(target - transform.position);
        lifeCounter = lifeSpan;
    }
    void Update()
    {

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        lifeCounter -= Time.deltaTime;
        if (lifeCounter <= 0)
        {
            Destroy(gameObject);
        }
    }
}
