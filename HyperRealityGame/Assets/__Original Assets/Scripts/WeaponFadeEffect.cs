using UnityEngine;
using System.Collections;

public class WeaponFadeEffect : FadeEffect
{
    public GameObject weapon;
    public GameObject newGameobject;

    void OnEnable()
    {
        CameraClipController.instance.OnClip += FadeOut;
        CameraClipController.instance.OnUnclip += FadeIn;
        StopAllCoroutines();
    }

    public void Init (bool UIBar = false)
    {
        newGameobject = gameObject;
        InitMaterialInfoArray();
        if (UIBar)
        {
            SetInitialAlpha();
        }
    }

    void SetInitialAlpha ()
    {
        foreach (WeaponMaterialInfo materialInfo in materialInfoArray)
        {
            Material material = materialInfo.material;
            Color newColor = material.color;
            newColor.a = 0f;
            material.color = newColor;
            float holoMetalicAmount = Mathf.InverseLerp(1, holoAlpha, newColor.a);
            float metalic = Mathf.Lerp(materialInfo.initMetalic, holoMetalic, holoMetalicAmount);
            material.SetFloat("_Metalic", metalic);
        }
    }

    public Collider newCollider;

    void FadeOut(Collider otherCollider)
    {
        if (otherCollider == newCollider)
        {
            ChangeFadeParameters(0, 2f);
        }
    }

    void FadeIn(Collider otherCollider)
    {
        if (otherCollider == newCollider)
        {
            ChangeFadeParameters(holoAlpha, 0.5f);
        }
    }

    public void FadeOutAndDisable(float fadeTime)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAndDisableRoutine(fadeTime));
    }

    IEnumerator FadeOutAndDisableRoutine (float fadeTime)
    {
        ChangeFadeParameters(0, 0.9f);
        float timeLeft = fadeTime;
        do
        {
            timeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }
        while (timeLeft > 0f);
        newGameobject.SetActive(false);
    }

    void OnDisable ()
    {
        StopAllCoroutines();
        if (CameraClipController.instance != null)
        {      
            CameraClipController.instance.OnClip -= FadeOut;
            CameraClipController.instance.OnUnclip -= FadeIn;
        }
    }

    public bool fadedOut
    {
        get { return materialInfoArray[0].material.color.a == 0; }
    }

    public WeaponFloatEffect weaponFloatEffect;
}
