/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audio Manager.
//
// This code is release under the MIT licence. It is provided as-is and without any warranty.
//
// Developed by Daniel Rodríguez (Seth Illgard) in April 2010
// http://www.silentkraken.com
//
// /////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    public DestructiveSingletonManager dSingletonManager;
    private Transform clipContainer;

    void Awake ()
    {
        if (DestructiveSingletonManager.instance == dSingletonManager)
        {
            CreateClipContainer();
            SceneManager.sceneLoaded += Reinitialize;
        }
    }

    void CreateClipContainer()
    {
        if (clipContainer != null)
        {
            Destroy(clipContainer.gameObject);
        }
        clipContainer = new GameObject().GetComponent<Transform>();
        clipContainer.name = "Clip Container";
    }

    void Reinitialize (Scene scene, LoadSceneMode loadSceneMode)
    {
        CreatePool();
        CreateClipContainer();
    }

    public GameObject clipTemplate;

    void CreatePool()
    {
        PoolManager.instance.CreatePool(clipTemplate, 1000);
    }

    /*
    /// <summary>
    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="point"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource Play(AudioClip clip, Transform emitter, VolumeGroup volumeGroup = VolumeGroup.SoundEffect, float volume = 1f, float pitch = 1f, float range = 1, bool spatial = true, int priority = 128)
    {
        AudioSource source;
        GameObject go = GetClipObject("Audio: " + clip.name, out source);
        go.transform.position = emitter.position;
        go.transform.parent = emitter;
        UpdateSource(source, clip, go, volumeGroup, volume, pitch, range, spatial, priority);
        return source;
    }
    */

    GameObject GetClipObject (string name, out AudioSource source, out bool wasInUse)
    {
        GameObject clipObject = PoolManager.instance.ReuseObject(clipTemplate, Vector3.zero, Quaternion.identity, out wasInUse);
        clipObject.name = name;
        source = clipObject.GetComponent<AudioSource>();
        //source.Stop();
        return clipObject;
    }

    AudioSource UpdateSource (AudioSource source, AudioClip clip, GameObject go, VolumeGroup volumeGroup, float volume, float pitch, float range, bool spatial, int priority, bool wasInUse)
    {
        VolumeController volumeController;
        source = SetSourceProperties(source, go, volumeGroup, out volumeController, volume, pitch, range, spatial, false);
        source.PlayOneShot(clip);
        if (volumeController != null)
        {
            volumeController.StopAllCoroutines();
        }
        StartCoroutine(volumeController.DelayedDisable(clip.length));
        return source;
    }

    AudioSource SetSourceProperties(AudioSource source, GameObject go, VolumeGroup volumeGroup, out VolumeController volumeController, float volume, float pitch, float range = 1, bool spatial = true, bool createController = false)
    {
        if (createController)
        {
            volumeController = go.AddComponent<VolumeController>();   
        }
        else
        {
            volumeController = go.GetComponent<VolumeController>();
        }
        volumeController.volumeGroup = volumeGroup;
        volumeController.volume = volume;
        source.pitch = pitch;
        source.minDistance = range;
        source.spatialBlend = spatial ? 1 : 0;
        source.Play();
        return source;
    }

    /// <summary>
    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="point"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    public AudioSource Play (AudioClip clip, Vector3 point, VolumeGroup volumeGroup = VolumeGroup.SoundEffect, float volume = 1f, float pitch = 1f, float range = 1, bool spatial = true, int priority = 128)
    {
        AudioSource source;
        bool wasInUse;
        GameObject go = GetClipObject("Audio: " + clip.name, out source, out wasInUse);
        if (wasInUse && priority >= source.priority)
        {
            return source;
        }
        go.transform.position = point; 
        UpdateSource(source, clip, go, volumeGroup, volume, pitch, range, spatial, priority, wasInUse);
        return source;
    }

    public AudioSource PlayLoop(AudioClip clip, Transform emitter, VolumeGroup volumeGroup, float volume, float loopTime, float pitch, float range = 1, bool spatial = true, int priority = 128)
    {
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.SetParent(emitter);
        go.transform.localPosition = Vector3.zero;
        AudioSource source = CreateLoopSource(clip, go, volumeGroup, volume, loopTime, pitch, range, spatial, priority);
        return source;
    }

    AudioSource CreateLoopSource (AudioClip clip, GameObject go, VolumeGroup volumeGroup, float volume, float loopTime, float pitch, float range, bool spatial, int priority)
    {
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.priority = priority;
        VolumeController volumeController;
        source = SetSourceProperties(source, go, volumeGroup, out volumeController, volume, pitch, range, spatial, true);
        if (loopTime > 0)
        {
            Destroy(go, loopTime);
        }
        return source;
    }

    public AudioSource PlayLoop(AudioClip clip, Vector3 point, VolumeGroup volumeGroup, float volume, float loopTime, float pitch, float range = 1, bool spatial = true, int priority = 128)
    {
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;
        go.transform.parent = clipContainer;
        AudioSource source = CreateLoopSource(clip, go, volumeGroup, volume, loopTime, pitch, range, spatial, priority);
        return source;
    }

    public IEnumerator AnimateVolume(AudioSource audioSource, AudioClip clip, VolumeController volumeController, AnimationCurve animationCurve)
    {
        float clipLength = clip.length;
        float timeLeft = clipLength;
        while (audioSource != null)
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(clipLength, 0, timeLeft);
            float volumeLevel = animationCurve.Evaluate(progress);
            volumeController.volume = volumeLevel;
            yield return null;
        }
    }

    public IEnumerator AnimateLoopedVolume(AudioSource audioSource, VolumeController volumeController, AnimationCurve animationCurve, float loopLength, bool useCurrentVolAsBase = false)
    {
        float baseVolume = useCurrentVolAsBase ? volumeController.volume : 1f;
        float timeLeft = loopLength;
        while (audioSource != null && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(loopLength, 0, timeLeft);
            float volumeLevel = animationCurve.Evaluate(progress);
            volumeController.volume = volumeLevel * baseVolume;
            yield return null;
        }
    }
}