using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponRef : MonoBehaviour
{
  
    [SerializeField]
    public Weapon weapon;

    [SerializeField]
    int levelToUnlock;

    private void Start()
    {
        levelToUnlock = weapon.levelToUnlock;
        if (levelToUnlock <= GameManager.instance.GetCurrentLevel())
        {
            UnlockWeapon();
        }
    }
    //set button interactable if weapon is unlocked
    void UnlockWeapon()
    {
      GetComponent<Button>().interactable = true;
    }
}
