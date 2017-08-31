using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour
{
	public AudioClip[] playList;
	public GameObject player; 
	private bool playing;
	private bool stop; 

	void Start ()
    {
		stop = true; 
	}

	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.C) ||
            ArduinoManager.instance.GetButtonDownState(0))
        {
            Stop();
            Play(true);
        }
        if (ArduinoManager.instance.GetButtonDownState(3))
        {
            if (stop)
            {
                Play(false);
            }
            else if (playing)
            {
                Pause();
            }
        }
	}

    private int songIndex = 0;

    void Play (bool randomize = true)
    {
        playing = true;
        stop = false;
        if (randomize)
        {
            songIndex = Random.Range(0, playList.Length);
        }
        GetComponent<AudioSource>().clip = playList[songIndex];
        GetComponent<AudioSource>().Play();
    }

    void Stop ()
    {
        stop = true;
        playing = false;
        GetComponent<AudioSource>().Stop();
    }

    void Pause ()
    {
        stop = true;
        playing = false;
        GetComponent<AudioSource>().Pause();
    }
}
