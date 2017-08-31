using UnityEngine;
using System.Collections;

public class DestroyedShip : MonoBehaviour
{
    private Transform originalTransform;
    private Transform newTransform;
    private RigidbodyConstraints originalConstraints;
    public ObjectIdentifier objectIdentifier;

    public void Init (Transform originalTransform)
    {
        this.originalTransform = originalTransform;
        newTransform = GetComponent<Transform>();
        originalConstraints = newRigidBody.constraints;
        ToggleMesh(false);
        StartCoroutine(FollowOriginal());
    }

    public GameObject mesh;
    public GameObject colliders;
    public Rigidbody newRigidBody;

    public void ToggleMesh(bool activate)
    {
        mesh.SetActive(activate);
        colliders.SetActive(activate);
        objectIdentifier.dontSave = !activate;
        if (activate)
        {
            newRigidBody.constraints = originalConstraints;
            newRigidBody.useGravity = true;
        }
        else
        {
            newRigidBody.constraints = RigidbodyConstraints.FreezeAll;
            newRigidBody.useGravity = false;
        } 
    }

    public void ToggleSaving (bool save)
    {
        objectIdentifier.dontSave = !save;
    }

    IEnumerator FollowOriginal ()
    {
        while (true)
        {
            newTransform.position = originalTransform.position;
            newTransform.rotation = originalTransform.rotation;
            yield return null;
        }
    }
}
