using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewgunPlugin : MonoBehaviour
{
    [Header("Hand Tracking (직접 연결)")]
    public Transform rightPalm;        
    public Transform rightIndexTip;   

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

    void Update()
    {
        if (rightPalm == null || rightIndexTip == null)
        {
            Debug.LogWarning("Palm 또는 IndexTip이 연결되지 않았습니다.");
            return;
        }

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
            Fire();
        }
    }

    void Fire()
    {
        Debug.Log("▶ 발사!");
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        lastShotTime = Time.time;
    }

    bool IsFist()
    {
        float dist = Vector3.Distance(rightPalm.position, rightIndexTip.position);
        return dist < 0.05f; 
    }
}