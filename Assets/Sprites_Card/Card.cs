using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject front;
    public GameObject back;
    public int cardId;
    public int cardMonth;
    public CardManager manager;

    private bool isFlipped = false;
    private bool isMatched = false;
    public bool useManager = true;

    public void TryFlip()
    {
        if (isMatched || isFlipped || (useManager && manager.IsProcessing()))
            return;

        Flip();

        if (useManager)
            manager.OnCardFlipped(this);
    }

    public void Flip()
    {
        isFlipped = !isFlipped;

        if (isFlipped)
            ShowFront();
        else
            ShowBack();
    }

    public void ShowFront()
    {
        front.SetActive(true);
        back.SetActive(false);
    }

    public void ShowBack()
    {
        front.SetActive(false);
        back.SetActive(true);
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public bool IsMatched()
    {
        return isMatched;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter with: " + other.name);

        // Tag 검사 없이 바로 카드 뒤집기
        TryFlip();
    }
}
