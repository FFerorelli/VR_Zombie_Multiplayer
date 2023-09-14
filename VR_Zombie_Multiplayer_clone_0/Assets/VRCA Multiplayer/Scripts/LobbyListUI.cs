using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyListUI : MonoBehaviour
{
    public Transform contentParent;
    public LobbyListElement lobbyListElementPrefab;
    public float refreshTime = 2;
    private float timer = 0;

    public async void UpdateLobbyList()
    {
        QueryLobbiesOptions queryOptions = new QueryLobbiesOptions();

        queryOptions.Count = 10;
        queryOptions.Order = new List<QueryOrder>();
        QueryOrder byNewOrder = new QueryOrder(false, QueryOrder.FieldOptions.Created);

        queryOptions.Order.Add(byNewOrder);

        queryOptions.Filters = new List<QueryFilter>();
        QueryFilter available = new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT);
        QueryFilter nonLocked = new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ);

        queryOptions.Filters.Add(available);
        queryOptions.Filters.Add(nonLocked);

        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);

        for (int i = 0; i < contentParent.childCount; i++)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        foreach (var lobby in response.Results)
        {
            LobbyListElement spawnElement = Instantiate(lobbyListElementPrefab, contentParent);
            spawnElement.Initialize(lobby);
        }
    }

    private void OnEnable()
    {
        UpdateLobbyList();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > refreshTime)
        {
            UpdateLobbyList();

            timer -= refreshTime;
        }

        timer += Time.deltaTime;
    }
}
