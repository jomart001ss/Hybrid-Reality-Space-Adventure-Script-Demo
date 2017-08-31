using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFader : DestructiveSingleton<ScreenFader> 
{
    public Image screen;
	public Image loadingBar, loadingBarBackground;
	private float originaBarAlpha, originalBackBarAlpha;

    //on new scene see if there's a new camera
    new void Awake () 
	{
        base.Awake();
        if (instance == this)
        {
            //screen.gameObject.SetActive (false);
            loadingBarBackground.gameObject.SetActive(false);
            originaBarAlpha = loadingBar.color.a;
            originalBackBarAlpha = loadingBarBackground.color.a;
            ActivateScreenFaderCamera();
        }
	}

    void ActivateScreenFaderCamera ()
    {
        ScreenFaderCamera fader = (ScreenFaderCamera)FindObjectOfType(typeof(ScreenFaderCamera));
        if (fader != null)
        {
            fader.checkScreenFader = true;
        }
    }

	public delegate void genericDelegate ();
	private bool pressed;

    public void LoadLevel(string level, bool fadeIn = true, bool fadeAudio = false)
	{
		pressed = true;
		StartCoroutine (DisplayLoadingScreen (level, -1, fadeIn, fadeAudio));
	}

    public void LoadLevel(int levelIndex, bool fadeIn = true, bool fadeAudio = false)
    {
        pressed = true;
        StartCoroutine(DisplayLoadingScreen("", levelIndex, fadeIn, fadeAudio));
    }

    [HideInInspector] public bool isFading = false;
	public Material fadeMaterial = null;
	public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);
	[HideInInspector] public float fadeVolume;
	public float loadingBarStart, loadingBarEnd;
	[HideInInspector] public bool sceneLoading;

	IEnumerator DisplayLoadingScreen (string level, int levelIndex, bool fadeIn, bool fadeAudio) 
	{
		isFading = true;
        ToggleLayerCameras(false);
        if (fadeIn)
        {
            yield return StartCoroutine(FadeIn(fadeAudio));
        }
        sceneLoading = true;
        AsyncOperation async;
        ToggleLayerCameras(false);
        if (levelIndex != -1)
        {
            async = SceneManager.LoadSceneAsync(levelIndex);
        }
        else
        {
            async = SceneManager.LoadSceneAsync(level);
        }
        yield return async;
        yield return StartCoroutine(FadeOut(fadeAudio));
		isFading = false;
        ToggleLayerCameras(true);
        sceneLoading = false;
	}

    void ToggleLayerCameras (bool topLayer)
    {
        int layer = topLayer ? 1 : -1;
        foreach (Transform layerCameraTransform in VRManager.instance.layerCameras)
        {
            Camera layerCamera = layerCameraTransform.GetComponent<Camera>();
            layerCamera.depth = layer;
        }
    }

    [HideInInspector] public float fadeTime = 2f;

    IEnumerator FadeIn (bool fadeAudio)
    {
        float timeLeft = fadeTime;
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            float progress = Mathf.InverseLerp(fadeTime, 0, timeLeft);
            SetFade(progress);
            if (fadeAudio)
            {
                AudioListener.volume = 1 - progress;
            }
            yield return null;
        }
        while (timeLeft > 0);
    }

    public void SetFade (float progress)
    {
        Color newColor = fadeMaterial.color;
        newColor.a = progress; //no need to convert
        fadeMaterial.color = newColor;
        fadeVolume = 1 - progress;
    }

    IEnumerator FadeOut (bool fadeAudio)
    {
        float timeLeft = fadeTime;
        yield return new WaitForSeconds(1f);
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            float progress = Mathf.InverseLerp(fadeTime, 0, timeLeft);
            SetFade(1 - progress);
            if (fadeAudio)
            {
                AudioListener.volume = progress;
            }
            yield return null;
        }
        while (timeLeft > 0);
    }

    void OnLevelWasLoaded(int sceneIndex)
    {
        ActivateScreenFaderCamera();
    }

    public string mainScene, cutScene;
    public bool test = false;
}
