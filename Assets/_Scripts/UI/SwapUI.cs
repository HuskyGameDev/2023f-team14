using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class SwapUI : MonoBehaviour
{
    [SerializeField]
    private GameObject options;
    public bool IsOpen { get; private set; }

    void Start()
    {
        options.SetActive(false);
        IsOpen = false;
    }

    public void Open()
    {
        options.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        options.SetActive(false);
        IsOpen = false;
    }
}