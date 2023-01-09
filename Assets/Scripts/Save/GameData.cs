using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currentLevel;
    public float levelProgress;
    public float playerHp;


    public float levelShots;
    public float shotsMissed;
    public int levelKills;

    public float levelTime;

    public List<Weapon> weapons;
    public List<float> weaponTimers;
    public int weaponIndex;

    public bool lastLevelCleared;
    public bool lastLevelUnlockClaimed;
    public int subLevel;
    public int scoreCounter;
    //values defined in this constructor will be the default values
    //game starts with when theres no data to load

    public GameData()
    {
        this.currentLevel = 1;
        this.levelProgress = 0;
        this.playerHp = 100;

        
        this.levelShots=0;
        this.shotsMissed=0;
        this.levelKills = 0;

        this.levelTime = 0;

        this.weapons = new List<Weapon>();
        this.weaponTimers = new List<float>();
        this.weaponIndex = 0;

        this.lastLevelCleared = false;
        this.lastLevelUnlockClaimed = false;
        this.subLevel = 1;
        this.scoreCounter = 0;
    }
}
