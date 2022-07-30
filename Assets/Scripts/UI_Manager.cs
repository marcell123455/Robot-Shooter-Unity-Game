using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;


public class UI_Manager : MonoBehaviour
{
    //variables

    [Header("References")]
    public Player player;
    public AudioMixer mixer;
    public GameSettings GS;
    [Header("Settings UI")]
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider effectsVolume;
    public Slider sensChange;



    [Header("Gameplay UI")]
    public Image healthBar;
    public Image staminaBar;
    public Image weaponIconImage;
    public Text ClipsLeft;
    public Text BulletsLeft;
    public Text GrenadesLeft;
    public GameObject Cross;

    [Header("WeaponIcos")]
    public Sprite[] weaponIcon;

    [Header("PickUpInfo")]
    public CanvasGroup PickupInfo;
    public Text gunAmmoPickup;
    public Text machineGunAmmoPickup;
    public Text grenadesPickup;
    public Text techPartsPickup;

    [Header("DebugPlayerBlends")]
    public Image KatanaHold;
    public Image KatanaAttack;

    public Image GrenadeThrow;
    public Image GunAimWalkIdle;
    public Image GunAimRun;

    public Image MachineGunHold;
    public Image MachineGunAim;
    public Image MachineGunAimRun;

    // Start is called before the first frame update
    void Start()
    {
        if(mixer != null)
        {
            if (PlayerPrefs.HasKey("unlockedLevels"))
            {
                masterVolume.value = PlayerPrefs.GetFloat("MasterVol");
                musicVolume.value = PlayerPrefs.GetFloat("MusicVol");
                effectsVolume.value = PlayerPrefs.GetFloat("EffectsVol");
                sensChange.value = PlayerPrefs.GetFloat("SensetivityChange");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = player.health / player.maxHealth;
            staminaBar.fillAmount = player.stamina / player.maxStamina;
            weaponIconImage.sprite = weaponIcon[player.lastUsedWeapon];
            BulletsLeft.text = player.weapons[player.currentWeapon].bulletsLeft.ToString();
            ClipsLeft.text = player.weapons[player.currentWeapon].magazinesLeft.ToString();
            GrenadesLeft.text = player.grenadesLeft.ToString();

            if (player.currentInput == 0)
            {
                Cross.SetActive(false);
            }
            else
            {
                Cross.SetActive(true);
            }
        }

        if(mixer != null)
        {
            mixer.SetFloat("Master", masterVolume.value);
            mixer.SetFloat("Music", musicVolume.value);
            mixer.SetFloat("Effects", effectsVolume.value);
        }

        if(player != null)
        {
            player.mouseSens = 700f + sensChange.value;
        }

        if(KatanaHold != null)
        {
            KatanaHold.fillAmount = player.KatanaHold.weight;
            KatanaAttack.fillAmount = player.KatanaAttack1.weight;
            GrenadeThrow.fillAmount = player.GrenadeThrow.weight;
            GunAimWalkIdle.fillAmount = player.GunAimWalkIdle.weight;
            GunAimRun.fillAmount = player.GunAimRun.weight;
            MachineGunHold.fillAmount = player.MachineGunHold.weight;
            MachineGunAim.fillAmount = player.MachineGunAim.weight;
            MachineGunAimRun.fillAmount = player.MachineGunAimRun.weight;
        }

    }

    public void SaveAudioSettings()
    {
        print("Saved Settings");
        GS.SaveGameSettings(masterVolume.value, musicVolume.value, effectsVolume.value, sensChange.value);
    }
    public IEnumerator DisplayPickup(int MagazinesGun, int MagazinesMachinegun, int Grenades, int TechParts)
    {
        if (MagazinesGun > 0)
        {
            gunAmmoPickup.text = "Gun Magazines +" + MagazinesGun.ToString();
        }
        else
        {
            gunAmmoPickup.text = "";
        }

        if (MagazinesMachinegun > 0)
        {
            machineGunAmmoPickup.text = "Rifle Magazines +" + MagazinesMachinegun.ToString();
        }
        else
        {
            machineGunAmmoPickup.text = "";
        }

        if (Grenades > 0)
        {
            grenadesPickup.text = "Grenades +" + Grenades.ToString();
        }
        else
        {
            grenadesPickup.text = "";
        }

        if (TechParts > 0)
        {
            techPartsPickup.text = "Tech Parts +" + TechParts.ToString();

        }
        else
        {
            techPartsPickup.text = "";
        }

        PickupInfo.alpha = 1f;

        yield return new WaitForSecondsRealtime(2f);

        PickupInfo.alpha = 0f;

    }


    
}
