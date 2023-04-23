using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteahtlhManager : MonoBehaviour
{
    bool isSprinting=false; 
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && isSprinting == false)
        {
            isSprinting = true; 
          transform.localScale = new Vector3(60, 60, 60);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) | Input.GetButton("Fire1") | Input.GetButton("Fire2"))
        {
          isSprinting = false;
          transform.localScale = new Vector3(40, 40, 40);
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.localScale = new Vector3(20, 20, 20);
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
           transform.localScale = new Vector3(40, 40, 40);
        }
    }




}
