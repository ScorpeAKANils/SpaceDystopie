using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SPAWNER : MonoBehaviour
{
    [SerializeField] GameObject Zombey;
    [SerializeField] int EnemyCounter = 30;
    [SerializeField] Transform[] SpawnPonts; 
    int i = 0;
    int spawnIndex; 
    public int ZombeysAlive = 0;
    int SpawnCounter = 0;
    bool spawningAllowed = true;
    public float waitTime; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ZombeysAlive <= 0 && !spawningAllowed)
        {
            StartCoroutine(spawnDelay()); 
        }
        if (ZombeysAlive <= 0 && spawningAllowed)
        {
            SpawnCounter++; 
            for (i = ZombeysAlive; i < EnemyCounter; i++)
            {
                spawnIndex = Random.Range(0, SpawnPonts.Length - 1); 
                var Enemy1 = Instantiate(Zombey, SpawnPonts[spawnIndex].position, SpawnPonts[spawnIndex].rotation);
                ZombeysAlive++;
            }
            if (SpawnCounter >= 3)
            {
                System.GC.Collect();
            }
                spawningAllowed = false; 
        }

       
    }

    IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(waitTime);
        spawningAllowed = true; 
    }
}
