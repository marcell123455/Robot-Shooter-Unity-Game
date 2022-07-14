using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int lifetime = 150;
    public int FXlifetime = 100;
    public MeshRenderer grenadeObject;
    public GameObject explosionEffects;
    public AudioSource explosionAudio;
    bool exploded;
    private void FixedUpdate()
    {
     if (lifetime > 0)
        {
            lifetime--;
        }
        else
        {
            if (!exploded)
            {
                Explode();
            }

            if(FXlifetime > 0)
            {
                FXlifetime--;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }   
    }

    public void Explode()
    {
        exploded = true;
        explosionAudio.Play();
        grenadeObject.enabled = false;

    }
}
