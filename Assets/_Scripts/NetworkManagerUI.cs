using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button ServerButton;
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button DCButton;

    private void Awake()
    {
        DCButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            ChangeUI(true);
        });
        ServerButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            ChangeUI(false);
        });
        HostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            ChangeUI(false);
        });
        ClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            ChangeUI(false);
        });

        ChangeUI(true);
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientStopped += (b) => ChangeUI(true);
    }

    private void ChangeUI(bool c)
    {
        ServerButton.gameObject.SetActive(c);
        HostButton.gameObject.SetActive(c);
        ClientButton.gameObject.SetActive(c);
        DCButton.gameObject.SetActive(!c);
    }
}
