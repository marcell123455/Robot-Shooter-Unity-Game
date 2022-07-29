using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableArea : MonoBehaviour
{
    [Header("GeneralSettings")]
    public LayerMask interactableObjectLayer;
    public List<Interactable> Interactables = new List<Interactable>();


    private void OnTriggerEnter(Collider other)
    {
        if (UnityExtensions.Contains(interactableObjectLayer, other.gameObject.layer))
        {
            Interactables.Add(other.GetComponent<Interactable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (UnityExtensions.Contains(interactableObjectLayer, other.gameObject.layer))
        {
            Interactables.Remove(other.GetComponent<Interactable>());
        }
    }

    public void InteractWithInteractables()
    {
        foreach(Interactable I in Interactables)
        {
            I.Interact();
        }
    }
}
