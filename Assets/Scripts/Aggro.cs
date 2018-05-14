using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro : MonoBehaviour {

    private List<Unit> nearsRTSGameObjects;
    private List<Unit> nearFriendliesUnit;

    void Awake()
    {
        NearsRTSGameObjects = new List<Unit>();
        NearFriendliesUnit = new List<Unit>();
    }

    void Update()
    {
        for (int i = 0; i < NearFriendliesUnit.Count; i++)
            if (NearFriendliesUnit[i] == null)
                NearFriendliesUnit.RemoveAt(i);

        for (int i = 0; i < NearsRTSGameObjects.Count; i++)
            if (NearsRTSGameObjects[i] == null)
                NearsRTSGameObjects.RemoveAt(i);

        this.GetComponentInParent<UnitAttack>().Aggro(AggroNearestOne(NearsRTSGameObjects));
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Unit>() && !collider.GetComponent<Collectable>() &&
            !collider.GetComponent<Unit>().faction.Equals(this.GetComponentInParent<Unit>().faction))
        {
            NearsRTSGameObjects.Add(collider.GetComponent<Unit>());
        }

        if (collider.GetComponent<WalkingUnit>() &&
            collider.GetComponent<WalkingUnit>().faction.Equals(this.GetComponentInParent<WalkingUnit>().faction))
            NearFriendliesUnit.Add(collider.GetComponent<WalkingUnit>());
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Unit>() && !collider.GetComponent<Collectable>() &&
            !collider.GetComponent<Unit>().faction.Equals(this.GetComponentInParent<Unit>().faction))
        {
            NearsRTSGameObjects.Remove(collider.GetComponent<Unit>());
        }

        if (collider.GetComponent<WalkingUnit>() &&
            collider.GetComponent<WalkingUnit>().faction.Equals(this.GetComponentInParent<WalkingUnit>().faction))
        {
            NearFriendliesUnit.Remove(collider.GetComponent<WalkingUnit>());
        }
    }

    private Unit AggroNearestOne(List<Unit> rTSGameObjects)
    {
        Unit nearestRTSGameObject = null;
        float minDistance = 9999;
        float distance = 0;

        for (int i = 0; i < rTSGameObjects.Count; i++)
        {
            if (rTSGameObjects[i] != null)
            {
                distance = Vector3.Distance(transform.position, rTSGameObjects[i].gameObject.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestRTSGameObject = rTSGameObjects[i];
                }
            }
        }

        return nearestRTSGameObject;
    }

    public List<Unit> NearsRTSGameObjects
    {
        get
        {
            return nearsRTSGameObjects;
        }

        set
        {
            nearsRTSGameObjects = value;
        }
    }

    public List<Unit> NearFriendliesUnit
    {
        get
        {
            return nearFriendliesUnit;
        }

        set
        {
            nearFriendliesUnit = value;
        }
    }
}
