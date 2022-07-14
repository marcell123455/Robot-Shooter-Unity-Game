using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Weapon[] weapons;
    public float health;
    public float shield;
    [Header("Control Settings")]
    public int moveSpeed;
    public Rigidbody playerRB;
    [Header("Weapon Settings")]
    public Transform WeaponOrigin;
    public LayerMask WeaponAimlayerMask;
    public int currentWeapon;
    Weapon bulletDelay;

    [Header("Grenades")]
    public GameObject grenadePrefab;
    public Transform GrenadeSpawnPoint;
    public int grenadeThrowPower;
    public int grenadesLeft;


    Vector2 moveDir;
    Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void FixedUpdate()
    {
        PhysicsCalculation();
    }

    public void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

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




    }
    RaycastHit hit;
    public void PhysicsCalculation()
    {
        playerRB.velocity = new Vector3(moveDir.x * moveSpeed,0, moveDir.y * moveSpeed);

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundplane = new Plane(Vector3.up, Vector3.zero);
        float raylengh;

        RaycastHit hit;

        if (Physics.Raycast(cameraRay.origin, cameraRay.direction, out hit, Mathf.Infinity, WeaponAimlayerMask))
        {
            WeaponOrigin.transform.LookAt(new Vector3(hit.point.x, hit.point.y, hit.point.z));
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
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
        if(weapons[currentWeapon].bulletsLeft > 0)
        {
            GameObject bullet = Instantiate(weapons[currentWeapon].BulletPrefab, weapons[currentWeapon].bulletSpawnPoint.position,Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddRelativeForce(weapons[currentWeapon].bulletSpawnPoint.forward * weapons[currentWeapon].bulletFlySpeedForce,ForceMode.Impulse);
            weapons[currentWeapon].weaponObject.GetComponent<AudioSource>().PlayOneShot(weapons[currentWeapon].shootSound);
        }
        else
        {
            ReloadWeapon();
        }
    }

    public void ThrowGrenade()
    {
        if(grenadesLeft > 0)
        {
            GameObject grenade = Instantiate(grenadePrefab, GrenadeSpawnPoint.position, Quaternion.identity);
            grenade.GetComponent<Rigidbody>().AddRelativeForce(GrenadeSpawnPoint.forward * grenadeThrowPower, ForceMode.Impulse);
        }
    }

    public void ReloadWeapon()
    {

    }
}
