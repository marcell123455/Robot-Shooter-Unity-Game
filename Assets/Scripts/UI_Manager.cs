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

            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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
    }

    public void SaveAudioSettings()
    {

        GS.SaveGameSettings(masterVolume.value, musicVolume.value, effectsVolume.value);
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

        yield return new WaitForSecondsRealtime(2);

        PickupInfo.alpha = 0f;

    }


    
}
