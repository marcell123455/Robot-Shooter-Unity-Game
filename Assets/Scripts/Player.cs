using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour
{
    public UI_Manager UI;
    private int currentTechParts;
    [Header("Player Values")]
    public int currentInput;
    public float maxHealth;
    int healthRechargeCounter;
    public int healthRechargeDelay;
    public int healthRechargeSpeed;
    public float maxStamina;
    public int staminaRechargeSpeed;
    public int staminaRechargeDelay;
    int staminaRechargeCounter;

    [HideInInspector]
    public float health;
    [HideInInspector]
    public float stamina;
    
    public int shieldDeactivatedAtHealth;
    public InteractableArea interactableArea;
    public SkinnedMeshRenderer[] playerMeshes;
    public Color HitColor;
    [Header("Control Settings")]
    public CinemachineVirtualCamera TopDownCam;
    public CinemachineVirtualCamera FirstPerson;
    public Camera camera;
    public LayerMask Everything;
    public LayerMask WithoutPlayer;
    public LayerMask RayShoot;
    public ConstantForce playerForce;
    public Transform FPC;
    public Transform FPCpos;
    public Transform orientation;
    public LayerMask Ground;
    public int moveSpeed;
    public int sprintSpeed;
    public float moveAcceleration;
    public int jumpStrengh;
    public bool onGround;
    public int maxSlope = 45;
    RaycastHit slopeHit;
    float sprint;
    float moveX;
    float moveY;
    float mouseX;
    float mouseY;
    public float mouseSens;

    float xRotation;
    float yRotation;

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
    [HideInInspector]
    public int lastUsedWeapon;

    int bulletDelayCounter;
    bool reloading;
    
    int weaponHoldAimAfterShoot;
    public TwoBoneIKConstraint GunAimWalkIdle;
    public TwoBoneIKConstraint GunAimRun;
    public Rig MachineGunHold;
    public Rig MachineGunAim;
    public Rig MachineGunAimRun;
    bool fadeAimIn;

    [Header("Grenades")]
    public GameObject grenadePrefab;
    public Transform GrenadeSpawnPoint;
    public TwoBoneIKConstraint GrenadeThrow;
    bool fadeThrowIn;
    public int grenadeThrowPower;
    public int grenadesLeft;
    public int maxGrenades;
    [Header("Lightsaber")]
    public GameObject lightSaberObject;
    public Rig KatanaHold;
    public Rig KatanaAttack1;
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
        health = maxHealth;
        stamina = maxStamina;
        grenadesLeft = maxGrenades;
        UpdateWeapon(currentWeapon);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.V))
        {
            if(currentInput == 0)
            {
                currentInput = 1;
            }
            else
            {
                currentInput = 0;
            }
        }


        if(currentInput == 0)
        {
            TopDownCam.Priority = 1;
            FirstPerson.Priority = 0;
            camera.cullingMask = Everything;
        }
        else
        {
            TopDownCam.Priority = 0;
            FirstPerson.Priority = 1;
            camera.cullingMask = WithoutPlayer;
        }

        FPC.position = FPCpos.position;
        ProcessInput();
        AnimatePlayer();
    }


    private void FixedUpdate()
    {
        

        if (Input.GetMouseButton(0) && weapons[currentWeapon].allowButtonHold)
        {
            UpdateWeapon(currentWeapon);
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

        if(health < maxHealth)
        {
            if(healthRechargeCounter > 0)
            {
                healthRechargeCounter--;
            }
            else
            {
                health += healthRechargeSpeed;
            }
        }
        else
        {
            healthRechargeCounter = healthRechargeDelay;
        }
        
    }

    public void DealDamage(int damage)
    {
        health = health - damage;
        healthRechargeCounter = healthRechargeDelay;
        StartCoroutine(Hit());
    }
    public void ProcessInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSens;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSens;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        if (Input.GetKeyDown("space"))
        {
            if (onGround)
            {
                playerRB.AddForce(0, jumpStrengh, 0, ForceMode.Impulse);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && stamina - sprintStamina > 0 && (moveX != 0 || moveY != 0) && onGround)
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
            UpdateWeapon(currentWeapon);
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
            UpdateWeapon(-1);
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
            UpdateWeapon(currentWeapon);
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
            UpdateWeapon(currentWeapon);
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
        if (Physics.Raycast(transform.position, Vector3.down, 2f)){
            onGround = true;
        }
        else
        {
            onGround = false;
        }

        //Debug.DrawLine(transform.position, transform.position - new Vector3(0,2,0), Color.green);

        if (currentInput == 0)
        {
            moveDir = new Vector2(moveX, moveY).normalized;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


            //playerRB.velocity = new Vector3(moveDir.x * (moveSpeed + (sprint * sprintSpeed)), playerRB.velocity.y, moveDir.y * (moveSpeed + (sprint * sprintSpeed)));
            if (onGround)
            {
                playerForce.force = new Vector3(moveDir.x * (moveSpeed + (sprint * sprintSpeed)), playerRB.velocity.y, moveDir.y * (moveSpeed + (sprint * sprintSpeed))) * 200;
                playerForce.relativeForce = new Vector3(0, 0, 0);
                playerRB.drag = 0.8f;
            }
            else
            {
                playerForce.force = new Vector3(0, 0, 0);
                playerForce.relativeForce = new Vector3(0, 0, 0);
                playerRB.drag = 0.2f;
            }


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
                playerRB.transform.LookAt(new Vector3(pointToLook.x, playerRB.transform.position.y, pointToLook.z));
            }

        }
        else
        {


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //moveDir = new Vector2(moveX, moveY).normalized;



            FPC.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            playerRB.transform.rotation = Quaternion.Euler(0, yRotation, 0);

            if (onGround)
            {
                playerForce.force = new Vector3(0, 0, 0);
                playerForce.relativeForce = new Vector3(moveX * (moveSpeed + (sprint * sprintSpeed)), 0, moveY * (moveSpeed + (sprint * sprintSpeed))) * 200;
                playerRB.drag = 0.8f;
            }
            else
            {
                playerForce.force = new Vector3(0, 0, 0);
                playerForce.relativeForce = new Vector3(0, 0, 0);
                playerRB.drag = 0.2f;
            }
            if (OnSlope())
            {
                print("OnSlope");
                playerRB.AddForce(GetSlopeDirection() * moveSpeed * 300f, ForceMode.Force);
            }

            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;

            if(Physics.Raycast(camera.transform.position, camera.transform.forward + camera.transform.position, out hit, 80,RayShoot))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(45);
            }

            Debug.DrawLine(camera.transform.position, targetPoint, Color.red);

            //Debug.DrawLine(camera.transform.position, camera.transform.forward * 10 + camera.transform.position, Color.red);

            WeaponOrigin.transform.LookAt(targetPoint);


        }
    }

    private bool OnSlope()
    {
        Debug.DrawLine(transform.position, slopeHit.point, Color.cyan);
        if (Physics.Raycast(playerRB.transform.position + (playerRB.transform.forward / 2), Vector3.down,out slopeHit, 5))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlope && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
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

    public void UpdateWeapon(int weapon)
    {
        foreach(Weapon W in weapons)
        {
            W.weaponObject.SetActive(false);
        }
        lightSaberObject.SetActive(false);

        if (weapon < 0)
        {
            lastUsedWeapon = 0;
            lightSaberObject.SetActive(true);
        }
        else
        {
            lastUsedWeapon = weapon + 1;
            weapons[weapon].weaponObject.SetActive(true);
        }
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
                grenadesLeft--;
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

        if (!lightSaberObject.activeInHierarchy)
        {
            if (KatanaHold.weight > 0)
            {
                KatanaHold.weight = KatanaHold.weight - 0.1f;
            }

            if (KatanaAttack1.weight > 0)
            {
                KatanaAttack1.weight = KatanaAttack1.weight - 0.1f;
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


                    if (MachineGunAim.weight > 0)
                    {
                        MachineGunAim.weight = MachineGunAim.weight - 0.1f;
                    }

                    if (MachineGunHold.weight > 0)
                    {
                        MachineGunHold.weight = MachineGunHold.weight - 0.1f;
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

                    if (fadeAimIn)
                    {
                        if (weaponHoldAimAfterShoot > 0)
                        {
                            weaponHoldAimAfterShoot--;

                            if (MachineGunAimRun.weight <= 1)
                            {
                                MachineGunAimRun.weight = MachineGunAimRun.weight + 0.1f;
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

                        if (MachineGunAimRun.weight > 0)
                        {
                            MachineGunAimRun.weight = MachineGunAimRun.weight - 0.1f;
                        }


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

                        if (MachineGunAim.weight > 0)
                        {
                            MachineGunAim.weight = MachineGunAim.weight - 0.1f;
                        }

                        if (MachineGunHold.weight > 0)
                        {
                            MachineGunHold.weight = MachineGunHold.weight - 0.1f;
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
                        if (GunAimRun.weight > 0)
                        {
                            GunAimRun.weight = GunAimRun.weight - 0.1f;
                        }

                        if (MachineGunAim.weight > 0)
                        {
                            MachineGunAim.weight = MachineGunAim.weight - 0.1f;
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
        else
        {
            //Katana

            //Disable other rigs
            if (GunAimRun.weight > 0)
            {
                GunAimRun.weight = GunAimRun.weight - 0.1f;
            }

            if (GunAimWalkIdle.weight > 0)
            {
                GunAimWalkIdle.weight = GunAimWalkIdle.weight - 0.1f;
            }

            if (MachineGunAim.weight > 0)
            {
                MachineGunAim.weight = MachineGunAim.weight - 0.1f;
            }

            if (MachineGunAimRun.weight > 0)
            {
                MachineGunAimRun.weight = MachineGunAimRun.weight - 0.1f;
            }

            if (MachineGunHold.weight > 0)
            {
                MachineGunHold.weight = MachineGunHold.weight - 0.1f;
            }

            //////////////////////////////////////////////////////////////////
            ///


            if (lightSabernAttacking)
            {
                //Attacking
                if (KatanaHold.weight > 0)
                {
                    KatanaHold.weight = KatanaHold.weight - 0.2f;
                }
                
                if (KatanaAttack1.weight <= 1)
                {
                    KatanaAttack1.weight = KatanaAttack1.weight + 0.2f;
                }
                
            }
            else
            {
                //HoldWeapon
                if (KatanaHold.weight <= 1)
                {
                    KatanaHold.weight = KatanaHold.weight + 0.2f;
                }

                if (KatanaAttack1.weight > 0)
                {
                    KatanaAttack1.weight = KatanaAttack1.weight - 0.2f;
                }
            }
        }
    }

    private IEnumerator Hit()
    {
        foreach (SkinnedMeshRenderer SMR in playerMeshes)
        {
            SMR.material.EnableKeyword("_EMISSION");
            SMR.material.SetColor("_EmissionColor", HitColor);
        }
        yield return new WaitForSecondsRealtime(0.2f);

        foreach (SkinnedMeshRenderer SMR in playerMeshes)
        {
            SMR.material.SetColor("_EmissionColor", Color.black);
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

    public void AddLoot(int MagazinesGun, int MagazinesMachinegun, int Grenades, int TechParts)
    {
        weapons[0].magazinesLeft += MagazinesGun;
        weapons[1].magazinesLeft += MagazinesMachinegun;

        if (grenadesLeft + Grenades < maxGrenades) {
            grenadesLeft += Grenades;
        }
        else
        {
            grenadesLeft = maxGrenades;
        }



        StartCoroutine(UI.DisplayPickup(MagazinesGun, MagazinesMachinegun, Grenades, TechParts));
    }
}
