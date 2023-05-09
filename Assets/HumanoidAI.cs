using System.Collections;
using System.Collections.Generic;
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

    //Class references
    //Player:
    private PlayerHealth _playerHealth; 

    // Start is called before the first frame update
    void Start()
    {
        enemyNav = this.GetComponent<NavMeshAgent>();
        _playerHealth = player.GetComponent<PlayerHealth>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
