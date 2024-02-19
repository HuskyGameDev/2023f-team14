using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletHoleSprite : MonoBehaviour
{
    private float assignedTime = 0f;
    private float endTime = 0f;
    private GameObject prefab;
    public void SetReleaseTimeout(TimeSpan time, GameObject prefab)
    {
        assignedTime = Time.time;
        endTime = assignedTime + (float)time.TotalSeconds;

        this.prefab = prefab;
    }

    void Update()
    {
        if (assignedTime == 0f) return;
        if (endTime < Time.time)
        {
            assignedTime = endTime = 0f;
            GameObjectPool.Instance.ReturnObject(gameObject, prefab);
        }
    }

}
