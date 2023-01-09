using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject [] objectToSpawn;
    public float spawnRate;
    float spawnTime;

    public List<GameObject> enemiesSpawned = new List<GameObject>();
    
    void Start()
    {
        spawnRate = 10 / GameManager.instance.GetCurrentLevel();
        spawnTime = spawnRate;
        if (GameManager.instance.lastLevelCleared)
        {
            spawnRate = 10 / GameManager.instance.subLevel;
        }
      

    }

    private void Update()
    {
        spawnTime -= Time.deltaTime;
        if(spawnTime<0)
        {
            SpawnObject();
            spawnTime = spawnRate;
        }
    }
    //spawn enemies outside of camera view based on spawn rate
    void SpawnObject()
    {
        if (GameManager.instance.lastLevelCleared)
        {
            spawnRate = 10 / GameManager.instance.subLevel;
            spawnTime=spawnRate;
        }

        int rng = Random.Range(0,objectToSpawn.Length);
        float height = Camera.main.orthographicSize + 1;
        float width = Camera.main.orthographicSize * Camera.main.aspect + 1;

        //spawn top side of cam
        Vector3 spawnLocation = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width), 3, Camera.main.transform.position.z + height + Random.Range(30, 40));
        GameObject enemy = Instantiate(objectToSpawn[rng], spawnLocation, Quaternion.identity);
        enemiesSpawned.Add(enemy);

        //spawn bottom side of cam
        rng = Random.Range(0, objectToSpawn.Length);
        Vector3 spawnLocation2 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width), 3, Camera.main.transform.position.z + height + Random.Range(-30, -40));

        enemy= Instantiate(objectToSpawn[rng], spawnLocation2, Quaternion.identity);
        enemiesSpawned.Add(enemy);

        //spawn right side of cam
        rng = Random.Range(0, objectToSpawn.Length);
        Vector3 spawnLocation3 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width) + Random.Range(30, 40), 3 , Camera.main.transform.position.z);

        enemy = Instantiate(objectToSpawn[rng], spawnLocation3, Quaternion.identity);
        enemiesSpawned.Add(enemy);

        //spawn left side of cam
        rng = Random.Range(0, objectToSpawn.Length);
        Vector3 spawnLocation4 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width) + Random.Range(-30, -40), 3, Camera.main.transform.position.z);

        enemy = Instantiate(objectToSpawn[rng], spawnLocation4, Quaternion.identity);
        enemiesSpawned.Add(enemy);
    }
}
