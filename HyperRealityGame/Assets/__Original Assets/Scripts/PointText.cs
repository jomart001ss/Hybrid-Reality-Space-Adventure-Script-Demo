using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PointText : MonoBehaviour
{
    private Text text;
    
	void Start ()
    {
        text = GetComponent<Text>();
	}

    private int displayedPoints = 0;
    public float speed = 10;

    void Update ()
    {
        displayedPoints = (int)Mathf.MoveTowards(displayedPoints, ResourceManager.instance.currentPoints, speed * Time.deltaTime);
        text.text = displayedPoints.ToString("D6");
	}
}
