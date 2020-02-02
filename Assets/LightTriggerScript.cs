using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTriggerScript : MonoBehaviour
{
    Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        light.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        light.enabled |= other.gameObject.CompareTag("Player");
    }

    private void OnTriggerExit(Collider other)
    {
        light.enabled &= !other.gameObject.CompareTag("Player");
    }
}
