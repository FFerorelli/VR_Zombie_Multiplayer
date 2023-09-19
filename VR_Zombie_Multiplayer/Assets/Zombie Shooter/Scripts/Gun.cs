using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    public float speed = 40;
    public GameObject bullet;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void Fire()
    {
        //cast bullet locally (fast)
        ActualFire(barrel.position, barrel.rotation, barrel.forward);

        FireServerRpc(barrel.position, barrel.rotation, barrel.forward, NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireServerRpc(Vector3 pos, Quaternion rot, Vector3 dir, ulong sender)
    {
        FireClientRpc(pos, rot, dir, sender);
    }

    [ClientRpc]
    public void FireClientRpc(Vector3 pos, Quaternion rot, Vector3 dir, ulong sender)
    {
        if (NetworkManager.LocalClientId != sender)
            ActualFire(pos, rot, dir);
    }

    public void ActualFire(Vector3 pos, Quaternion rot, Vector3 dir)
    {
        GameObject spawnedBullet = Instantiate(bullet, pos, rot);

        spawnedBullet.GetComponent<Rigidbody>().velocity = speed * dir;
        audioSource.PlayOneShot(audioClip);
        Destroy(spawnedBullet, 2);
    }
}