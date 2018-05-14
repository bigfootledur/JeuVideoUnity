using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreateCollectorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject buttonDescription;
    private Townhall townhall;

    public void CreateCollector()
    {
        Townhall.CreateCollector();
    }

    public void CancelCreateCollector()
    {
        Townhall.CancelCreateCollector();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonDescription)
        {
            buttonDescription.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonDescription)
        {
            buttonDescription.SetActive(false);
        }
    }

    #region Getters/Setters
    public Townhall Townhall
    {
        get
        {
            return townhall;
        }

        set
        {
            townhall = value;
        }
    }
    #endregion
}
