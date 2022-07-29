using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interact Settings")]
    public UnityEvent interactEvents;
    [Header("Trigger Settings")]
    public bool DestroyAfterTriggerEnter;
    public LayerMask triggerMask;
    public UnityEvent onTriggerEnterEvents;
    public UnityEvent onTriggerExitEvents;
    [Header("SceneLoadingSettings")]
    public bool loadNextScene;
    public int sceneToLoad;
    [Header("TechPartSettings")]
    public bool isTechPart;
    public int techPartAmount;
    [Header("ChangeView")]
    public bool changePlayerView;
    public int view;


    public void Start()
    {
        if (isTechPart)
        {
            this.transform.SetParent(null);
        }
    }

    public void Interact()
    {
        interactEvents.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (UnityExtensions.Contains(triggerMask, other.gameObject.layer))
        {
            onTriggerEnterEvents.Invoke();

            if (loadNextScene)
            {
                GameObject.Find("SceneLoader").GetComponent<SceneLoadingManager>().LoadScene(sceneToLoad);
            }

            if (isTechPart)
            {
                GameObject.Find("Scriptmanager").GetComponent<GameSettings>().TechPartsTransaction(techPartAmount);
                GameObject.Find("Scriptmanager").GetComponent<UI_Manager>().StartCoroutine(GameObject.Find("Scriptmanager").GetComponent<UI_Manager>().DisplayPickup(0, 0, 0, techPartAmount));
                Destroy(this.gameObject);
            }

            if (changePlayerView)
            {
                GameObject.Find("Player").GetComponent<Player>().currentInput = view;
            }

            if (DestroyAfterTriggerEnter)
            {
                Destroy(this.GetComponent<Interactable>());
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (UnityExtensions.Contains(triggerMask, other.gameObject.layer))
            onTriggerExitEvents.Invoke();
    }

    public void OnTriggerStay(Collider other)
    {
        if (UnityExtensions.Contains(triggerMask, other.gameObject.layer))
        {
            if (changePlayerView)
            {
                GameObject.Find("Player").GetComponent<Player>().currentInput = view;
            }
        }
    }
}
