using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int lifetime = 100;
    public void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

    public void FixedUpdate()
    {


        if(lifetime > 0)
        {
            lifetime--;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
