using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class PhysicalUITrigger : MonoBehaviour
{
    public Button targetButton;
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            isPressed = true;
            ChangeButtonState(targetButton, "Pressed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default") && isPressed)
        {
            isPressed = false;
            ChangeButtonState(targetButton, "Normal");

            if (targetButton != null)
            {
                targetButton.onClick.Invoke();
            }
        }
    }

    private void ChangeButtonState(Button button, string state)
    {
        if (button == null) return;

        var method = typeof(Selectable).GetMethod("DoStateTransition",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (method != null)
        {
            var selectionState = typeof(Selectable)
                .GetNestedType("SelectionState", BindingFlags.NonPublic)
                .GetField(state, BindingFlags.Static | BindingFlags.Public)
                .GetValue(null);

            method.Invoke(button, new object[] { selectionState, false });
        }
    }
}
