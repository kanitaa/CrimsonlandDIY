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

        bulletAmountText = GameManager.instance.aim.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        bulletAmountText.text = bulletAmount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //if mousebutton is released during rapid fire, stop shooting
        if (Input.GetMouseButtonUp(0) && weapon.weaponName == "P90" && bulletAmount!=0)
        {
            StopAllCoroutines();
        }
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //if holding mousebutton and able to shoot based on weapons firerate
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

    //rapid fire, firerate between autoshots
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
        GameManager.instance.IncreaseLevelShots(); //for level stats tracking
        StartCoroutine(ShotEffect()); //visual laserline for shot

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //start point of line renderer
        laserLine.SetPosition(0, gunEnd.position);

        //end point of laserline
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameManager.instance.aim.ShowAim(); //show aim indicator based on distance

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
            else //player misses, give gamemanager info for accuracy tracking
            {
                GameManager.instance.ShotsMissed();
            }

           

            laserLine.SetPosition(1, new Vector3(hit.point.x, 3, hit.point.z));

        }
       //out of bullets, start to reload weapon
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

    //weapon reload time
    private IEnumerator ReloadWeapon()
    {
        GameManager.instance.reloadingWeapon = true;
        GameManager.instance.aim.OutOfAmmoTexture();
        yield return new WaitForSeconds(weapon.reloadTime);
        bulletAmount=weapon.bulletAmount; //set bullets back to full based on weapon bullet amount
        bulletAmountText.text = bulletAmount.ToString();
        GameManager.instance.aim.AmmoAvailableTexture();
        GameManager.instance.reloadingWeapon = false; //let gamemanager know reloading is over
    }
}
