using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject menuButtons;

    private Button singePlayerButton;
    private Button multiPlayerButton;
    private Button optionsButton;
    private Button quitGameButton;

    void Start()
    {
        SetUpButtons();
    }
    void StartSinglePlayer()
    {
        SceneManager.LoadScene("Level1");
    }

    void StartMultiPlayer()
    {
        SceneManager.LoadScene("Lobby");
    }

    void OpenOptionsPanel()
    {
        Debug.Log("options");
    }

    void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    void SetUpButtons()
    {
        singePlayerButton = menuButtons.transform.GetChild(0).GetComponent<Button>();
        multiPlayerButton = menuButtons.transform.GetChild(1).GetComponent<Button>();
        optionsButton = menuButtons.transform.GetChild(2).GetComponent<Button>();
        quitGameButton = menuButtons.transform.GetChild(3).GetComponent<Button>();

        singePlayerButton.onClick.AddListener(StartSinglePlayer);
        multiPlayerButton.onClick.AddListener(StartMultiPlayer);
        optionsButton.onClick.AddListener(OpenOptionsPanel);
        quitGameButton.onClick.AddListener(QuitGame);
    }
}
