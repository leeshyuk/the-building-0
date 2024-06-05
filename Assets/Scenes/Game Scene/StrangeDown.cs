using UnityEngine;

public class StrangeDown : MonoBehaviour
{
    public GameObject player;
    public GameManager manager;
    public float time = 0.01f;

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<BasicFPCC>().enabled = false;
        Vector3 initPosition = other.transform.position;
        other.transform.position = initPosition + new Vector3(0, 4.66666f, 0);
        Invoke(nameof(UnfreezePlayer), time);
        manager.onStage = false;
        if (manager.isStrange) manager.Stop();
        manager.InitStage(1);
    }
    void UnfreezePlayer()
    {
        player.GetComponent<BasicFPCC>().enabled = true;
    }
}
