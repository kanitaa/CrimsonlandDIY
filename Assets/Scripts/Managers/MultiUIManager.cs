using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiUIManager : MonoBehaviour
{
    [SerializeField]
    MultiWeaponManager weaponManager;
   

    [SerializeField]
    private Slider progressBar;

    public MultiPlayerController pc;

    [SerializeField]
    private Image health;
    float removedSegments;

    [SerializeField]
    public GameObject powerUpBackground;
    public TextMeshProUGUI powerUpTimer;



    [SerializeField]
    GameObject levelEndPanel, levelComplete;



    //Stuff in the levelcomplete panel
    TextMeshProUGUI levelFavouriteWeapon, levelTime, levelAccuracy, levelKills, levelShots;
    Image levelFavouriteWeaponImage;
    Button levelMenuButton;


    [SerializeField]
    GameObject weaponPanel;


    //pausemenu
    [SerializeField]
    GameObject pausePanel;
    [SerializeField]
    Button continueButton, returnMenuButton;
    [SerializeField]
    GameObject deathPanel;


    private void Start()
    {
        ToggleWeaponPanel();
    }
    //increase progress bar value
    public void IncreaseProgress(float amount)
    {
        progressBar.value += amount;
    }


    public void ChangeHealthValue(float amount)
    {
        //when health is reduced healthvalue needs to be positive number because value is actually the amount of segments removed
        removedSegments -= amount;
        health.material.SetFloat("_RemovedSegments", removedSegments);
        if (removedSegments > 100) removedSegments = 100;
    }
    //set up values for level end screen
    void SetupLevelEndValues()
    {
        levelFavouriteWeapon.text = MultiGameManager.instance.GetFavouriteWeaponName();
        levelFavouriteWeaponImage.sprite = MultiGameManager.instance.GetFavouriteWeaponSprite();
        levelTime.text = MultiGameManager.instance.GetLevelTime();
        levelAccuracy.text = MultiGameManager.instance.GetLevelAccuracy();
        levelKills.text = MultiGameManager.instance.GetLevelKills();
        levelShots.text = MultiGameManager.instance.GetLevelShots();

    }
    //set up variable references through script
    public void SetupUIReferencesAndButtonClicks()
    {
        if (pc.photonView.IsMine)
        {

            health = pc.gameObject.transform.GetChild(3).GetChild(0).GetComponent<Image>();
            health.enabled = true;

            removedSegments = 100;

            health.material.SetFloat("_RemovedSegments", removedSegments);


            levelFavouriteWeapon = levelComplete.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            levelFavouriteWeaponImage = levelComplete.transform.GetChild(1).GetChild(2).GetComponent<Image>();
            levelTime = levelComplete.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            levelAccuracy = levelComplete.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            levelKills = levelComplete.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
            levelShots = levelComplete.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();


            levelMenuButton = levelComplete.transform.GetChild(3).GetComponent<Button>();


            levelMenuButton.onClick.AddListener(ContinueToMainMenu);


            returnMenuButton.onClick.AddListener(ContinueToMainMenu);
            continueButton.onClick.AddListener(TogglePause);

          
        }
    }
    private void Update()
    {
        if (MultiGameManager.instance.myPhotonView == null) return;
        if (!MultiGameManager.instance.myPhotonView.IsMine)
            return;
        if (Input.GetKeyDown(KeyCode.Q) && !MultiGameManager.instance.reloadingWeapon)
        {
            ToggleWeaponPanel();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && MultiGameManager.instance.reloadingWeapon)
        {
            Debug.Log("cant change weapon while reloading current weapon");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    void TogglePause()
    {
        if (!pausePanel.activeSelf)
        {
            if (weaponPanel.activeInHierarchy)
                weaponPanel.SetActive(false);
            pausePanel.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);
        }
    }
    public void ToggleWeaponPanel()
    {
        if (!weaponPanel.activeSelf)
        {
            if (pausePanel.activeInHierarchy)
                pausePanel.SetActive(false);
            weaponPanel.SetActive(true);
            MultiGameManager.instance.aim.GetComponent<Image>().enabled = false;
        }
        else
        {
            weaponPanel.SetActive(false);

        }
    }
    void ContinueToMainMenu()
    {
        MultiGameManager.instance.CancelInvoke();
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowLevelEndPanel()
    {
        MultiGameManager.instance.CancelInvoke();
        MultiGameManager.instance.aim.GetComponent<Image>().enabled = false;
        SetupLevelEndValues();
        levelEndPanel.SetActive(true);
       
        //hide extra UI stuff
        progressBar.gameObject.SetActive(false);
        powerUpBackground.SetActive(false);
        health.gameObject.SetActive(false);
        MultiGameManager.instance.aim.gameObject.SetActive(false);
        weaponManager.GetCurrentWeapon().SetActive(false);
        deathPanel.SetActive(false);

    }
    public void ShowDeathPanel()
    {
       
      deathPanel.SetActive(true);

    }

    public void HideDeathPanel()
    {
       
       deathPanel.SetActive(false);
        
    }
}
