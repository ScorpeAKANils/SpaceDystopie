using System;
using UnityEngine;
using UnityEngine.AI;
public class HumanoidAI : MonoBehaviour
{
    //Orientation
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Transform[] enemyWayPoints; 
    private NavMeshAgent enemyNav;


    //Attack Behaviour:
    [SerializeField, Tooltip("Distance, when the Enemy attacks")]
    private float attackDistance;
    [SerializeField, Tooltip("The distance, when the Enemy stops")]
    private float stopDistance;
    private Vector3 desiredPosition; 
    //Class references
    //Player:
    private PlayerHealth _playerHealth;
    private bool isOnPoint;
    private int index = 0; 
    // Start is called before the first frame update
    void Start()
    {
        enemyNav = this.GetComponent<NavMeshAgent>();
        _playerHealth = player.GetComponent<PlayerHealth>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //if((player.transform.position - transform.position).sqrMagnitude < Mathf.Pow(attackDistance, 2))// when player is out of range, walk on patrol
        //{

        //}
        Patrol(); 
    }


    void Patrol() 
    {
        if (Vector3.Distance(enemyWayPoints[index].position, gameObject.transform.position) <= enemyNav.stoppingDistance)
        {
            index = UnityEngine.Random.Range(0, enemyWayPoints.Length);

        }
        enemyNav.SetDestination(enemyWayPoints[index].position);


    }
}
