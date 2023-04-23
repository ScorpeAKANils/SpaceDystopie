using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Firering : MonoBehaviour
{
    [SerializeField] MouseLoock ml;
    public Animator WeaponAnim;
    [SerializeField] Camera PlayerView;
    [SerializeField] float Range;
    [SerializeField] Transform GunBarrel;
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    [SerializeField] TextMeshProUGUI AmmoTXT;
    Transform refSize;
    bool isAiming;
    [SerializeField] float baseFov;
    [SerializeField] float aimFov;
    public bool shootAble = true; 
    [SerializeField] float damage = 4;
    [SerializeField] int MagSize = 20;
    [SerializeField] int amoInMag = 20;
    [SerializeField] float recoilUp;
    [SerializeField] float recoilSide;
    [SerializeField] float recoilUpAim;
    [SerializeField] float recoilSideAim;
    public  int allAmmo = 27;
   
    [SerializeField] GameObject MuzzleLight;
    // Start is called before the first frame update
    void Start()
    {
        amoInMag = MagSize; 
        AmmoTXT.text = "Ammo: " + amoInMag.ToString() + "/" + allAmmo.ToString();
        WeaponAnim = this.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && shootAble)
        {
            if (amoInMag >= 1)
            {
                if (!isAiming)
                {
                    ml.recoilY = recoilUp;
                    ml.recoilX = Random.Range(-recoilSideAim, recoilSideAim);
                    WeaponAnim.SetBool("Fire", true);
                }

                if (isAiming)
                {
                    ml.recoilY = recoilUp;
                    ml.recoilX = Random.Range(-recoilSideAim, recoilSideAim);
                    WeaponAnim.SetBool("fireAim", true);
                }
               
                //CamAnim.transform.rotation = Quaternion.Lerp();
            }


        }
        if (Input.GetButtonUp("Fire1") || amoInMag <= 0)
        {
            turnlightoff();
            ml.recoilY = 0;
            ml.recoilX = 0;
            if (!isAiming)
            {
                WeaponAnim.SetBool("Fire", false);
            }

            if (isAiming)
            {
                WeaponAnim.SetBool("fireAim", false);
            }

        }

        if (Input.GetButton("Fire2"))
        {
            isAiming = true;
            WeaponAnim.SetBool("aim", true);
            WeaponAnim.SetBool("Fire", false);
            PlayerView.fieldOfView = aimFov;
         
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
            WeaponAnim.SetBool("aim", false);
            WeaponAnim.SetBool("fireAim", false);
            PlayerView.fieldOfView = baseFov;
        }

        if (Input.GetKeyDown(KeyCode.R) && amoInMag < MagSize)
        {
            turnlightoff(); 
            shootAble = false;
            WeaponAnim.SetBool("reload", true);
        }

      
    }

    void shoot()
    {
        turlightOn(); 
        amoInMag--;
        AmmoTXT.text = "Ammo: " + amoInMag.ToString() + "/" + allAmmo.ToString();
        RaycastHit myHit;
        if (Physics.Raycast(GunBarrel.position, GunBarrel.TransformDirection(Vector3.forward), out myHit, Range))
        {
            if (myHit.transform.gameObject.CompareTag("GegnerHead"))
            {
                myHit.transform.gameObject.GetComponent<healthEnemy>().GetHeadShot();
            }

            if (myHit.transform.gameObject.CompareTag("GegnerBody"))
            {
                myHit.transform.gameObject.GetComponent<healthEnemy>().GetDamage(damage);
            }

        }

    }

    void Reload()
    {
        for (int i = amoInMag; i < MagSize; i++)
        {
            if (allAmmo > 0)
            {
                amoInMag++;
                allAmmo--;
            }
            /*else
            {
                
            }*/
        }
        shootAble = true;
        AmmoTXT.text = "Ammo: " + amoInMag.ToString() + "/" + allAmmo.ToString();
        WeaponAnim.SetBool("reload", false);
      
    }

void turlightOn()
{
        MuzzleLight.SetActive(true); 
}

public void turnlightoff()
    {
        MuzzleLight.SetActive(false);
}
}
