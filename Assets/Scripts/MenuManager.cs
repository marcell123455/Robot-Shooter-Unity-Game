using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public GameObject[] menuParent;
    public GameObject[] Popup;

    public void SetMenu(int index)
    {
        foreach(GameObject M in menuParent)
        {
            M.SetActive(false);
        }
        menuParent[index].SetActive(true);
    }

    public void SetPopup(int index)
    {
        foreach (GameObject P in Popup)
        {
            P.SetActive(false);
        }

        Popup[index].SetActive(true);
    }

}
