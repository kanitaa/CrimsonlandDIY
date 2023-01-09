using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField]
    private string fileName;
    [SerializeField]
    private bool useEncryption;
    public static SaveManager instance {get; private set; }
    private GameData gameData;
    private List<ISaveManager> dataObjects;

    private FileDataHandler dataHandler;


    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one SaveManager");
        }
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataObjects = FindAllDataObjects();
       
        LoadGame();
        SceneManager.activeSceneChanged += ChangedActiveScene;
        //autosave every 30sec
        InvokeRepeating("SaveGame", 30, 30);
    }
    //Save game when scene changes
    private void ChangedActiveScene(Scene current, Scene next)
    {
        string currentName = current.name;
   
        if (currentName == null)
        {
            // Scene1 has been removed
            currentName = "Replaced";
            
        }
        if (GameManager.instance.levelOver)
        {
            ResetLevelData();
        }
        if(GameManager.instance!=null)
            GameManager.instance.CancelInvoke();
        SaveGame();
        Debug.Log("Scenes: " + currentName + ", " + next.name);
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        //Load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();
        //if no data can be loaded, initialize to a new game
        if(this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        //push the loaded data to all other scripts that need it
        foreach(ISaveManager data in dataObjects)
        {
            data.LoadData(gameData);
        }
    }
  
    public void SaveGame()
    {
        //pass the data to other scripts so they can update it
        foreach(ISaveManager data in dataObjects)
        {
            data.SaveData(gameData);
        }
        //save that data to a file using the data handlers
        dataHandler.Save(gameData);
    }

    public void ResetLevelData()
    {
        //pass the data to other scripts so they can update it
        foreach (ISaveManager data in dataObjects)
        {
            data.ResetLevelData(gameData);
          
        }
        //save that data to a file using the data handlers
        dataHandler.Save(gameData);
        Debug.Log("Level data reset");
    }

   public void ResetAllData()

    {   //pass the data to other scripts so they can update it
        foreach (ISaveManager data in dataObjects)
        {
            data.ResetAllData(gameData);

        }
        //save that data to a file using the data handlers
        dataHandler.Save(gameData);
       
        Debug.Log("All data reset");


    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
 
    //find all scripts that implement the save manager interface
    private List<ISaveManager> FindAllDataObjects()
    {
        IEnumerable<ISaveManager> dataObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(dataObjects);
    }

}

