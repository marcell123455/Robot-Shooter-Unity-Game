using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageArea : MonoBehaviour
{

    public List<Enemy> affectedEnemys = new List<Enemy>();
    public Player player;
    //Add Layermask!!
    public LayerMask Targets;

    private void OnTriggerEnter(Collider other)
    {
        if (UnityExtensions.Contains(Targets,other.gameObject.layer))
        {
            if (other.GetComponent<Enemy>() != null)
            {
                affectedEnemys.Add(other.GetComponent<Enemy>());
            }

            if (other.GetComponent<Player>() ?? true)
            {
                player = other.GetComponent<Player>();
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < affectedEnemys.Count; i++)
        {
            if (affectedEnemys[i] == null)
            {
                affectedEnemys.RemoveAt(i);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (UnityExtensions.Contains(Targets, other.gameObject.layer))
        {
            if (other.GetComponent<Enemy>() != null)
            {
                affectedEnemys.Remove(other.GetComponent<Enemy>());
            }

            if (other.GetComponent<Player>() ?? true)
            {
                player = null;
            }
        }
    }

    public void DealDamage(int damage)
    {
        foreach (Enemy E in affectedEnemys)
        {
            E.DealEnemyDamage(damage);
        }

        if(player != null)
        player.DealDamage(damage);
    }
}
