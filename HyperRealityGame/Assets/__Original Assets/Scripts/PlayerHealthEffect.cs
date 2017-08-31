using UnityEngine;
using System.Collections;

[System.Serializable]
public class FilterInfo
{
    public Texture LutTexture;
    public float blend;
}

public class PlayerHealthEffect : Singleton<PlayerHealthEffect>
{
    public GameObject toEnable;
    
    public void Init()
    {
        InitializeVolumes();
        toEnable.SetActive(false);
    }

    void InitializeVolumes ()
    {
        VolumeManager.instance.ChangeVolume(1f, VolumeGroup.SoundEffect);
        VolumeManager.instance.ChangeVolume(1f, VolumeGroup.AmbientSound);
        VolumeManager.instance.ChangeVolume(1f, VolumeGroup.BackgroundMusic);
    }

    public void UpdateEffect (float health, float fullHealth)
    {
        float effectThreshold = fullHealth * 0.75f;
        float deathProgress = Mathf.InverseLerp(effectThreshold, 0f, health);
        if (health == 0f)
        {  
            DisplayDeathScreen();
            SetDeathVolume();
        }
        else
        {
            UpdateVolumeGroups(deathProgress);
            UpdateDyingSoundVolume(deathProgress, effectThreshold);
        }
    }

    public AnimationCurve soundEffectCurve;
    public AnimationCurve backgroundMusicCurve;
    public AnimationCurve ambientSoundCurve;

    void UpdateVolumeGroups (float deathProgress)
    {
        float soundEffectVolume = soundEffectCurve.Evaluate(deathProgress);
        float ambientSoundVolume = ambientSoundCurve.Evaluate(deathProgress);
        float backgroundMusicVolume = backgroundMusicCurve.Evaluate(deathProgress);
        VolumeManager.instance.ChangeVolume(soundEffectVolume, VolumeGroup.SoundEffect);
        VolumeManager.instance.ChangeVolume(ambientSoundVolume, VolumeGroup.AmbientSound);
        VolumeManager.instance.ChangeVolume(backgroundMusicVolume, VolumeGroup.BackgroundMusic);
    }

    public AudioClip heartBeatSound;
    private AudioSource heartBeatSource;
    private VolumeController heartBeatVolController;
    public AnimationCurve heartBeatCurve;

    void UpdateDyingSoundVolume (float deathProgress, float effectThreshold)
    {
        if (deathProgress > 0f)
        {
            if (heartBeatSource == null)
            {
                heartBeatSource = AudioManager.instance.PlayLoop(heartBeatSound, Vector3.zero, VolumeGroup.Unaffected, 0f, -1f, 1.0f, 1f, false);
                heartBeatVolController = heartBeatSource.GetComponent<VolumeController>();
            }
            heartBeatVolController.volume = heartBeatCurve.Evaluate(deathProgress);
        }
        else if (heartBeatSource != null)
        {
            Destroy(heartBeatSource.gameObject);
        }  
    }

    void SetDeathVolume ()
    {
        VolumeManager.instance.ChangeVolume(0f, VolumeGroup.SoundEffect);
        VolumeManager.instance.ChangeVolume(0f, VolumeGroup.AmbientSound);
        if (heartBeatSource != null)
        {
            Destroy(heartBeatSource);
        }
    }

    [HideInInspector] public bool endGame;

    void DisplayDeathScreen(CollisionDetection collisionDetection = null)
    {
        if (endGame)
        {
            return;
        }
        Pause();
        endGame = true;
        DestroyedShipSpawner.instance.ToggleSavingState(true);
        SaveManager.instance.SaveGame();
        StartCoroutine(PlayDeathSequence());
    }

    void Pause()
    {
        Debug.Log("PauseManager.instance.Pause(false, false);");
        PauseManager.instance.Pause(false, false);
    }

    public AnimationCurve invertionAnimation;
    public CanvasGroup restartCGroup;

    IEnumerator PlayDeathSequence ()
    {
        yield return StartCoroutine(FreezeScreen(0.2f));
        ShowMessage();
        float fadeTime = 0.7f;
        float timeLeft = fadeTime;
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            float progress = Mathf.InverseLerp(fadeTime, 0f, timeLeft);
            float fadeProgress = 1 - progress;
            float invertionProgress = invertionAnimation.Evaluate(progress);
            //PlayerHitEffect.instance.blend = invertionProgress;
            float canvasGroupProgress = 1 - invertionProgress;
            restartCGroup.alpha = canvasGroupProgress;
            yield return null;
        }
        while (timeLeft > 0f);
    }

    IEnumerator FreezeScreen (float freezeTime)
    {
        float timeLeft = freezeTime;
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }
        while (timeLeft > 0f);
    }

    void ShowMessage()
    {
        toEnable.SetActive(true);
    }

    public void FadeOutInvertion ()
    {
        StartCoroutine(FadeOutInvertionRoutine());
    }

    private bool fadingOutInvertion = false;

    public IEnumerator FadeOutInvertionRoutine ()
    {
        if (fadingOutInvertion)
        {
            yield break;
        }
        fadingOutInvertion = true;
        VolumeManager.instance.ApplyVolumeBeforePause();
        float fadeTime = 0.3f;
        float timeLeft = fadeTime;
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            float progress = Mathf.InverseLerp(fadeTime, 0f, timeLeft);
            float invertionProgress = progress;
            //PlayerHitEffect.instance.blend = invertionProgress;
            VolumeManager.instance.ChangeVolume(progress, VolumeGroup.AmbientSound);
            VolumeManager.instance.ChangeVolume(progress, VolumeGroup.BackgroundMusic);
            yield return null;
        }
        while (timeLeft > 0f);
    }
}
