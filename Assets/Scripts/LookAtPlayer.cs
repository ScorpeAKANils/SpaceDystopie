using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    //Transforms
    [Tooltip("the Position of the Player")]
    [SerializeField] private Transform myPlayer;
    [Tooltip("position of the raycasts to shoot")]
    [SerializeField] private Transform[] gunBerrals;
    [Tooltip("The Raycast that checks, if Player is in Sight")]
    [SerializeField] private Transform Sensor;

    //Variables
    [Tooltip("the distance when the torret should start fireing")]
    public float attackDistance = 10f;
    [Tooltip("damage per hit")]
    [SerializeField] float damage = 10f;
    [Tooltip("is the Player visible?")]
    private bool playerInSight;
    bool isCheckForPlayerAllowed = true; 
    private int usedGun = 0;

    //class references
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth = myPlayer.GetComponent<PlayerHealth>(); 
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(myPlayer.position);

        if (Vector3.Distance(this.transform.position, myPlayer.position) < attackDistance)
        {
            RaycastHit hit;
            if (isCheckForPlayerAllowed==false)
            {
                return; 
            }
            if (Physics.Raycast(Sensor.position, Sensor.TransformDirection(Vector3.forward), out hit, attackDistance))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        isCheckForPlayerAllowed = false;
                        playerInSight = true;

                        if (playerInSight)
                        {
                            StartCoroutine(shootingYield());
                        }
                    }
                    else
                    {
                        Debug.Log("scheiße, wo ist der wichser??????");
                        usedGun = 0;
                    }
                }
         
        }
    }

    private void attack()
    {
        RaycastHit hit;
            Debug.Log("Spieler gesichtet, jetzt wird der Hund gefistet");
            if (Physics.Raycast(gunBerrals[usedGun].position, Sensor.TransformDirection(Vector3.forward), out hit, attackDistance))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    _playerHealth.GetDamage(damage);
                    StopCoroutine(shootingYield()); 
                }
            
        }
    }



    IEnumerator shootingYield()
    {
        if (!playerInSight)
        {
            yield break;
        }
        if (playerInSight)
        {
            attack();
            usedGun = 1;
            yield return new WaitForSeconds(0.2f);
            attack();
            usedGun = 0;
            yield return new WaitForSeconds(0.2f);
        }
        playerInSight = false;
        yield return new WaitForSeconds(0.5f);
        isCheckForPlayerAllowed = true; 

    }
}
