using UnityEngine;
using System.Collections;

public class ToggleControllerVisuals : MonoBehaviour {

    public GameObject[] theControllersL;
    public GameObject[] theControllersR;

    int curIndex;
   
	// Use this for initialization
	void Start ()
    {
        ChangeIndex(3);
    }
	
	// Update is called once per frame
	void Update ()
    {   
        if(Input.GetKeyDown(KeyCode.Space))
        {
            curIndex++;
            if(curIndex == theControllersL.Length)
            {
                curIndex = 0;
            }
        }
	}

    public void ChangeIndex(int index)
    {
        curIndex = index;
        ToggleVisuals();
    }

    public void ToggleVisuals()
    {
        for (int i = 0; i < theControllersL.Length; i++)
        {
            if (theControllersL[i].activeInHierarchy)
            {
                theControllersL[i].SetActive(false);
            }
            if (theControllersR[i].activeInHierarchy)
            {
                theControllersR[i].SetActive(false);
            }
        }
        theControllersL[curIndex].SetActive(true);
        theControllersR[curIndex].SetActive(true);
    }
}
