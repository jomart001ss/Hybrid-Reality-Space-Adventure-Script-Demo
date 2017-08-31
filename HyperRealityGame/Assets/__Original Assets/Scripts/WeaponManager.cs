using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : Singleton<WeaponManager>
{
    private Crosshair crosshair;
    public Transform crosshairOrigin;

    void Start ()
    {
        crosshair = crosshairOrigin.GetComponent<Crosshair>();
        UpdateAimingHand(VRManager.primaryController);
        CreatePools();
        VRManager.instance.OnNewPrimaryHand += UpdateAimingHand;
        SwitchWeapon(Unlockable.MachineGun);
    }

    public GameObject machineGunProjectile;

    void CreatePools ()
    {
        PoolManager.instance.CreatePool(machineGunProjectile, 20);
    }

    void UpdateAimingHand (PrimaryController primaryController)
    {
        if (VRManager.VRMode == VRMode.Oculus)
        {
            crosshairOrigin.SetParent(VRManager.instance.primaryHand);
        }
        else if (VRManager.VRMode == VRMode.Desktop)
        {
            crosshairOrigin.SetParent(VRManager.instance.currentCamera.GetComponent<Transform>());
        }
        crosshairOrigin.localPosition = Vector3.zero;
        crosshairOrigin.localRotation = Quaternion.identity;
        crosshair.crosshairOrigin = crosshairOrigin;
        crosshair.ResetCrosshairs();
    }

    public delegate void WeaponSwitchNotifer(Unlockable weapon);
    public event WeaponSwitchNotifer OnWeaponSwitch;
    [HideInInspector]
    public Unlockable currentWeapon;
    
    public bool SwitchWeapon (Unlockable weapon)
    {
        int index;
        UnlockableInfo unlockableInfo = ResourceManager.instance.GetUnlockableInfo(weapon, out index);
        if (currentWeapon != weapon &&
            !unlockableInfo.locked)
        {
            currentWeapon = weapon;
            if (OnWeaponSwitch != null)
            {
                OnWeaponSwitch(weapon);
            }
            return true;
        }
        else
        {
            return false;
        }
    } 

    public GameObject defaultCrosshair, enemyTargetCrosshair;
    public GameObject hitParticles;
    public LayerMask crosshairMask;
}
