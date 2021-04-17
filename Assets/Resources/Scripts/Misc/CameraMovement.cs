using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float moveSpeed = 5.0f;
    public float mouseSensitivityX = 3.0f;
    public float mouseSensitivityY = 3.0f;

    float rotY = 0.0f;

    

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            float rotX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivityX;
            rotY += Input.GetAxis("Mouse Y") * mouseSensitivityY;
            rotY = Mathf.Clamp(rotY, -89.5f, 89.5f);
            transform.localEulerAngles = new Vector3(-rotY, rotX, 0.0f);
        }
        
        
        if (Input.GetKey("w"))
        {
            gameObject.transform.position += moveSpeed * Time.deltaTime * transform.forward;
        }
        if (Input.GetKey("s"))
        {
            gameObject.transform.position += moveSpeed * Time.deltaTime * -transform.forward;
        }
        if (Input.GetKey("a"))
        {
            gameObject.transform.position += moveSpeed * Time.deltaTime * -transform.right;
        }
        if (Input.GetKey("d"))
        {
            gameObject.transform.position += moveSpeed * Time.deltaTime * transform.right;
        }


    }
}
