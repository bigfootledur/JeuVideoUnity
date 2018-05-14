using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreateCollector : MonoBehaviour    
{
    public Canvas canvas;
    public GameObject createCollectorButtonPrefab;
    public GameObject cancelCreateCollectorButtonPrefab;

    private GameObject createCollectorButton;
    private GameObject cancelCreateCollectorButton;

    private Townhall townhall;

    private bool pointerEntered = false;
    
    void Awake()
    {
        if (!canvas)
            canvas = FindObjectOfType<Canvas>();

        townhall = GetComponent<Townhall>();
    }

    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit output;
        if (Physics.Raycast(ray, out output, 1000))
        {
            if (output.collider.gameObject.Equals(gameObject) && output.collider.GetComponent<Townhall>().faction.Equals(GameMaster.PlayerFaction))
            {

                if (!createCollectorButton)
                {
                    createCollectorButton = Instantiate(createCollectorButtonPrefab,
                                                        Camera.main.WorldToScreenPoint(transform.position) + Vector3.left * 20, Quaternion.identity, canvas.transform);
                    createCollectorButton.GetComponentInChildren<CreateCollectorButton>().Townhall = townhall;
                }

                if (!cancelCreateCollectorButton)
                {
                    cancelCreateCollectorButton = Instantiate(cancelCreateCollectorButtonPrefab,
                                                              Camera.main.WorldToScreenPoint(transform.position) + Vector3.right * 20,
                                                              Quaternion.identity, canvas.transform);
                    cancelCreateCollectorButton.GetComponentInChildren<CreateCollectorButton>().Townhall = townhall;
                }
                pointerEntered = true;
            }
            else if (pointerEntered)
            {
                Destroy(createCollectorButton);
                Destroy(cancelCreateCollectorButton);
                pointerEntered = false;
            }
        }


    }

    //public void CreateNewCollector()
    //{
    //    print(townhall);
    //    townhall.CreateCollector();
    //}

    //public void CancelCreateCollector()
    //{
    //    townhall.CancelCreateCollector();
    //}

}
