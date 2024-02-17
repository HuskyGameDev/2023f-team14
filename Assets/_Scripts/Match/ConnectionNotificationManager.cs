using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ConnectionNotificationManager : MonoBehaviour
{
    public static ConnectionNotificationManager Instance { get; internal set; }

    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }

    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    private void Awake()
    {
        if (Instance != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(ConnectionNotificationManager)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}?");
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != this)
        {
            return;
        }

        if (NetworkManager.Singleton == null)
        {
            throw new Exception($"There is no {nameof(NetworkManager)} for the {nameof(ConnectionNotificationManager)} to do stuff with! " +
                $"Please add a {nameof(NetworkManager)} to the scene.");
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;
    }

    private void OnClientConnectedCallback(ulong clientid)
    {
        OnClientConnectionNotification?.Invoke(clientid, ConnectionStatus.Connected);

    }
    private void OnClientDisconnectedCallback(ulong clientid)
    {
        OnClientConnectionNotification?.Invoke(clientid, ConnectionStatus.Disconnected);
    }
}
