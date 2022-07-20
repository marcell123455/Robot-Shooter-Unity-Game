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
    public int runSpeed;

    [Header("Animator Settings")]
    public Animator animator;
    bool isAwake;
    
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
    public int currentState;
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
    public int secondsBeforeDestroy;
    public int secondsDieAnimation;
    bool killedInitiated;
    public UnityEvent whenKilledEvents;
    public UnityEvent killedFXEvents;

    [Header("Audio")]
    public AudioSource stepAudio;
    public AudioSource awakeAudio;
    private void Step()
    {
        stepAudio.PlayOneShot(stepAudio.clip);
    }
    private void AwakeAudio()
    {
        awakeAudio.PlayOneShot(awakeAudio.clip);
    }
    public void SetAnimationState()
    {
        if(type == enemyType.Mech)
        {
            if (isAwake)
            {
                animator.SetBool("Awake", true);
            
                if(currentState == 0)
                {
                    //Patrolling
                    animator.SetBool("Walk", true);
                    animator.SetBool("Run", false);
                    animator.SetBool("AttackBig", false);
                }

                if (currentState == 1)
                {
                    //Chasing
                    animator.SetBool("Walk", true);
                    animator.SetBool("Run", false);
                    animator.SetBool("AttackBig", false);
                }

                if (currentState == 2)
                {
                    //Patrolling
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("AttackBig", true);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (type != enemyType.DestroyableProp)
        {
            SetEnemyState();
            SetAnimationState();
        }
    }

    public void SetEnemyState()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        if (!killedInitiated)
        {
            if (!playerInSightRange && !playerInAttackRange)
            {
                Patroling();
                agent.speed = moveSpeed;
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
                agent.speed = runSpeed;
            }
            if (playerInAttackRange && playerInSightRange)
            {
                AttackPlayer();
                agent.speed = moveSpeed;
            }
        }
        else
        {
            agent.speed = 0;
        }
    }

    private void Patroling()
    {
        currentState = 0;
        if (isAwake)
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
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
        isAwake = true;
        currentState = 1;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        currentState = 2;
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
            if(!killedInitiated)
            StartCoroutine(KilledEvent(secondsBeforeDestroy,secondsDieAnimation));
        }
    }


    private IEnumerator KilledEvent(int destroyTimer, int FXTimer)
    {
        if (type != enemyType.DestroyableProp)
            animator.SetBool("Dead", true);
        
        killedInitiated = true;
        whenKilledEvents.Invoke();
        yield return new WaitForSecondsRealtime(FXTimer);
        killedFXEvents.Invoke();
        yield return new WaitForSecondsRealtime(destroyTimer);
        Destroy(this.gameObject);
    }
}
