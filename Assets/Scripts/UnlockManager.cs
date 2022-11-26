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

    
    //define variable values
    public void UnlockLevelUpgrade(string unlockTitle, string unlockDesTitle, string unlockDescription, Sprite unlockSprite)
    {
        this.unlockTitle = "New "+ unlockTitle+" unlocked!";
        this.unlockDescriptionTitle = unlockDesTitle;
        this.unlockDescriptionText = unlockDescription;
        this.unlockSprite = unlockSprite;
    }

    //get functions for unlock variables
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
}
