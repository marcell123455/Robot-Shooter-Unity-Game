using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent Events;

    public void Interact()
    {
        Events.Invoke();
    }
}
