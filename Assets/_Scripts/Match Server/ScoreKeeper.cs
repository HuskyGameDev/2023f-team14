using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper Instance;
    [SerializeField]
    public GameMode gameMode;
    [SerializeField]
    private Button startButton;
    void Awake()
    {
        if (Instance != null)
        {
            throw new Exception($"Detected more than one instance of {nameof(ScoreKeeper)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}?");
        }
        Instance = this;

        startButton.onClick.AddListener(() =>
        {
            InitializeScene();
        });
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void InitializeScene()
    {
        Debug.Log("Initializing");
        gameMode.Initialize();
    }
    // override OnDestroy if needed
}