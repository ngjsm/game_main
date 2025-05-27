using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceManager : MonoBehaviour
{
    [System.Serializable]
    public class Clue
    {
        public string placeName;     // 장소명 (예: 병원)
        public Sprite clueImage;     // 단서 이미지 (예: 청진기)
    }

    [System.Serializable]
    public class ProblemItem
    {
        public string questionText;  // 문제 질문 (예: 이 물건은 어디에서 쓰일까요?)
        public Clue[] clues;         // 보기 구성용 장소+이미지 목록
    }

    [Header("문제 리스트")]
    public ProblemItem[] problems;

    [Header("UI 구성")]
    public Image clueImageUI;                     // 단서 이미지 표시
    public TextMeshProUGUI questionTextUI;        // 질문 텍스트
    public Transform buttonContainer;             // 버튼을 배치할 부모
    public Button optionButtonPrefab;             // 보기 버튼 프리팹
    public TextMeshProUGUI feedbackText;          // 피드백 출력

    [Range(2, 4)] public int buttonCount = 3;
    public float nextDelay = 2f;

    private int currentIndex = 0;
    private Clue correctClue;
    private bool inputLocked = false;

    void Start()
    {
        currentIndex = 0;
        ShowNextQuestion();
    }

    void ShowNextQuestion()
    {
        inputLocked = false;
        feedbackText.text = "";

        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        if (currentIndex >= problems.Length)
        {
            questionTextUI.text = "";  // ❗ 아무 것도 띄우지 않음
            clueImageUI.enabled = false;
            feedbackText.text = "<color=green>퀴즈가 모두 끝났어요!\n수고하셨습니다.</color>";
            return;
        }

        ProblemItem problem = problems[currentIndex];
        currentIndex++;

        // 문제 질문 표시
        questionTextUI.text = problem.questionText;

        // 정답 clue 1개 랜덤 선택
        correctClue = problem.clues[Random.Range(0, problem.clues.Length)];
        clueImageUI.sprite = correctClue.clueImage;

        // 정답과 오답 보기 생성
        List<string> options = new List<string> { correctClue.placeName };

        List<string> distractors = problem.clues
            .Where(c => c.placeName != correctClue.placeName)
            .Select(c => c.placeName)
            .Distinct()
            .OrderBy(_ => Random.value)
            .ToList();

        while (options.Count < buttonCount && distractors.Count > 0)
        {
            options.Add(distractors[0]);
            distractors.RemoveAt(0);
        }

        options = options.OrderBy(_ => Random.value).ToList();

        // 보기 버튼 생성
        foreach (string opt in options)
        {
            Button btn = Instantiate(optionButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnOptionSelected(opt));
        }
    }

    void OnOptionSelected(string selected)
    {
        if (inputLocked) return;
        inputLocked = true;

        if (selected == correctClue.placeName)
        {
            feedbackText.text = "<color=green>정답입니다!</color>";
        }
        else
        {
            feedbackText.text = "<color=orange>조금 아쉬워요.\n다음 문제로 넘어갈게요.</color>";
        }

        Invoke(nameof(ShowNextQuestion), nextDelay);
    }
}
