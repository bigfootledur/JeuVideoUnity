using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopObject : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image imageDisplay;

    public int slotNumber = 0;
    private GameMaster gameMaster;

    void Awake()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            gameMaster.ShopLeftClick(slotNumber);
        }
    }

    public Image ImageDisplay
    {
        get
        {
            return imageDisplay;
        }

        set
        {
            imageDisplay = value;
        }
    }
}
