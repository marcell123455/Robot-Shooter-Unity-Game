using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interact Settings")]
    public UnityEvent interactEvents;
    [Header("Trigger Settings")]
    public LayerMask triggerMask;
    public UnityEvent onTriggerEnterEvents;
    public UnityEvent onTriggerExitEvents;

    public void Interact()
    {
        interactEvents.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(UnityExtensions.Contains(triggerMask,other.gameObject.layer))
        onTriggerEnterEvents.Invoke();
    }

    public void OnTriggerExit(Collider other)
    {
        if (UnityExtensions.Contains(triggerMask, other.gameObject.layer))
            onTriggerExitEvents.Invoke();
    }
}
