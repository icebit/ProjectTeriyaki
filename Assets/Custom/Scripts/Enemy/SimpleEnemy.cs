using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public float attackSpeed = 3.0f;
    float nextAttack = 0;
    public int strength;

    EnemyHealthBar healthBar;
    PlayerInfo playerInfo;

    private void Awake()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        playerInfo = GameObject.FindWithTag("Player").GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        // temporary debug method call
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time > nextAttack && collision.gameObject.CompareTag("Player"))
        {
            nextAttack = Time.time + attackSpeed;

            playerInfo.playerHealth -= strength;
        }


    }
}
