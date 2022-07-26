using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlattform : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform[] targets;
    public int stayDelay;
    public int travelSpeed;
    int currentTarget;
    int delay;
    Transform player;
    void Start()
    {
        currentTarget = 0;
        delay = stayDelay;
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targets[currentTarget].position, travelSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targets[currentTarget].position) < 0.001f)
        {
            if(delay > 0)
            {
                delay--;
            }
            else
            {
                if(currentTarget == targets.Length - 1)
                {
                    currentTarget = 0;
                }
                else
                {
                    currentTarget++;
                }


                delay = stayDelay;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            player.SetParent(this.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            player.SetParent(null);
        }
    }


}
