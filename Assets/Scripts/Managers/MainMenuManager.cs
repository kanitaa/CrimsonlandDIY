using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuButtons, optionsPanel;

    private Button singePlayerButton;
    private Button multiPlayerButton;
    private Button optionsButton;
    private Button quitGameButton;

    private Button resetDataButton;
    private Button backToMenuButton;

    [SerializeField]
    AudioSource audioS;

    [SerializeField]
    AudioClip buttonClick, resetSound;

    [SerializeField]
    GameObject networkController, lobbyController;
    void Start()
    {
        SetUpButtons();
        Time.timeScale = 1;
        //theres a saved game
        if (GameManager.instance.GetLevelProgress()!=0 || GameManager.instance.GetCurrentLevel()!=1)
        {
            singePlayerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Continue";
        }
        //Enable multiplayer
        if(GameManager.instance.lastLevelCleared)
        {
            multiPlayerButton.gameObject.SetActive(true);
            singePlayerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Endless";
            networkController.SetActive(true);
            lobbyController.SetActive(true);
        }
    }
    void StartSinglePlayer()
    {
        audioS.PlayOneShot(buttonClick);
        SceneManager.LoadScene("Level"+GameManager.instance.GetCurrentLevel());
    }
    void StartMultiPlayer()
    {
        audioS.PlayOneShot(buttonClick);
       
    }

    void OpenOptionsPanel()
    {
        audioS.PlayOneShot(buttonClick);
        optionsPanel.SetActive(true);
    }

    void QuitGame()
    {
        audioS.PlayOneShot(buttonClick);
        Debug.Log("quit");
        Application.Quit();
    }
    void ResetData()
    {
        audioS.PlayOneShot(resetSound);
        singePlayerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "New Game";
        singePlayerButton.gameObject.SetActive(true);
        multiPlayerButton.gameObject.SetActive(false);
        networkController.SetActive(false);
        lobbyController.SetActive(false);

        GameManager.instance.saveManager.ResetAllData(); 
    }
    void BackToMenu()
    {
        audioS.PlayOneShot(buttonClick);
        optionsPanel.SetActive(false);
    }
    void SetUpButtons()
    {
        singePlayerButton = menuButtons.transform.GetChild(0).GetComponent<Button>();
        multiPlayerButton = menuButtons.transform.GetChild(1).GetComponent<Button>();
        optionsButton = menuButtons.transform.GetChild(2).GetComponent<Button>();
        quitGameButton = menuButtons.transform.GetChild(3).GetComponent<Button>();

        resetDataButton = optionsPanel.transform.GetChild(0).GetComponent<Button>();
        backToMenuButton = optionsPanel.transform.GetChild(1).GetComponent<Button>();

        singePlayerButton.onClick.AddListener(StartSinglePlayer);
        multiPlayerButton.onClick.AddListener(StartMultiPlayer);
   
        optionsButton.onClick.AddListener(OpenOptionsPanel);
        quitGameButton.onClick.AddListener(QuitGame);

        resetDataButton.onClick.AddListener(ResetData);
        backToMenuButton.onClick.AddListener(BackToMenu);
    }
}
