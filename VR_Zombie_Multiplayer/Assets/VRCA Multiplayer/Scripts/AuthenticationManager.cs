using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class AuthenticationManager : MonoBehaviour
{
    private void Awake()
    {
        Login();
    }

    public async void Login()
    {
        InitializationOptions options = new InitializationOptions();

#if UNITY_EDITOR

        if (ClonesManager.IsClone())
            options.SetProfile(ClonesManager.GetArgument());
        else
            options.SetProfile("primary");

#endif


        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        VivoxService.Instance.Initialize();
        VivoxVoiceManager.Instance.Login();
    }
}
