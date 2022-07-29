using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask TriggerDoor;
    public Transform[] Doors;
    public MeshRenderer[] DoorMeshes;
    public Color OpenColor;
    public Color LockedColor;

    public int moveSpeed;
    public bool open;
    public bool locked;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!locked)
        {
            foreach (MeshRenderer MR in DoorMeshes)
            {
                   MR.material.EnableKeyword("_EMISSION");
                   MR.material.SetColor("_EmissionColor", OpenColor);    
            }

            if (open)
            {
                foreach (Transform D in Doors)
                {
                    D.localPosition = Vector3.MoveTowards(D.localPosition, D.forward * 2, moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                foreach (Transform D in Doors)
                {
                    D.localPosition = Vector3.MoveTowards(D.localPosition, new Vector3(0, 0, 0), moveSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            foreach (Transform D in Doors)
            {
                D.localPosition = Vector3.MoveTowards(D.localPosition, new Vector3(0, 0, 0), moveSpeed * Time.deltaTime);
            }

            foreach (MeshRenderer MR in DoorMeshes)
            {
                MR.material.EnableKeyword("_EMISSION");
                MR.material.SetColor("_EmissionColor", LockedColor);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (UnityExtensions.Contains(TriggerDoor, other.gameObject.layer))
        {
            open = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (UnityExtensions.Contains(TriggerDoor, other.gameObject.layer))
        {
            open = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (UnityExtensions.Contains(TriggerDoor, other.gameObject.layer))
        {
            open = true;
        }

    }

    public void ToggleLockState()
    {
        if (locked)
        {
            locked = false;
        }
        else
        {
            locked = true;
        }
    }
}
