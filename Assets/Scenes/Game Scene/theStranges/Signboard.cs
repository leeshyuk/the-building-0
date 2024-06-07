using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Signboard : MonoBehaviour
{
    public GameObject[] signboards;

    private void OnEnable()
    {
        for (int i = 0; i < signboards.Length; i++)
        {
            signboards[i].GetComponent<SignboardController>().strangeKorean = "강의실";
            signboards[i].GetComponent<SignboardController>().strangeEnglish = "Leacture Room";
            signboards[i].GetComponent<SignboardController>().strangeState = true;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < signboards.Length; i++)
        {
            signboards[i].GetComponent<SignboardController>().strangeState = false;
        }
    }
}
