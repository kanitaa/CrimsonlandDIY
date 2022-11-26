using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    UnlockManager unlockManager;

    [SerializeField]
    GameObject weaponPanel;

    [SerializeField]
    GameObject currentWeapon;

    [SerializeField]
    WeaponRef [] weaponRef;


    private void Start()
    {
        GameManager.instance.FindManagers(); //help gamemanager to find references, replace with OnLevelLoad if time!
        ToggleWeaponPanel();
        EquipWeapon(weaponRef[0]);
        unlockManager = GetComponent<UnlockManager>();
       
        //check based on level which weapons need to be unlocked
        for(int i = GameManager.instance.currentLevel; i > 0; i--)
        {
            weaponRef[GameManager.instance.currentLevel - i].UnlockWeapon(weaponRef[GameManager.instance.currentLevel - i].weapon.weaponName);
        }
       
      
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.instance.reloadingWeapon)
        {
            ToggleWeaponPanel();
        }else if(Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.reloadingWeapon)
        {
            Debug.Log("cant change weapon while reloading current weapon");
        }
    }
    //equip weapon, get reference from ui button which has a script that defines the name, MUST match scriptable objects weapon name!!
    public void EquipWeapon(WeaponRef weaponRef)
    {
        //if there is already old weapon, destroy it to avoid multiple weapons
        if (playerController.currentWeapon != null)
        {
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

    //used after level is cleared to show new unlock in end panel
    public void SetWeaponUnlock()
    {
        currentWeapon.SetActive(false);
        //if level is 2 or 3, unlock weapon from list that is one index smaller than current level
        if(GameManager.instance.currentLevel==2 || GameManager.instance.currentLevel == 3)
        {
            Weapon wep = weaponRef[GameManager.instance.currentLevel - 1].weapon;
            unlockManager.UnlockLevelUpgrade("weapon", wep.weaponName, wep.weaponDescription, wep.weaponSprite);
            weaponRef[GameManager.instance.currentLevel - 1].UnlockWeapon(weaponRef[GameManager.instance.currentLevel - 1].weapon.weaponName);
        }
      
    }
    public void ToggleWeaponPanel()
    {
        if (!weaponPanel.activeSelf)
        {
            weaponPanel.SetActive(true);
            Time.timeScale = 0;
            GameManager.instance.aim.GetComponent<Image>().enabled = false;
        }
        else
        {
            weaponPanel.SetActive(false);
            Time.timeScale = 1;
          
        }
    }
  
}
