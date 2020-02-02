using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator doorLAnim;
    public Animator doorRAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doorLAnim.Rebind();
            doorLAnim.SetInteger("doorState", 0);
            doorRAnim.Rebind();
            doorRAnim.SetInteger("doorState", 0);
            Debug.Log("OnTriggerEnter Player is here, open the door!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            doorLAnim.Rebind();
            doorLAnim.SetInteger("doorState", 1);
            doorRAnim.Rebind();
            doorRAnim.SetInteger("doorState", 1);

            Debug.Log("OnTriggerExit Player left, close the door!");
        }
    }
}
