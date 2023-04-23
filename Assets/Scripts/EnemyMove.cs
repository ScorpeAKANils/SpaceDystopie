using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyMove : MonoBehaviour
{
    [SerializeField] NavMeshAgent GegnerBrain;
    [SerializeField] Transform PlayerPos;
    [SerializeField] Transform[] WayPoints;
    int index;
    int oldIndex; 
    float attackDist;
    public  bool isAttacking;
    public bool canPatroll;
    float Damage = 10f;
    bool isDamageAllowed = true;

    // Start is called before the first frame update
    void Start()
    {
        GegnerBrain = this.GetComponent<NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isAttacking); 
        if (canPatroll)
        {
            if (isAttacking)
            {
                GegnerBrain.SetDestination(PlayerPos.position);

                if (Vector3.Distance(PlayerPos.position, this.transform.position) <= GegnerBrain.stoppingDistance)
                {
                    if (isDamageAllowed)
                    {
                        isDamageAllowed = false;
                        PlayerPos.GetComponent<PlayerHealth>().GetDamage(Damage);
                        StartCoroutine(DamageCoolDown());
                    }
                  
                }
            }

              if (!isAttacking)
              {
                  if (WayPoints.Length <= 0)
                  {
                      return;
                  }
                  if (Vector3.Distance(WayPoints[index].position, gameObject.transform.position) <= GegnerBrain.stoppingDistance)
                  {

                      oldIndex = index; 
                      index = Random.Range(0, WayPoints.Length);
                      if (index == oldIndex)
                      {
                          index = Random.Range(0, WayPoints.Length);
                      }
                  }
                  if (isAttacking == false)
                  {
                      GegnerBrain.SetDestination(WayPoints[index].position);
                  }



              }
        }

        if (!canPatroll)
        {
            GegnerBrain.SetDestination(PlayerPos.position);
        }
    }

    IEnumerator DamageCoolDown()
    {
        yield return new WaitForSeconds(1f);
        isDamageAllowed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Radius"))
        {
            isAttacking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Radius"))
        {
            isAttacking = false;
        }
    }

}
