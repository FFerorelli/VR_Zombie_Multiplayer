using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class NetworkAvatar : NetworkBehaviour
{
    public GameObject head;
    public GameObject namePlate;

    public GameObject[] headParts;
    public GameObject[] bodyParts;
    public Renderer[] skinParts;
    public Gradient skinGradient;
    public TMPro.TextMeshPro playerName;

    public NetworkVariable<NetworkAvatarData> networkAvatarData = new NetworkVariable<NetworkAvatarData>(
        new NetworkAvatarData(),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public struct NetworkAvatarData : INetworkSerializable
    {
        public int headIndex;
        public int bodyIndex;
        public float skinColor;
        public FixedString128Bytes avatarName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref headIndex);
            serializer.SerializeValue(ref bodyIndex);
            serializer.SerializeValue(ref skinColor);
            serializer.SerializeValue(ref avatarName);
        }
    }

    public NetworkAvatarData GenerateRandom()
    {
        int randomHeadIndex = Random.Range(0, headParts.Length);
        int randomBodyIndex = Random.Range(0, bodyParts.Length);
        float randomSkinColor = Random.Range((float)0, (float) 1);

        string avatarName = "Player " + NetworkManager.Singleton.LocalClientId.ToString();

        NetworkAvatarData randomData = new NetworkAvatarData
        {
            headIndex = randomHeadIndex,
            bodyIndex = randomBodyIndex,
            skinColor = randomSkinColor,
            avatarName = avatarName,
        };

        return randomData;
    }

    public void UpdateAvatarFromData(NetworkAvatarData newData)
    {
        for (int i = 0; i < headParts.Length; i++)
        {
            headParts[i].SetActive(i == newData.headIndex);
        }

        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].SetActive(i == newData.bodyIndex);
        }

        foreach (var item in skinParts)
        {
            item.material.color = skinGradient.Evaluate(newData.skinColor);
        }

        playerName.text = newData.avatarName.ToString();
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            networkAvatarData.Value = GenerateRandom();
            head.layer = 7;
            namePlate.SetActive(false);

            AvatarSelectionUI.Singleton.Initialize(this);
        }

        UpdateAvatarFromData(networkAvatarData.Value);

        networkAvatarData.OnValueChanged += (x, y) => UpdateAvatarFromData(y);
    }
}
