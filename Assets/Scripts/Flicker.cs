using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    [SerializeField] private List<GameObject> flickerObjects;
    [SerializeField] private float minTime = 0.1f;
    [SerializeField] private float maxTime = 0.3f;

    private float waitUntil;

    void Start()
    {
        waitUntil = Time.time + Random.Range(minTime, maxTime);
    }

    void Update()
    {
        if (Time.time >= waitUntil)
        {
            foreach (GameObject obj in flickerObjects)
            {
                if (obj != null)
                    obj.SetActive(!obj.activeSelf);
            }

            waitUntil = Time.time + Random.Range(minTime, maxTime);
        }
    }
}