using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    private float normalSpeed;
    public float pasteMultiplicator = 1f;

    // Use this for initialization
    void Start()
    {
        normalSpeed = speed;
        // turn off the cursor
       //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Input.GetAxis() is used to get the user's input
        // You can furthor set it on Unity. (Edit, Project Settings, Input)
        float z  = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        if(z != 0f || x != 0f)
        {
            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftShift)) speed = normalSpeed * pasteMultiplicator;
        else speed = normalSpeed;
     

        //if (Input.GetKeyDown("escape"))
        //{
        //    // turn on the cursor
        //    Cursor.lockState = CursorLockMode.None;
        //}

        //transform.Translate(GetInputTranslationDirection() * speed * Time.deltaTime);
    }

    Vector3 GetInputTranslationDirection()
    {
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E))
        {
            direction += Vector3.up;
        }
        return direction;
    }
}



