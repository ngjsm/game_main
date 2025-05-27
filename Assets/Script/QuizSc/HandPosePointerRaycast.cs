using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Leap;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class HandPosePointerRaycast : MonoBehaviour
{
    [Header("▶ Pose Detectors")]
    public HandPoseDetector offDetector;
    public HandPoseDetector onDetector;

    [Header("▶ Index Tip")]
    public Transform indexTip; // 손끝 Transform

    [Header("▶ Raycast Settings")]
    public float maxDistance = 5f;
    public LayerMask interactableLayer;

    [Header("▶ Visual Pointer")]
    public LineRenderer lineRenderer;
    public float lineWidth = 0.005f;

    private bool pointerActive = false;
    private bool suppressPointerOff = false;
    private Coroutine pendingPointerOffCoroutine = null;
    private Coroutine clearSuppressionCoroutine = null;
    private float suppressTimeWindow = 0.2f;

    void Awake()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.enabled = false;

        // Pose Detector 이벤트 연결
        offDetector.OnPoseDetected.AddListener(OnOffPoseDetected);
        offDetector.WhilePoseDetected.AddListener(OnOffPoseHeld);
        offDetector.OnPoseLost.AddListener(OnOffPoseLost);

        onDetector.OnPoseDetected.AddListener(OnOnPoseDetected);
        onDetector.WhilePoseDetected.AddListener(OnOnPoseHeld);
        onDetector.OnPoseLost.AddListener(OnOnPoseLost);
    }

    // ───────────── Pointer ON (Off Pose) ─────────────
    public void OnOffPoseDetected()
    {
        pointerActive = true;
        lineRenderer.enabled = true;
        Debug.Log("▶ OffPose Detected → Pointer ON");
    }

    public void OnOffPoseHeld()
    {
        if (pointerActive)
            UpdatePointer();
    }

    public void OnOffPoseLost()
    {
        if (pendingPointerOffCoroutine != null)
            StopCoroutine(pendingPointerOffCoroutine);

        pendingPointerOffCoroutine = StartCoroutine(DelayedPointerOff());
    }

    private IEnumerator DelayedPointerOff()
    {
        yield return new WaitForSeconds(suppressTimeWindow);

        if (suppressPointerOff)
        {
            Debug.Log("⏸ OffPose Lost 무시됨 (OnPose 후 보호)");
            yield break;
        }

        pointerActive = false;
        lineRenderer.enabled = false;
        Debug.Log("✖ OffPose Lost → Pointer OFF");
    }

    // ───────────── Pointer 유지 & Click 처리 (On Pose) ─────────────
    public void OnOnPoseDetected()
    {
        if (!pointerActive)
        {
            pointerActive = true;
            lineRenderer.enabled = true;
            Debug.Log("▶ OnPose Detected → Pointer ON");
        }
    }

    public void OnOnPoseHeld()
    {
        if (pointerActive)
            UpdatePointer();
    }

    public void OnOnPoseLost()
    {
        if (!pointerActive) return;

        suppressPointerOff = true;
        UpdatePointer();   // 마지막 위치 업데이트
        PerformClick();    // 클릭 실행
        Debug.Log("✔ OnPose Lost → Click Triggered");

        if (clearSuppressionCoroutine != null)
            StopCoroutine(clearSuppressionCoroutine);

        clearSuppressionCoroutine = StartCoroutine(ClearSuppressionAfterDelay());
    }

    private IEnumerator ClearSuppressionAfterDelay()
    {
        yield return new WaitForSeconds(suppressTimeWindow + 0.05f);
        suppressPointerOff = false;
        Debug.Log("🟢 suppressPointerOff 해제됨");
    }

    // ───────────── Raycast & Line Update ─────────────
    private void UpdatePointer()
    {
        Vector3 origin = indexTip.position;
        Vector3 dir = indexTip.right; // 필요시 forward로 수정

        lineRenderer.SetPosition(0, origin);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, interactableLayer, QueryTriggerInteraction.Collide))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, origin + dir * maxDistance);
        }
    }

    // ───────────── UI 버튼 클릭 실행 ─────────────
    private void PerformClick()
    {
        Vector3 origin = indexTip.position;
        Vector3 dir = indexTip.right;

        if (!Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, interactableLayer, QueryTriggerInteraction.Collide))
            return;

        GameObject target = hit.collider.gameObject;

        var evtData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
            position = Camera.main.WorldToScreenPoint(hit.point),
            pressPosition = Camera.main.WorldToScreenPoint(hit.point),
            button = PointerEventData.InputButton.Left
        };

        ExecuteEvents.ExecuteHierarchy(target, evtData, ExecuteEvents.pointerClickHandler);

        var btn = target.GetComponent<Button>();
        if (btn != null)
        {
            Debug.Log($"✔ UI Button Clicked: {btn.name}");
            btn.onClick.Invoke();
        }
        else
        {
            Debug.Log($"🟡 Raycast Hit: {target.name}, but not a Button.");
        }
    }
}
