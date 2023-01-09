using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    UIManager uiManager;
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

    AudioSource audioS;
    [SerializeField]
    AudioClip deathClip;

    bool godMode;

    public bool mushroomPower;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = normalSpeed;
        health = GameManager.instance.GetPlayerHealth();
        audioS=GetComponent<AudioSource>();
        //add player power up functions to gamemanager
        GameManager.OnPowerDisable += DisablePowerUps;
        GameManager.OnPowerEnable += EnablePowerUp;
    }

 
    void Update()
    {

          moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
          moveDirection *= moveSpeed;
          controller.Move(moveDirection * Time.deltaTime);
       
        //rotate to raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 hit = ray.GetPoint(rayLength);
            transform.LookAt(new Vector3(hit.x, transform.position.y,hit.z));
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

    //check which power up is hit and do the power effect
    void EnablePowerUp()
    {
            if (power == Powers.Freeze.ToString())
            {
            for(int i=0; i< GameManager.instance.spawner.enemiesSpawned.Count; i++)
            {
                GameManager.instance.spawner.enemiesSpawned[i].gameObject.GetComponent<Enemy>().Freeze();
            }
               
               
            }else if (power == Powers.Forcefield.ToString())
            {
                forcefield.SetActive(true);
              
            }
            else if(power== Powers.Speed.ToString())
            {
               
                moveSpeed = powerSpeed;
               
            }else if(power== Powers.Mushroom.ToString())
            {
                mushroomPower = true;
                weaponSlot.GetComponentInChildren<RaycastShoot>().mushroomPower = mushroomPower;
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            }
        
    }

    //disable all powerups
    void DisablePowerUps()
    {
        if (forcefield.activeInHierarchy)
        {
            forcefield.SetActive(false);
        }
       
        moveSpeed = normalSpeed;
        for (int i = 0; i < GameManager.instance.spawner.enemiesSpawned.Count; i++)
        {
            GameManager.instance.spawner.enemiesSpawned[i].gameObject.GetComponent<Enemy>().UnFreeze();
        }

        if (power == "Mushroom")
        {
            mushroomPower = false;
            weaponSlot.GetComponentInChildren<RaycastShoot>().mushroomPower = mushroomPower;
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        power = "";
        Debug.Log("Power up over");

      
    }

    //change players health, works for damage and heals, send info to uimanager
    public void ChangeHealth(float hp)
    {
        if (forcefield.activeInHierarchy || godMode) return;
        health += hp;
        GameManager.instance.SetPlayerHealth(hp);
       
        if (health <= 0)
        {
            //player dies, do something
            audioS.PlayOneShot(deathClip);
          
             
            uiManager.ShowDeathPanel();
            GameManager.instance.DeathDataReset();

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp") && power=="")
        {
            power = other.GetComponent<PowerUp>().GetPowerUpType();
            Destroy(other.gameObject);
        }
    }

    //remove functions from delegate or else they will be added second time when level changes because gamemanager stays the same
    private void OnDestroy()
    {
        GameManager.OnPowerDisable -= DisablePowerUps;
        GameManager.OnPowerEnable -= EnablePowerUp;
    }
}
