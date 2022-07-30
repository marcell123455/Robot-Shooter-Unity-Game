using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("General Settings")]
    public Transform HealthBar;
    public enemyType type;
    public float health;
    float orgHealth;
    public int shieldDeactivatedAtHealth;
    public int moveSpeed;
    public int runSpeed;

    public Vector3 localVelocity;
    public Rigidbody RB;
    public SkinnedMeshRenderer[] enemySkinnedMeshes;
    public MeshRenderer[] enemyMeshes;
    public Color HitColor;
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
    public bool moveDuringAttack;
    public int moveDuringAttackDistance;
    int moveAttackTimer;

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
    public Weapon[] weapons;
    public int delayBetweenMagazines;
    int currentWeapon;
    bool shooting;


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
    public AudioSource attackAudio;

    [Header("Enemy Specific Settings")]
    public Transform MechBody;
    public int turretTurnSpeed;
    public Transform TurretWeapon;
    public LineRenderer lineRenderer;
    private void Step()
    {
        if(stepAudio != null)
        stepAudio.PlayOneShot(stepAudio.clip);
    }
    private void AwakeAudio()
    {
        if(awakeAudio != null)
        awakeAudio.PlayOneShot(awakeAudio.clip);
    }

    public void SetAnimationState()
    {
        localVelocity = new Vector3(this.transform.InverseTransformDirection(RB.velocity).x, RB.transform.InverseTransformDirection(RB.velocity).y, RB.transform.InverseTransformDirection(RB.velocity).z);
        
        if (type == enemyType.Mech)
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
                    //Attacking
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("AttackBig", true);
                }
            }
        }

        if (type == enemyType.Soldier)
        {
            

                if(currentState == 2)
                {
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                }
                else
                {
                    if (currentState == 1)
                    {
                        animator.SetBool("Walk", false);
                        animator.SetBool("Run", true);
                    }
                    else
                    {
                        animator.SetBool("Walk", true);
                        animator.SetBool("Run", false);
                    }
                }



            
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player").transform;
        orgHealth = health;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (type != enemyType.DestroyableProp)
        {
            SetEnemyState();
            SetAnimationState();
        }
        if (HealthBar != null)
        {
            HealthBar.localScale = new Vector3(health / orgHealth, 1f, 1f);

        }

        if(lineRenderer != null)
        {
            lineRenderer.SetPosition(0, lineRenderer.transform.position);
            lineRenderer.SetPosition(1, lineRenderer.transform.position + (lineRenderer.transform.forward*10));
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
                if(agent != null)
                agent.speed = moveSpeed;
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
                if (agent != null)
                    agent.speed = runSpeed;
            }
            if (playerInAttackRange && playerInSightRange)
            {
                AttackPlayer();

            }
        }
        else
        {
            if (agent != null)
                agent.speed = 0;
        }
    }

    private void Patroling()
    {
        currentState = 0;
        if (isAwake)
        {
            if (MechBody != null)
                MechBody.localRotation = new Quaternion(0, -180, 0, 0); 
            
            

            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet && agent != null)
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
        if (agent != null)
            agent.SetDestination(player.position);
        if (MechBody != null)
        {
            MechBody.LookAt(new Vector3(player.position.x, MechBody.position.y, player.position.z));
            MechBody.rotation *= Quaternion.Euler(0, 0, -90);
        }
    }

    private void AttackPlayer()
    {
        currentState = 2;
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.SetDestination(transform.position);
        }

        if (type == enemyType.Soldier || type == enemyType.HeavySoldier)
        {

            transform.LookAt(player);
        }

        if (MechBody != null)
        {
            MechBody.LookAt(new Vector3(player.position.x, player.position.y, player.position.z));
            MechBody.rotation *= Quaternion.Euler(0, 0, -90);
        }

        if(type == enemyType.Turret)
        {
            Vector3 lTargetDir = player.position - TurretWeapon.position;
            lTargetDir.y = 0.0f;
            TurretWeapon.rotation = Quaternion.RotateTowards(TurretWeapon.rotation, Quaternion.LookRotation(lTargetDir), Time.deltaTime * turretTurnSpeed);
        }

     

        if (weapons.Length > 0 && (type != enemyType.Turret && type != enemyType.Mech && type != enemyType.Soldier && type != enemyType.HeavySoldier))
        {
            weapons[currentWeapon].bulletSpawnPoint.LookAt(player);




            if (weapons[currentWeapon].SecBulletSpawnPoint != null)
            {
                weapons[currentWeapon].SecBulletSpawnPoint.LookAt(player);
            }
        }



            if (weapons.Length > 0)
            {
                if(!shooting)
                StartCoroutine(ShootBullets());
            }

    }

    private IEnumerator ShootBullets()
    {
        if (!killedInitiated)
        {
            shooting = true;
            int i = 0;
            while (i < weapons[currentWeapon].shotsInRow)
            {
                if (!killedInitiated)
                {
                    if (attackAudio != null)
                        attackAudio.PlayOneShot(attackAudio.clip);

                    GameObject bullet = Instantiate(weapons[currentWeapon].BulletPrefab, weapons[currentWeapon].bulletSpawnPoint.position, Quaternion.identity);
                    if (bullet.GetComponent<Bullet>())
                    {
                        bullet.GetComponent<Bullet>().damage = weapons[currentWeapon].damage; //+ random damage 
                        bullet.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].bulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce, ForceMode.Impulse);
                    }
                    else
                    {
                        bullet.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].bulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce, ForceMode.Impulse);
                    }


                    if (weapons[currentWeapon].SecBulletSpawnPoint != null)
                    {

                        GameObject bullet2 = Instantiate(weapons[currentWeapon].BulletPrefab, weapons[currentWeapon].SecBulletSpawnPoint.position, Quaternion.identity);
                        if (bullet2.GetComponent<Bullet>())
                        {
                            bullet2.GetComponent<Bullet>().damage = weapons[currentWeapon].damage; //+ random damage 
                            bullet2.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].SecBulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce, ForceMode.Impulse);
                        }
                        else
                        {
                            bullet2.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].SecBulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce, ForceMode.Impulse);
                        }
                    }

                    yield return new WaitForSeconds(weapons[currentWeapon].bulletDelayEnemy);
                    i++;
                }
                else
                {
                    i = weapons[currentWeapon].shotsInRow;
                }
            }



            yield return new WaitForSecondsRealtime(timeBetweenAttacks);
            shooting = false;
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
            if(type != enemyType.DestroyableProp)
            StartCoroutine(Hit());
            
            health = health - damage;
        }

        if(health <= 0)
        {
            if(!killedInitiated)
            StartCoroutine(KilledEvent(secondsBeforeDestroy,secondsDieAnimation));
        }
    }

    private IEnumerator Hit()
    {
        foreach (SkinnedMeshRenderer SMR in enemySkinnedMeshes)
        {
            Material[] mats;


            mats = SMR.materials;
            mats[0].EnableKeyword("_EMISSION");
            mats[0].SetColor("_EmissionColor", HitColor);

            SMR.materials = mats;

        }

        foreach (MeshRenderer MR in enemyMeshes)
        {
            MR.material.EnableKeyword("_EMISSION");
            MR.material.SetColor("_EmissionColor", HitColor);
        }
        yield return new WaitForSecondsRealtime(0.2f);
        
        foreach (SkinnedMeshRenderer SMR in enemySkinnedMeshes)
        {
            Material[] mats;


            mats = SMR.materials;
            mats[0].DisableKeyword("_EMISSION");
            mats[0].SetColor("_EmissionColor", Color.black);

            SMR.materials = mats;
        }

        foreach (MeshRenderer MR in enemyMeshes)
        {
            MR.material.SetColor("_EmissionColor", Color.black);
        }
    }
    private IEnumerator KilledEvent(int destroyTimer, int FXTimer)
    {
        if (type != enemyType.DestroyableProp && type != enemyType.Turret && type != enemyType.Drone)
        {
            animator.SetBool("Dead", true);
        }
        
        killedInitiated = true;
        whenKilledEvents.Invoke();
        yield return new WaitForSecondsRealtime(FXTimer);
        killedFXEvents.Invoke();
        yield return new WaitForSecondsRealtime(destroyTimer);
        Destroy(this.gameObject);
    }
}
