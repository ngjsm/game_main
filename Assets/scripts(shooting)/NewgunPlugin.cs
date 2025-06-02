using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewgunPlugin : MonoBehaviour
{
    private Transform rightPalm;
    private Transform rightIndexTip;

    [Header("Gun Settings")]
    public Transform gunTransform;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float rotationSmooth = 5f;
    public float shootCooldown = 0.5f;

    [Header("Translation-to-Yaw Settings")]
    [Tooltip("손이 수평으로 1m 이동했을 때 얼마나 회전할지 (°)")]
    public float yawSensitivity = 100f;
    [Tooltip("최대 인식할 손 이동 거리 (m)")]
    public float maxPalmMovement = 0.2f;

    private bool calibrated = false;
    private Vector3 initialPalmPos;
    private float initialYaw;
    private float lastShotTime;

    void Start()
    {
        // 실행 중 생성되는 손 구조를 런타임에 찾아서 할당
        GameObject rightHand = GameObject.Find("RightHand(Clone)");
        if (rightHand != null)
        {
            rightPalm = rightHand.transform.Find("Palm");
            rightIndexTip = rightHand.transform.Find("index/tip");
        }

        if (rightPalm == null || rightIndexTip == null)
            Debug.LogWarning("▶ 손 트랜스폼을 찾지 못했습니다. 구조를 확인하세요.");
    }

    void Update()
    {
        if (rightPalm == null || rightIndexTip == null) return;

        Vector3 palmPos = rightPalm.position;

        if (!calibrated)
        {
            initialPalmPos = palmPos;
            initialYaw = gunTransform.rotation.eulerAngles.y;
            calibrated = true;
            return;
        }

        float dx = Mathf.Clamp(palmPos.x - initialPalmPos.x, -maxPalmMovement, maxPalmMovement);
        float targetYaw = initialYaw + dx * yawSensitivity;
        Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

        gunTransform.rotation = Quaternion.Slerp(
            gunTransform.rotation,
            targetRot,
            rotationSmooth * Time.deltaTime
        );

        if (IsFist() && Time.time - lastShotTime > shootCooldown)
        {
            Debug.Log("▶ 주먹 인식됨: 발사!");
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            lastShotTime = Time.time;
        }
    }

    bool IsFist()
    {
        float dist = Vector3.Distance(rightPalm.position, rightIndexTip.position);
        return dist < 0.05f;
    }
}
