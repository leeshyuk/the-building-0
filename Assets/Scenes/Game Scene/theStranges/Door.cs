using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject player;
    public float thresholdDistance = 5.0f;
    public GameObject[] doors;


    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        print(distance);
        if (distance <= thresholdDistance)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].GetComponent<LRD2Controller>()) doors[i].GetComponent<LRD2Controller>().angle = 45;
                else doors[i].GetComponent<LRD1Controller>().angle = 45;
            }
        }
        if (distance > thresholdDistance)
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].GetComponent<LRD2Controller>()) doors[i].GetComponent<LRD2Controller>().angle = 0;
                else doors[i].GetComponent<LRD1Controller>().angle = 0;
            }
    }
}
