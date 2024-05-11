using UnityEngine;

public class LRD1Controller : MonoBehaviour
{
    public GameObject PanelPrefab;

    public float angle = 0f;
    public float speed = 200f;

    private void Update()
    {
        if (PanelPrefab != null)
        {
            if (angle < 0f) angle = 0f;
            if (angle > 90f) angle = 90f;
            PanelPrefab.GetComponent<Panel1Controller>().angle = angle;
            PanelPrefab.GetComponent<Panel1Controller>().speed = speed;
        }
    }
}
