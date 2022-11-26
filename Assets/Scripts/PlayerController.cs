using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    [SerializeField]
    private float moveSpeed;
    private Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    public GameObject weaponSlot;

    
    public Weapon currentWeapon;
    int health;

  
    public string power;  
    enum Powers { Speed, Forcefield, Kappa };

    [SerializeField]
    private GameObject forcefield;

    [SerializeField]
    UIManager uiManager;

    void Start()
    {
        health = 100;
        controller = GetComponent<CharacterController>();
        GameManager.OnPowerDisable += DisablePowerUps;
        GameManager.OnPowerEnable += EnablePowerUp;


    }

    // Update is called once per frame
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

    void EnablePowerUp()
    {
       
        for (int i = 0; i < 2; i++)
        {
            if (power == Powers.Kappa.ToString())
            {
                Debug.Log(power);
                return;
            }else if (power == Powers.Forcefield.ToString())
            {
                forcefield.SetActive(true);
                return;
            }
            else if(power== Powers.Speed.ToString())
            {
               
                moveSpeed = 15;
                return;
            }
        }
    }
    void DisablePowerUps()
    {
        if (forcefield.activeInHierarchy)
        {
            forcefield.SetActive(false);
        }
       
        moveSpeed = 10;    
        
        power = "";
        Debug.Log("Power up over");

      
    }
    public void ChangeHealth(int hp)
    {
        health += hp;
        uiManager.ChangeHealthValue(hp);
        Debug.Log("health changed: " + hp);
        if (health <= 0)
        {
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
}
