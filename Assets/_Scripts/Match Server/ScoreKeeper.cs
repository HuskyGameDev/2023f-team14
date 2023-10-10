using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper Instance;
    [SerializeField]
    public GameMode gameMode { get; private set; }
    void Awake()
    {
        if (Instance != null)
        {
            throw new Exception($"Detected more than one instance of {nameof(ScoreKeeper)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}?");
        }
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }


    // override OnDestroy if needed
}