using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class spawnob : MonoBehaviour
{
    public static spawnob Instance { get; private set; }

    [Header("Test Controls")]
    [SerializeField] private bool enableKeyboardControl = false;

    [Header("Spawn Settings")]
    [Tooltip("생성할 과일 프리팹 목록")]
    [SerializeField] private GameObject[] fruitPrefabs;
    [Tooltip("생성할 쓰레기 프리팹 목록")]
    [SerializeField] private GameObject[] trashPrefabs;

    [Header("Probability")]
    [Range(0f, 1f), Tooltip("과일 스폰 확률")]
    [SerializeField] private float fruitSpawnProbability = 0.7f;

    [Header("Spawn Area")]
    [Tooltip("스폰 반경 (m)")]
    [SerializeField] private float spawnRadius = 0.5f;

    [Header("게임 종료 UI")]
    [Tooltip("게임 종료 텍스트가 붙은 오브젝트")]
    [SerializeField] private GameObject endTextObject;
    [Tooltip("게임 종료 결과를 표시할 TextMeshPro")]
    [SerializeField] private TextMeshProUGUI endText;

    private Transform spawnPoint;
    private GameObject currentObject;

    private int placedCount = 0;
    private const int maxPlaceCount = 10;
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        spawnPoint = transform;
    }

    public void SpawnRandom()
    {


        if ((fruitPrefabs == null || fruitPrefabs.Length == 0) ||
            (trashPrefabs == null || trashPrefabs.Length == 0))
        {
            Debug.LogWarning("[spawnob] 프리팹이 비어있습니다.");
            return;
        }

        bool isFruit = Random.value < fruitSpawnProbability;
        var pool = isFruit ? fruitPrefabs : trashPrefabs;
        var prefab = pool[Random.Range(0, pool.Length)];

        var offset = Random.insideUnitCircle * spawnRadius;
        var pos = spawnPoint.position + new Vector3(offset.x, 0f, offset.y);

        currentObject = Instantiate(prefab, pos, Quaternion.identity);

        if (enableKeyboardControl)
        {
            currentObject.AddComponent<keyboardtest>();
        }
    }

    public void OnObjectPlaced()
    {
        currentObject = null;
        placedCount++;

        if (placedCount >= maxPlaceCount)
        {
            EndGame();
        }
        else
        {
            SpawnRandom();
        }
    }

    private void EndGame()
    {
        gameEnded = true;

        if (endTextObject != null)
            endTextObject.SetActive(true);

        if (endText != null)
        {
            int score = gamemanager2.Instance.GetScore();
            endText.text = $"게임 종료!\n점수: {score} / {maxPlaceCount}";
        }

        Debug.Log("게임 종료됨");

        StartCoroutine(LoadMainScene());
    }
    private IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(5f);  // 5초 대기
        SceneManager.LoadScene("scene_selector");  // 메인 씬 이름으로 변경
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }

}