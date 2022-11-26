using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    WeaponManager weaponManager;
    [SerializeField]
    UnlockManager unlockManager;

    [SerializeField]
    private Slider progressBar;

  
 
    [SerializeField]
    private Image health;
    float healthValue;

    [SerializeField]
    public GameObject powerUpBackground;
    public TextMeshProUGUI powerUpTimer;



    [SerializeField]
    GameObject levelEndPanel, unlock, levelComplete;

    //Stuff in the unlock panel
    TextMeshProUGUI unlockTitle, unlockDescriptionTitle, unlockDescriptionText;
    Image unlockImage;
    Button unlockContinueButton;

    //Stuff in the levelcomplete panel
    TextMeshProUGUI levelFavouriteWeapon, levelTime, levelAccuracy, levelKills, levelShots;
    Image levelFavouriteWeaponImage;
    Button levelContinueButton, levelMenuButton;



    private void Start()
    {
        powerUpTimer = powerUpBackground.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        healthValue = 0;
        health.material.SetFloat("_RemovedSegments", healthValue);
        SetupUIReferencesAndButtonClicks();
      
       
            
    
    }
    //function to increase progress in the level
    public void IncreaseProgress(int amount)
    {
        progressBar.value += amount;
        if (progressBar.value >= 100)
        {
            //level cleared
            progressBar.value = 0;
            GameManager.instance.currentLevel++;
            weaponManager.SetWeaponUnlock();
            ShowLevelEndPanel();

        }
    }
    public float ProgressValue()
    {
        return progressBar.value;
    }
    public void ChangeHealthValue(int amount)
    {
        //when health is reduced healthvalue needs to be positive number because value is actually the amount of segments removed
        healthValue -= amount;
        health.material.SetFloat("_RemovedSegments", healthValue);
    }
    //set up values for level end screen
    void SetupLevelEndValues()
    {
        unlockTitle.text = unlockManager.GetUnlockTitle();
        unlockDescriptionTitle.text = unlockManager.GetUnlockDescriptionTitle();
        unlockDescriptionText.text = unlockManager.GetUnlockDescriptionText();
        unlockImage.sprite = unlockManager.GetUnlockSprite();


        levelFavouriteWeapon.text = GameManager.instance.GetFavouriteWeaponName();
        levelFavouriteWeaponImage.sprite = GameManager.instance.GetFavouriteWeaponSprite();
        levelTime.text = GameManager.instance.GetLevelTime();
        levelAccuracy.text = GameManager.instance.GetLevelAccuracy();
        levelKills.text = GameManager.instance.GetLevelKills();
        levelShots.text = GameManager.instance.GetLevelShots();

    }
    //set up variable references through script
    void SetupUIReferencesAndButtonClicks()
    {
       
        unlockTitle = unlock.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        unlockDescriptionTitle = unlock.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        unlockDescriptionText = unlock.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        unlockImage = unlock.transform.GetChild(3).GetComponent<Image>();
        unlockContinueButton = unlock.transform.GetChild(4).GetComponent<Button>();

        unlockContinueButton.onClick.AddListener(ShowLevelDetails);

        levelFavouriteWeapon = levelComplete.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        levelFavouriteWeaponImage = levelComplete.transform.GetChild(1).GetChild(2).GetComponent<Image>();
        levelTime = levelComplete.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        levelAccuracy = levelComplete.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        levelKills = levelComplete.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        levelShots = levelComplete.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();

        levelContinueButton = levelComplete.transform.GetChild(3).GetComponent<Button>();
        levelMenuButton = levelComplete.transform.GetChild(4).GetComponent<Button>();

        levelContinueButton.onClick.AddListener(ContinueToNextLevel);
        levelMenuButton.onClick.AddListener(ContinueToMainMenu);
    }

    void ContinueToNextLevel()
    {
        SceneManager.LoadScene("Level" + GameManager.instance.currentLevel);
    }
    void ContinueToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void ShowLevelDetails()
    {
        unlock.SetActive(false);
        levelComplete.SetActive(true);
    }

    void ShowLevelEndPanel()
    {
        Time.timeScale = 0; //stop time when level is over
        AimIndicator.instance.GetComponent<Image>().enabled = false;
        SetupLevelEndValues();
        levelEndPanel.SetActive(true);
        unlock.SetActive(true);
        //TO DO hide extra UI stuff
    }
}
