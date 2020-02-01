using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownPointSpot : MonoBehaviour
{
    BoxCollider CheckBox;
    public bool isSpawnSpotBussy;

    void Start()
    {
        CheckBox = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        isSpawnSpotBussy = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isSpawnSpotBussy = false;
    }
}
