using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;
using UnityEngine.Events;

public class startBT : MonoBehaviour
{
    [Tooltip("게임 방법 설명 텍스트 오브젝트")]
    public GameObject instructionText;

    private bool hasGrabbed = false;
    private bool gameStarted = false;

    void Update()
    {
        if (gameStarted) return;

        var provider = FindObjectOfType<LeapProvider>();
        if (provider == null) return;

        Frame frame = provider.CurrentFrame;
        foreach (var hand in frame.Hands)
        {
            float pinch = hand.PinchStrength;

            // 주먹을 쥔 상태로 인식
            if (pinch > 0.8f)
            {
                hasGrabbed = true;
            }

            // 쥐었다가 손을 펴면 게임 시작
            if (hasGrabbed && pinch < 0.2f)
            {
                gameStarted = true;

                if (instructionText != null)
                    instructionText.SetActive(false);

                spawnob.Instance.SpawnRandom();
                Debug.Log("손 제스처로 게임 시작됨");

                break;
            }
        }
    }
}