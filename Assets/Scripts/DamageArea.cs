using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{

    public List<Enemy> affectedEnemys = new List<Enemy>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            affectedEnemys.Add(other.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            affectedEnemys.Remove(other.GetComponent<Enemy>());
        }
    }

    public void DealDamage(int damage)
    {
        foreach (Enemy E in affectedEnemys)
        {
            E.DealEnemyDamage(damage);
        }
    }
}
