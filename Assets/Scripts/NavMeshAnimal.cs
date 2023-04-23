using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 
public class NavMeshAnimal : MonoBehaviour
{
    NavMeshAgent GetNav;
    [SerializeField] Transform[] WayPoints;
    bool isOnPoint = true;
    int index;

    // Start is called before the first frame update
    private void Start()
    {
        GetNav = GetComponent<NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
      
        if (WayPoints.Length <= 0)
        {
            return;
        }
        if (Vector3.Distance(WayPoints[index].position, gameObject.transform.position) <= GetNav.stoppingDistance)
        {
            index = Random.Range(0, WayPoints.Length);

        }
        GetNav.SetDestination(WayPoints[index].position); 
    }
}
