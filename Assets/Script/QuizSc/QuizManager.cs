using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizItem
    {
        public GameObject objectPrefab;
        public string displayNameKorean;
        public bool rotateOnY;
    }

    [Header("퀴즈 설정")]
    public QuizItem[] quizItems;
    public Transform objectSpawnPoint;

    [Header("UI 설정")]
    public Transform buttonContainer;
    public Button optionButtonPrefab;
    public TextMeshProUGUI feedbackText;

    [Range(3, 4)] public int buttonCount = 3;
    public float nextDelay = 2f;

    private GameObject currentObject;
    private string correctAnswer;
    private List<int> questionQueue;
    private int correctCount = 0;
    private int totalAnswered = 0;
    private bool inputLocked = false; // ✅ 입력 중복 방지용

    void Start()
    {
        questionQueue = Enumerable.Range(0, quizItems.Length).OrderBy(_ => Random.value).ToList();
        ShowNextQuestion();
    }

    void ShowNextQuestion()
    {
        inputLocked = false; // ✅ 추가: 잠금 해제

        if (currentObject != null) Destroy(currentObject);
        foreach (Transform t in buttonContainer)
            Destroy(t.gameObject);
        feedbackText.text = "";

        if (questionQueue.Count == 0)
        {
            feedbackText.text = $"퀴즈 완료!\n정답 수: {correctCount}/{quizItems.Length}";
            Invoke(nameof(LoadResultScene), 5f);
            return;
        }

        int quizIndex = questionQueue[0];
        questionQueue.RemoveAt(0);
        QuizItem item = quizItems[quizIndex];
        correctAnswer = item.displayNameKorean;
        totalAnswered++;

        Quaternion rot = item.rotateOnY ? Quaternion.Euler(0, 90f, 0) : Quaternion.identity;
        currentObject = Instantiate(item.objectPrefab, objectSpawnPoint.position, rot);

        List<string> options = new List<string> { correctAnswer };
        while (options.Count < buttonCount)
        {
            string wrong = quizItems[Random.Range(0, quizItems.Length)].displayNameKorean;
            if (!options.Contains(wrong)) options.Add(wrong);
        }
        options = options.OrderBy(_ => Random.value).ToList();

        foreach (string opt in options)
        {
            Button btn = Instantiate(optionButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt;
            btn.onClick.RemoveAllListeners();
            string choice = opt;
            btn.onClick.AddListener(() => OnOptionSelected(choice));

            // 손 충돌용 트리거 자동 부착
            PhysicalUITrigger trigger = btn.gameObject.AddComponent<PhysicalUITrigger>();
            trigger.targetButton = btn;
        }
    }
    public void OnOptionSelected(string selected)
    {
        if (inputLocked) return;         // ✅ 중복 입력 방지
        inputLocked = true;              // ✅ 잠금 활성화

        if (selected == correctAnswer)
        {
            correctCount++;
            feedbackText.text = "<color=green>정답입니다!</color>";
        }
        else
        {
            feedbackText.text = "<color=orange>아쉬워요... \n다음에는 잘 하실 수 있어요</color>";
        }

        Invoke(nameof(ShowNextQuestion), nextDelay);
    }
    void LoadResultScene()
    {
        SceneManager.LoadScene("scene_selector");
    }
}
