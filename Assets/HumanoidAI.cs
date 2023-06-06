using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HumanoidAI : MonoBehaviour
{
    //Orientation
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform[] enemyWayPoints;
    private NavMeshAgent enemyNav;
    private float speed = 30f;
    private bool isOnPoint = true;
    private int wayPointIndex;

    //Attack Behaviour:
    [SerializeField, Tooltip("Distance, when the Enemy attacks")]
    private float attackDistance;
    [SerializeField, Tooltip("The distance, when the Enemy stops")]
    private float stopDistance;
    [SerializeField]
    private LayerMask playerLayer; 
    [SerializeField]
    private Transform gunPos;
    private Transform enemyPos;
    private bool isAttacking = false;
    private float damage = 5f;
    private bool canShoot = true;
    private bool playerIsInSight;
    //Class references
    //Player:
    private PlayerHealth _playerHealth;


    // Start is called before the first frame update
    void Start()
    {

        enemyNav = this.GetComponent<NavMeshAgent>();
        enemyPos = this.GetComponent<Transform>();
        _playerHealth = player.gameObject.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        gunPos.LookAt(player.position);
        //check ob wichtige referencen == null sind, wenn ja, stopp diese iteration
        if (player == null)
        {
            return;
        }

        if (enemyWayPoints == null)
        {
            return;
        }

        if (enemyNav == null)
        {
            return;
        }
        isAttacking = Vector3.Distance(player.position, enemyPos.position) <= attackDistance;
        //wegpunkt verarbeitung 
        if (Vector3.Distance(player.position, enemyPos.position) > attackDistance && isAttacking == false)
        {
            Patrol();
        } else if (isAttacking)
        {
            HountPlayer();
        }
    }

    void Patrol()
    {
        Debug.Log(wayPointIndex);
        if (Vector3.Distance(enemyWayPoints[wayPointIndex].position, enemyPos.position) < 3)
        {
            isOnPoint = true;
        }

        //setze Wegpunkt
        if (isOnPoint)
        {
            int oldIndex = wayPointIndex;
            int wayPointSize = enemyWayPoints.Length - 1;
            wayPointIndex = Random.Range(0, enemyWayPoints.Length - 1);
            GetNewWayPoint(oldIndex, wayPointSize);
            isOnPoint = false;
        }
        //Gegnerläuft den Wegpunkt ab
        enemyNav.SetDestination(enemyWayPoints[wayPointIndex].position);

    }

    void GetNewWayPoint(int oldIndex, int wayPointSize)
    {
        if (wayPointIndex == oldIndex && wayPointIndex < wayPointSize)
        {
            wayPointIndex += 1;
        }
        else if (wayPointIndex == oldIndex && wayPointIndex >= wayPointSize)
        {
            wayPointIndex -= 1;
        }
    }
    void HountPlayer()
    {
        enemyPos.LookAt(new Vector3(player.position.x, this.transform.position.y,player.position.z));
     
        enemyNav.SetDestination(player.position);
        playerIsInSight = Physics.Raycast(gunPos.position, gunPos.TransformDirection(gunPos.forward), 60f, playerLayer);
        if (playerIsInSight)
        {
            Shoot(); 
        }
    }

    void Shoot()
    {
     
        RaycastHit hit;
        if (canShoot == false)
        {
            return;
        }
        Debug.Log("habe geschossen!");
        canShoot = false;
        if (Physics.Raycast(gunPos.position, gunPos.TransformDirection(gunPos.forward), out hit, 60f, playerLayer))
        {
            ValidHitCheck(hit);
        }

    }

    void ValidHitCheck(RaycastHit hit)
    {
        if (hit.collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("treffer versenkt"); 
            _playerHealth.GetDamage(damage);
            Debug.Log("habe getroffen!");
            StopCoroutine(shootingYield());
        }
    }
    private IEnumerator shootingYield()
    {
        yield return new WaitForSeconds(0.5f);
        canShoot = true; 
    }
}
