using UnityEngine;

public class NormalDown : MonoBehaviour
{
    public GameObject player;
    public GameManager manager;
    public float time = 0.01f;

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<BasicFPCC>().enabled = false;
        Vector3 initRotation = other.transform.eulerAngles;
        Vector3 initPosition = other.transform.position;
        initPosition = new Vector3(initPosition.z - -18.2250004f, initPosition.y, initPosition.x - -1.73000038f);
        other.transform.eulerAngles = initRotation + new Vector3(0, 90, 0);
        other.transform.position = new Vector3(-1.34998083f + initPosition.x, initPosition.y + 4.66666f, 25.6099968f - initPosition.z);
        Invoke(nameof(UnfreezePlayer), time);
        manager.onStage = false;
        manager.InitStage(1);
    }

    void UnfreezePlayer()
    {
        player.GetComponent<BasicFPCC>().enabled = true;
    }
}
