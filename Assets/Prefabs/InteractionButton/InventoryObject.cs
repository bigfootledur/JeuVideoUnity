using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryObject : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber = 0;
    private GameMaster gameMaster;

    void Awake()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            gameMaster.InventoryLeftClick(slotNumber);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            gameMaster.InventoryRightClick(slotNumber);
        }
    }
}
