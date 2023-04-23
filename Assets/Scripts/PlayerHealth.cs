using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float Health = 100f;
    [SerializeField] TextMeshProUGUI HealthCounter;
   
    private void Start()
    {
        HealthCounter.text = "Health: " + Health.ToString(); 
    }

    // Start is called before the first frame update
   /* private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GegnerBody"))
        {
            GetDamage(10f); 
        }
    }*/
    public void GetDamage(float damage)
    {
        Health -= damage;
        HealthCounter.text = "Health: " + Health.ToString();
        if (Health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
        }
    }
}
