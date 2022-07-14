using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Weapon
{
    [Header("Weapon Settings")]
    public bool unlocked;
    public string Name;
    public GameObject weaponObject;
    public Transform bulletSpawnPoint;
    public int maxMagazine;
    public int magazineBulletCount;
    public int reloadTime;
    public bool allowButtonHold;
    [Header("Bullet Settings")]
    public GameObject BulletPrefab;
    public AudioClip shootSound;
    public int damage;
    public int additionalRandomDamageMax;
    public int bulletDelay;
    public float bulletFlySpeedForce;
    [Header("Runtime Values")]
    public int magazinesLeft;
    public int bulletsLeft;


}
