using UnityEngine;
using System.Collections;

public class WeaponItemFadeEffect : FadeEffect
{
    void Start()
    {
        InitMaterialInfoArray();
        StartCoroutine(RandomlyChangeFade());
    }

    IEnumerator RandomlyChangeFade ()
    {
        float interval = 1.5f;
        float chanceOfTranslucent = 0.5f;
        while (true)
        {
            float newAlpha;
            if (chanceOfTranslucent > Random.value)
            {
                newAlpha = Random.Range(0f, 1f);
            }
            else
            {
                newAlpha = 1;
            }
            ChangeFadeParameters(newAlpha, 1.5f);
            yield return new WaitForSeconds(interval);   
        }
    }
}
