using UnityEngine;

public class LRD2Controller : MonoBehaviour
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
            PanelPrefab.GetComponent<Panel2Controller>().angle = angle;
            PanelPrefab.GetComponent<Panel2Controller>().speed = speed;
        }
    }
}
