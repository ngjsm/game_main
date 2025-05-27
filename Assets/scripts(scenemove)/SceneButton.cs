using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneButton : MonoBehaviour
{
    public string sceneName;

    [Header("Visual Components")]
    public Renderer boxRenderer;       // 박스 오브젝트의 Renderer
    public Material normalMaterial;    // 기본 색상 메터리얼
    public Material highlightMaterial; // 선택되었을 때 메터리얼

    public TextMeshPro label;          // 텍스트 (자식 오브젝트에 붙여야 함)

    public void Highlight(bool isSelected)
    {
        if (boxRenderer != null)
        {
            boxRenderer.material = isSelected ? highlightMaterial : normalMaterial;
        }

        if (label != null)
        {
            label.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}