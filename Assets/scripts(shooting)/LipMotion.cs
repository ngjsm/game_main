using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipMotion : MonoBehaviour
{
    public delegate void LipAction();
    public static event LipAction OnLipOpen;

    void Update()
    {
        if (DetectLipOpenGesture())
        {
            OnLipOpen?.Invoke();
        }
    }
    bool DetectLipOpenGesture()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
