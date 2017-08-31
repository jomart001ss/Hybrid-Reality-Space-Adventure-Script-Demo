using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class DropInfo 
{
	public GameObject drop;
	public float chance;
}

public class DropManager : Singleton<DropManager> 
{ 
	public List<DropInfo> drops;

	public GameObject GetDrop () 
	{
		int listLength = drops.Count;
		int index = Random.Range (0, listLength);
		int randomNumber = Random.Range(0, 100);
		for (int i = 0; i < listLength; i++) 
		{
			index = (int)Mathf.Repeat(index + 1, listLength);
			DropInfo drop = drops[index];
			if (drop.chance >= randomNumber) 
			{
				return drop.drop;
			}
		}
		return null;
	}

	public List<GameObject> weapons;

	public GameObject GetWeapon () 
	{
		GameObject weapon;
		if (weapons.Count > 0)
		{
			weapon = weapons[Random.Range(0, weapons.Count)];
		}
		else
		{
			weapon = null;
		}
		return weapon;
	}

	public delegate void genericDelagate ();
	public event genericDelagate OnWeaponRemoval;

	public void RemoveWeapon (GameObject toRemove) 
	{
		weapons.Remove (toRemove);
		if (OnWeaponRemoval != null)
		{
			OnWeaponRemoval();
		}
	}
	
	public GameObject dropSpawner;
}
