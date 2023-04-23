using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthEnemy : MonoBehaviour
{
    [SerializeField] float health = 10f;
    [SerializeField] GameObject gegner;
    public SPAWNER spawnerScript;
    public bool attack;
    [SerializeField] bool isSpawned;

    private void Update()
    {
        Debug.Log(attack); 
    }
    public void GetHeadShot()
    {
        spawnerScript.ZombeysAlive--;
        Destroy(gegner); 
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (isSpawned)
            {
                spawnerScript.ZombeysAlive--;
            }
            Destroy(gegner); 
        }
    }
}
