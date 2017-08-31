using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour
{
	void Start ()
    {
        WeaponManager.instance.OnWeaponSwitch += SwitchWeapon;
	}

    public Unlockable weapon;

    void OnTriggerEnter (Collider otherCollider)
    {
        if (otherCollider.transform.parent == VRManager.instance.secondaryHand)
        {
            PlayEnterSound();
            WeaponManager.instance.SwitchWeapon(weapon);
        }
    }

    void PlayEnterSound ()
    {

    }
    
    public GameObject weaponObject;
	
	void SwitchWeapon (Unlockable weapon)
    {
        if (weapon == this.weapon)
        {
            weaponObject.SetActive(true);
        }
        else
        {
            weaponObject.SetActive(false);
        }
    }

    void OnTriggerExit (Collider otherCollider)
    {
        if (weapon == WeaponManager.instance.currentWeapon)
        {
            PlayReleaseSound();
        }
    }

    void PlayReleaseSound ()
    {

    }
}
