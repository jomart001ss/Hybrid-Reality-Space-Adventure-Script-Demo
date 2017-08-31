using UnityEngine;
using System.Collections;

public enum VolumeGroup
{
    SoundEffect = 0,
    BackgroundMusic = 1,
    MenuSound = 2,
    AmbientSound = 3,
    Unaffected = 4
}

public class VolumeManager : Singleton<VolumeManager>
{
    private float[] groupVolume;
    private float[] groupVolsBeforePause;

    void Awake()
    {
        Initialize();
    }

    void Initialize ()
    {
        int groupAmount = System.Enum.GetValues(typeof(VolumeGroup)).Length;
        groupVolume = new float[groupAmount];
        for (int i = 0; i < groupVolume.Length; i++)
        {
            groupVolume[i] = 1.0f;
        }
        groupVolsBeforePause = new float[groupAmount];
    }

    private bool pauseMenuVolApplied = false;

    void ApplyPauseMenuVolume ()
    {
        if (pauseMenuVolApplied)
        {
            return;
        }
        pauseMenuVolApplied = true;
        for (int i = 0; i < groupVolsBeforePause.Length; i++)
        {
            groupVolsBeforePause[i] = groupVolume[i];
        }
        float ambientSoundVol = groupVolume[(int)VolumeGroup.AmbientSound];
        ChangeVolume(ambientSoundVol, VolumeGroup.AmbientSound);
        float backgroundMusicVol = groupVolume[(int)VolumeGroup.BackgroundMusic];
        ChangeVolume(Mathf.Min(0.3f, backgroundMusicVol), VolumeGroup.BackgroundMusic);
        ChangeVolume(0f, VolumeGroup.SoundEffect);
    }

    public void ApplyVolumeBeforePause ()
    {
        if (!pauseMenuVolApplied)
        {
            return;
        }
        pauseMenuVolApplied = false;
        for (int i = 0; i < groupVolsBeforePause.Length; i++)
        {
            ChangeVolume(groupVolsBeforePause[i], (VolumeGroup)i);
        }
    }

    public delegate void VolumeHandler(float groupVolume, VolumeGroup volumeGroup);
    public event VolumeHandler OnVolumeChange;

    public void ChangeVolume (float groupVolume, VolumeGroup volumeGroup)
    {
        if (OnVolumeChange != null)
        {
            int index = (int)volumeGroup;
            //Settings settings = DataManager.instance.settings;
            OnVolumeChange(groupVolume, volumeGroup);
            this.groupVolume[index] = groupVolume;
        }
    }

    public void NotifyVolumeSettingChange (VolumeGroup volumeGroup)
    {
        if (OnVolumeChange != null)
        {
            int index = (int)volumeGroup;
            //Settings settings = DataManager.instance.settings;
            OnVolumeChange(groupVolume[index], volumeGroup);
        }
    }

    public void NotifyMasterVolumeChange ()
    {
        if (OnVolumeChange != null)
        {
            for (int i = 0; i < groupVolume.Length; i++)
            {
                NotifyVolumeSettingChange((VolumeGroup)i);
            }
        }
    }

    public float GetGroupVolume (VolumeGroup volumeGroup)
    {
        int index = (int)volumeGroup;
        //Settings settings = DataManager.instance.settings;
        float groupVolume = this.groupVolume[index];
        return groupVolume;
    }
}
