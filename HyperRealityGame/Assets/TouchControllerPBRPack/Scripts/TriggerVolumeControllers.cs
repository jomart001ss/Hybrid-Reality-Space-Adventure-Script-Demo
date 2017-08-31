using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class TriggerVolumeControllers : MonoBehaviour {

    public UnityEvent theEvent;
	void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Finish")
        {
            theEvent.Invoke();
        }
    }
}
