using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponManager : MonoBehaviour
{
    public Movement Player;
    int i;
    int wI = -1;
    [SerializeField]
    GameObject[] text; 
     [SerializeField] GameObject[] Weapons;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Player.weaponInUse = false;
            foreach (GameObject weapon in Weapons)
            {
                weapon.SetActive(false); 
            }
            foreach(GameObject text in text)
            {
                text.SetActive(false); 
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Player.weaponInUse = true; 
            i = 0;
            wI = 0;
            Player.WeaponIndex = wI;
            Weapons[wI].SetActive(true);
            text[wI].SetActive(true);
            foreach (GameObject weapon in Weapons)
            {
                
                if (i != wI)
                {
              
                    weapon.SetActive(false);
                    text[i].SetActive(false); 
                }
                i++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Player.weaponInUse = true;
           

            i = 0;
            wI = 1;
            text[wI].SetActive(true);
            Player.WeaponIndex = wI;
            Weapons[wI].SetActive(true);

            foreach (GameObject weapon in Weapons)
            {
              
                if (i != wI)
                {
                    weapon.SetActive(false);
                    text[i].SetActive(false);
                }
                i++;
            }
        }
    }
}
