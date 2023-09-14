using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollowHead : MonoBehaviour
{
    public float verticalOffset;
    public Transform head;

    private Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerCamera)
            playerCamera = Camera.main.transform;

        transform.position = head.position + Vector3.up * verticalOffset;
        transform.LookAt(playerCamera, Vector3.up);
    }
}
