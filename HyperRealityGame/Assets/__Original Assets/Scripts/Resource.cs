using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour
{
    public int points;

    void OnTriggerEnter ()
    {
        ResourceManager.instance.UpdatePoints(points);
        SpawnEffect();
        Destroy(gameObject);
    }

    public void SpawnEffect ()
    {

    }
}
