/*
 * 
 *      프리팹 Lecture Room Door 2의 하위 프리팹 Panel 2의 스크립트
 *      
 *      *기능
 *      이 오브젝트가 회전함으로써 상위 프리팹 Lecture Room Door의 문이 열리는 것을 구현
 *      1.  문이 열려있는 각도 설정 가능, 0 ~ 90도
 *      2.  문이 열리는 속도 설정 가능
 *      
 *      
 *      *주의 사항
 *      1.  원하는 각도로 정확히 움직이는 것은 불가능 (유니티의 모든 움직임이 이러함)
 *          연속된 값의 변화(움직임)이라도 결국 디지털값이라서 간격이 존재함
 *          속도가 빠르면 간격이 커지기 때문에 빠른 속도에서는 이상하게 보일 수 있음
 *
 */
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public float angle = 0f; // 목표 각도, 0 ~ 90도
    public float speed = 200f; // 문이 열리는 속도

    private float initialAngle = 0f; // 초기값

    private void Start()
    {
        // transform.eulerAngles[1]는 Transform 컴포넌트의 Rotation의 현재 Y값을 의미
        initialAngle = transform.eulerAngles[1]; // 문이 닫혀있을 때의 각도
        print(initialAngle);
    }

    private void Update()
    {
        // 각도를 0 ~ 90도 사이로 제한
        if (angle < 0f) angle = 0f;
        if (angle > 90f) angle = 90f;

        // 현재 각도
        // 문이 열리는 것이 각도가 작아지는 것이라서 360에서 빼야함
        float currentAngle = 360 - transform.eulerAngles[1];
        if (currentAngle == 360) currentAngle = 0f;

        // 현재 각도가 목표하는 각도보다 작을 때 (- 1은 허용하는 오차 범위)
        if (currentAngle < angle + initialAngle - 1) transform.Rotate(Vector3.down, speed * Time.deltaTime); // 열기

        // 현재 각도가 목표하는 각도보다 클 때 (+ 1은 허용하는 오차 범위)
        else if (currentAngle > angle + initialAngle + 1) transform.Rotate(Vector3.up, speed * Time.deltaTime); // 닫기

        // 각도가 범위를 벗어날 때, 오차로 인해 발생
        if (currentAngle > 90f + initialAngle || currentAngle < initialAngle) transform.rotation = Quaternion.Euler(0f, angle + initialAngle, 0f); // 목표 각도로 바로 지정
    }
}