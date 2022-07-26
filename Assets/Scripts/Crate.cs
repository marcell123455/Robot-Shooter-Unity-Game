using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [Header("Crate Settings:")]
    bool opened;
    public Material opendMaterial;
    public MeshRenderer[] meshRenderers;
    public Animation animation;
    Player player;
    [Header("Loot Settings")]
    public int grenades;
    public int gunMagazines;
    public int machineGunMagazines;
    public int techParts;
    public void openCrate()
    {

        if (!opened)
        {
            player = GameObject.Find("Player").GetComponent<Player>();

            foreach (MeshRenderer MR in meshRenderers)
            {
                MR.material = opendMaterial;
            }
            animation.Play();
            opened = true;

            player.AddLoot(gunMagazines, machineGunMagazines, grenades, techParts);
        }
    }




}
