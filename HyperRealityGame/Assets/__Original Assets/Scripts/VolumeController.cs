using UnityEngine;
using System.Collections;

public class VolumeController : MonoBehaviour
{
    private AudioSource newAudioSource;
    private float baseVolume = 1.0f;
    private float groupVolume = 1.0f;
    public bool updateVolumeAtStart;
    private string _debugName;

    void Awake ()
    {
        newAudioSource = GetComponent<AudioSource>();
        baseVolume = newAudioSource.volume;
        groupVolume = VolumeManager.instance.GetGroupVolume(volumeGroup);
        if (updateVolumeAtStart)
        {
            UpdateAudioSourceVolume();
        }
	}

    void OnEnable ()
    {
        VolumeManager.instance.OnVolumeChange += UpdateVolumeMultiplier;
    }

    [SerializeField]
    private VolumeGroup _volumeGroup;
    public VolumeGroup volumeGroup
    {
        get { return _volumeGroup; }
        set
        {
            _volumeGroup = value;
            groupVolume = VolumeManager.instance.GetGroupVolume(volumeGroup);
        }
    }

    void UpdateVolumeMultiplier (float groupVolume, VolumeGroup volumeGroup)
    {
        if (volumeGroup == this.volumeGroup)
        {
            this.groupVolume = groupVolume;
            UpdateAudioSourceVolume();
        }
    } 

    void UpdateAudioSourceVolume ()
    {
        float newVolume = baseVolume * groupVolume;
        //temporary fix
        if (newAudioSource != null)
        {
            newAudioSource.volume = newVolume;
        }
    }
 
    public float volume
    {
        set
        {
            baseVolume = value;
            UpdateAudioSourceVolume();
        }
        get
        {
            return baseVolume;
        }
    }

    void OnLevelWasLoaded (int sceneIndex)
    {
        //Update subscription with new instance
        groupVolume = VolumeManager.instance.GetGroupVolume(volumeGroup);
        VolumeManager.instance.OnVolumeChange += UpdateVolumeMultiplier;
        UpdateAudioSourceVolume();
    }

    public void Disable (float delay)
    {
        StartCoroutine(DelayedDisable(delay));
    }

    public IEnumerator DelayedDisable(float delay)
    {
        GameObject go = gameObject;
        yield return new WaitForSeconds(delay);
        //if the game restarts by this point it'll still get called
        if (go != null)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDisable ()
    {
        StopAllCoroutines();
        if (VolumeManager.instance != null)
        {
            VolumeManager.instance.OnVolumeChange -= UpdateVolumeMultiplier;
        }
    }

    public void UpdateGroupVolume ()
    {
        groupVolume = VolumeManager.instance.GetGroupVolume(volumeGroup);
    }
}
