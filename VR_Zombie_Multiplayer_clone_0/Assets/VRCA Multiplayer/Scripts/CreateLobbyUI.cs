using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateLobbyUI : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Slider maxPlayerSlider;
    public Button createLobbyButton;
    public TMP_Dropdown gameModeDropDown;

    // Start is called before the first frame update
    void Start()
    {
        createLobbyButton.onClick.AddListener(CreateLobbyFromUI);
    }

    private void Update()
    {
        createLobbyButton.gameObject.SetActive(nameInputField.text != "");
    }

    public void CreateLobbyFromUI()
    {
        LobbyManager.LobbyData lobbyData = new LobbyManager.LobbyData();
        lobbyData.maxPlayer = (int)maxPlayerSlider.value;
        lobbyData.lobbyName = nameInputField.text;
        lobbyData.gameMode = gameModeDropDown.options[gameModeDropDown.value].text;

        LobbyManager.Instance.CreateLobby(lobbyData);
    }
}
