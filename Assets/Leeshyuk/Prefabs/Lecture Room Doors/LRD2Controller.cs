using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class LRD2Controller : MonoBehaviour
{
    public GameObject PanelPrefab;

    public float angle = 0f;
    public float speed = 200f;

    private bool isOpen = false;

    private void Update()
    {
        if (PanelPrefab != null)
        {
            if (angle < 0f) angle = 0f;
            if (angle > 90f) angle = 90f;
            PanelPrefab.GetComponent<Panel2Controller>().angle = angle;
            PanelPrefab.GetComponent<Panel2Controller>().speed = speed;
        }
    }

    public void ExecuteInteraction()
    {
        GetComponent<AudioSource>().Play();
        if (isOpen)
        {
            angle = 0f;
            isOpen = false;
        }
        else
        {
            angle = 90f;
            isOpen = true;
        }
    }
}
