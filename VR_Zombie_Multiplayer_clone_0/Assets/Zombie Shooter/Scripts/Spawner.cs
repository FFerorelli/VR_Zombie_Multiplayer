using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawner : NetworkBehaviour
{
    public float spawnTime = 1;
    public GameObject spawnGameObject;
    public Transform[] spawnPoints;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;

        if(timer > spawnTime)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spawnedZombie = Instantiate(spawnGameObject, randomPoint.position, randomPoint.rotation);
            spawnedZombie.GetComponent<NetworkObject>().Spawn(true);

            timer = 0;
        }

        timer += Time.deltaTime;
    }
}
