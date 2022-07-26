using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Weapon
{
    [Header("Player / General Settings")]
    public bool unlocked;
    public string Name;
    public GameObject weaponObject;
    public int maxMagazine;
    public int magazinesLeft;
    public int bulletsLeft;
    [Header("Weapon Settings")]

    public int magazineBulletCount;
    public float reloadTime;
    public bool allowButtonHold;
    [Header("Bullet Settings")]
    public Transform bulletSpawnPoint;
    public Transform SecBulletSpawnPoint;
    public GameObject BulletPrefab;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public int damage;
    public int additionalRandomDamageMax;
    public int bulletDelay;
    public float bulletDelayEnemy;
    public float bulletFlySpeedForce;
    public int shotsInRow = 10;




}
