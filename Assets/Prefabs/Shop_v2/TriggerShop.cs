using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerShop : MonoBehaviour {

    private List<WalkingUnit> nearsWalkingUnits = new List<WalkingUnit>();
    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<WalkingUnit>() && collider.GetComponent<WalkingUnit>().HasInventory())
            NearsWalkingUnits.Add(collider.GetComponent<WalkingUnit>());
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<WalkingUnit>() && collider.GetComponent<WalkingUnit>().HasInventory())
            NearsWalkingUnits.Remove(collider.GetComponent<WalkingUnit>());
    }

    public WalkingUnit GetNearestOne()
    {
        WalkingUnit nearestRTSGameObject = null;
        float minDistance = 9999;
        float distance = 0;

        for (int i = 0; i < NearsWalkingUnits.Count; i++)
        {
            if (NearsWalkingUnits[i] != null)
            {
                distance = Vector3.Distance(transform.position, NearsWalkingUnits[i].gameObject.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestRTSGameObject = NearsWalkingUnits[i];
                }
            }
        }

        return nearestRTSGameObject;
    }

    public List<WalkingUnit> NearsWalkingUnits
    {
        get
        {
            return nearsWalkingUnits;
        }

        set
        {
            nearsWalkingUnits = value;
        }
    }
}
