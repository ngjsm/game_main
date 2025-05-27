using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// 립모션 손가락(또는 Cube) Collider가 닿으면 콜백을 호출하는 버튼 컴포넌트
/// Layer 검사 + Tag 검사 둘 다 지원합니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class QuizButton : MonoBehaviour
{
    [Tooltip("손가락으로 쓰이는 레이어만 체크할 마스크\n다른 레이어는 무시됩니다.")]
    [SerializeField] private LayerMask fingerLayerMask;

    private string answerText;
    private Action<string> callback;

    /// <summary>
    /// 외부에서 보기 텍스트와 콜백을 할당합니다.
    /// </summary>
    public void SetAnswer(string answer, Action<string> onClick)
    {
        answerText = answer;
        callback = onClick;
    }

    private void Awake()
    {
        // Collider 는 Trigger 로 설정되어 있어야 합니다.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1) 태그 검사: 기존 방식 지원
        bool isFingerTag = other.CompareTag("Finger");

        // 2) 레이어 검사: LayerMask 에 포함된 레이어인지 확인
        bool isFingerLayer = (fingerLayerMask.value & (1 << other.gameObject.layer)) != 0;

        if (isFingerTag || isFingerLayer)
        {
            callback?.Invoke(answerText);
        }
    }
}
