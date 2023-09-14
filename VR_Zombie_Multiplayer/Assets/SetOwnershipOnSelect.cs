using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class SetOwnershipOnSelect : NetworkBehaviour
{

    private XRBaseInteractable interactable;
    public NetworkVariable<bool> isNetworkGrabbed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(x => SetOwnership());
        interactable.selectExited.AddListener(x => Ungrab());

    }

    public void SetOwnership()
    {
        SetOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void Ungrab()
    {
        UngrabServerRpc();
    }

    // ( Called by the Client )
    [ServerRpc(RequireOwnership = false)]
    public void UngrabServerRpc()
    {
        isNetworkGrabbed.Value = true;
    }

    // ( Called by the Client )
    [ServerRpc(RequireOwnership = false)]
    void SetOwnershipServerRpc(ulong id)
    {
        NetworkObject.ChangeOwnership(id);
        isNetworkGrabbed.Value = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
