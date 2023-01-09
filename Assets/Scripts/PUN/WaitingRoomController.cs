using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class WaitingRoomController : MonoBehaviourPunCallbacks
{
    private PhotonView myPhotonView;

    [SerializeField]
    int multiplayerSceneIndex;
    [SerializeField]
    int menuSceneIndex;

    int playerCount;
    int roomSize;

    [SerializeField]
    int minPlayersToStart;

    [SerializeField]
    TextMeshProUGUI playerCountDisplay;
    [SerializeField]
    TextMeshProUGUI timerToStartDisplay;

    //bool values for timer
    bool readyToCountDown;
    bool readyToStart;
    bool startinGame;

    float timerToStartGame;
    float notFullGameTimer;
    float fullGameTimer;

    [SerializeField]
    float maxWaitTime;
    [SerializeField]
    float maxFullGameWaitTime;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

      
        PlayerCountUpdate();
      
    }

    private void PlayerCountUpdate()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCountDisplay.text ="Players: "+ playerCount + "/" + roomSize;

        if (playerCount == roomSize)
        {
            readyToStart = true;
        }else if (playerCount >= minPlayersToStart)
        {
            readyToCountDown = true;
        }
        else
        {
            readyToCountDown = false;
            readyToStart=false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //called when new player joins
        PlayerCountUpdate();
        //send master clients countdown timer to all other players in order to sync time
        if (PhotonNetwork.IsMasterClient)
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
    }
    [PunRPC]
    void RPC_SendTimer(float timeIn)
    {
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if(timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }
    private void Update()
    {
        WaitingForMorePlayers();
    }
    void WaitingForMorePlayers()
    {
        //if there is only 1 player in the room, timer will stop and reset
        if (playerCount <= 1)
        {
            ResetTimer(); 
        }
        //when there is enough players in the room, start timer will begin counting down
        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }else if (readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame=notFullGameTimer;
        }
        //format and display countdown timer
        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerToStartDisplay.text = tempTimer;

        //if countdown timer reaches 0 the game will then start
        if (timerToStartGame <= 0f)
        {
            if (startinGame)
                return;
            StartGame();
        }
    }

    void ResetTimer()
    {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }
    void StartGame()
    {
        startinGame = true;
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    public void CancelGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }
}
