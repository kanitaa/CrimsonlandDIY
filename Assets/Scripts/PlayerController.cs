using System.Collections;
using System.Collections.Generic;
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
    int health;

  
    //power variables, powers name needs to match the one in enum
    public string power;  
    enum Powers { Speed, Forcefield, Kappa };

    [SerializeField]
    private GameObject forcefield;

   

    void Start()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = normalSpeed;
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

    }

    //check which power up is hit and do the power effect
    void EnablePowerUp()
    {
            if (power == Powers.Kappa.ToString())
            {
                Debug.Log(power);
               
            }else if (power == Powers.Forcefield.ToString())
            {
                forcefield.SetActive(true);
              
            }
            else if(power== Powers.Speed.ToString())
            {
               
                moveSpeed = powerSpeed;
               
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
        
        power = "";
        Debug.Log("Power up over");

      
    }

    //change players health, works for damage and heals, send info to uimanager
    public void ChangeHealth(int hp)
    {
        health += hp;
        uiManager.ChangeHealthValue(hp);
       
        if (health <= 0)
        {
            //player dies, do something
            Debug.Log("ded");
            
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            //disable old power up before activing new one
            DisablePowerUps();
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
