using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabCollider : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (playerStateMachine.IsAttacking && !playerStateMachine.AttackLanded && collision.gameObject.tag == "enemy")
        {
            Debug.Log("collision");
            playerStateMachine.AttackLanded = true;
            GameObject enemy = collision.gameObject;
            SimpleEnemy enemyInfo = enemy.GetComponent<SimpleEnemy>();
            enemy.GetComponent<Rigidbody>().AddForce(-transform.forward * 10);
            enemyInfo.health -= 10;

        }
    }

    /*
     * detect whether we're in the attack state or not
     * if we are, find the collision with an enymy if any
     * if there is a collision, subtract health and apply knockback
     */
}
