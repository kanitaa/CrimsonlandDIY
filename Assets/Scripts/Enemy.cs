using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    public int health=20;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float attackSpeed;
    [SerializeField]
    int attackDamage;

    [SerializeField]
    int killProgress;

    NavMeshAgent agent;
    GameObject player;
    PlayerController pc;
    bool attacking;
    bool dead;

    [SerializeField]
    GameObject freezeEffect;
    public GameObject[] powerUp;

    AudioSource audioS;
    [SerializeField]
    AudioClip deathClip;

    float weaponMultiplier;
    private void Start()
    {
        //init variables
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        agent.speed = moveSpeed;
        audioS = GetComponent<AudioSource>();
        weaponMultiplier = 1;
    }

    private void Update()
    {
        if (dead) return;
        //keep moving towards player all the time
        agent.destination =player.transform.position;
        if(Vector3.Distance(agent.transform.position, player.transform.position) < 1.8f && !attacking &&!freezeEffect.activeInHierarchy)
        {
            //if player is close enough then attack
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        attacking = true;
        pc.ChangeHealth(-attackDamage);
        //delay for attack
        yield return new WaitForSeconds(attackSpeed);
        attacking = false;
    }
    public void TakeDamage(int damage, float weaponMultiplier)
    {
        if (dead) return;
        health -=damage;
        this.weaponMultiplier = weaponMultiplier;
        if (health <= 0)
        {
            StartCoroutine("Death");
        }
        
    }

    public void Freeze()
    {
        freezeEffect.SetActive(true);
        agent.isStopped = true;
    }
    public void UnFreeze()
    {
        freezeEffect.SetActive(false);
        agent.isStopped = false;
    }
    private void Die()
    {
        //track kills in current level
        GameManager.instance.SetLevelKills();
        //when enemy dies increase progress 
        GameManager.instance.SetLevelProgress(killProgress*weaponMultiplier);
        int rng = Random.Range(1, 100);
        if (rng <= 15)
        {
            Instantiate(powerUp[Random.Range(0, powerUp.Length)], new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
        }
        GameManager.instance.spawner.enemiesSpawned.Remove(this.gameObject);
        Destroy(gameObject);
    }

    IEnumerator Death()
    {
        dead = true;
        GetComponent<CapsuleCollider>().enabled = false;
        audioS.PlayOneShot(deathClip);
        agent.isStopped = true;
        yield return new WaitForSeconds(0.6f);
        Die();

    }
    private void OnTriggerEnter(Collider other)
    {
        //player power up
        if (other.gameObject.CompareTag("Forcefield"))
        {
            StartCoroutine("Death");
        }
    }
}
