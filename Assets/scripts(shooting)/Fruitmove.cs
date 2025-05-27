using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruitmove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Vector3 moveDirection = Vector3.forward;
    public float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}