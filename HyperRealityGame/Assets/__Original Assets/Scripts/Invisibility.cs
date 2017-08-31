using UnityEngine;
using System.Collections;

public class Invisibility : MonoBehaviour
{
	public Material invisible;
	public Material realtexture; 
	public bool on;
	public bool off;
    public Renderer[] renderers;

	void Start ()
    {
		off = true;
		on = false; 
	}
	
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Z) ||
            ArduinoManager.instance.GetButtonDownState(4))
        {
			Renderer rend = GetComponent<Renderer>();
			if (off)
            {
                SwitchInvisibility(true);
			}
			else if (on)
            {
                SwitchInvisibility(false);
			}
		}
	}

    void SwitchInvisibility (bool invisibleState)
    {
        on = invisibleState;
        off = !on;
        Material newMaterial = on ? invisible : realtexture;
        foreach (Renderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }

    }
}
