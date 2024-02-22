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
        //ChangeUI(true);
    }

    void Start()
    {

        NetworkManager.Singleton.OnClientStopped += (b) => ChangeUI(true);
    }

    private void ChangeUI(bool c)
    {
        //disable buttons for relay
        ServerButton.gameObject.SetActive(!c);
        HostButton.gameObject.SetActive(!c);
        ClientButton.gameObject.SetActive(!c);
        DCButton.gameObject.SetActive(!c);
    }
}
