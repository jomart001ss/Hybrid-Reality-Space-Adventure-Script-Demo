using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public Crosshair crosshair;

    protected Quaternion GetRotationToCrosshair (Transform forward)
    {
        Vector3 directionToCrosshair = crosshair.crosshairPos - forward.position;
        Quaternion rotation = Quaternion.LookRotation(directionToCrosshair);
        return rotation;
    }
}
