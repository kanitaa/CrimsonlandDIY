using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
   
    
    float currentTimer;
    float timerValue;
    float valueToAdd;

    public bool reloadingWeapon;

    [SerializeField]
    public int currentLevel=1;
    [SerializeField]
    WeaponManager weaponManager;
    [SerializeField]
    UIManager uiManager;

   
    public AimIndicator aim;


    public delegate void EnablePowerup();
    public static event EnablePowerup OnPowerEnable;

    public delegate void DisablePowerup();
    public static event DisablePowerup OnPowerDisable;


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
   
    private void Awake()
    {
       
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);
       
     
           
       
    }

    //find managers and init values when level changes
    public void FindManagers()
    {
        CancelInvoke();
        Time.timeScale = 1;
        weaponManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<WeaponManager>();
        uiManager = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<UIManager>();
        aim = uiManager.transform.GetChild(3).GetComponent<AimIndicator>();


        //init values back to zero
        weapons = new List<Weapon>();
        weaponTimers = new List<float>();
        currentWeapon=null;
        sameWeapon=false;
        weaponIndex=0;
        levelShots =0;
        shotsMissed=0;
        levelKills = 0;

    }
    public void IncreaseProgress(int amount)
    {
        uiManager.IncreaseProgress(amount);
      
    }

    public float GetCurrentProgress()
    {
        return uiManager.ProgressValue();
    }

    //use power up
    public void PowerUp(float time)
    {
        CancelInvoke(); //cancels previous timer if there is one
        OnPowerEnable(); //call OnPowerEnable(), playercontroller reacts to this
        currentTimer = time; //number value for the timer
        timerValue = 0.0f;
        valueToAdd = 6 / time; //Number of segments divided by time
       
        uiManager.powerUpBackground.SetActive(true);
        InvokeRepeating("UpdateTimer",0,1); //Invoke function until timer is gone, update values within function
        
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

    public void IncreaseLevelShots()
    {
        levelShots++;
    }
    public string GetLevelShots()
    {
        return levelShots.ToString();
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
            float accuracy = Mathf.Round(100-missedShots);
            return accuracy.ToString() + "%";
        }
        else
        {
            return "100%";
        }
        
    }
    public void IncreaseLevelKills()
    {
        levelKills++;
    }

    public string GetLevelKills()
    {
        return levelKills.ToString();
    }

    //time played current level in minutes and seconds
    public string GetLevelTime()
    {
        int levelTime = Mathf.RoundToInt(Time.timeSinceLevelLoad);
        string minutes = Mathf.Floor(levelTime / 60).ToString("00");
        string seconds = Mathf.RoundToInt(levelTime % 60).ToString("00");
        return minutes +":"+ seconds;
    }


    private void Update()
    {
        //if there is a weapon equipped, then increase its timer on timers list
       if(currentWeapon != null)
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
    //check which weapon was used the longest time and get its name and sprite for ui purposes
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

}
