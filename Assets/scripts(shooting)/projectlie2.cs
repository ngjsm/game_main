using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class projectlie2 : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StartButton"))
        {
            gamemanager.Instance?.StartGame();
            Destroy(gameObject);
            return;
        }

        if (!gamemanager.Instance.gameStarted) return;

        if (other.CompareTag("Target"))
        {
            score.Instance?.AddTargetHit();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Fruit"))
        {
            score.Instance?.AddOtherHit();
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}