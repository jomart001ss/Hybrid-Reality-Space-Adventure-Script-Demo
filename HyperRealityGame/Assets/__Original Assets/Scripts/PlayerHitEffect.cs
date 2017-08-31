using UnityEngine;
using System.Collections;

public class PlayerHitEffect : Singleton<PlayerHitEffect>
{
	public void Init ()
    {
        AddFilter();
    }

    public FilterInfo filter;
    private AmplifyColorEffect amplifyColorEffect;

    void AddFilter()
    {
        amplifyColorEffect = VRManager.instance.currentCamera.gameObject.AddComponent<AmplifyColorEffect>();
        amplifyColorEffect.LutTexture = filter.LutTexture;
        blend = 1;
    }

    private float _blend, _blendNormalized;
    public float blend
    {
        get { return _blendNormalized; }
        set
        {
            _blendNormalized = value;
            _blend = Mathf.Lerp(filter.blend, 1, _blendNormalized);
            amplifyColorEffect.BlendAmount = _blend;
        }
    }

    private Coroutine playHitEffect;

    public void TriggerEffect ()
    {
        StopRoutine(playHitEffect);
        playHitEffect = StartCoroutine(PlayHitEffect());
    }

    void StopRoutine (Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    public float fadeTime = 0.5f;

    IEnumerator PlayHitEffect ()
    {
        float timeLeft = fadeTime;
        do
        {
            timeLeft -= Time.deltaTime;
            float progress = Mathf.InverseLerp(fadeTime, 0f, timeLeft);
            blend = progress;
            yield return null;
        }
        while (timeLeft > 0);
    }
}
