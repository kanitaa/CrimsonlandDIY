using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    float currentTimer;
    float timerValue;
    float valueToAdd;

    public bool reloadingWeapon;
    [SerializeField]
    int currentLevel = 1;
    public bool lastLevelCleared;
    public bool lastLevelUnlockClaimed;
  
    public int subLevel { get; private set; }
    int xpReqMultiplier=30;

  
    public int xpRequired { get; private set; }
    [SerializeField]
    float levelProgress = 0;
    float playerHealth;

    [SerializeField]
    WeaponManager weaponManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    UnlockManager unlockManager;
    public SaveManager saveManager;
    public Spawner spawner;
    public AimIndicator aim;


    public delegate void EnablePowerup();
    public static event EnablePowerup OnPowerEnable;
    public delegate void DisablePowerup();
    public static event DisablePowerup OnPowerDisable;

    public bool levelOver;
    //variables to check favourite weapon
    [SerializeField]
    List<Weapon> weapons = new List<Weapon>();
    List<float> weaponTimers = new List<float>();
    Weapon currentWeapon;
    bool sameWeapon;
    int weaponIndex;

    //variables for level complete 
    float levelShots;
    float shotsMissed;
    int levelKills;
    float currentTime;
    float levelTime;
    int scoreCounter;

    public int bulletsP90, bulletsRifle, bulletsPistol;
    AudioSource audioS;
    [SerializeField]
    AudioClip levelCompleteClip;
    private void Awake()
    {
        CancelInvoke(); 

        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        saveManager = GetComponent<SaveManager>();
        subLevel = 1;
        DontDestroyOnLoad(this);
        if (lastLevelCleared)
        {
            xpRequired = 70 + (xpReqMultiplier * subLevel);
        }
        else
        {
            xpRequired = 70 + (xpReqMultiplier * currentLevel);
        }
        audioS = GetComponent<AudioSource>();
    

    }
    public void LoadData(GameData data)
    {
        this.currentLevel = data.currentLevel;
        this.levelProgress = data.levelProgress;
        this.playerHealth = data.playerHp;


        this.levelShots = data.levelShots;
        this.shotsMissed = data.shotsMissed;
        this.levelKills = data.levelKills;

        this.levelTime = data.levelTime;

        this.weapons = data.weapons;
        this.weaponTimers = data.weaponTimers;
        this.weaponIndex = data.weaponIndex;

        this.lastLevelCleared = data.lastLevelCleared;
        this.lastLevelUnlockClaimed = data.lastLevelUnlockClaimed;
        this.subLevel = data.subLevel;
        this.scoreCounter = data.scoreCounter;
    }

    public void SaveData(GameData data)
    {
        data.currentLevel = this.currentLevel;
        data.levelProgress = this.levelProgress;
        data.playerHp = this.playerHealth;

        data.levelShots = this.levelShots;
        data.shotsMissed = this.shotsMissed;
        data.levelKills = this.levelKills;

        data.levelTime += this.currentTime;

        data.weapons = this.weapons;
        data.weaponTimers = this.weaponTimers;
        data.weaponIndex = this.weaponIndex;

        data.lastLevelCleared = this.lastLevelCleared;
        data.lastLevelUnlockClaimed = this.lastLevelUnlockClaimed;
        data.subLevel = this.subLevel;
        data.scoreCounter = this.scoreCounter;

    }
    public void DeathDataReset()
    {
        saveManager.ResetLevelData();
    }
    public void ResetLevelData(GameData data)
    {
       
        data.levelShots = 0;
        data.shotsMissed = 0;
        data.levelKills = 0;
        data.levelTime = 0;
        data.weapons = new List<Weapon>();
        data.weaponTimers = new List<float>();
        data.weaponIndex = 0;
        data.levelProgress = 0;
        data.playerHp = 100;
        data.subLevel = 1;
        data.scoreCounter = 0;

        playerHealth = 100;
        levelProgress = 0;
        levelShots = 0;
        shotsMissed = 0;
        levelKills = 0;
        levelTime = 0;
        currentTime = 0;
        reloadingWeapon = false;
        weapons = new List<Weapon>();
        weaponTimers = new List<float>();
        weaponIndex = 0;
        subLevel = 1;
        scoreCounter = 0;

    }
   public void ResetAllData(GameData data)
    {
        data.currentLevel = 1;
        data.levelProgress = 0;
        data.playerHp = 100;
        data.lastLevelCleared = false;
        data.lastLevelUnlockClaimed = false;
        data.subLevel = 1;
        data.scoreCounter = 0;

        reloadingWeapon = false;
        lastLevelCleared = false;
        lastLevelUnlockClaimed = false;
        currentLevel = 1;
        levelProgress = 0;
        playerHealth = 100;
        subLevel = 1;
        scoreCounter = 0;
        ResetLevelData(data);
    }
    //find managers and init level based values when level changes
    public void FindManagers()
    {
        CancelInvoke();
        Time.timeScale = 1;
        weaponManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<WeaponManager>();
        uiManager = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<UIManager>();
        aim = uiManager.transform.GetChild(3).GetComponent<AimIndicator>();
        unlockManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UnlockManager>();
        spawner = GameObject.FindGameObjectWithTag("Managers").GetComponent<Spawner>();

        //init values back to zero
        currentWeapon = null;
        sameWeapon = false;
        levelOver = false;
       
    }
    //use power up
    public void PowerUp(float time)
    {
        CancelInvoke(); //cancels previous timer if there is onew
        OnPowerEnable(); //call OnPowerEnable(), playercontroller reacts to this
        currentTimer = time; //number value for the timer
        timerValue = 0.0f;
        valueToAdd = 6 / time; //Number of segments divided by time

        uiManager.powerUpBackground.SetActive(true);
        InvokeRepeating("UpdateTimer", 0, 1); //Invoke function until timer is gone, update values within function

    }
    //timer for powerups
    void UpdateTimer()
    {
        uiManager.powerUpTimer.text = currentTimer.ToString(); //"clock" timer
        uiManager.powerUpBackground.GetComponent<Image>().material.SetFloat("_RemovedSegments", timerValue); //background of timer, change shader graphs value
        if (currentTimer == 0) //power up timer ran out, disable powerup
        {
            CancelInvoke();
            uiManager.powerUpBackground.SetActive(false);
            OnPowerDisable();

        }
        currentTimer--;
        timerValue += valueToAdd;
    }

    #region Getters and setters for game manager values
    //values to get at start of the game

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public float GetPlayerHealth()
    {
        return playerHealth;
    }
    public void SetPlayerHealth(float health)
    {
        playerHealth += health;
        uiManager.ChangeHealthValue(health);
    }

    public float GetLevelProgress()
    {
        return levelProgress;
    }
    //check if level is complete
    public void SetLevelProgress(float amount)
    {
        uiManager.IncreaseProgress(amount);
        levelProgress += amount;
        if (lastLevelCleared)
        {
            SetScore(Mathf.RoundToInt(amount));
            uiManager.UpdateScoreText();
        }
        if (levelProgress >= xpRequired)
        {
            if (currentLevel == 3)
            {
                if (!lastLevelCleared)
                {
                    lastLevelCleared = true;
                    unlockManager.UnlockLevelUpgrade();  //unlock new weapon and show end panel
                    uiManager.ShowLevelEndPanel(); //show end panel for level 

                    //start new level with fresh bullets, reset bullets if player continues straight to endless mode
                    bulletsP90 = 0;
                    bulletsPistol = 0;
                    bulletsRifle = 0;

                }
                //last level cleared, stop increasing level and change to "endless" mode
                
                subLevel++;
                xpRequired = 70 + (xpReqMultiplier * subLevel);
                uiManager.IncreaseProgress(-xpRequired);
                uiManager.SetProgressMaxValue();
            }
            else
            {
                //level cleared
                currentLevel++; //increase level 
                unlockManager.UnlockLevelUpgrade();  //unlock new weapon and show end panel
                
                uiManager.ShowLevelEndPanel(); //show end panel for level 
                xpRequired = 70 + (xpReqMultiplier*currentLevel);

                //start new level with fresh bullets, dont do this in endless mode!
                bulletsP90 = 0;
                bulletsPistol = 0;
                bulletsRifle = 0;

            }
            //change cursor back to normal if reloading cursor is up
            aim.AmmoAvailableTexture();
       
            audioS.PlayOneShot(levelCompleteClip);
            levelOver = true;
            levelProgress = 0;
            reloadingWeapon = false;
           
        }

    }

    //for level end statistics
    public string GetLevelShots()
    {
        return levelShots.ToString();
    }
    public void SetLevelShots()
    {
        levelShots++;
    }
    public void ShotsMissed()
    {
        shotsMissed++;
    }

    public float GetScore()
    {
        return scoreCounter;
    }
   void SetScore(int amount)
    {
        scoreCounter += amount;
    }

    //accuracy for player shots
    public string GetLevelAccuracy()
    {
        if (shotsMissed != 0)
        {
            float missedShots = (shotsMissed / levelShots) * 100;
            float accuracy = Mathf.Round(100 - missedShots);
            return accuracy.ToString() + "%";
        }
        else
        {
            return "100%";
        }

    }

    public string GetLevelKills()
    {
        return levelKills.ToString();
    }
    public void SetLevelKills()
    {
        levelKills++;
    }

    //time played current level in minutes and seconds
    public string GetLevelTime()
    {
        int levelT = Mathf.RoundToInt(levelTime+currentTime);
        string minutes = Mathf.Floor(levelT / 60).ToString("00");
        string seconds = Mathf.RoundToInt(levelT % 60).ToString("00");
        return minutes + ":" + seconds;
    }

    //check which weapon was used the longest time and get its name and sprite 
    public string GetFavouriteWeaponName()
    {
        float maxValue = weaponTimers.Max();
        int index = weaponTimers.IndexOf(maxValue);
        return weapons[index].weaponName;
    }
    public Sprite GetFavouriteWeaponSprite()
    {
        float maxValue = weaponTimers.Max();
        int index = weaponTimers.IndexOf(maxValue);
        return weapons[index].weaponSprite;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;
        if (SceneManager.GetActiveScene().name == "Level" + currentLevel)
        {
            currentTime = Time.timeSinceLevelLoad;
        }
       
        //if there is a weapon equipped, then increase its timer on timers list
        if (currentWeapon != null && Time.timeScale!=0)
        {
            weaponTimers[weaponIndex] += Time.deltaTime;
        }
    }
    //function for finding out which weapon has been used the most (timewise)
    public void SetCurrentWeapon(Weapon weapon)
    {
        //check list is not empty
        if (weapons.Count != 0)
        {
            //go through list to see if there is same weapon already on it
            foreach (Weapon x in weapons)
            {
                if (x == weapon)
                {
                    sameWeapon = true;
                }
            }
            //if not, add new weapon to list and give it initial timer of 0
            if (!sameWeapon)
            {
                weapons.Add(weapon);
                weaponTimers.Add(0);
                currentWeapon = weapon;
            }
            sameWeapon = false;
        }
        else //if there is nothing on the list yet
        {
            weapons.Add(weapon);
            weaponTimers.Add(0);
            currentWeapon = weapon;

        }
        //update index for timer list so correct weapon timer gets updated
        foreach (Weapon x in weapons)
        {
            if (x == weapon)
            {
                weaponIndex = weapons.IndexOf(x);

            }
        }

    }

    #endregion
}
