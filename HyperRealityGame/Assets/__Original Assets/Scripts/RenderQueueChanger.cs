using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderQueueChanger : MonoBehaviour
{
    public int renderQueue;

	void Start ()
    {
        SetQueue();
    }

    void Update ()
    {
        SetQueue();
    }

    void SetQueue ()
    {
        Renderer[] newRenderers = GetComponents<Renderer>();
        foreach (Renderer newRenderer in newRenderers)
        {
            Material[] sharedMaterials = newRenderer.sharedMaterials;
            foreach (Material sharedMaterial in sharedMaterials)
            {
                sharedMaterial.renderQueue = renderQueue;
            }
            newRenderer.sharedMaterials = sharedMaterials;
        }
    }
}
