using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] LevelButtons;
    
    public void Start()
    {
        if(LevelButtons.Length > 0)
        {
            int avaibleLevels;
            avaibleLevels = PlayerPrefs.GetInt("unlockedLevels");
        
            foreach(GameObject B in LevelButtons)
            {
                B.SetActive(false);
            }

            for(int i = 0; i < avaibleLevels; i++)
            {
                LevelButtons[i].SetActive(true);
            }
        }
    }

}
