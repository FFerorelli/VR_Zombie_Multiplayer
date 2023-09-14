using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour
{
    public GameObject authentication;
    public GameObject lobbyMenu;
    public GameObject createLobby;
    public GameObject lobbyList;
    public GameObject insideLobby;
    public GameObject loading;

    public Button quickJoinButton;
    public Button createLobbyButton;
    public Button lobbyListButton;

    public void UIEnabler(int index)
    {
        GameObject[] uiElements = new GameObject[] { lobbyMenu, createLobby, lobbyList, authentication, insideLobby, loading };

        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].SetActive(i == index);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UIEnabler(3);

        AuthenticationService.Instance.SignedIn += () => UIEnabler(0);

        quickJoinButton.onClick.AddListener(() => LobbyManager.Instance.QuickJoinLobby());
        createLobbyButton.onClick.AddListener(() => UIEnabler(1));
        lobbyListButton.onClick.AddListener(() => UIEnabler(2));

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        LobbyManager.Instance.OnStartJoinLobby.AddListener(() => UIEnabler(5));
        LobbyManager.Instance.OnFailedJoinLobby.AddListener(() => UIEnabler(0));
    }


    private void OnClientDisconnectCallback(ulong obj)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            LeaveLobbyUI();
        }
    }

    public void LeaveLobbyUI()
    {
        UIEnabler(0);
        LobbyManager.Instance.LeaveLobbyAsync();
    }

    private void OnClientConnected(ulong obj)
    {
        if(obj == NetworkManager.Singleton.LocalClientId)
        {
            UIEnabler(4);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
