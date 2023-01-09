using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
 
    //variables for item to be unlocked
    string unlockTitle;
    string unlockDescriptionTitle;
    string unlockDescriptionText;
    Sprite unlockSprite;

    [SerializeField]
    Sprite multiPlayerSprite;

    [SerializeField]
    WeaponRef[] weaponToUnlock;

    //set values
   void Unlock(string unlockTitle, string unlockDesTitle, string unlockDescription, Sprite unlockSprite)
    {
        this.unlockTitle = "New "+ unlockTitle+" unlocked!";
        this.unlockDescriptionTitle = unlockDesTitle;
        this.unlockDescriptionText = unlockDescription;
        this.unlockSprite = unlockSprite;
    }
    //unlock different stuff when level ends
    public void UnlockLevelUpgrade()
    {
        if (GameManager.instance.GetCurrentLevel() == 2)
        {
            Weapon wep = weaponToUnlock[0].weapon;
            //set unlock info for level end panel
            Unlock("weapon", wep.weaponName, wep.weaponDescription, wep.weaponSprite);
           
        }
        if(GameManager.instance.GetCurrentLevel() == 3)
        {
            Weapon wep = weaponToUnlock[1].weapon;
            //set unlock info for level end panel
            Unlock("weapon", wep.weaponName, wep.weaponDescription, wep.weaponSprite);
        }

        if (GameManager.instance.lastLevelCleared)
        {
            Unlock("game modes", "Endless and Multiplayer!", "Play this awesome game with other players or try the new solo mode and see how far you can get. Wooo!", multiPlayerSprite);
        }
    }
    #region forward values for UI references
    public string GetUnlockTitle()
    {
        return unlockTitle;
    }
    public string GetUnlockDescriptionTitle()
    {
        return unlockDescriptionTitle;
    }
    public string GetUnlockDescriptionText()
    {
        return unlockDescriptionText;
    }
    public Sprite GetUnlockSprite()
    {
        return unlockSprite;
    }
    #endregion
}
