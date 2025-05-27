using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Leap;  // HandPoseDetector.cs 의 네임스페이스

[RequireComponent(typeof(HandPoseDetector))]
public class HandPosePointer : MonoBehaviour
{
    [Header("Pose Detectors")]
    [Tooltip("TriggerOff 포즈를 감지하는 HandPoseDetector")]
    public HandPoseDetector offDetector;
    [Tooltip("TriggerOn 포즈를 감지하는 HandPoseDetector")]
    public HandPoseDetector onDetector;

    [Header("Pointer Settings")]
    [Tooltip("Index finger tip Transform (예: HandModel/IndexTip)")]
    public Transform indexTip;
    [Tooltip("레이저 포인터 프리팹 (LineRenderer 포함)")]
    public GameObject laserPrefab;

    // 내부 상태
    private GameObject laserInstance;
    private LineRenderer laserLine;
    private bool isPointerActive = false;

    void Start()
    {
        // OffPose: OnPoseDetected → 레이저 생성, OnPoseLost → 레이저 제거
        offDetector.OnPoseDetected.AddListener(ActivatePointer);
        offDetector.OnPoseLost.AddListener(DeactivatePointer);

        // OnPoseDetected → UI 클릭
        onDetector.OnPoseDetected.AddListener(PerformClick);
    }

    void Update()
    {
        if (isPointerActive && laserInstance != null)
        {
            UpdateLaserTransform();
        }
    }

    /// <summary>
    /// TriggerOff 포즈 인식 시 호출 → 레이저 생성
    /// </summary>
    private void ActivatePointer()
    {
        if (isPointerActive) return;

        laserInstance = Instantiate(laserPrefab);
        laserLine = laserInstance.GetComponent<LineRenderer>();
        isPointerActive = true;
    }

    /// <summary>
    /// TriggerOff 포즈 해제 시 호출 → 레이저 제거
    /// </summary>
    private void DeactivatePointer()
    {
        if (!isPointerActive) return;

        Destroy(laserInstance);
        laserInstance = null;
        laserLine = null;
        isPointerActive = false;
    }

    /// <summary>
    /// TriggerOn 포즈 인식 시 호출 → UI 버튼 클릭
    /// </summary>
    private void PerformClick()
    {
        if (!isPointerActive) return;

        // 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(
            indexTip.position + indexTip.forward * 2f);

        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        // UI 레이캐스트
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count > 0)
        {
            var go = results[0].gameObject;
            // 1) Button 컴포넌트가 있으면 onClick.Invoke()
            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.Invoke();
                return;
            }
            // 2) IPointerClickHandler 계열에도 전달
            ExecuteEvents.ExecuteHierarchy(go, data, ExecuteEvents.pointerClickHandler);
        }
    }

    /// <summary>
    /// 매 프레임 레이저의 시작/끝점 갱신
    /// </summary>
    private void UpdateLaserTransform()
    {
        Vector3 start = indexTip.position;
        Vector3 dir = indexTip.forward;

        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, start + dir * 5f);  // 길이 5m 예시
    }
}
