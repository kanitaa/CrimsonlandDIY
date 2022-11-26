using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRef : MonoBehaviour
{
  
    [SerializeField]
    public Weapon weapon;


    //set button interactable if weapon is unlocked
    public void UnlockWeapon(string weaponName)
    {
        if (weapon.weaponName == weaponName)
        {
            GetComponent<Button>().interactable = true;
        }
        
    }
}
