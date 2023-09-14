using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFollowHead : MonoBehaviour
{
    public float verticalOffset;
    public Transform head;


    // Update is called once per frame
    void Update()
    {
        transform.position = head.position + Vector3.up * verticalOffset;
        transform.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
    }
}
