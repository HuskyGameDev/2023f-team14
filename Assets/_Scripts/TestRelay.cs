using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestRelay : MonoBehaviour
{

    string userInput;
    string joinCode;
    public TMP_Text joinCodeDisplay;
    //[SerializeField] NetworkManager networkManager;
    private async void Start(){
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public void ReadStringInput(string codeEntry){
        
        userInput = codeEntry;
        if(userInput.Equals("host")){
            
            CreateRelay();
            
        }else{
            JoinRelay(userInput);
        }
    }

    private async void CreateRelay(){
        //Note: the number '3' indicates how many players (NOT INCLUDING THE HOST) can connect to the relay
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join Code: " + joinCode);
            GameObject canvas = GameObject.Find("Canvas");
            if(canvas != null){
                joinCodeDisplay = canvas.transform.Find("HUD_JoinCode").GetComponent<TMP_Text>();
                joinCodeDisplay.text = "JoinCode: " + joinCode;
            }
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }

    }

    private async void JoinRelay(string joinCode){
        try{
            Debug.Log("Joining Relay with code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

       
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
          
        } catch (RelayServiceException e){
            Debug.Log(e);
        }
    }

}
