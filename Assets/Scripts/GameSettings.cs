using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    [Header("References")]
    public UI_Manager uI_Manager;
    public int techParts;

    public void Awake()
    {
        //set default keys if no save data exists
        if (!PlayerPrefs.HasKey("unlockedLevels")) {
            SaveGameSettings(0, -10, 0);
            PlayerPrefs.SetInt("unlockedLevels", 1);
            PlayerPrefs.SetInt("techParts", 0);

            PlayerPrefs.SetInt("gunStage", 0);
            PlayerPrefs.SetInt("machineGunStage", 0);

            PlayerPrefs.SetInt("reloadDecreasementStage", 0);
            PlayerPrefs.SetInt("maxHealthStage", 0);
            PlayerPrefs.SetInt("maxStaminaStage", 0);
            PlayerPrefs.SetInt("maxGrenadesStage", 0);
        }
    }

    public void UnlockNextLevel()
    {
        if (PlayerPrefs.GetInt("unlockedLevels") < 4 && SceneManager.GetActiveScene().buildIndex -1 == PlayerPrefs.GetInt("unlockedLevels"))
        {
            PlayerPrefs.SetInt("unlockedLevels", PlayerPrefs.GetInt("unlockedLevels") + 1);
        }
    }

    public void LoadLevel(int level)
    {
        GameObject.Find("SceneLoader").GetComponent<SceneLoadingManager>().LoadScene(level);
    }

    public void LoadLastUnlockedLevel()
    {
        GameObject.Find("SceneLoader").GetComponent<SceneLoadingManager>().LoadScene(PlayerPrefs.GetInt("unlockedLevels") + 1);
    }

    public void UpgradePlayerStat(int stat, int stage)
    {
        if (stat == 0)
            PlayerPrefs.SetInt("reloadDecreasementStage", stage);
        
        if (stat == 1)
            PlayerPrefs.SetInt("maxHealthStage", stage);
        
        if (stat == 2)
            PlayerPrefs.SetInt("maxStaminaStage", stage);
        
        if (stat == 3)
            PlayerPrefs.SetInt("maxGrenadesStage", stage);
    }

    public void TechPartsTransaction(int amount)
    {
        PlayerPrefs.SetInt("techParts", PlayerPrefs.GetInt("techParts") + amount);
    }

    public void SaveGameSettings(float masterVol, float musicVol, float effectsVol)
    {
        PlayerPrefs.SetFloat("MasterVol", masterVol);
        PlayerPrefs.SetFloat("MusicVol", musicVol);
        PlayerPrefs.SetFloat("EffectsVol", effectsVol);
    }

    public void DeleteSavegame()
    {
        PlayerPrefs.DeleteAll();
    }


}
