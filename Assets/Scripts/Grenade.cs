using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Grenade : MonoBehaviour
{
    public int lifetime = 150;
    public int FXlifetime = 100;
    bool exploded;
    public UnityEvent ExplodeEvents;

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
        ExplodeEvents.Invoke();

    }
}
