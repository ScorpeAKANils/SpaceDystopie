using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLoock : MonoBehaviour
{
    //von Nils 
    float mouseX;
    float mouseY;

    public Transform Player;
    public float Sensitivity = 100f;
    float xRotation;
    public float LockRot = 90f;
    public float recoilY;
    public float recoilX; 


    // Update is called once per frame
    void Update()
    {
        //Maus input abfrage 
        mouseX = Input.GetAxis("Mouse Y") + recoilY * Sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse X")  + recoilX*Sensitivity * Time.deltaTime;

        /*Begrenzen des Winkels in dem Mann hoch und runter schauen kann
        +  Kamera Rotation auf der X Achse*/
        xRotation -= mouseX;
        xRotation = Mathf.Clamp(xRotation, -LockRot, LockRot);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, -0f);

        //Rotation auf der Y Achse 
       Player.Rotate(Player.TransformDirection(Player.up) * mouseY);
 
    }
}

