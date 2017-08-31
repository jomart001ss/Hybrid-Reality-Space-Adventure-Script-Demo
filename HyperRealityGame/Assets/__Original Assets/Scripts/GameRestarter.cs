using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameRestarter : MonoBehaviour
{
    public string startScene;
    private bool restarted;

    void Update()
    {
        GetKeyInput();
    }

    void GetKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7) &&
            !restarted)
        {
            restarted = true;
            RestartGame();
        }
    }

    void RestartGame()
    {
        SaveManager.instance.SaveGame();
        SceneManager.LoadScene(startScene);
    }
}
