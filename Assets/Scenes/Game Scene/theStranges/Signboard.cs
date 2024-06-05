using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Signboard : MonoBehaviour
{
    public GameObject[] signboards;
    public int index;

    private void OnEnable()
    {
        index = Random.Range(0, signboards.Length);
        signboards[index].GetComponent<SignboardController>().strangeKorean = "강의실";
        signboards[index].GetComponent<SignboardController>().strangeEnglish = "Leacture Room";
        signboards[index].GetComponent<SignboardController>().strangeState = true;
    }

    private void OnDisable()
    {
        signboards[index].GetComponent<SignboardController>().strangeState = false;
    }
}
