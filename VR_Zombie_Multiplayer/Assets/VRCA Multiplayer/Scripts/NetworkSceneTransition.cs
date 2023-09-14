using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneTransition : MonoBehaviour
{
    private bool isLoading = false;
    public static NetworkSceneTransition Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += ServerStarted;
    }

    private void ServerStarted()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        isLoading = false;
    }

    public void LoadSceneForEverybody(string sceneName)
    {
        if(!isLoading)
        {
            isLoading = true;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
