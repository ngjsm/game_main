using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] frontSprites;
    public Sprite backSprite;
    public Transform cardParent;

    private List<Card> flippedCards = new List<Card>();
    private bool isProcessing = false;

    public bool IsProcessing() => isProcessing;

    void Start()
    {
        CreateShuffledCards();
    }

    void CreateShuffledCards()
    {
        List<int> cardIDs = new List<int>();
        for (int i = 0; i < 10; i++) // 1~10월, 각 2장씩
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        for (int i = 0; i < cardIDs.Count; i++)
        {
            int rand = Random.Range(0, cardIDs.Count);
            int temp = cardIDs[i];
            cardIDs[i] = cardIDs[rand];
            cardIDs[rand] = temp;
        }

        int columns = 5;
        float spacingX = 0.17f;  // ➔ 카드 가로 간격 더 넓게
        float spacingZ = 0.22f;  // ➔ 카드 세로 간격 더 넓게
        Vector3 center = new Vector3(0f, 0.8f, 2f);  // (중앙 높이 유지)
        int rows = Mathf.CeilToInt(cardIDs.Count / (float)columns);

        for (int i = 0; i < cardIDs.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            float offsetX = (columns - 1) * spacingX / 2f;
            float offsetZ = (rows - 1) * spacingZ / 2f;

            Vector3 pos = new Vector3(col * spacingX - offsetX, 0f, -row * spacingZ + offsetZ) + center;
            GameObject cardObj = Instantiate(cardPrefab, pos, Quaternion.Euler(90, 0, 0), cardParent);
            cardObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f); 

            // 카드 초기화
            Card card = cardObj.GetComponent<Card>();
            card.cardId = cardIDs[i];
            card.cardMonth = (card.cardId % 12) + 1;
            card.manager = this;

            card.front.GetComponent<SpriteRenderer>().sprite = frontSprites[card.cardId];
            card.back.GetComponent<SpriteRenderer>().sprite = backSprite;
            card.ShowBack();
        }
    }


    public void OnCardFlipped(Card card)
    {
        if (isProcessing || flippedCards.Contains(card)) return;

        flippedCards.Add(card);
        if (flippedCards.Count == 2)
            StartCoroutine(CheckMatch());
    }

    IEnumerator CheckMatch()
    {
        isProcessing = true;
        yield return new WaitForSeconds(0.4f);

        var cardA = flippedCards[0];
        var cardB = flippedCards[1];

        if (cardA.cardMonth == cardB.cardMonth)
        {
            cardA.SetMatched();
            cardB.SetMatched();
            Destroy(cardA.gameObject, 0.3f);
            Destroy(cardB.gameObject, 0.3f);
        }
        else
        {
            cardA.Flip();
            cardB.Flip();
        }

        flippedCards.Clear();
        isProcessing = false;
    }
}
