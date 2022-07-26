using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponUpgradeStage
{
    public int damage;
    public int maxMagazineBulletCount;
    public int fireSpeed;
}
    public class UpgradeStats : MonoBehaviour
    {
    [Header("References")]
    public GameSettings GS;
    public Player player;
    public UI_Manager uI_Manager;
    [Header("WeaponUpgradeStages")]
    public WeaponUpgradeStage[] gunStages;
    public WeaponUpgradeStage[] machinegunStages;

    [Header("PlayerUpgradeStages")]
    public int[] reloadStages;
    public int[] healthStages;
    public int[] staminaStages;
    public int[] maxGrenadesStages;
    public void UpgradeWeapon(int weapon,int costTechParts)
    {

    }

    public void SetWeaponUpgradeValues()
    {

    }

    public void UpgradePlayerStat(int stat)
    {
        if (stat == 0)
            GS.UpgradePlayerStat(stat, PlayerPrefs.GetInt("reloadDecreasementStage"));

        if (stat == 0)
            GS.UpgradePlayerStat(stat, PlayerPrefs.GetInt("maxHealthStage"));

        if (stat == 0)
            GS.UpgradePlayerStat(stat, PlayerPrefs.GetInt("maxStaminaStage"));

        if (stat == 0)
            GS.UpgradePlayerStat(stat, PlayerPrefs.GetInt("maxGrenadesStage"));
    }
}
