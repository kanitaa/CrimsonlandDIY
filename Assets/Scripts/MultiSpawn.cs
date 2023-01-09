using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MultiSpawn : MonoBehaviour
{
    public PhotonView myPhotonView;
    public GameObject [] objectToSpawn;
    public float spawnRate;
    float spawnTime;

    public List<GameObject> enemiesSpawned = new List<GameObject>();
    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
       

        spawnRate = 30/(PhotonNetwork.PlayerList.Length+MultiGameManager.instance.GetCurrentLevel());
        spawnTime = spawnRate;
       
      
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        spawnTime -= Time.deltaTime;
        if (spawnTime < 0)
        {
            myPhotonView.RPC("SpawnObject", RpcTarget.MasterClient);
            spawnTime = spawnRate;
        }
    }

    [PunRPC]
    void SpawnObject()
    {
        spawnRate = 30 / (PhotonNetwork.PlayerList.Length + MultiGameManager.instance.GetCurrentLevel());
        spawnTime = spawnRate;
        int rng = Random.Range(0, objectToSpawn.Length);
        float height = Camera.main.orthographicSize + 1;
        float width = Camera.main.orthographicSize * Camera.main.aspect + 1;

        //spawn top side of cam
        Vector3 spawnLocation = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width), 3, Camera.main.transform.position.z + height + Random.Range(20, 30));
        GameObject enemy = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objectToSpawn[rng].name), spawnLocation, Quaternion.identity);
        enemiesSpawned.Add(enemy);

        //spawn bottom side of cam
        Vector3 spawnLocation2 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width), 3, Camera.main.transform.position.z + height + Random.Range(-20, -30));

        rng = Random.Range(0, objectToSpawn.Length);
        enemy =PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objectToSpawn[rng].name), spawnLocation2, Quaternion.identity);

        enemiesSpawned.Add(enemy);

        //spawn right side of cam
        rng = Random.Range(0, objectToSpawn.Length);
        Vector3 spawnLocation3 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width) + Random.Range(20, 30), 3, Camera.main.transform.position.z);

        enemy = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objectToSpawn[rng].name), spawnLocation3, Quaternion.identity);
        enemiesSpawned.Add(enemy);

        //spawn left side of cam
        rng = Random.Range(0, objectToSpawn.Length);
        Vector3 spawnLocation4 = new Vector3(Camera.main.transform.position.x + Random.Range(-width, width) + Random.Range(-20, -30), 3, Camera.main.transform.position.z);

        enemy = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objectToSpawn[rng].name), spawnLocation4, Quaternion.identity);
        enemiesSpawned.Add(enemy);
    }
}
