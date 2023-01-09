using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class RaycastShoot : MonoBehaviour
{
    public PlayerController player;

    public Weapon weapon;
    public int weaponDamage;
    public float fireRate;
    public int bulletAmount;


   
    public float hitForce = 100f;
    [SerializeField]
    Transform gunEnd;


    private AudioSource weaponAudio;


    [SerializeField]
    GameObject hitEffect, shotEffect;

    [SerializeField]
    AudioClip shootClip;

    [SerializeField]
    AudioClip reloadClip;

    private float nextFire;

    public bool mushroomPower;
    bool isShooting;

    TextMeshProUGUI bulletAmountText;
    void Start()
    {
        player=FindObjectOfType<PlayerController>();
        weaponAudio = GetComponent<AudioSource>();
        mushroomPower = GetComponentInParent<PlayerController>().mushroomPower;
        weaponDamage = weapon.bulletDamage;
        fireRate = weapon.fireRate;
        //set correct bullet amount, if first time use get default amount, else get amount from gamemanager
        if (weapon.weaponName == "P90" && GameManager.instance.bulletsP90 != 0)
        {
            bulletAmount = GameManager.instance.bulletsP90;
        }
        else if (weapon.weaponName == "Pistol" && GameManager.instance.bulletsPistol != 0)
        {
            bulletAmount = GameManager.instance.bulletsPistol;
        }
        else if (weapon.weaponName == "Rifle" && GameManager.instance.bulletsRifle != 0)
        {
            bulletAmount = GameManager.instance.bulletsRifle;
        }
        else
        {
            bulletAmount = weapon.bulletAmount;
        }
      
       
        bulletAmountText = GameManager.instance.aim.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        bulletAmountText.text = bulletAmount.ToString();


    }

    // Update is called once per frame
    void Update()
    {
        //if player wants to reload weapon, this needs to happen while not shooting
        if(Input.GetKeyDown(KeyCode.R) && !Input.GetMouseButton(0))
        {
            StartCoroutine(ReloadWeapon());
        }
        if (!GameManager.instance.reloadingWeapon)
        {
            //if mousebutton is released during rapid fire, stop shooting
            if (Input.GetMouseButtonUp(0) && weapon.weaponName == "P90" && bulletAmount != 0 &&isShooting)
            {
                StopAllCoroutines();
                isShooting = false;
            }
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //if holding mousebutton and able to shoot based on weapons firerate
                if (Input.GetMouseButton(0) && weapon.weaponName == "P90" && bulletAmount != 0 && !isShooting)
                {
                    StartCoroutine(ShootMachineGun());

                }

                //if pressing mousebutton and able to shoot based on weapons firerate
                if (Input.GetMouseButtonDown(0) && Time.time > nextFire && weapon.weaponName != "P90" && bulletAmount != 0)
                {
                    Shoot();
                }
            }
        }
       
    }

    //rapid fire, firerate between autoshots
    IEnumerator ShootMachineGun()
    {
        isShooting = true;
        while(bulletAmount > 0)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
       
    }

    void Shoot()
    {
        bulletAmount--;
        bulletAmountText.text = bulletAmount.ToString();
        GameManager.instance.SetLevelShots(); //for level stats tracking
        weaponAudio.PlayOneShot(shootClip);
        Instantiate(shotEffect, gunEnd.transform.position, Quaternion.identity); //visual for firing a gun
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            //if player hits enemy
            if (hit.transform.GetComponent<Enemy>() != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.identity); //player hits enemy, show visual

                if (mushroomPower)
                {
                    hit.transform.GetComponent<Enemy>().TakeDamage(100, weapon.experienceMultiplier); //mushroom power up enabled, do huge damage with shots
                }
                else //else do normal weapon damage
                {
                    //if player is moving, lower damage
                    if (player.GetComponent<CharacterController>().velocity.x != 0 || player.GetComponent<CharacterController>().velocity.z != 0)
                    {
                        GameManager.instance.aim.ShowAim(true); //show aim indicator based on movement
                        hit.transform.GetComponent<Enemy>().TakeDamage(weaponDamage / 2, weapon.experienceMultiplier);
                    }
                    else
                    {
                        GameManager.instance.aim.ShowAim(false); //show aim indicator based on movement
                        hit.transform.GetComponent<Enemy>().TakeDamage(weaponDamage, weapon.experienceMultiplier);
                    }
                    //do bonus damage to ghost if player is using rifle
                    if (hit.transform.GetComponent<Enemy>().name == "Ghost(Clone)" && weapon.weaponName == "Rifle")
                    {
                        hit.transform.GetComponent<Enemy>().TakeDamage(weaponDamage, weapon.experienceMultiplier);
                    }
                }
            }
            else //player misses, give gamemanager info for accuracy tracking
            {
                GameManager.instance.ShotsMissed();
            }

          
        }
       //out of bullets, start to reload weapon
        if (bulletAmount == 0)
        {
            StopAllCoroutines();
            StartCoroutine(ReloadWeapon());
        }
       
    }

    //weapon reload time
    private IEnumerator ReloadWeapon()
    {
        weaponAudio.PlayOneShot(reloadClip);
        GameManager.instance.reloadingWeapon = true;
        GameManager.instance.aim.OutOfAmmoTexture();
        yield return new WaitForSeconds(weapon.reloadTime);
        bulletAmount=weapon.bulletAmount; //set bullets back to full based on weapon bullet amount
        bulletAmountText.text = bulletAmount.ToString();
        GameManager.instance.aim.AmmoAvailableTexture();
        GameManager.instance.reloadingWeapon = false; //let gamemanager know reloading is over
    }
}
