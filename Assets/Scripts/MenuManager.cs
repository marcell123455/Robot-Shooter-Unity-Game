using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public GameObject[] menuParent;

    public void SetMenu(int index)
    {
        foreach(GameObject M in menuParent)
        {
            M.SetActive(false);
        }
        if(index >= 0)
        menuParent[index].SetActive(true);
    }

}
