using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resoucesText = null;
    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        ClientHandleResourcesUpdated(player.GetResources());
        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }
  
    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resoucesText.text = $"Resources : {resources}";   
    }
}
