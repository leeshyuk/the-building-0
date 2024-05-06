using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float angle = 0f; // 목표 각도, 0 ~ 90도
    public float speed = 200f; // 문이 열리는 속도

    // Update is called once per frame
    void Update()
    {
        // 각도를 0 ~ 90도 사이로 제한
        if (angle < 0f) angle = 0f;
        if (angle > 90f) angle = 90f;

        // transform.eulerAngles[1]는 Transform 컴포넌트의 Rotation의 현재 Y값을 의미, 즉 현재 각도
        float currentAngle = transform.eulerAngles[1];

        // 현재 각도가 목표하는 각도보다 작을 때 (- 1은 허용하는 오차 범위)
        if (currentAngle < angle - 1) transform.Rotate(Vector3.up, speed * Time.deltaTime); // 열기

        // 현재 각도가 목표하는 각도보다 클 때 (+ 1은 허용하는 오차 범위)
        else if (currentAngle > angle + 1) transform.Rotate(Vector3.down, speed * Time.deltaTime); // 닫기

        // 현재 각도가 0 ~ 90도가 아닐 때, 0도 아래도 음수가 아니라 360도라서 조건 하나면 됨.
        // 보통 speed가 너무 높으면 한 프레임 당 변하는 각도가 커서 발생
        if (currentAngle > 90f) transform.rotation = Quaternion.Euler(0f, angle, 0f); // 목표 각도로 바로 지정
    }
}
