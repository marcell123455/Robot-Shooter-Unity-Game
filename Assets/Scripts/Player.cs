using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Header("Control Settings")]
    public int moveSpeed;
    public int sprintSpeed;
    public float moveAcceleration;
    public float sprint;
    public int sprintStamina;

    public Rigidbody playerRB;
    Vector2 moveDir;
    Vector2 mousePos;
    [Header("Weapon Settings")]
    public Weapon[] weapons;
    public Transform WeaponOrigin;
    public LayerMask WeaponAimlayerMask;
    public int currentWeapon;
    Weapon bulletDelay;

    [Header("Grenades")]
    public GameObject grenadePrefab;
    public Transform GrenadeSpawnPoint;
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
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
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
    }

    public void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");


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
            Fire();
        }

        if (Input.GetMouseButton(0) && weapons[currentWeapon].allowButtonHold)
        {
            Fire();
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
            Vector3 pointToLook = cameraRay.GetPoint(raylengh);
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
                GameObject bullet = Instantiate(weapons[currentWeapon].BulletPrefab, weapons[currentWeapon].bulletSpawnPoint.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].bulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce, ForceMode.Impulse);
                weapons[currentWeapon].weaponObject.GetComponent<AudioSource>().PlayOneShot(weapons[currentWeapon].shootSound);
            }
            else
            {
                ReloadWeapon();
            }
        }
    }

    public void PlayerInteraction()
    {

    }

    public void ThrowGrenade()
    {
        if (!lightSabernAttacking)
        {
            if (grenadesLeft > 0)
            {
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

    public void ReloadWeapon()
    {

    }
}
