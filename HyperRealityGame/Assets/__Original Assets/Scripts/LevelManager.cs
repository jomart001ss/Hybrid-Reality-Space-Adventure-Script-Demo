using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : Singleton<LevelManager>
{
    private int lastSceneIndex = -1;
    [HideInInspector] public int currentSceneIndex = -1;
    [HideInInspector] public string currentSceneName;
    [HideInInspector] public bool newLevelLoaded;

    void Awake ()
    {
        newLevelLoaded = false;
        Scene currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        SetCurrentLevel();
        SetSceneIndexes(currentScene.buildIndex);
    }

    [HideInInspector] public int currentLevel;

    void SetCurrentLevel()
    {
        if (currentSceneName == "Level One")
        {
            currentLevel = 1;
        }
        else if (currentSceneName == "Level Two")
        {
            currentLevel = 2;
        }
        else if (currentSceneName == "Level Three")
        {
            currentLevel = 3;
        }
    }

    void SetSceneIndexes (int sceneIndex)
    {
        //on new level
        if (sceneIndex == 1 ||
            sceneIndex == 3 ||
            sceneIndex == 4)
        { 
            try
            {
                lastSceneIndex = DataManager.instance.data.currentSceneIndex;
            }
            catch
            {
                Debug.LogError("If you're getting an error here it's probably because you didn't start the cutscene from the start menu");
            }
            
            currentSceneIndex = sceneIndex;
            if (lastSceneIndex != -1 && currentSceneIndex != lastSceneIndex)
            {
                NotifyNewLevel();
            }
        }
        DataManager.instance.RecordSceneIndex(currentSceneIndex);
    }

    public delegate void LevelFinishedHandler();
    public event LevelFinishedHandler OnLevelFinished;

    public void NotifyLevelFinished ()
    {
        if (OnLevelFinished != null)
        {
            OnLevelFinished();
        }
    }

    public delegate void NewLevelHandler();
    public event NewLevelHandler OnNewLevel;

    void NotifyNewLevel ()
    {
        newLevelLoaded = true;
        if (OnNewLevel != null)
        {
            OnNewLevel();
        }
    }

    /// <summary>
    /// All scripts that require OnLevelWasLoaded that aren't DestructiveSingletons
    /// are called from here. The order this event is called in is random so explicitly 
    /// calling them here makes things more gaurenteed.
    /// </summary>
    /// <param name="sceneIndex"></param>
    void OnLevelWasLoaded(int sceneIndex)
    {
        if (sceneIndex == 1 || sceneIndex == 3 || sceneIndex == 4)
        {
            SetSceneIndexes(sceneIndex);
        }
    }

    [HideInInspector] public bool nextLevelTriggered;
}
