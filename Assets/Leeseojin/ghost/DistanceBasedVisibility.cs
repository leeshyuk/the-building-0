using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedVisibility : MonoBehaviour
{
    public GameObject player; // 플레이어 오브젝트를 참조합니다.
    public float thresholdDistance = 5.0f; // 플레이어와의 거리 임계값입니다.
    private Renderer objectRenderer; // 오브젝트의 렌더러를 참조합니다.

    void Start()
    {
        objectRenderer = GetComponentInChildren<Renderer>(); // 렌더러 컴포넌트를 가져옵니다.
    }

    void Update()
    {
        // 플레이어와의 거리를 계산합니다.
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 플레이어와의 거리가 임계값보다 작으면 오브젝트를 보이게 하고, 그렇지 않으면 보이지 않게 합니다.
        if (distance <= thresholdDistance)
        {
            objectRenderer.enabled = true; // 플레이어가 가까우면 오브젝트를 보입니다.
        }
        else
        {
            objectRenderer.enabled = false; // 플레이어가 멀면 오브젝트를 숨깁니다.
        }
    }
}
