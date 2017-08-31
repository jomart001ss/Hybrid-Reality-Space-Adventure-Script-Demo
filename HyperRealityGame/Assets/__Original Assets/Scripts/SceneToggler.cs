using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneToggler : MonoBehaviour
{
    private bool newSceneTriggered;

	void Update ()
    {
        if (!newSceneTriggered && 
            Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeScene();
        }
    }

    public string sceneName;

    void ChangeScene ()
    {
        newSceneTriggered = true;
        SceneManager.LoadScene(sceneName);
    }
}
