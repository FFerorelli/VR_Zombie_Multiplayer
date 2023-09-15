using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Zombie : NetworkBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 4;
    public AudioClip deathAudio;
    public Transform target;
    private NavMeshAgent agent;
    private Rigidbody[] rbs;

    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        if (IsServer)
        {
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();

            target = players[Random.Range(0, players.Length)].root;
        }
        else
        {
            agent.enabled = false;
        }


        DisactivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            agent.SetDestination(target.position);

            if (Vector3.Distance(target.position, transform.position) < 1.5f)
                NetworkSceneTransition.Instance.LoadSceneForEverybody("Zombie");
        }

    }

    public void Death()
    {
        DeathClientRPC();

        Destroy(gameObject, 10);
    }

    [ClientRpc]
    public void DeathClientRPC()
    {
        ActivateRagdoll();
        agent.enabled = false;
        GetComponent<Animator>().enabled = false;
        AudioSource audioS = GetComponent<AudioSource>();
        audioS.loop = false;
        audioS.PlayOneShot(deathAudio);

        Destroy(this);
    }

    void ActivateRagdoll()
    {
        foreach (var item in rbs)
        {
            item.isKinematic = false;
        }
    }

    void DisactivateRagdoll()
    {
        foreach (var item in rbs)
        {
            item.isKinematic = true;
        }
    }
}
