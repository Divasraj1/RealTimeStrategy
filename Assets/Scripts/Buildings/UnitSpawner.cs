using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour,IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TextMeshProUGUI remainingUnitText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }
        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }
    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ProduceUnits()
    {
        if(queuedUnits == 0) { return; }
        unitTimer += Time.deltaTime;
        if(unitTimer < unitSpawnDuration) { return; }
        GameObject unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnoffset = Random.insideUnitSphere * spawnMoveRange;
        spawnoffset.y = unitSpawnPoint.position.y;
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnoffset);

        queuedUnits--;
        unitTimer = 0f;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if(queuedUnits == maxQueue) { return; }
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        if(player.GetResources() < unitPrefab.GetResourceCost()) { return; }
        queuedUnits++;
        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());
    }
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }
        if (!hasAuthority) { return; }
        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int oldQueuedunits,int newQueuedunits)
    {
        remainingUnitText.text = newQueuedunits.ToString();
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;
        if(newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity,0.1f);
        }
    }
    #endregion
}
