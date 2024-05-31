using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player; // 플레이어 오브젝트
    public float distanceThreshold = 5f; // 플레이어와 중간 지점 사이의 거리 임계값
    public float rotationSpeed = 50f; // 회전 속도

    private Quaternion originalRotation; // 천장의 원래 회전 상태
    private Quaternion targetRotation; // 천장의 목표 회전 상태

    void Start()
    {
        // 천장의 초기 회전 상태를 저장합니다.
        originalRotation = transform.rotation;
        // 목표 회전 상태를 설정합니다. (천장이 바닥이 되도록)
        targetRotation = Quaternion.Euler(180f, 0f, 0f);
    }

    void Update()
    {
        // 플레이어와 중간 지점 사이의 거리를 계산합니다.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 만약 플레이어와 중간 지점 사이의 거리가 임계값 이하라면
        if (distanceToPlayer <= distanceThreshold)
        {
            // 플레이어가 중간 지점에 도달했으므로 천장을 회전시킵니다.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // 플레이어가 중간 지점을 벗어나면 천장을 원래 상태로 회전시킵니다.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originalRotation, rotationSpeed * Time.deltaTime);
        }
    }
}