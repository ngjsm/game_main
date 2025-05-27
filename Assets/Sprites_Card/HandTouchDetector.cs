using UnityEngine;

public class HandTouchDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Card"))
        {
            Card card = other.GetComponent<Card>();
            if (card != null)
            {
                card.TryFlip();
            }
        }
    }
}


