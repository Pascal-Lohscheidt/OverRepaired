using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTriggerScript : MonoBehaviour
{
    Light light;
    private bool shouldFadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        light.enabled = true;
    }

    private void Update()
    {
        if (shouldFadeIn)
            light.intensity = Mathf.Lerp(light.intensity, 8, 0.9f * Time.deltaTime);
        else
            light.intensity = Mathf.Lerp(light.intensity, 0, 0.9f * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        shouldFadeIn |= other.gameObject.CompareTag("Player");
    }

    private void OnTriggerExit(Collider other)
    {
        shouldFadeIn &= !other.gameObject.CompareTag("Player");
    }
}
