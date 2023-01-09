using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
    float removedSegments;

    [SerializeField]
    public GameObject powerUpBackground;
    public TextMeshProUGUI powerUpTimer;



    [SerializeField]
    GameObject levelEndPanel, unlock, levelComplete, scoreText;

    //Stuff in the unlock panel
    TextMeshProUGUI unlockTitle, unlockDescriptionTitle, unlockDescriptionText;
    Image unlockImage;
    Button unlockContinueButton;

    //Stuff in the levelcomplete panel
    TextMeshProUGUI levelFavouriteWeapon, levelTime, levelAccuracy, levelKills, levelShots, levelsCleared, scoreCounter;
    Image levelFavouriteWeaponImage;
    Button levelContinueButton, levelMenuButton;


    [SerializeField]
    GameObject weaponPanel;

    //pausemenu
    [SerializeField]
    GameObject pausePanel;

    [SerializeField]
    GameObject deathPanel;
    Button resetButton, menuButton;

    private void Start()
    {
        //init some ui values
        powerUpTimer = powerUpBackground.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        removedSegments = 100;
        health.material.SetFloat("_RemovedSegments", removedSegments);
        SetupUIReferencesAndButtonClicks();
        
        //init progress value and player health from game manager
        IncreaseProgress(GameManager.instance.GetLevelProgress());
        ChangeHealthValue(GameManager.instance.GetPlayerHealth());

        //level max progress depends on current level
        SetProgressMaxValue();
        ToggleWeaponPanel();
        //enable score visual for endless level
        if (GameManager.instance.lastLevelCleared)
        {
            scoreText.SetActive(true);
            UpdateScoreText();
        }
        
    }
    //increase progress bar value
    public void IncreaseProgress(float amount)
    {
        progressBar.value += amount;
    }
    public void SetProgressMaxValue()
    {
        progressBar.maxValue = GameManager.instance.xpRequired;
    }
  
    public void ChangeHealthValue(float amount)
    {
        //when health is reduced healthvalue needs to be positive number because value is actually the amount of segments removed
        removedSegments -= amount;
        health.material.SetFloat("_RemovedSegments", removedSegments);
    }
    //set up values for level end panel with different infos from gamemanager
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

       levelsCleared.text = GameManager.instance.subLevel.ToString();
       scoreCounter.text = GameManager.instance.GetScore().ToString();

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

        levelsCleared = levelComplete.transform.GetChild(2).GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>();
        scoreCounter = levelComplete.transform.GetChild(2).GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>();


        levelContinueButton = levelComplete.transform.GetChild(3).GetComponent<Button>();
        levelMenuButton = levelComplete.transform.GetChild(4).GetComponent<Button>();

        //if last level,  next level button on click should reload current level again
        if (GameManager.instance.lastLevelCleared)
        {
            levelContinueButton.onClick.AddListener(ResetLevel);
        }
        else
        {   //else load next level
            levelContinueButton.onClick.AddListener(ContinueToNextLevel);
        }
     
        levelMenuButton.onClick.AddListener(ContinueToMainMenu);

        menuButton = deathPanel.transform.GetChild(3).GetComponent<Button>();
        menuButton.onClick.AddListener(ContinueToMainMenu);

        resetButton = deathPanel.transform.GetChild(2).GetComponent<Button>();
        resetButton.onClick.AddListener(ResetLevel);


    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.Q) && !GameManager.instance.reloadingWeapon)
        {
            ToggleWeaponPanel();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && GameManager.instance.reloadingWeapon)
        {
            Debug.Log("cant change weapon while reloading current weapon");
        }
    }
    public void TogglePause()
    {
        if (!pausePanel.activeSelf)
        {
            if (weaponPanel.activeInHierarchy)
                weaponPanel.SetActive(false);
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ToggleWeaponPanel()
    {
        if (!weaponPanel.activeSelf)
        {
            if(pausePanel.activeInHierarchy)
                TogglePause();
            weaponPanel.SetActive(true);

            GameManager.instance.aim.GetComponent<Image>().enabled = false;
        }
        else
        {
            weaponPanel.SetActive(false);


        }
    }
    void ContinueToNextLevel()
    {
        SceneManager.LoadScene("Level" + GameManager.instance.GetCurrentLevel());
    }
    public void ContinueToMainMenu()
    {
           SceneManager.LoadScene("MainMenu");
    }
    public void UpdateScoreText()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + GameManager.instance.GetScore().ToString();
    }
    void ShowLevelDetails()
    {
        unlock.SetActive(false);
        levelComplete.SetActive(true);
    }

    public void ShowLevelEndPanel()
    {
        Time.timeScale = 0; //stop time when level is over
        GameManager.instance.aim.GetComponent<Image>().enabled = false;
       
        SetupLevelEndValues();
        levelEndPanel.SetActive(true);
        //hide extra UI stuff
        progressBar.gameObject.SetActive(false);
        powerUpBackground.SetActive(false);
        health.gameObject.SetActive(false);
        GameManager.instance.aim.gameObject.SetActive(false);
        weaponManager.GetCurrentWeapon().SetActive(false);
        //last level, skip unlock panel and show extra stuff in level complete
        if (GameManager.instance.lastLevelCleared && GameManager.instance.lastLevelUnlockClaimed)
        {
            levelComplete.SetActive(true);
            levelContinueButton.onClick.RemoveListener(ContinueToNextLevel);
            levelContinueButton.onClick.AddListener(ResetLevel);
            levelContinueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Endless Mode";
            levelComplete.transform.GetChild(2).GetChild(4).gameObject.SetActive(true);
            levelComplete.transform.GetChild(2).GetChild(5).gameObject.SetActive(true);
            scoreText.SetActive(false);

        }
        else //not last level yet, show new unlocked stuff
        {
            unlock.SetActive(true);
            if (GameManager.instance.lastLevelCleared)
            {
                levelContinueButton.onClick.RemoveListener(ContinueToNextLevel);
                levelContinueButton.onClick.AddListener(ResetLevel);
                levelContinueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Endless Mode";
                GameManager.instance.lastLevelUnlockClaimed = true;
            }
        }

    }

    public void ShowDeathPanel()
    {
        if (GameManager.instance.lastLevelCleared)
        {
            //instead of death panel show level end stats
            ShowLevelEndPanel();
           
        }
        else
        {
            deathPanel.SetActive(true);
            Time.timeScale = 0;
        }
      
    }
    void ResetLevel()
    {
        Time.timeScale = 1;
        GameManager.instance.DeathDataReset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
