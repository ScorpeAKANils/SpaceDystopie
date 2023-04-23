using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    float Damage = 10f;
    bool isDamageAllowed = true;

    IEnumerator DamageCoolDown()
    {
        yield return new WaitForSeconds(1f);
        isDamageAllowed = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isDamageAllowed)
            {
                isDamageAllowed = false; 
                other.GetComponent<PlayerHealth>().GetDamage(Damage);
                StartCoroutine(DamageCoolDown());
            }
        }
    }
}
