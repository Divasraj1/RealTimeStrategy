using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TextMeshProUGUI priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera maincamera;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

   

    private void Start()
    {
        maincamera = Camera.main;
        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
    }
    private void Update()
    {
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        
    }
    private void LateUpdate()
    {
        if (buildingPreviewInstance == null) { return; }
        UpdateBuildingPreview();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

         
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(buildingPreviewInstance == null) { return; }
        Ray ray = maincamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            //place building
        }
        buildingPreviewInstance.SetActive(false);
        Destroy(buildingPreviewInstance);
    }
    public void UpdateBuildingPreview()
    {
        Ray ray = maincamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if(!Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }
        buildingPreviewInstance.transform.position = hit.point;

        if (buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
    }
}