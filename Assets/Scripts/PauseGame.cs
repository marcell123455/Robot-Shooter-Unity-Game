using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject GameHUD;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            PauseMenu.SetActive(true);
            GameHUD.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            PauseMenu.SetActive(false);
            GameHUD.SetActive(true);
        }
    }
}
