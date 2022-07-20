using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour
{
    [Header("Player Values")]
    public float maxHealth;
    public float maxStamina;
    public int staminaRechargeSpeed;
    public int staminaRechargeDelay;
    int staminaRechargeCounter;
    float health;
    float stamina;
    public int shieldDeactivatedAtHealth;
    public InteractableArea interactableArea;
    [Header("Control Settings")]
    public int moveSpeed;
    public int sprintSpeed;
    public float moveAcceleration;
    float sprint;
    float moveX;
    float moveY;
    Vector3 pointToLook;
    Vector3 localVelocity;
    public int sprintStamina;

    public Rigidbody playerRB;
    public Animator CharacterAnimator;
    public float AnimationSpeed;
    Vector2 moveDir;
    Vector2 mousePos;
    [Header("Weapon Settings")]
    public Weapon[] weapons;
    public Transform WeaponOrigin;
    public LayerMask WeaponAimlayerMask;
    public AudioSource WeaponShotAudio;
    public int currentWeapon;
    int bulletDelayCounter;
    bool reloading;
    
    int weaponHoldAimAfterShoot;
    public TwoBoneIKConstraint GunAimWalkIdle;
    public TwoBoneIKConstraint GunAimRun;
    public Rig MachineGunHold;
    public Rig MachineGunAim;
    bool fadeAimIn;

    [Header("Grenades")]
    public GameObject grenadePrefab;
    public Transform GrenadeSpawnPoint;
    public TwoBoneIKConstraint GrenadeThrow;
    bool fadeThrowIn;
    public int grenadeThrowPower;
    public int grenadesLeft;
    [Header("Lightsaber")]
    public bool lightSabernAttacking;
    public int lightSaberDamage;
    public int additionalLightSaberRandomDamageMax;
    public int criticalHitChance;
    public int lightSaberAttackSpeed;
    int lightSaberAttackCooldown;
    public int lightSaberAttackStamina;
    public DamageArea lightSaberDamageZone;
    public AudioClip[] lightSaberAudioClips;
    public AudioSource lightSaberAudioSource;
    




    // Start is called before the first frame update
    void Start()
    {
        stamina = maxStamina;
        UpdateWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        AnimatePlayer();
    }

    private void FixedUpdate()
    {
        

        if (Input.GetMouseButton(0) && weapons[currentWeapon].allowButtonHold)
        {
            weaponHoldAimAfterShoot = 20;
            fadeAimIn = true;
            if (bulletDelayCounter == 0)
            {
                Fire();
                bulletDelayCounter = weapons[currentWeapon].bulletDelay;
            }
            else
            {
                bulletDelayCounter--;
            }
        }



        if (!weapons[currentWeapon].allowButtonHold)
        {
            if (bulletDelayCounter > 0)
                bulletDelayCounter--;
        }

        PhysicsCalculation();

        if (lightSabernAttacking)
        {
            if(lightSaberAttackCooldown > 0)
            {
                lightSaberAttackCooldown--;
            }
            else
            {
                lightSabernAttacking = false;
            }
        }

        BlendRigs();
        
    }

    public void ProcessInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");


        if (Input.GetKey(KeyCode.LeftShift) && stamina - sprintStamina > 0 && (moveX != 0 || moveY != 0))
        {
            sprint = Mathf.Clamp(sprint + moveAcceleration,0, 1f);
            stamina = stamina - sprintStamina;
            staminaRechargeCounter = staminaRechargeDelay;
        }
        else
        {
            sprint = Mathf.Clamp(sprint - moveAcceleration, 0, 1f);
        }

        if (Input.GetMouseButtonDown(0) && !weapons[currentWeapon].allowButtonHold)
        {
            weaponHoldAimAfterShoot = 20;
            fadeAimIn = true;
            if (bulletDelayCounter <= 0)
            {
                Fire();
                bulletDelayCounter = weapons[currentWeapon].bulletDelay;
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (weapons[currentWeapon].allowButtonHold)
            {
                bulletDelayCounter = 0;
            }
        }


        if (Input.GetMouseButtonDown(2))
        {
            ThrowGrenade();
        }

        if (Input.GetMouseButtonDown(1) && stamina - lightSaberAttackStamina > 0)
        {
            if (!lightSabernAttacking)
            {
                LightSaberAttack();
                stamina = stamina - lightSaberAttackStamina;
                staminaRechargeCounter = staminaRechargeDelay;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerInteraction();
        }

        moveDir = new Vector2(moveX, moveY).normalized;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if(Input.mouseScrollDelta.y == 1)
        {
            if(currentWeapon == weapons.Length - 1)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon++;
            }
            UpdateWeapon();
        }

        if (Input.mouseScrollDelta.y == -1)
        {
            if (currentWeapon == 0)
            {
                currentWeapon = weapons.Length - 1;
            }
            else
            {
                currentWeapon--;
            }
            UpdateWeapon();
        }

        if(staminaRechargeCounter <= 0)
        {
            stamina = Mathf.Clamp(stamina + staminaRechargeSpeed, 0, maxStamina);
        }
        else
        {
            staminaRechargeCounter--;
        }

        


    }
    RaycastHit hit;
    public void PhysicsCalculation()
    {
        playerRB.velocity = new Vector3(moveDir.x * (moveSpeed + (sprint * sprintSpeed)) ,playerRB.velocity.y, moveDir.y * (moveSpeed + (sprint * sprintSpeed)));

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundplane = new Plane(Vector3.up, Vector3.zero);
        float raylengh;

        RaycastHit hit;
        //Aim at enemy
        if (Physics.Raycast(cameraRay.origin, cameraRay.direction, out hit, Mathf.Infinity, WeaponAimlayerMask))
        {
            WeaponOrigin.transform.LookAt(new Vector3(hit.point.x, hit.point.y, hit.point.z));
        }
        else
        {
            WeaponOrigin.transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        if (groundplane.Raycast(cameraRay, out raylengh))
        {
            pointToLook = cameraRay.GetPoint(raylengh);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.green);
            playerRB.transform.LookAt(new Vector3(pointToLook.x,playerRB.transform.position.y,pointToLook.z));
        }

        
    }

    public void Fire()
    {
        if (!lightSabernAttacking)
        {
            if (weapons[currentWeapon].bulletsLeft > 0)
            {
                if (!reloading)
                {
                    GameObject bullet = Instantiate(weapons[currentWeapon].BulletPrefab, weapons[currentWeapon].bulletSpawnPoint.position, Quaternion.identity);
                    bullet.GetComponent<Bullet>().damage = weapons[currentWeapon].damage; //+ random damage 
                    bullet.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].bulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce, ForceMode.Impulse);
                    WeaponShotAudio.PlayOneShot(weapons[currentWeapon].shootSound);

                    weapons[currentWeapon].bulletsLeft--;
                }
            }
            else
            {
                if(!reloading)
                StartCoroutine(ReloadWeapon(currentWeapon));
            }
        }
    }

    public void UpdateWeapon()
    {
        foreach(Weapon W in weapons)
        {
            W.weaponObject.SetActive(false);
        }
        weapons[currentWeapon].weaponObject.SetActive(true);
    }

    public void PlayerInteraction()
    {
        interactableArea.InteractWithInteractables();
    }

    public void ThrowGrenade()
    {
        if (!lightSabernAttacking)
        {
            if (grenadesLeft > 0)
            {
                fadeThrowIn = true;
                GameObject grenade = Instantiate(grenadePrefab, GrenadeSpawnPoint.position, Quaternion.identity);
                grenade.GetComponent<Rigidbody>().AddRelativeForce(GrenadeSpawnPoint.forward * grenadeThrowPower, ForceMode.Impulse);
            }
        }
    }

    public void LightSaberAttack()
    {
        lightSaberAttackCooldown = lightSaberAttackSpeed;
        lightSabernAttacking = true;

        int randomDamage = Random.Range(0, additionalLightSaberRandomDamageMax);
        lightSaberDamageZone.DealDamage(lightSaberDamage + randomDamage);

        lightSaberAudioSource.PlayOneShot(lightSaberAudioClips[Random.Range(0, lightSaberAudioClips.Length)]);
    }


    private IEnumerator ReloadWeapon(int weapon)
    {
        if (weapons[weapon].magazinesLeft > 0) {
            //Reload
            reloading = true;
            yield return new WaitForSecondsRealtime(weapons[weapon].reloadTime);
            weapons[weapon].magazinesLeft--;
            weapons[weapon].bulletsLeft = weapons[weapon].magazineBulletCount;
            reloading = false;
        }
        else
        {
            //Out of Ammo
        }
    }


    public void BlendRigs()
    {
        //Grenade throw
        if (fadeThrowIn)
        {
            if (GrenadeThrow.weight != 1)
            {
                GrenadeThrow.weight = GrenadeThrow.weight + 0.1f;
            }
            else
            {
                fadeThrowIn = false;
            }
        }
        else
        {
            if (GrenadeThrow.weight > 0)
            {
                GrenadeThrow.weight = GrenadeThrow.weight - 0.1f;
            }
        }

        if (CharacterAnimator.GetBool("isSprintingFront") || CharacterAnimator.GetBool("isSprintingLeft") || CharacterAnimator.GetBool("isSprintingRight"))
        {
            //Running Rigs
            if (currentWeapon == 0)
            {
                //Disable other rigs
                if (GunAimWalkIdle.weight > 0)
                {
                    GunAimWalkIdle.weight = GunAimWalkIdle.weight - 0.1f;
                }

                if (fadeAimIn)
                {
                    if (weaponHoldAimAfterShoot > 0)
                    {
                        weaponHoldAimAfterShoot--;

                        if (GunAimRun.weight <= 1)
                        {
                            GunAimRun.weight = GunAimRun.weight + 0.1f;
                        }
                    }
                    else
                    {
                        fadeAimIn = false;
                    }
                }
                else
                {
                    if (GunAimRun.weight > 0)
                    {
                        GunAimRun.weight = GunAimRun.weight - 0.1f;
                    }
                }
            }

            if (currentWeapon == 1)
            {
                //Disable other rigs
                if (GunAimWalkIdle.weight > 0)
                {
                    GunAimWalkIdle.weight = GunAimWalkIdle.weight - 0.1f;
                }

                if (GunAimRun.weight > 0)
                {
                    GunAimRun.weight = GunAimRun.weight - 0.1f;
                }
            }
        }
        else
        {
            if (CharacterAnimator.GetBool("isWalkingFront") || CharacterAnimator.GetBool("isWalkingBack"))
            {
                //Walking Rigs
                if (currentWeapon == 0)
                {
                    //Disable other rigs
                    if (GunAimRun.weight > 0)
                    {
                        GunAimRun.weight = GunAimRun.weight - 0.1f;
                    }

                    if (fadeAimIn)
                    {
                        if (weaponHoldAimAfterShoot > 0)
                        {
                            weaponHoldAimAfterShoot--;

                            if (GunAimWalkIdle.weight <= 1)
                            {
                                GunAimWalkIdle.weight = GunAimWalkIdle.weight + 0.1f;
                            }
                        }
                        else
                        {
                            fadeAimIn = false;
                        }
                    }
                    else
                    {
                        if (GunAimWalkIdle.weight > 0)
                        {
                            GunAimWalkIdle.weight = GunAimWalkIdle.weight - 0.1f;
                        }
                    }
                }

                if (currentWeapon == 1)
                {

                }

            }
            else
            {
                //Idle Rigs
                if (currentWeapon == 0)
                {
                    //Disable other rigs
                    if (GunAimRun.weight > 0)
                    {
                        GunAimRun.weight = GunAimRun.weight - 0.1f;
                    }

                    if (MachineGunHold.weight > 0)
                    {
                        MachineGunHold.weight = MachineGunHold.weight - 0.1f;
                    }

                    if (GunAimWalkIdle.weight <= 1)
                    {
                        GunAimWalkIdle.weight = GunAimWalkIdle.weight + 0.1f;
                    }

                }

                if (currentWeapon == 1)
                {
                    //Disable other rigs
                    if (GunAimRun.weight > 0)
                    {
                        GunAimRun.weight = GunAimRun.weight - 0.1f;
                    }

                    if (GunAimWalkIdle.weight > 0)
                    {
                        GunAimWalkIdle.weight = GunAimWalkIdle.weight - 0.1f;
                    }

                    if (fadeAimIn)
                    {
                        if (weaponHoldAimAfterShoot > 0)
                        {
                            weaponHoldAimAfterShoot--;

                            if (MachineGunAim.weight <= 1)
                            {
                                MachineGunAim.weight = MachineGunAim.weight + 0.1f;
                            }

                            if (MachineGunHold.weight > 0)
                            {
                                MachineGunHold.weight = MachineGunHold.weight - 0.1f;
                            }
                        }
                        else
                        {
                            fadeAimIn = false;
                        }
                    }
                    else
                    {
                        if (MachineGunHold.weight <= 1)
                        {
                            MachineGunHold.weight = MachineGunHold.weight + 0.1f;
                        }

                        if (MachineGunAim.weight > 0)
                        {
                            MachineGunAim.weight = MachineGunAim.weight - 0.1f;
                        }


                    }

                }
            }
        }
    }
    public void AnimatePlayer()
    {

        localVelocity = new Vector3(playerRB.transform.InverseTransformDirection(playerRB.velocity).x, playerRB.transform.InverseTransformDirection(playerRB.velocity).y, playerRB.transform.InverseTransformDirection(playerRB.velocity).z);


        if (localVelocity.z > 0.1f)
        {
            //Forward
            if (sprint < 0.1f)
            {
                //Walking
                CharacterAnimator.SetBool("isWalkingFront", true);
                CharacterAnimator.SetBool("isWalkingBack", false);
                
                CharacterAnimator.SetBool("isSprintingFront", false);
                CharacterAnimator.SetBool("isSprintingBack", false);
                CharacterAnimator.SetBool("isSprintingLeft", false);
                CharacterAnimator.SetBool("isSprintingRight", false);
                CharacterAnimator.speed = 3f;
            }
            else
            {
                //Sprinting
                CharacterAnimator.SetBool("isWalkingFront", false);
                CharacterAnimator.SetBool("isWalkingBack", false);
                
                if (localVelocity.x > 2f)
                {
                    //Sprinting ForwardRight
                    CharacterAnimator.SetBool("isSprintingFront", false);
                    CharacterAnimator.SetBool("isSprintingBack", false);
                    CharacterAnimator.SetBool("isSprintingLeft", false);
                    CharacterAnimator.SetBool("isSprintingRight", true);

                }
                else
                {
                    if (localVelocity.x < -2f)
                    {
                        //Sprinting ForwardLeft
                        CharacterAnimator.SetBool("isSprintingFront", false);
                        CharacterAnimator.SetBool("isSprintingBack", false);
                        CharacterAnimator.SetBool("isSprintingLeft", true);
                        CharacterAnimator.SetBool("isSprintingRight", false);
                    }
                    else
                    {
                        //Sprinting Forward
                        CharacterAnimator.SetBool("isSprintingFront", true);
                        CharacterAnimator.SetBool("isSprintingBack", false);
                        CharacterAnimator.SetBool("isSprintingLeft", false);
                        CharacterAnimator.SetBool("isSprintingRight", false);
                    }
                }
                CharacterAnimator.speed = 2f;
            }
        }
        else
        { // Backwards
            if (localVelocity.z < -0.1f)
            {
                if (sprint < 0.1f)
                {
                    //Walking Back
                    CharacterAnimator.SetBool("isWalkingBack", true);
                    CharacterAnimator.SetBool("isWalkingFront", false);

                    CharacterAnimator.SetBool("isSprintingFront", false);
                    CharacterAnimator.SetBool("isSprintingBack", false);
                    CharacterAnimator.SetBool("isSprintingLeft", false);
                    CharacterAnimator.SetBool("isSprintingRight", false);
                    CharacterAnimator.speed = 3f;
                }
                else
                {
                    //Sprinting Back
                    CharacterAnimator.SetBool("isWalkingBack", false);
                    CharacterAnimator.SetBool("isWalkingFront", false);

                    CharacterAnimator.SetBool("isSprintingFront", false);
                    CharacterAnimator.SetBool("isSprintingBack", true);
                    CharacterAnimator.SetBool("isSprintingLeft", false);
                    CharacterAnimator.SetBool("isSprintingRight", false);
                    CharacterAnimator.speed = 2f;
                }
            }
            else
            {
                CharacterAnimator.speed = 1f;
                CharacterAnimator.SetBool("isWalkingFront", false);
                CharacterAnimator.SetBool("isWalkingBack", false);

                CharacterAnimator.SetBool("isSprintingFront", false);
                CharacterAnimator.SetBool("isSprintingBack", false);
                CharacterAnimator.SetBool("isSprintingLeft", false);
                CharacterAnimator.SetBool("isSprintingRight", false);
            }
        }
    }
}
