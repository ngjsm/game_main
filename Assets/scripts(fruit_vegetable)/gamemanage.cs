using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gamemanager2 : MonoBehaviour
{
    public static gamemanager2 Instance { get; private set; }

    [Header("UI (TextMeshPro)")]
    public TMP_Text scoreText;
    public TMP_Text penaltyText;

    private int score = 0;
    private int penalty = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int amt)
    {
        score += amt;
        scoreText.text = $"성공!: {score}";
    }

    public void AddPenalty(int amt)
    {
        penalty += amt;
        penaltyText.text = $"실수...: {penalty}";
    }

    public int GetScore()
    {
        return score;
    }
}