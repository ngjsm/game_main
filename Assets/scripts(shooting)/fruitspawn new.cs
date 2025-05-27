using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class fruitspawnnew : MonoBehaviour
{
    public GameObject[] fruitPrefabs;
    public float spawnInterval = 2f;
    public float xRange = 8f;
    public float yRange = 4f;
    public Vector3 spawnPoint;

    private float spawnDuration = 30f;
    private GameObject targetFruitPrefab;

    private bool hasStarted = false;

    void Update()
    {
        if (!gamemanager.Instance.gameStarted || hasStarted)
            return;

        hasStarted = true;

        int targetIndex = Random.Range(0, fruitPrefabs.Length);
        targetFruitPrefab = fruitPrefabs[targetIndex];
        score.Instance?.SetTargetName(targetFruitPrefab.name);

        InvokeRepeating("SpawnFruit", 1f, spawnInterval);
        Invoke("StopSpawning", spawnDuration);
    }

    void SpawnFruit()
    {
        int index = Random.Range(0, fruitPrefabs.Length);
        Vector3 spawnPos = spawnPoint + new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), 0);
        GameObject fruit = Instantiate(fruitPrefabs[index], spawnPos, Quaternion.identity);

        if (fruitPrefabs[index].name == targetFruitPrefab.name)
            fruit.tag = "Target";
        else
            fruit.tag = "Fruit";
    }

    void StopSpawning()
    {
        CancelInvoke("SpawnFruit");
        Debug.Log("게임 종료!");

        // 종료 UI 출력 or 씬 전환 등
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        int finalscore = score.Instance.targetHit;
        Debug.Log($"최종 점수: {finalscore}");


        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("scene_selector");
    }
}