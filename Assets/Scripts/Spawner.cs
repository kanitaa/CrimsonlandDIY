using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnRate;
    
    void Start()
    {
        //spawn enemies outside of camera view based on spawn rate
        InvokeRepeating("SpawnObject",1,spawnRate);
    }

    void SpawnObject()
    {
        
        float height = Camera.main.orthographicSize + 1;
        float width = Camera.main.orthographicSize * Camera.main.aspect + 1;
        
        Vector3 spawnLocation = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width), 3, Camera.main.transform.position.z + height + Random.Range(20, 30));
        Instantiate(objectToSpawn, spawnLocation, Quaternion.identity);

        Vector3 spawnLocation2 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width), 3, Camera.main.transform.position.z + height + Random.Range(-20, -30));

        Instantiate(objectToSpawn, spawnLocation2, Quaternion.identity);
    }
}
