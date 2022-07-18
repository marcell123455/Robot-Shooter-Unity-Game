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
    public int reloadTime;
    public bool allowButtonHold;
    [Header("Bullet Settings")]
    public Transform bulletSpawnPoint;
    public GameObject BulletPrefab;
    public AudioClip shootSound;
    public int damage;
    public int additionalRandomDamageMax;
    public int bulletDelay;
    public float bulletFlySpeedForce;


}
