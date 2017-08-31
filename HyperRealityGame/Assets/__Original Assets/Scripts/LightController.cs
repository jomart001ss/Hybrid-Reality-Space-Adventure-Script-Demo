using UnityEngine;
using System.Collections;

/// <summary>
/// Used to light up the enemies' surroundings
/// when they attack.
/// </summary>
public class LightController : MonoBehaviour 
{
    private float defaultExponent = 9.3f;
    private float defaultFogIntensity = 1.1f;
    private float defaultRange = 10f;
	private float maxRange;
    private FoggyLight foggyLight;
    private Light newLight;
    private enum TransitionState { Transitioning, Done }
    private TransitionState transitionState;
    private int level;

	void Awake () 
	{
        foggyLight = GetComponent<FoggyLight>();
        newLight = GetComponent<Light>();
        transitionState = TransitionState.Done;
    }

	public void Fade (float targetFade, float fadeTime = 0.7f) 
	{
		StopAllCoroutines ();
		StartCoroutine (FadeRoutine (targetFade, fadeTime));
	}
	
	IEnumerator FadeRoutine (float targetFade, float fadeTime) 
	{
        transitionState = TransitionState.Transitioning;
        float levelNormalized = Mathf.InverseLerp(1, 100, level);
        float startExponent, endExponent;
        GetExponents(levelNormalized, targetFade, out startExponent, out endExponent);
        float startFogIntensity, endFogIntensity;
        GetFogIntensities(levelNormalized, targetFade, out startFogIntensity, out endFogIntensity);
        float startRange, endRange;
        GetRanges(levelNormalized, targetFade, out startRange, out endRange);
        float timeLeft = fadeTime;
        foggyLight.enabled = true;
        newLight.enabled = true;
        do
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(fadeTime, 0, timeLeft);
            foggyLight.PointLightExponent = Mathf.Lerp(startExponent, endExponent, progress);
            foggyLight.FoggyLightIntensity = Mathf.Lerp(startFogIntensity, endFogIntensity, progress);
            newLight.range = Mathf.Lerp(startRange, endRange, progress);
            yield return null;
        }
        while (timeLeft > 0f);
        transitionState = TransitionState.Done;
    }

    void GetExponents(float levelNormalized, float targetFade, out float startExponent, out float endExponent)
    {
        startExponent = foggyLight.PointLightExponent;
        float maxExponent = Mathf.Lerp(9.3f, 2.07f, levelNormalized);
        endExponent = Mathf.Lerp(defaultExponent, maxExponent, targetFade);
    }

    void GetFogIntensities (float levelNormalized, float targetFade, out float startFogIntensity, out float endFogIntensity)
    {
        startFogIntensity = foggyLight.FoggyLightIntensity;
        float maxFogIntensity = Mathf.Lerp(3f, 1.1f, levelNormalized);
        endFogIntensity = Mathf.Lerp(defaultFogIntensity, maxFogIntensity, targetFade);
    }

    void GetRanges (float levelNormalized, float targetFade, out float startRange, out float endRange)
    {
        startRange = newLight.range;
        float maxRange = Mathf.Lerp(10f, 173.71f, levelNormalized);
        endRange = Mathf.Lerp(defaultRange, maxRange, targetFade);
    }

    public void FadeInAndOut (float fadeInTime = 0.35f, float waitTime = 0.7f, float fadeOutTime = 0.35f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInAndOutRoutine(fadeInTime, waitTime, fadeOutTime));
    }

    IEnumerator FadeInAndOutRoutine (float fadeInTime, float waitTime, float fadeOutTime)
    {
        StartCoroutine(FadeRoutine(1, fadeInTime));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(FadeRoutine(0, fadeOutTime));
    }

    public void UpdateEyeBrightness(int level)
    {
        this.level = level;
        Color color = Color.red;
        foggyLight.PointLightColor = color;
        if (transitionState == TransitionState.Done)
        {
            foggyLight.PointLightExponent = defaultExponent;
            foggyLight.FoggyLightIntensity = defaultFogIntensity;
            newLight.range = defaultRange;
        }
    }
}
