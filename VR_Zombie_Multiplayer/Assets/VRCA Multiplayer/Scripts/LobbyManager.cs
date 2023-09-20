using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Netcode;
using UnityEngine.Events;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    private float heartBeatTimer = 0;
    private float updateLobbyTimer = 0;
    private Lobby currentLobby;

    public UnityEvent OnStartJoinLobby;
    public UnityEvent OnFailedJoinLobby;
    public UnityEvent OnFinishJoinLobby;
    public UnityEvent OnLeaveLobby;

    private bool hasPlayerDataToUpdate = false;
    private Dictionary<string, PlayerDataObject> newPlayerData;

    public Lobby CurrentLobby { get => currentLobby; }

    private void Awake()
    {
        Instance = this;

        OnFinishJoinLobby.AddListener(JoinVivoxChannel);
        OnLeaveLobby.AddListener(LeaveVivoxChannel);
    }

    public void JoinVivoxChannel()
    {
        VivoxVoiceManager.Instance.JoinChannel(currentLobby.Id, VivoxUnity.ChannelType.NonPositional, VivoxVoiceManager.ChatCapability.AudioOnly);
    }
    public void LeaveVivoxChannel()
    {
        VivoxVoiceManager.Instance.DisconnectAllChannels();
    }

    public struct LobbyData
    {
        public string lobbyName;
        public int maxPlayer;
        public string gameMode;
    }

    public async void UpdatePlayer(Dictionary<string,PlayerDataObject> data)
    {
        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions();
        updateOptions.Data = data;
        currentLobby = await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId, updateOptions);
    }

    public async void LockLobby()
    {
        currentLobby = await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
    }

    public async void LeaveLobbyAsync()
    {
        if(NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown();
        }

        if(currentLobby != null)
        {
            string id = currentLobby.Id;
            currentLobby = null;
            await Lobbies.Instance.RemovePlayerAsync(id, AuthenticationService.Instance.PlayerId);

            OnLeaveLobby.Invoke();
        }
    }

    public void UpdatePlayerData(Dictionary<string, PlayerDataObject> data)
    {
        newPlayerData = data;
        hasPlayerDataToUpdate = true;
    }

    public async void CreateLobby(LobbyData lobbyData)
    {
        OnStartJoinLobby.Invoke();

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>();

            string joinCode = await RelayManager.Instance.CreateRelayGame(lobbyData.maxPlayer);

            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, joinCode);
            lobbyOptions.Data.Add("Join Code Key", dataObject);

            DataObject gameDataObject = new DataObject(DataObject.VisibilityOptions.Public, lobbyData.gameMode);
            lobbyOptions.Data.Add("Game Mode", gameDataObject);

            currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyData.lobbyName, lobbyData.maxPlayer, lobbyOptions);

            OnFinishJoinLobby.Invoke();
        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoinLobby.Invoke();
        }
    }

    public async void QuickJoinLobby()
    {
        OnStartJoinLobby.Invoke();

        try
        {
            currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = currentLobby.Data["Join Code Key"].Value;

            RelayManager.Instance.JoinRelayGame(relayJoinCode);

            OnFinishJoinLobby.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoinLobby.Invoke();
        }
    }

    public async void JoinLobby(string lobbyId)
    {
        OnStartJoinLobby.Invoke();

        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = currentLobby.Data["Join Code Key"].Value;

            RelayManager.Instance.JoinRelayGame(relayJoinCode);

            OnFinishJoinLobby.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoinLobby.Invoke();
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    async void Update()
    {
        if(heartBeatTimer > 15)
        {
            heartBeatTimer -= 15;
            if(currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }

        heartBeatTimer += Time.deltaTime;

        if(updateLobbyTimer > 1.5f)
        {
            updateLobbyTimer -= 1.5f;
            if (currentLobby != null)
            {
                if(hasPlayerDataToUpdate)
                {
                    UpdatePlayer(newPlayerData);
                    hasPlayerDataToUpdate = false;
                }
                else
                {
                    currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                }
            }
        }

        updateLobbyTimer += Time.deltaTime;
    }
}
