using UnityEngine;
using System.Collections;

public class WeaponMaterialInfo
{
    public Material material;
    public float initMetalic;
    public bool goalAlphaReached;
}

public class FadeEffect : MonoBehaviour
{
    private Shader solidShader;
    private Shader translucentShader;

    void Awake ()
    {
        //solidShader = WeaponManager.instance.weaponMaterialSolid;
        //translucentShader = WeaponManager.instance.weaponMaterialTranslucent; 
    }

    protected WeaponMaterialInfo[] materialInfoArray;

    protected void InitMaterialInfoArray()
    {
        Material[] materials = GetComponent<Renderer>().materials;
        materialInfoArray = new WeaponMaterialInfo[materials.Length];
        for (int i = 0; i < materialInfoArray.Length; i++)
        {
            WeaponMaterialInfo materialInfo = new WeaponMaterialInfo();
            Material material = materials[i];
            materialInfo.material = material;
            materialInfo.initMetalic = material.GetFloat("_Metallic");
            materialInfoArray[i] = materialInfo;
        }
    }

    protected void Update()
    {
        ChangeFade();
    }

    private float goalAlpha = 1;
    private float fadeSpeed = 0;
    protected float holoMetalic = 0.5f;
    [HideInInspector] public float holoAlpha = 0.6f;

    protected void ChangeFade()
    {
        foreach (WeaponMaterialInfo materialInfo in materialInfoArray)
        {
            if (materialInfo.goalAlphaReached)
            {
                continue;
            }
            Material material = materialInfo.material;
            Color newColor = material.color;
            float currentAlpha = material.color.a;
            if (currentAlpha != goalAlpha)
            {
                newColor.a = Mathf.MoveTowards(currentAlpha, goalAlpha, Time.unscaledDeltaTime * fadeSpeed);
                material.color = newColor;
                float holoMetalicAmount = Mathf.InverseLerp(1, holoAlpha, newColor.a);
                float metalic = Mathf.Lerp(materialInfo.initMetalic, holoMetalic, holoMetalicAmount);
                material.SetFloat("_Metalic", metalic);
            }
            else
            {
                materialInfo.goalAlphaReached = true;
                if (goalAlpha == 1)
                {
                    materialInfo.material.shader = solidShader;
                    materialInfo.material.renderQueue = 2500;
                }
            }
        }
    }

    public void ChangeFadeParameters(float goalAlpha, float fadeSpeed)
    {
        this.goalAlpha = goalAlpha;
        this.fadeSpeed = fadeSpeed;
        foreach (WeaponMaterialInfo materialInfo in materialInfoArray)
        {
            materialInfo.goalAlphaReached = false;
            materialInfo.material.shader = translucentShader;
        }
    } 
}
