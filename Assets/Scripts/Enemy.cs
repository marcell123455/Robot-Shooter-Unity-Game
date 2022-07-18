using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("General Settings")]
    public enemyType type;
    public int health;
    public int shieldDeactivatedAtHealth;
    public int moveSpeed;
    [Header("Pathfinding Settings")]
    public NavMeshAgent agent;
    public LayerMask Player, Ground;
    Transform player;
    //Patroling
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;
    public enum enemyType
    {
        Soldier,
        HeavySoldier,
        Turret,
        MoveableTurret,
        Drone,
        Mech,
        DestroyableProp
    }


    [Header("Attack Settings")]
    public int AttackRange;
    public bool canThrowGrenades;
    public Transform grenadeSpawnPoint;
    public Weapon Weapons;
    public int delayBetweenMagazines;

    [Header("Loot Settings")]
    public int TechPartsDropped;

    [Header("Killed Settings")]
    public UnityEvent whenKilledEvents;




    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(type != enemyType.DestroyableProp)
        SetEnemyState();
    }

    public void SetEnemyState()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here
            
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    public void DealEnemyDamage(int damage)
    {
        if(health > 0)
        {
            health = health - damage;
        }

        if(health <= 0)
        {
            KilledEvent();
        }
    }

    public void KilledEvent()
    {
        whenKilledEvents.Invoke();
    }
}
