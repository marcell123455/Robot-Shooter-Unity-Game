using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}
    public class Bullet : MonoBehaviour
{
    public LayerMask Damagemask;
    public int damage;
    int lifetime = 100;
    public void OnCollisionEnter(Collision collision)
    {
        if (UnityExtensions.Contains(Damagemask, collision.gameObject.layer))
        {
            Debug.Log("Target in mask");
            collision.gameObject.GetComponent<Enemy>().DealEnemyDamage(damage);
        }
        else
        {
            Debug.Log("target not in mask");
        }
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
