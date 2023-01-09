using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    [SerializeField]
    GameObject currentWeapon;

    [SerializeField]
    WeaponRef [] weaponRef;

    AudioSource audioS;

    [SerializeField]
    AudioClip changeWeaponClip;
    private void Start()
    {
      
        audioS = GetComponent<AudioSource>();
        GameManager.instance.FindManagers(); //help gamemanager to find references, replace with OnLevelLoad if time!
        
        EquipWeapon(weaponRef[0]);
     
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
                GameManager.instance.bulletsP90 = playerController.GetComponentInChildren<RaycastShoot>().bulletAmount; 
            }
            else if (playerController.currentWeapon.weaponName == "Pistol")
            {
                GameManager.instance.bulletsPistol = playerController.GetComponentInChildren<RaycastShoot>().bulletAmount;
            }
            else if (playerController.currentWeapon.weaponName == "Rifle")
            {
                GameManager.instance.bulletsRifle = playerController.GetComponentInChildren<RaycastShoot>().bulletAmount;
            }
        
            GameManager.instance.aim.GetComponent<Image>().enabled = false;
            Destroy(playerController.weaponSlot.gameObject.transform.GetChild(0).gameObject);
        }
      
        //change players current weapon
        playerController.currentWeapon = weaponRef.weapon;
        //show current weapon in UI
        currentWeapon.GetComponent<Image>().sprite = weaponRef.weapon.weaponSprite;
            
        //instantiate selected weapon
        Instantiate(playerController.currentWeapon.weaponPrefab, playerController.weaponSlot.transform);

        //Start counter on chosen weapon to see which one has been used most
        GameManager.instance.SetCurrentWeapon(weaponRef.weapon);
    
    }

  public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
