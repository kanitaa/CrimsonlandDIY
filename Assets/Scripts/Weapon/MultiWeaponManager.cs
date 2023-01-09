using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MultiWeaponManager : MonoBehaviour
{
    [SerializeField]
    public MultiPlayerController playerController;

  
    [SerializeField]
    GameObject currentWeapon;

    [SerializeField]
    WeaponRef[] weaponRef;


    
    public WeaponRef firstWeaponRef;

    AudioSource audioS;

    [SerializeField]
    AudioClip changeWeaponClip;
    private void Start()
    {
        audioS = GetComponent<AudioSource>();
        firstWeaponRef = weaponRef[0];

    }

    //equip weapon, get reference from ui button which has a script that defines the name, MUST match scriptable objects weapon name!!
    public void EquipWeapon(WeaponRef weaponRef)
    {
          
        audioS.PlayOneShot(changeWeaponClip);
        //if there is already old weapon, destroy it to avoid multiple weapons
        if (playerController.currentWeapon != null)
        {
                if (playerController.currentWeapon.weaponName == "P90") //save amount of bullets left on weapon on game manager depending on which weapon was used
                {
                    MultiGameManager.instance.bulletsP90 = playerController.GetComponentInChildren<MultiShoot>().bulletAmount;
                }
                else if (playerController.currentWeapon.weaponName == "Pistol")
                {
                    MultiGameManager.instance.bulletsPistol = playerController.GetComponentInChildren<MultiShoot>().bulletAmount;
                }
                else if (playerController.currentWeapon.weaponName == "Rifle")
                {
                    MultiGameManager.instance.bulletsRifle = playerController.GetComponentInChildren<MultiShoot>().bulletAmount;
                }
            
            MultiGameManager.instance.aim.GetComponent<Image>().enabled = false;
            playerController.DestroyWeapon();
        }

        //change players current weapon
        playerController.currentWeapon = weaponRef.weapon;
        //show current weapon in UI
        currentWeapon.GetComponent<Image>().sprite = weaponRef.weapon.weaponSprite;

       
        //Start counter on chosen weapon to see which one has been used most
        MultiGameManager.instance.SetCurrentWeapon(weaponRef.weapon);

        //instantiate selected weapon
        playerController.InstantiateWeapon();


    }



    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
