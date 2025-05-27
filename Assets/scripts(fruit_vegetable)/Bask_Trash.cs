using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bask_Trash : MonoBehaviour
{
    [Header("Bin Settings")]
    [Tooltip("과일 바구니인지 여부 (false면 쓰레기통)")]
    [SerializeField] private bool isFruitBin = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody) return;

        if (spawnob.Instance.IsGameEnded()) return;

        bool correct = (isFruitBin && other.CompareTag("Fruit"))
                    || (!isFruitBin && other.CompareTag("Trash"));

        if (correct)
            gamemanager2.Instance.AddScore(1);
        else
            gamemanager2.Instance.AddPenalty(1);

        other.gameObject.SetActive(false);

        spawnob.Instance.OnObjectPlaced();
    }
}
