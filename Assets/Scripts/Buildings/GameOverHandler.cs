using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<string> ClientOnGameOver;
    private List<UnitBase> bases = new List<UnitBase>();
    #region server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        if(bases.Count != 1)
        {
            return;
        }
        int playerId = bases[0].connectionToClient.connectionId;
        RpcGameOver($"Player {playerId}");
    }
    #endregion

    #region client
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }
    #endregion
}
