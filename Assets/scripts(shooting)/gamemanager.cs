using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemanager : MonoBehaviour
{
    public static gamemanager Instance;

    public bool gameStarted = false;
    public GameObject startButton;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartGame()
    {
        gameStarted = true;
        if (startButton != null)
            startButton.SetActive(false);
        Debug.Log("∞‘¿” Ω√¿€µ !");
    }

    public void ResetGame()
    {
        gameStarted = false;
        if (startButton != null)
            startButton.SetActive(true);
        Debug.Log("∞‘¿” ∏Æº¬µ !");
    }
}
