using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MultiShoot : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public MultiPlayerController player;
    PhotonView myPhotonView;
    public Weapon weapon;
    public int weaponDamage;
    public float fireRate;
    public int bulletAmount;



    public float hitForce = 100f;
    public Transform gunEnd;
    public Vector3 shootPoint;


    private AudioSource weaponAudio;

    [SerializeField]
    AudioClip shootClip;

    [SerializeField]
    AudioClip reloadClip;

    private float nextFire;


    TextMeshProUGUI bulletAmountText;

    LayerMask enemyLayer;
    public bool mushroomPower;
    bool isShooting;
    void Start()
    {
        player = GetComponentInParent<MultiPlayerController>();
        weaponAudio = GetComponent<AudioSource>();
        myPhotonView = GetComponent<PhotonView>();
      
        weaponDamage = weapon.bulletDamage;
        fireRate = weapon.fireRate;
 
        mushroomPower = player.mushroomPower;

        enemyLayer = MultiGameManager.instance.enemyLayer;
        //set correct bullet amount, if first time use get default amount, else get amount from gamemanager
        if (player.photonView.IsMine)
        {
            if (weapon.weaponName == "P90" && MultiGameManager.instance.bulletsP90 != 0)
            {
                bulletAmount = MultiGameManager.instance.bulletsP90;
            }
            else if (weapon.weaponName == "Pistol" && MultiGameManager.instance.bulletsPistol != 0)
            {
                bulletAmount = MultiGameManager.instance.bulletsPistol;
            }
            else if (weapon.weaponName == "Rifle" && MultiGameManager.instance.bulletsRifle != 0)
            {
                bulletAmount = MultiGameManager.instance.bulletsRifle;
            }
            else
            {
                bulletAmount = weapon.bulletAmount;
            }

            bulletAmountText = MultiGameManager.instance.aim.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            bulletAmountText.text = bulletAmount.ToString();
        }
       

    }
   
    // Update is called once per frame
    void Update()
    {
        if (!myPhotonView.IsMine ||player.dead)
            return;

        //if player wants to reload weapon, this needs to happen while not shooting
        if (Input.GetKeyDown(KeyCode.R) && !Input.GetMouseButton(0))
        {
            StartCoroutine(ReloadWeapon());
        }
        if (!MultiGameManager.instance.reloadingWeapon)
        {
            //if mousebutton is released during rapid fire, stop shooting
            if (Input.GetMouseButtonUp(0) && weapon.weaponName == "P90" && bulletAmount != 0 && isShooting)
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
        while (bulletAmount > 0)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }

    }

    void Shoot()
    {
        if (myPhotonView.IsMine)
        {
            bulletAmount--;
            bulletAmountText.text = bulletAmount.ToString();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            myPhotonView.RPC("GunSound", RpcTarget.All);
            //visual for firing a gun
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ShotParticleEffect"), gunEnd.transform.position, Quaternion.identity);

           if (Physics.Raycast(ray, out RaycastHit hit, enemyLayer))
            {
                MultiGameManager.instance.SetLevelShots();
                //if player hits enemy
                if (hit.transform.GetComponent<MultiEnemy>() != null)
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "HitParticleEffect"), hit.point, Quaternion.identity); //player hits enemy, show visual
                    if (mushroomPower)
                    {
                        hit.transform.GetComponent<MultiEnemy>().TakeDamage(100, weapon.experienceMultiplier); //mushroom power up enabled
                    }
                    else
                    {                                                                                                              //if player is moving, "lower accuracy"
                        if (player.GetComponent<CharacterController>().velocity.x != 0 || player.GetComponent<CharacterController>().velocity.z != 0)
                        {
                            MultiGameManager.instance.aim.ShowAim(true); //show aim indicator based on movement
                            hit.transform.GetComponent<MultiEnemy>().TakeDamage(weaponDamage / 2, weapon.experienceMultiplier);
                            if (hit.transform.GetComponent<MultiEnemy>().dead && myPhotonView.IsMine)
                            {
                                MultiGameManager.instance.SetLevelKills();
                            }
                        }
                        else
                        {
                            MultiGameManager.instance.aim.ShowAim(false); //show aim indicator based on distance
                            hit.transform.GetComponent<MultiEnemy>().TakeDamage(weaponDamage, weapon.experienceMultiplier);
                            if (hit.transform.GetComponent<MultiEnemy>().dead && myPhotonView.IsMine)
                            {
                                MultiGameManager.instance.SetLevelKills();
                            }
                        }
                        //do bonus damage to ghost if player is using rifle
                        if (hit.transform.GetComponent<MultiEnemy>().name == "Ghost(Clone)" && weapon.weaponName == "Rifle")
                        {
                            hit.transform.GetComponent<MultiEnemy>().TakeDamage(weaponDamage, weapon.experienceMultiplier);
                        }
                    }
                }
                else //player misses, give gamemanager info for accuracy tracking
                {
                    MultiGameManager.instance.ShotsMissed();
                }

            }
            //out of bullets, start to reload weapon
            if (bulletAmount == 0)
            {
                StopAllCoroutines();
                StartCoroutine(ReloadWeapon());
            }
        }
    }
    [PunRPC]
    void GunSound()
    {
        weaponAudio.PlayOneShot(shootClip);
    }

    //weapon reload time
    private IEnumerator ReloadWeapon()
    {
        weaponAudio.PlayOneShot(reloadClip);
        MultiGameManager.instance.reloadingWeapon = true;
        MultiGameManager.instance.aim.OutOfAmmoTexture();
        yield return new WaitForSeconds(weapon.reloadTime);
        bulletAmount = weapon.bulletAmount; //set bullets back to full based on weapon bullet amount
        bulletAmountText.text = bulletAmount.ToString();
        MultiGameManager.instance.aim.AmmoAvailableTexture();
        MultiGameManager.instance.reloadingWeapon = false; //let gamemanager know reloading is over
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        string stringid = info.Sender.ActorNumber + "001";
        int id = Int32.Parse(stringid);
      
        transform.SetParent(PhotonView.Find(id).gameObject.transform.GetChild(0));
        transform.localPosition = new Vector3(0, 0, 0);
      
     
    }
}
