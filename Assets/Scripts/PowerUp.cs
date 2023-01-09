using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private string power;

    [SerializeField]
    private int timer;

    [SerializeField]
    GameObject powerName;

    bool powerActive;
    public string GetPowerUpType()
    {
        return power;
    }
    private void Start()
    {
        powerActive = true;
        Invoke("DestroyWithoutPowerUp", 20);
    }
    private void OnDestroy()
    {

        if (MultiGameManager.instance != null)
        {
            if (powerActive && MultiGameManager.instance.myPhotonView.IsMine)
            {
                MultiGameManager.instance.PowerUp(timer);
                PhotonView myPV = GetComponent<PhotonView>();
                myPV.RPC("DestroyPower", RpcTarget.All);
            }
           
        }
        else 
        {
            if (powerActive)
                GameManager.instance.PowerUp(timer);
        }
       
        
    }
    void DestroyWithoutPowerUp()
    {
        powerActive = false;
        Destroy(gameObject);
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //check if mouse is on powerup, if so show power ups name
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform != this.gameObject.transform && !powerName.activeInHierarchy)
            {
                return;
            }
                if (hit.transform==this.gameObject.transform && !powerName.activeInHierarchy)
            {
                powerName.SetActive(true);
            }
            else if(hit.transform != this.gameObject.transform && powerName.activeInHierarchy)
            {
                powerName.SetActive(false);
            }
        }
    }
    [PunRPC]
    void DestroyPower()
    {
        powerActive = false;
        Destroy(gameObject);
    }
}

