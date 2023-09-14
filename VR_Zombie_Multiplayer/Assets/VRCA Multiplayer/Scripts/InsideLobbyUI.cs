using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using TMPro;

public class InsideLobbyUI : MonoBehaviour
{
    public UnityEngine.UI.Toggle isReadyToggle;
    public TextMeshProUGUI playersInside;

    // Start is called before the first frame update
    void Start()
    {
        isReadyToggle.onValueChanged.AddListener(SetReady);
    }


    public void SetReady(bool isReady)
    {
        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;

        if(currentLobby != null)
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            Player myPlayer = currentLobby.Players.Find(x => x.Id == playerId);

            if(myPlayer != null)
            {
                if(myPlayer.Data ==null)
                {
                    myPlayer.Data = new Dictionary<string, PlayerDataObject>();
                }

                PlayerDataObject isReadyData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isReady ? "yes" : "no");

                if(myPlayer.Data.ContainsKey("isReady"))
                {
                    myPlayer.Data["isReady"] = isReadyData;
                }
                else
                {
                    myPlayer.Data.Add("isReady", isReadyData);
                }

                LobbyManager.Instance.UpdatePlayerData(myPlayer.Data);
            }
        }
    }

    private void Update()
    {
        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;

        if(currentLobby == null)
        {
            playersInside.text = "0/0";
            return;
        }

        int numberOfReady = GetNumberOfReady();
        playersInside.text = numberOfReady + "/" + currentLobby.Players.Count;

        if(currentLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            if (numberOfReady == currentLobby.Players.Count)
            {
                LobbyManager.Instance.LockLobby();
                NetworkSceneTransition.Instance.LoadSceneForEverybody(currentLobby.Data["Game Mode"].Value);
            }
        }
    }

    public int GetNumberOfReady()
    {
        int numberOfReady = 0;

        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;

        foreach (var item in currentLobby.Players)
        {
            if (item.Data != null && item.Data.ContainsKey("isReady") && item.Data["isReady"].Value == "yes")
                numberOfReady += 1;
        }

        return numberOfReady;
    }
}
