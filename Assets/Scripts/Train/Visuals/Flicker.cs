using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField] private List<GameObject> flickerObjects;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private List<int> materialIndices;
    [SerializeField] private float minTime = 0.1f;
    [SerializeField] private float maxTime = 0.3f;

    private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    private float waitUntil;
    private bool materialOn;
    private Color[] baseEmission;
    private MaterialPropertyBlock block;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        baseEmission = new Color[materialIndices.Count];

        if (targetRenderer == null) return;

        Material[] mats = targetRenderer.sharedMaterials;
        for (int i = 0; i < materialIndices.Count; i++)
        {
            int idx = materialIndices[i];
            if (idx >= 0 && idx < mats.Length && mats[idx] != null && mats[idx].HasProperty(EmissionId))
                baseEmission[i] = mats[idx].GetColor(EmissionId);
        }
    }

    private void OnEnable()
    {
        waitUntil = Time.time + Random.Range(minTime, maxTime);

        foreach (GameObject obj in flickerObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        SetMaterials(true);
    }

    private void OnDisable()
    {
        foreach (GameObject obj in flickerObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        SetMaterials(false);
    }

    private void Update()
    {
        if (Time.time >= waitUntil)
        {
            foreach (GameObject obj in flickerObjects)
            {
                if (obj != null)
                    obj.SetActive(!obj.activeSelf);
            }

            SetMaterials(!materialOn);

            waitUntil = Time.time + Random.Range(minTime, maxTime);
        }
    }

    private void SetMaterials(bool on)
    {
        materialOn = on;

        if (targetRenderer == null) return;

        int slots = targetRenderer.sharedMaterials.Length;

        for (int i = 0; i < materialIndices.Count; i++)
        {
            int idx = materialIndices[i];
            if (idx < 0 || idx >= slots) continue;

            targetRenderer.GetPropertyBlock(block, idx);
            block.SetColor(EmissionId, on ? baseEmission[i] : Color.black);
            targetRenderer.SetPropertyBlock(block, idx);
        }
    }
}