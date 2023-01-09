using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiGameManager : MonoBehaviour
{
    public static MultiGameManager instance;

    public PhotonView myPhotonView;
    float currentTimer;
    float timerValue;
    float valueToAdd;

    public bool reloadingWeapon;

    int currentLevel = 1;

    float levelProgress = 0;

    float playerHealth;

    [SerializeField]
    public MultiWeaponManager weaponManager;
    [SerializeField]
    MultiUIManager uiManager;
    [SerializeField]
    UnlockManager unlockManager;
    public MultiSpawn spawner;
    public AimIndicator aim;


    public delegate void EnablePowerup();
    public static event EnablePowerup OnPowerEnable;

    public delegate void DisablePowerup();
    public static event DisablePowerup OnPowerDisable;

    public bool levelOver;

   
    //variables to check favourite weapon
    List<Weapon> weapons = new List<Weapon>();
    List<float> weaponTimers = new List<float>();
    Weapon currentWeapon;
    bool sameWeapon;
    int weaponIndex;

    //variables for level complete 
    float levelShots;
    float shotsMissed;
    int levelKills;
    [SerializeField]
    float currentTime;
    [SerializeField]
    float levelTime;

    public LayerMask enemyLayer;
    public int playerIndex;

    public bool isPowerActive;
    public int playersAlive;


    public int bulletsP90, bulletsRifle, bulletsPistol;
    private void Awake()
    {

        if (MultiGameManager.instance == null)
        {
            MultiGameManager.instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerHealth = 100;

        playersAlive = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void ChangePlayerCount(int amount)
    {
        playersAlive += amount;
        if (playersAlive == 0)
        {
            //all players ded
            uiManager.ShowLevelEndPanel();
        }
       
    }
    public void SetPC(MultiPlayerController pc)
    {
        FindManagers();
        uiManager.pc = pc;
        weaponManager.playerController = pc;
        uiManager.SetupUIReferencesAndButtonClicks();
        aim.cc = pc.gameObject.GetComponent<CharacterController>();

    }
    //find managers and init level based values when level changes
    public void FindManagers()
    {
        CancelInvoke();
        Time.timeScale = 1;
        weaponManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<MultiWeaponManager>();
        uiManager = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<MultiUIManager>();
        aim = uiManager.transform.GetChild(3).GetComponent<AimIndicator>();
        unlockManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UnlockManager>();
        spawner = GameObject.FindGameObjectWithTag("Managers").GetComponent<MultiSpawn>();
        //init values back to zero
        currentWeapon = null;
        sameWeapon = false;
        levelOver = false;
       
    }
    //use power up
    public void PowerUp(float time)
    {
        if (!myPhotonView.IsMine) return;
        CancelInvoke(); //cancels previous timer if there is one
        OnPowerEnable(); //call OnPowerEnable(), playercontroller reacts to this
        isPowerActive = true;
        currentTimer = time; //number value for the timer
        timerValue = 0.0f;
        valueToAdd = 6 / time; //Number of segments divided by time

        uiManager.powerUpBackground.SetActive(true);
        InvokeRepeating("UpdateTimer", 0, 1); //Invoke function until timer is gone, update values within function

    }
    //timer for powerups
    void UpdateTimer()
    {
        if (!myPhotonView.IsMine) return;
        uiManager.powerUpTimer.text = currentTimer.ToString(); //"clock" timer
        uiManager.powerUpBackground.GetComponent<Image>().material.SetFloat("_RemovedSegments", timerValue); //background of timer, change shader graphs value
        if (currentTimer == 0) //power up timer ran out, disable powerup
        {
            CancelInvoke();
            uiManager.powerUpBackground.SetActive(false);
            OnPowerDisable();
            isPowerActive = false;

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
    public void SetPlayerHealthToFull()
    {
        playerHealth = 100;
        uiManager.ChangeHealthValue(100);
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
        if (levelProgress >= 100)
        {
            //level cleared
            currentLevel++; //increase level
            levelProgress = 0;
            uiManager.IncreaseProgress(-100);
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

    //accuracy for player shots
    public string GetLevelAccuracy()
    {
        if (shotsMissed != 0)
        {
            float missedShots = (shotsMissed / levelShots) * 100;
            float accuracy = Mathf.Round(100 - missedShots);
            return accuracy.ToString() + "%";
        }
        else if (levelShots == 0)
        {
            return "-";
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
        int levelT = Mathf.RoundToInt(levelTime + currentTime);
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
        if (SceneManager.GetActiveScene().name == "MultiLevel1")
        {
            currentTime = Time.timeSinceLevelLoad;
            //if there is a weapon equipped, then increase its timer on timers list
            if (currentWeapon != null)
            {
                weaponTimers[weaponIndex] += Time.deltaTime;
            }
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