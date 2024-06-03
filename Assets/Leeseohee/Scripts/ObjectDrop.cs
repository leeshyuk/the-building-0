using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goodpic : MonoBehaviour
{
    public GameObject player;
    public float thresholdDistance = 5.0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance < thresholdDistance && rb != null)
        {
            rb.useGravity = true;
        }
    }
}
