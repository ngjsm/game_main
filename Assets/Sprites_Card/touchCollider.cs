using UnityEngine;

public class DebugCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TouchCollider hit something: " + other.name);
    }
}