using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class EnviremontAiControll : MonoBehaviour
{

    NavMeshAgent GetNav;
    Transform thisObject; 
    [SerializeField] Transform[] WayPoints;
    bool isOnPoint = true;
    int index;
    float speed =200; 
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (WayPoints.Length <= 0)
        {
            return;
        }
        if (this.transform.position == WayPoints[index].position)
        {
            index = Random.Range(0, WayPoints.Length);
            
        }
        transform.LookAt(WayPoints[index].position);
        transform.position = Vector3.MoveTowards(transform.position, WayPoints[index].position, speed * Time.deltaTime);
    }

      
   
}


