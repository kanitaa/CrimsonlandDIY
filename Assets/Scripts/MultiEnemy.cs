using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class MultiEnemy : MonoBehaviour
{
    public PhotonView myPhotonView;


    public int health = 20;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float attackSpeed;
    [SerializeField]
    int attackDamage;

    NavMeshAgent agent;
    [SerializeField]
    GameObject[] player;
    [SerializeField]
    List<GameObject> players = new List<GameObject>();
    [SerializeField]
    MultiPlayerController target;
    bool attacking;
    public bool dead;
    int playerId;
    [SerializeField]
    GameObject freezeEffect;

    public GameObject[] powerUp;

    [SerializeField]
    float killProgress;
    float playersInRoom;

    AudioSource audioS;
    [SerializeField]
    AudioClip deathClip;

    float weaponMultiplier;
    private void Start()
    {
        //init variables
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectsWithTag("Player");
        for(int i= 0; i < player.Length; i++)
        {
            players.Add(player[i]);
        }
        playerId = Random.Range(0, player.Length);
        target = players[playerId].GetComponent<MultiPlayerController>();
        agent.speed = moveSpeed;
        myPhotonView = GetComponent<PhotonView>();
        audioS = GetComponent<AudioSource>();
        playersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;

        killProgress = killProgress / playersInRoom;

        weaponMultiplier = 1;

    }
    void Movement()
    {  //keep moving towards player all the time
        if (dead) return;
        if (players[playerId] != null)
        {
            if (target.dead)
            {
                UpdateTarget();
            }
            agent.destination = players[playerId].transform.position;
            if (Vector3.Distance(agent.transform.position, players[playerId].transform.position) < 1.8f && !attacking && !freezeEffect.activeInHierarchy)
            {
                //if player is close enough then attack
                StartCoroutine(Attack());
            }
        }
        else
        {
            UpdateTarget();
        }
       

    }

    public void Freeze()
    {
        myPhotonView.RPC("RPC_Freeze", RpcTarget.All);
    }

    public void UnFreeze()
    {
        myPhotonView.RPC("RPC_UnFreeze", RpcTarget.All);
    }
    [PunRPC]
    void RPC_Freeze()
    {
        freezeEffect.SetActive(true);
        agent.isStopped = true;
    }
    [PunRPC]
    void RPC_UnFreeze()
    {
        freezeEffect.SetActive(false);
        agent.isStopped = false;
    }
    private void Update()
    {
        Movement();
    }
    IEnumerator Attack()
    {
        attacking = true;
        target.ChangeHealth(-attackDamage);
        //delay for attack
        yield return new WaitForSeconds(attackSpeed);
       
        if (target.dead)
        {
            yield return new WaitForSeconds(1);
            UpdateTarget();
        }
        attacking = false;
    }
    void UpdateTarget()
    {
        //find new target
        if (playerId < players.Count - 1)
        {
            playerId++;
        }
        else
        {
            playerId = 0;
        }
        if (players[playerId] != null)
        {
            target = players[playerId].GetComponent<MultiPlayerController>();
            
        }
        else
        {   //player had left game, find new target
            players.Remove(players[playerId]);
            UpdateTarget();
        }
            
    }
    public void TakeDamage(int damage, float weaponMultiplier)
    {
        if (dead) return;
        this.weaponMultiplier = weaponMultiplier;
        myPhotonView.RPC("Damage", RpcTarget.All, damage);
    }


    [PunRPC]
    void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            audioS.PlayOneShot(deathClip);
            StartCoroutine("Death"); 
            
        }

    }


    [PunRPC]
    private void Die(float weaponMultiplier)
    {
        MultiGameManager.instance.SetLevelProgress(killProgress*weaponMultiplier);
        MultiGameManager.instance.spawner.enemiesSpawned.Remove(this.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
        int rng = Random.Range(1, 100);
        if (rng <= 15)
        {
            if(myPhotonView.IsMine) //drop only one power up
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", powerUp[Random.Range(0, powerUp.Length)].name), new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        }

       
        Destroy(gameObject);  
    }

    IEnumerator Death()
    {
        dead = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(0.6f);
        myPhotonView.RPC("Die", RpcTarget.AllViaServer, weaponMultiplier);

    }
    private void OnTriggerEnter(Collider other)
    {
        //player power up
        if (other.gameObject.CompareTag("Forcefield"))
        {
            myPhotonView.RPC("Die", RpcTarget.AllViaServer, weaponMultiplier);
        }
    }
}
