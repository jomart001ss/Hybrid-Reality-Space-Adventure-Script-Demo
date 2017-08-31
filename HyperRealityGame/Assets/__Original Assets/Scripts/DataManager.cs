using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Data
{
    public int[] weaponData;
    public int[] bossTriggerData;
    public int[] closedDoorData;
    public int[] completedStateData;
    public int lastState;
    public int checkpointIndex = -1;
    public int currentSceneIndex = -1;
    public int currentWeaponIndex = -1;
    //AreaEnemySpawner data
    public int spawnLevel;
    public int spawnCap;
    public float spawnChance;
    public int spawnAmount;
    public bool gameCleared = false;
    public bool firstEnemyKilled = false;
}

[Serializable]
public class Settings
{
    public float masterVolume;
    public float[] volumeSettings;
    public float rumble = 1;
    public float rotationSpeed = -1;
    public float superSampling = 1;
    public float superSamplingNormalized = -1;
    public int pixelLights = 1;
    public float pixelLightsNormalized = 0;
    public float? headOffsetNormalized = null;
    public float? headOffset = null;
    public bool specularity = false;
    public int controlScheme = 0;
    public int motionControllerHand = 0;
    public int headTracking = 0;
    public bool aimAssist = true;
}

/// <summary>
/// Class that loads and saves data. TempDataManager is used to
/// apply changes, it cannot apply any changes itself. 
/// </summary>
public class DataManager : DestructiveSingleton<DataManager>
{
    private string dataPath;
    private Data _data;
    public Data data { get { return _data; } }
    private Settings _settings;
    public Settings settings { get { return _settings; } }

    new void Awake ()
    {
        base.Awake();
        dataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Hybrid";
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (instance == this)
        {
            CreateNewFolderIfNeeded();
            LoadInSettings();
            if (sceneIndex == 1 ||  
                sceneIndex == 3 ||
                sceneIndex == 4)
            {
                //don't rely on data when working directly from the scene
                //instead make a new one
                CreateNewData();
            }
        }
    }

    void LoadInSettings ()
    {
        string filePath = dataPath + "/Settings.dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            _settings = (Settings)bf.Deserialize(file);
            file.Close();
            CheckForNewSettings();
        }
        else
        {
            CreateNewSettings();
        }
    }

    void CheckForNewSettings ()
    {
        if (settings.headOffset == null)
        {
            InitializeHeadOffset();
        }
    }

    void CreateNewSettings ()
    {
        _settings = new Settings();
        InitializeSettings();
        SaveSettings();
    }

    void InitializeSettings ()
    {
        InitializeMasterVolume();
        InitializeVolumeSettings();
        InitializeRumble();
        InitializeMouseRotation();
        InitializeSuperSampling();
        InitializePixelLights();
        InitializeHeadOffset();
        InitializeSpecularity();
        InitializeControlScheme();
        InitializeMotionControllerHand();
        InitializeHeadTracking();
        InitializeAimAssist();
    }

    void InitializeMasterVolume ()
    {
        settings.masterVolume = 1;
    }

    void InitializeVolumeSettings()
    {
        settings.volumeSettings = new float[Enum.GetNames(typeof(VolumeGroup)).Length];
        for (int i = 0; i < settings.volumeSettings.Length; i++)
        {
            settings.volumeSettings[i] = 1;
        }
    }

    void InitializeRumble ()
    {
        settings.rumble = 1;
    }

    void InitializeMouseRotation ()
    {
        settings.rotationSpeed = 0.327f;
    }

    void InitializeSuperSampling ()
    {
        settings.superSampling = 1;
        settings.superSamplingNormalized = Mathf.InverseLerp(0.5f, 2, settings.superSampling);
    }

    void InitializePixelLights()
    {
        settings.pixelLights = 7;
        settings.pixelLightsNormalized = 1;
    }

    void InitializeHeadOffset ()
    {
        settings.headOffset = 0.3f;
        settings.headOffsetNormalized = Mathf.InverseLerp(-1f, 1.5f, (float)settings.headOffset);
    }

    void InitializeSpecularity()
    {
        settings.specularity = false;
    }

    void InitializeControlScheme ()
    {
        settings.controlScheme = 0;
    }

    void InitializeMotionControllerHand ()
    {
        settings.motionControllerHand = 0;
    }

    void InitializeHeadTracking ()
    {
        //Set to standing by default so the game doesn't seem odd when they first put the headset on
        settings.headTracking = (int)HeadTracking.Standing;
    }

    void InitializeAimAssist ()
    {
        settings.aimAssist = true;
    }

    void CreateNewData ()
    {
        _data = new Data();
        InitializeDataObjects();
    }

    void CreateNewFolderIfNeeded ()
    {
        try
        {
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

        }
        catch (IOException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void InitializeDataObjects()
    {
    }

    public void CheckIfNewProfileNeeded ()
    {
        if (newProfileIndex != -1)
        {
            string filePath = dataPath + "/slot-" + newProfileIndex.ToString() + ".dat";
            newProfileIndex = -1;
            CreateProfileFile(filePath);
        }
    }

    void CreateProfileFile(string filePath)
    {
        CreateNewData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, data);
        file.Close();
    }

    [HideInInspector] public int newProfileIndex = -1;
    [HideInInspector] public int currentProfileIndex = -1;

    public bool CreateProfile(int profileIndex, bool writeOver = false)
    {
        string filePath = dataPath + "/slot-" + profileIndex.ToString() + ".dat";
        if (File.Exists(filePath))
        {
            if (writeOver)
            {
                newProfileIndex = profileIndex;
                currentProfileIndex = profileIndex;
                return true;
            }
        }
        else
        {
            newProfileIndex = profileIndex;
            currentProfileIndex = profileIndex;
            return true;
        }
        return false;
    }

    public void LoadData(int profileIndex)
    {
        string filePath = dataPath + "/slot-" + profileIndex.ToString() + ".dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Open);
        _data = (Data)bf.Deserialize(file);
        file.Close();
        currentProfileIndex = profileIndex;
        ScreenFader.instance.LoadLevel(data.currentSceneIndex);
    }

    public void RecordCompletedState (UIStates UIState)
    {
        data.completedStateData[(int)UIState] = 1;
        Save();
    }

    public void RecordLastestState (UIStates UIState)
    {
        data.lastState = (int)UIState;
        Save();
    }

    public bool IsStateCompleted (UIStates UIState)
    {
        bool completed = data.completedStateData[(int)UIState] == 1;
        //Debug.Log("IsStateCompleted UIState: " + UIState + ", completed: " + completed);
        return completed;
    }

    public void RecordCheckpoint(int checkpointIndex)
    {
        data.checkpointIndex = checkpointIndex;
        Save();
    }

    public void RecordSceneIndex (int currentSceneIndex)
    {
        data.currentSceneIndex = currentSceneIndex;
        Save();
    }

    public void RecordCurrentWeaponIndex (int currentWeaponIndex)
    {
        data.currentWeaponIndex = currentWeaponIndex;
        Save();
    }

    public void RecordGameClearance ()
    {
        data.gameCleared = true;
        Save();
    }

    public void RecordMasterVolume (float volume)
    {
        volume = Mathf.Clamp01(volume);
        settings.masterVolume = volume;
        VolumeManager.instance.NotifyMasterVolumeChange();
        SaveSettings();
    }

    public void RecordVolumeSettings(VolumeGroup volumeGroup, float volume)
    {
        volume = Mathf.Clamp01(volume);
        settings.volumeSettings[(int)volumeGroup] = volume;
        VolumeManager.instance.NotifyVolumeSettingChange(volumeGroup);
        SaveSettings();
    }

    public void RecordRumbleSettings(float rumble)
    {
        rumble = Mathf.Clamp01(rumble);
        settings.rumble = rumble;
        SaveSettings();
    }

    public void RecordMouseRotationSettings (float rotationSpeed)
    {
        Debug.Log("rotationSpeed: " + rotationSpeed);
        settings.rotationSpeed = rotationSpeed;
        SaveSettings();
    }

    public void RecordSuperSamplingSettings (float superSamplingNormalized)
    {
        settings.superSamplingNormalized = superSamplingNormalized;
        settings.superSampling = Mathf.Lerp(0.5f, 2f, settings.superSamplingNormalized);
        SaveSettings();
    }

    public void RecordPixeLightSettings(float pixelLightsNormalized)
    {
        settings.pixelLightsNormalized = pixelLightsNormalized;
        settings.pixelLights = Mathf.RoundToInt(Mathf.Lerp(1, 7, settings.pixelLightsNormalized));
        SaveSettings();
    }

    public void RecordHeadTrackingOffsetSettings(float headOffsetNormalized)
    {
        settings.headOffsetNormalized = headOffsetNormalized;
        settings.headOffset = Mathf.Lerp(-1f, 1.5f, (float)settings.headOffsetNormalized);
        SaveSettings();
    }

    public void RecordSpecularitySettings(bool on)
    {
        settings.specularity = on;
        SaveSettings();
    }

    public void RecordMotionControllerHandSettings (PrimaryController motionControllerHand)
    {
        settings.motionControllerHand = (int)motionControllerHand;
        SaveSettings();
    }

    public void RecordHeadTrackingSettings (HeadTracking headTracking)
    {
        settings.headTracking = (int)headTracking;
        SaveSettings();
        VRManager.instance.NotifyNewHeadTracking(headTracking);
    }

    public void RecordAimAssistSetting(bool on)
    {
        settings.aimAssist = on;
        SaveSettings();
    }

    public ControlScheme GetControlSchemeSettings ()
    {
        return (ControlScheme)settings.controlScheme;
    }

    public float GetVolumeSetting (VolumeGroup volumeGroup)
    {
        return settings.volumeSettings[(int)volumeGroup];
    }

    public HeadTracking GetHeadTrackingSetting ()
    {
        return (HeadTracking)settings.headTracking;
    }

    public int GetCurrentWeaponIndex ()
    {
        int currentWeaponIndex = data.currentWeaponIndex;
        if (currentWeaponIndex == -1)
        {
            return 0;
        }
        else
        {
            return currentWeaponIndex;
        }
    }

    public UIStates GetLastState ()
    {
        return (UIStates)data.lastState;
    }

    public Data GetDataProfile (int profileIndex)
    {
        string filePath = dataPath + "/slot-" + profileIndex.ToString() + ".dat";
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            Data _data = (Data)bf.Deserialize(file);
            file.Close();
            return _data;
        }
        else
        {
            return null;
        }      
    }

    void Save()
    {
        if (currentProfileIndex != -1)
        {
            string filePath = dataPath + "/slot-" + currentProfileIndex.ToString() + ".dat";
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(filePath);
            bf.Serialize(file, data);
            file.Close();
        } 
    }

    void SaveSettings()
    {
        string filePath = dataPath + "/Settings.dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, settings);
        file.Close();
    }
}
