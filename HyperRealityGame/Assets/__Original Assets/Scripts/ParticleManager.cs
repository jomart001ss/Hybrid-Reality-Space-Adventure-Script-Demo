using UnityEngine;
using System.Collections;

public class ComplexParticleInformation
{
    public bool hasLight;
    public float initialLightRange;
    public float initialParticleCount;
    public Light light;
    public float progress;
}

public class ParticleManager : Singleton<ParticleManager>
{
    public Coroutine RemoveParticles(GameObject particleInst, bool unParent, Light light = null)
    {
        if (unParent)
        {
            particleInst.GetComponent<Transform>().SetParent(null);
        }
        return StartCoroutine(RemoveParticles(particleInst, light));
    }

    IEnumerator RemoveParticles(GameObject particleInst, Light light)
    {
        ParticleSystem particleSystm = particleInst.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule emission = particleSystm.emission;
        emission.enabled = false;
        int initialParticleCount = 0;
        float initialLightRange = 0f;
        if (light != null)
        {
            initialParticleCount = particleSystm.particleCount;
            initialLightRange = light.range;
        }
        while (particleSystm.particleCount >= 0f)
        {
            if (light != null)
            {
                float progress = Mathf.InverseLerp(initialParticleCount, 0, particleSystm.particleCount);
                float lightRange = Mathf.Lerp(initialLightRange, 0f, progress);
                light.range = lightRange;
            }
            yield return null;
        }
        Destroy(particleInst);
    }

    public Coroutine RemoveComplexParticles(GameObject particleInst, bool unParent, bool fadeLights = false)
    {
        if (unParent)
        {
            particleInst.GetComponent<Transform>().SetParent(null);
        }
        return StartCoroutine(RemoveComplexParticlesRoutine(particleInst, fadeLights));
    }

    IEnumerator RemoveComplexParticlesRoutine(GameObject particleInst, bool fadeLights)
    {
        ParticleSystem[] particleSystems = particleInst.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = false;
        }
        ComplexParticleInformation[] complexParticleInformation = null;
        if (fadeLights)
        {
            complexParticleInformation = GetComplexParticleInformation(particleSystems);
        }
        int totalParticleCount = 0;
        do
        {
            totalParticleCount = 0;
            for (int i = 0; i < particleSystems.Length; i++)
            {
                totalParticleCount += particleSystems[i].particleCount;
            }
            if (fadeLights)
            {
                UpdateComplexLights(complexParticleInformation, particleSystems);
            }
            yield return null;
        }
        while (totalParticleCount > 0);
        Destroy(particleInst);
    }

    ComplexParticleInformation[] GetComplexParticleInformation (ParticleSystem[] particleSystems)
    {
        ComplexParticleInformation[] complexParticleInformation = new ComplexParticleInformation[particleSystems.Length];
        for (int i = 0; i < complexParticleInformation.Length; i++)
        {
            ParticleSystem particleSystem = particleSystems[i];
            Light light = particleSystem.GetComponentInChildren<Light>();
            bool hasLight = light != null;
            complexParticleInformation[i] = new ComplexParticleInformation();
            complexParticleInformation[i].hasLight = hasLight;
            if (hasLight)
            {
                complexParticleInformation[i].initialParticleCount = particleSystem.particleCount;
                complexParticleInformation[i].light = light;
                complexParticleInformation[i].initialLightRange = light.range;
            }
        }
        return complexParticleInformation;
    }

    void UpdateComplexLights(ComplexParticleInformation[] complexParticleInformation, ParticleSystem[] particleSystems)
    {
        for (int i = 0; i < complexParticleInformation.Length; i++)
        {
            ComplexParticleInformation complexParticleInfo = complexParticleInformation[i];
            if (complexParticleInfo.hasLight)
            {
                complexParticleInfo.progress = Mathf.InverseLerp(complexParticleInfo.initialParticleCount, 0, particleSystems[i].particleCount);
                complexParticleInfo.light.range = Mathf.Lerp(complexParticleInfo.initialLightRange, 0f, complexParticleInfo.progress);
            }
        }
    }

    public Coroutine RemoveLegacyParticles(GameObject particleInst, bool unParent)
    {
        if (unParent)
        {
            particleInst.GetComponent<Transform>().SetParent(null);
        }
        return StartCoroutine(RemoveLegacyParticles(particleInst));
    }

    IEnumerator RemoveLegacyParticles(GameObject particleInst)
    {
        ParticleEmitter particleEmitter = particleInst.GetComponent<ParticleEmitter>();
        particleEmitter.emit = false;
        while (particleEmitter.particleCount > 0f)
        {
            yield return null;
        }
        Destroy(particleInst);
    }
}
