using UnityEngine;

public class SpaceBorder : MonoBehaviour
{
    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        manager.onStage = true;
        manager.tpCount = 0;
    }
}
