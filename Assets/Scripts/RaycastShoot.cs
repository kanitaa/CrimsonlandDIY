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
    public Transform gunEnd;


    private WaitForSeconds shotDuration;
    private AudioSource weaponAudio;

    private LineRenderer laserLine;
    private float nextFire;


    TextMeshProUGUI bulletAmountText;
    void Start()
    {
        player=FindObjectOfType<PlayerController>();
        laserLine = GetComponent<LineRenderer>();
        //  weaponAudio = GetComponent<AudioSource>();

        weaponDamage = weapon.bulletDamage;
        fireRate = weapon.fireRate;
        bulletAmount = weapon.bulletAmount;
        shotDuration = new WaitForSeconds(weapon.shotDuration);

        bulletAmountText = AimIndicator.instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        bulletAmountText.text = bulletAmount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && weapon.weaponName == "P90" && bulletAmount!=0)
        {
         
            StopAllCoroutines();

        }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            
            //if pressing mousebutton and able to shoot based on weapons firerate
            if (Input.GetMouseButton(0) && weapon.weaponName == "P90" && bulletAmount != 0)
            {
                StartCoroutine(ShootMachineGun());
                
            }
           



            //if pressing mousebutton and able to shoot based on weapons firerate
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire && weapon.weaponName != "P90" && bulletAmount!=0)
            {
                Shoot();
            }
        }
    }


    IEnumerator ShootMachineGun()
    {
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
        GameManager.instance.IncreaseLevelShots();
        StartCoroutine(ShotEffect());

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //start point of line renderer
        laserLine.SetPosition(0, gunEnd.position);

        //end point of laserline
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float dist = Vector3.Distance(hit.point, transform.position);


            if (dist < 10)
            {
                AimIndicator.instance.startSize = dist * 10;
            } else if (dist < 15)
            {
                AimIndicator.instance.startSize = dist * 8;
            } else if (dist > 20)
            {
                AimIndicator.instance.startSize = dist * 6;
            }
            AimIndicator.instance.StartLerping();

            //if player hits enemy
            if (hit.transform.GetComponent<Enemy>() != null)
            {
                //if player is moving, "lower accuracy"
                if (player.GetComponent<CharacterController>().velocity.x != 0 || player.GetComponent<CharacterController>().velocity.z != 0)
                {
                    hit.transform.GetComponent<Enemy>().TakeDamage(weaponDamage / 2);
                }
                else
                {
                    hit.transform.GetComponent<Enemy>().TakeDamage(weaponDamage);
                }
            }
            else //player misses
            {
                GameManager.instance.ShotsMissed();
            }

           

            laserLine.SetPosition(1, new Vector3(hit.point.x, 3, hit.point.z));

        }
       
        if (bulletAmount == 0)
        {
            StopAllCoroutines();
            StartCoroutine(ReloadWeapon());
        }
       
    }
    //enable visual effect for shot with line renderer
    private IEnumerator ShotEffect()
    {
        // weaponAudio.Play();
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }

    private IEnumerator ReloadWeapon()
    {
        GameManager.instance.reloadingWeapon = true;
        AimIndicator.instance.OutOfAmmoTexture();
        yield return new WaitForSeconds(weapon.reloadTime);
        bulletAmount=weapon.bulletAmount;
        bulletAmountText.text = bulletAmount.ToString();
        AimIndicator.instance.AmmoAvailableTexture();
        GameManager.instance.reloadingWeapon = false;
    }
}
