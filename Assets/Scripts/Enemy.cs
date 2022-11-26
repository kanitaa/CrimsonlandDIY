using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    public int health=20;
    public float moveSpeed;
    public float attackSpeed;

    NavMeshAgent agent;
    GameObject player;
    PlayerController pc;
    bool attacking;

    public GameObject[] powerUp;

    private void Start()
    {
        //init variables
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        agent.speed = moveSpeed;
        
    }

    private void Update()
    {
        //keep moving towards player all the time
        agent.destination =player.transform.position;
        if(Vector3.Distance(agent.transform.position, player.transform.position) < 1.5f && !attacking)
        {
            //if player is close enough then attack
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        attacking = true;
        pc.ChangeHealth(-10);
        //delay for attack
        yield return new WaitForSeconds(attackSpeed);
        attacking = false;
    }
    public void TakeDamage(int damage)
    {
        health-=damage;
        if (health <= 0)
        {
            Die();
        }
        
    }
    private void Die()
    {
        //track kills in current level
        GameManager.instance.IncreaseLevelKills();
        //when enemy dies increase progress 
        GameManager.instance.IncreaseProgress(5);
        if (GameManager.instance.GetCurrentProgress() % 20 == 0) //drop power up after certain amount of progress
        {
            Instantiate(powerUp[Random.Range(0, powerUp.Length)], transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        //player power up
        if (other.gameObject.CompareTag("Forcefield"))
        {
            Die();
        }
    }
}
