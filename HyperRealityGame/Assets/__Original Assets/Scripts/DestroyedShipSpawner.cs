using UnityEngine;
using System.Collections;

public class DestroyedShipSpawner : Singleton<DestroyedShipSpawner>
{
    public GameObject destroyedShip;
    private DestroyedShip destroyedShipComp;

    void Start ()
    {
        GameObject ship = Instantiate(destroyedShip) as GameObject;
        destroyedShipComp = ship.GetComponent<DestroyedShip>();
        destroyedShipComp.Init(transform);
    }

    public void ToggleSavingState (bool on)
    {
        destroyedShipComp.ToggleSaving(on);
    }
}
