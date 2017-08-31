using UnityEngine;
using System.Collections;

public class PoolObject : MonoBehaviour
{
    [HideInInspector]
    public bool canBeReused;

    public virtual void OnObjectReuse()
    {

    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}