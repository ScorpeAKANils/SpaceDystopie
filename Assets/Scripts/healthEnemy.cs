using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthEnemy : MonoBehaviour
{
    [SerializeField] float health = 10f;
    [SerializeField] GameObject gegner;
    public SPAWNER spawnerScript;
    [SerializeField] bool isSpawned;
    bool despawn = false;
    [SerializeField] private Material demmaterilizeMat;
    public EnemyMove Enemy;
    

    public void GetHeadShot()
    {
        Enemy.isDead = true; 
        spawnerScript.ZombeysAlive--;
        gegner.GetComponent<MeshRenderer>().material = demmaterilizeMat;
        StartCoroutine(wait());
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Enemy.isDead = true;
            if (isSpawned)
            {
                spawnerScript.ZombeysAlive--;
            }
            gegner.GetComponent<MeshRenderer>().material = demmaterilizeMat;
          
            Debug.Log(gegner.layer.ToString()); 
            StartCoroutine(wait());
        }
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gegner); 
    }
}
