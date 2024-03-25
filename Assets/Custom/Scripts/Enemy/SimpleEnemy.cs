using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [SerializeField] float health = 20;
    [SerializeField] float maxHealth = 20;

    private EnemyHealthBar healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    // Update is called once per frame
    void Update()
    {
        // temporary debug method call
        healthBar.UpdateHealthBar(health, maxHealth);
    }
}
