using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public int shieldDeactivatedAtHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealEnemyDamage(int damage)
    {
        if(health > 0)
        {
            health = health - damage;
        }
    }
}
