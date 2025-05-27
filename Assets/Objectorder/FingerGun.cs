using UnityEngine;
using Leap;
using System;
using System.Collections;
//LowpolyHands Object

public class FingerGun : MonoBehaviour
{
    private Controller controller;  // Leap Motion 컨트롤러 인스턴스

    public GameObject bulletPrefab;
    public Transform shootOrigin;
    public float bulletSpeed = 1f;

    public float fireCooldown = 5f;  // 발사 간격 (초)
    private float lastFireTime = 0f; // 마지막 발사 시각

    void Start()
    {
        controller = new Controller(); // 컨트롤러 초기화

    }

    void Update()
    {
        Frame frame = controller.Frame(); // 현재 프레임 얻기

        if (frame.Hands.Count > 0)
        {
            Hand hand = frame.Hands[0]; // 첫 번째 손
            Vector3 fingerPosition = shootOrigin.position;        // ✅ Transform에서 위치만 뽑음
            Vector3 fingerDirection = shootOrigin.forward;
            if (Time.time - lastFireTime > fireCooldown)
            {
                
                Shoot(fingerPosition, fingerDirection);
                lastFireTime = Time.time;
            }
        }
    }

    private void Shoot(Vector3 position, Vector3 forward)
    {
        GameObject bullet = Instantiate(
            bulletPrefab,
            position,
            Quaternion.LookRotation(forward) // 전달받은 방향으로 회전 설정
        );
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // 중력 영향 제거
            rb.velocity = forward.normalized * bulletSpeed; // 전달받은 방향으로 속도 적용
        }
        else
        {
            Debug.LogWarning("Bullet prefab에 Rigidbody 컴포넌트가 없습니다.");
        }
    }
}