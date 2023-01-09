using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public class MultiPlayerController : MonoBehaviourPunCallbacks
{
    PhotonView myPhotonView;
    [SerializeField]
    MultiUIManager uiManager;
    CharacterController controller;

    //movement speed variables
    private float moveSpeed;
    [SerializeField]
    float normalSpeed;
    [SerializeField]
    float powerSpeed;
    private Vector3 moveDirection = Vector3.zero;

    //weaponmanager needs to access these
    public GameObject weaponSlot;
    public Weapon currentWeapon;
    


    [SerializeField]
    float health;

    //power variables, powers name needs to match the one in enum
    public string power;
    enum Powers { Speed, Forcefield, Freeze, Mushroom };

    [SerializeField]
    private GameObject forcefield;

    public bool dead;
    bool resurrectable;
   

    AudioSource audioS;
    [SerializeField]
    AudioClip deathClip, ressClip;


    [SerializeField]
    GameObject playerToResurrect;

    [SerializeField]
    float ressTimer = 0;
    float ressDuration=5;
    GameObject ressEffect;


    bool godMode;
    public bool mushroomPower;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = normalSpeed;
        audioS = GetComponent<AudioSource>();
        uiManager = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<MultiUIManager>();
        myPhotonView = GetComponent<PhotonView>();
        if (myPhotonView.IsMine)
        {
            //add player power up functions to gamemanager
            MultiGameManager.OnPowerDisable += DisablePowerUps;
            MultiGameManager.OnPowerEnable += EnablePowerUp;
            MultiGameManager.instance.myPhotonView = myPhotonView;
            MultiGameManager.instance.SetPC(this);
            health = MultiGameManager.instance.GetPlayerHealth();
            

            MultiGameManager.instance.weaponManager.EquipWeapon(MultiGameManager.instance.weaponManager.firstWeaponRef);
            MultiGameManager.instance.playerIndex = photonView.ViewID;
            uiManager.ChangeHealthValue(100);
            transform.GetChild(3).GetComponent<Canvas>().worldCamera = Camera.main;
            MultiGameManager.instance.aim.cc = GetComponent<CharacterController>();
            Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
        }
       
    }
   
    public void InstantiateWeapon()
    {
        photonView.RPC("WeaponToSpawn", RpcTarget.All);

    }
    public void DestroyWeapon()
    {
        photonView.RPC("WeaponToDestroy", RpcTarget.All);
    }
    [PunRPC]
    void WeaponToDestroy()
    {
        if (weaponSlot.gameObject.transform.childCount!=0)
        {
            Destroy(weaponSlot.gameObject.transform.GetChild(0).gameObject);
        }
    }
    [PunRPC]
    void WeaponToSpawn()
    {
        if(myPhotonView!=null && myPhotonView.IsMine)
       PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", currentWeapon.weaponPrefab.name),weaponSlot.transform.position, weaponSlot.transform.rotation);  
     
    }
  
    void PlayerMovement()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= moveSpeed;
        controller.Move(moveDirection * Time.deltaTime);

    }
    void PlayerRotation()
    {
        //rotate to raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 hit = ray.GetPoint(rayLength);
            transform.LookAt(new Vector3(hit.x, transform.position.y, hit.z));
        }
    }


    void Update()
    {
        if (photonView.IsMine && !dead)
        {
            PlayerMovement();
            PlayerRotation();

            if (resurrectable && Input.GetKeyDown(KeyCode.F))
            {
                ressEffect = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RessParticleEffect"), transform.position, Quaternion.identity);
            
            }
            if (Input.GetKey(KeyCode.F) && resurrectable)
            {
                ressTimer += Time.deltaTime;
                if (ressTimer >= ressDuration) //ress finished cast
                {
                    playerToResurrect.GetComponent<PhotonView>().RPC("Resurrect", RpcTarget.All);
                    ressTimer = 0;
                    resurrectable = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                ressTimer = 0;
                Destroy(ressEffect);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!godMode)
                {
                    godMode = true;
                }
                else
                {
                    godMode = false;
                }
               
            }
        }
       
    }


    

    //check which power up is hit and do the power effect
    void EnablePowerUp()
    {
        if (!myPhotonView.IsMine) return;
            if (power == Powers.Freeze.ToString())
            {
                myPhotonView.RPC("EnableFreeze", RpcTarget.All);

            }
            else if (power == Powers.Forcefield.ToString())
            {
                myPhotonView.RPC("EnableForcefield", RpcTarget.All);

            }
            else if (power == Powers.Speed.ToString())
            {

                moveSpeed = powerSpeed;

            }
             else if(power== Powers.Mushroom.ToString())
            {
                mushroomPower = true;
                weaponSlot.GetComponentInChildren<MultiShoot>().mushroomPower = mushroomPower;
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            }
    }

    [PunRPC]
    void EnableForcefield()
    {
        forcefield.SetActive(true);
    }
    [PunRPC]
    void DisableForcefield()
    {
        if (forcefield.activeInHierarchy)
            forcefield.SetActive(false);
    }

    [PunRPC]
    void EnableFreeze()
    {
        for (int i = 0; i < MultiGameManager.instance.spawner.enemiesSpawned.Count; i++)
        {
            MultiGameManager.instance.spawner.enemiesSpawned[i].gameObject.GetComponent<MultiEnemy>().Freeze();
        }
    }
    [PunRPC]
    void DisableFreeze()
    {
        for (int i = 0; i < MultiGameManager.instance.spawner.enemiesSpawned.Count; i++)
        {
            MultiGameManager.instance.spawner.enemiesSpawned[i].gameObject.GetComponent<MultiEnemy>().UnFreeze();
        }
    }
    //disable all powerups
    void DisablePowerUps()
    {
        if (myPhotonView.IsMine)
        {
            myPhotonView.RPC("DisableForcefield", RpcTarget.All);
            if(power=="Freeze")
                myPhotonView.RPC("DisableFreeze", RpcTarget.All);

            moveSpeed = normalSpeed;

            if (power == "Mushroom")
            {
                mushroomPower = false;
                weaponSlot.GetComponentInChildren<MultiShoot>().mushroomPower = mushroomPower;
                transform.localScale = new Vector3(1, 1, 1);
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            MultiGameManager.instance.isPowerActive = false;    
            power = "";
            Debug.Log("Power up over");
        }

    }

    //change players health, works for damage and heals, send info to uimanager
    public void ChangeHealth(float hp)
    {
        if (dead || forcefield.activeInHierarchy || godMode) return;
        if (myPhotonView.IsMine)
        {
            health += hp;
            MultiGameManager.instance.SetPlayerHealth(hp);

            if (health <= 0)
            {
                //player dies, do something
                Die();

            }
        }
            

    }

    void Die()
    {
        if (myPhotonView.IsMine)
        {
            myPhotonView.RPC("RPC_Die", RpcTarget.All);
            if (MultiGameManager.instance.playersAlive != 0)
            {
                uiManager.ShowDeathPanel();
            }
            DisablePowerUps();
            controller.enabled = false;
            transform.rotation = new Quaternion(0, 0, 90, -90);
            transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }
            
    }
    [PunRPC]
    void RPC_Die()
    {
        audioS.PlayOneShot(deathClip);
        dead = true;
        MultiGameManager.instance.ChangePlayerCount(-1);
    }
    [PunRPC]
    public void Resurrect()
    {
        Debug.Log("Resurrection cast");
        if (myPhotonView.IsMine)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            MultiGameManager.instance.SetPlayerHealthToFull();
            health = MultiGameManager.instance.GetPlayerHealth();
            controller.enabled = true;
            uiManager.HideDeathPanel();

        }
        audioS.PlayOneShot(ressClip);
        dead = false;
        MultiGameManager.instance.ChangePlayerCount(1);
    }

    IEnumerator CastResurrection()
    {
        float ressTime = 5;
        //visual for ress
        GameObject efx = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RessParticleEffect"), transform.position, Quaternion.identity);
        while (ressTime > 0)
        {
            if (!resurrectable)
            {
                Destroy(efx);
                StopCoroutine(CastResurrection());

            }
            yield return new WaitForSeconds(1);
            ressTime--;
        }
        playerToResurrect.GetComponent<PhotonView>().RPC("Resurrect", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Resurrect()
    {
        dead = false;
        MultiGameManager.instance.ChangePlayerCount(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp") && myPhotonView.IsMine && !MultiGameManager.instance.isPowerActive)
        {
            power = other.GetComponent<PowerUp>().GetPowerUpType();
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>()!=null && myPhotonView!=null)
        {
            if(other.gameObject.GetComponent<PhotonView>().Owner != myPhotonView.Owner  && other.GetComponent<MultiPlayerController>().dead)
            {
                //in ress range
                resurrectable = true;
                playerToResurrect = other.gameObject;
            }
           
         
        }
    }
   
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().Owner != myPhotonView.Owner)
        {
            Debug.Log("Too far to ress");
            resurrectable = false;
            playerToResurrect = null;
            ressTimer = 0;
        }
    }
    [PunRPC]
    void PlayerLeft()
    {
        MultiGameManager.instance.ChangePlayerCount(-1);
    }

    private void OnApplicationQuit()
    {
        if (!dead && myPhotonView.IsMine)
        {
            myPhotonView.RPC("PlayerLeft", RpcTarget.AllViaServer);
            PhotonNetwork.SendAllOutgoingCommands();
        }
       
    }
}
