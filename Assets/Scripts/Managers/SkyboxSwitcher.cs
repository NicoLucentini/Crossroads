using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public List<Material> skyboxes;

    public Material GetRandom()
    {
        return skyboxes[Random.Range(0, skyboxes.Count)];
    }
    public void SetSkybox()
    {
        RenderSettings.skybox = GetRandom();
    }
}
