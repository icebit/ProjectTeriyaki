using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabCollider : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;

    GameObject enemy;
    SimpleEnemy enemyInfo;
    SphereCollider sphereCollider;

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStateMachine.IsAttacking)
        {
            sphereCollider.radius = 0.02f;
        }
        else
        {
            sphereCollider.radius = 0.001f;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (playerStateMachine.IsAttacking && !playerStateMachine.AttackLanded && collision.gameObject.CompareTag("enemy"))
        {
            //Debug.Log("collision");
            playerStateMachine.AttackLanded = true;
            enemy = collision.gameObject;
            enemyInfo = enemy.GetComponent<SimpleEnemy>();
            enemyInfo.health -= 10;
            if (enemyInfo.health <= 0)
            {
                Destroy(enemy);
            }

        }
    }

    /*
     * detect whether we're in the attack state or not
     * if we are, find the collision with an enymy if any
     * if there is a collision, subtract health
     */
}
