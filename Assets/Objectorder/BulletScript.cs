using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//bullet Object
public class BulletScript : MonoBehaviour
{
    public string ExpectedObject; // 마지막으로 충돌한 오브젝트 이름 저장
    public Boolean isCollision;
    public ObjectManager goal;
    void Start()
    {
        goal = GameObject.Find("Target").GetComponent<ObjectManager>();
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        
        for (int i = 0; i < goal.Target.Length; i++)
        {
            if (collision.gameObject.name == goal.Target[i].name)
            {
                Debug.Log("총알이 통과한 오브젝트: " + collision.gameObject.name);
                
                ExpectedObject = collision.gameObject.name;
                Debug.Log(ExpectedObject);
                isCollision=true;
            }
        }
        
    }
}