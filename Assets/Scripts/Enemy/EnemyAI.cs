using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

// source: https://www.youtube.com/watch?v=UjkSFoLxesw

public class EnemyAI : MonoBehaviour
{
    // pathfinding component for the enemy
    public NavMeshAgent agent;

    // 3D location of the player
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // attacking (currently unused)
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // states
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    public Animator animator;

    private void Awake()
    {
        // find the location of the player at load
        player = GameObject.Find("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // check a sphere around the enemy for  the player
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();

        animator.SetBool("moving", agent.isStopped);
        //if (playerInSightRange && playerInAttackRange) AttackPLayer();
    }

    private void Patrolling()
    {  
        // if no walk point is set, calls function to generate a new one
        if (!walkPointSet) SearchWalkPoint();

        // set destination of the enemy to the current walk point
        if (walkPointSet) 
            agent.SetDestination(walkPoint);

        // calculate the distance to the target walk point
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // use calculated distance to check when the enemy reaches the walk point, and reset the walk point
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        Vector3 newPoint;

        // calculate random z and x points in range to walk to
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        // set the walk point to this new generated point
        // takes enemy's current position and adds the new points to find a target location
        newPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // very cool resource: https://gamedev.stackexchange.com/questions/147915/what-does-navmesh-allareas-specify-in-unity
        //check if the new point is within the bounds of the navmesh
        if (NavMesh.SamplePosition(newPoint, out _, 1.0f, 1))
        {
            walkPoint = newPoint;
        }


        // check that the new walk point is actually on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
        Debug.Log("Current enemy walkpoint: " + walkPoint);
    }

    private void ChasePlayer()
    {  
        // set the enemy to head towards the player's current position
        agent.SetDestination(player.position);
        Debug.Log("chasing player!!!!");

    }
}

