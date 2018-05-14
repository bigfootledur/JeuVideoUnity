using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    private Faction myFaction;
    private List<RTSGameObject> myUnits;
    public Transform initialBuildingPosition;
    private bool tooMuchBuilding = false;
    private GameMaster gameMaster;
    [SerializeField] private Transform constructorBasePosition;

    void Awake()
    {
        myUnits = new List<RTSGameObject>();
        gameMaster = FindObjectOfType<GameMaster>();
    }

	// Use this for initialization
	void Start () {
        myFaction = GameMaster.PlayerFaction.Equals(Faction.Blue) ? Faction.Red : Faction.Blue;

        RTSGameObject[] myUnitsObjects = FindObjectsOfType<RTSGameObject>();
        for (int i = 0; i < myUnitsObjects.Length; i++)
        {
            if (myUnitsObjects[i].GetComponent<RTSGameObject>().faction.Equals(myFaction))
                myUnits.Add(myUnitsObjects[i].GetComponent<RTSGameObject>());
        }
	}
	
	// Update is called once per frame
	void Update () {
        HandleCollector();
        if (!tooMuchBuilding)
        {
            Invoke("HandleConstructor", 0.1f);
        }
    }

    void HandleCollector()
    {
        for (int i = 0; i < myUnits.Count; i++)
        {
            if (myUnits[i] != null && myUnits[i].GetComponent<Collector>() && myUnits[i].GetComponent<Collector>().doingNothing())
            {
                Collector collector = myUnits[i].GetComponent<Collector>();
                Collectable target = GetClosestCollectable(collector.transform.position);
                collector.SendCollectOrder(target, true, false);
            }
        }
    }

    void HandleConstructor()
    {
        for (int i = 0; i < myUnits.Count; i++)
        {
            if(myUnits[i].GetComponent<Constructor>() && myUnits[i].GetComponent<Constructor>().doingNothing())
            {
                Constructor constructor = myUnits[i].GetComponent<Constructor>();

                CreationBuilding buildingToBuild;
                if (UnityEngine.Random.value < 0.5f)
                    buildingToBuild = constructor.Buildings[0];
                else
                    buildingToBuild = constructor.Buildings[1];

                float money = myFaction.Equals(Faction.Blue) ? gameMaster.BlueWool : gameMaster.RedWool;
                if (buildingToBuild.Cost <= money)
                {
                    GameObject tmp = Instantiate(buildingToBuild.BuildingGhost, initialBuildingPosition.position + Vector3.up * 0.5f, buildingToBuild.BuildingGhost.transform.rotation);
                    BuildingGhost bdGhostInstantied = tmp.GetComponent<BuildingGhost>();

                    int cap = 0;

                    RaycastHit ra;
                    bool hittedSomeGO = true;
                    Physics.SphereCast(bdGhostInstantied.transform.position + Vector3.up * 40, 5, -Vector3.up, out ra, 50);
                    if (ra.collider.gameObject.GetComponent<RTSGameObject>())
                        hittedSomeGO = true;
                    else
                        hittedSomeGO = false;

                    while (hittedSomeGO && cap < 7)
                    {

                        bdGhostInstantied.transform.position += Vector3.forward * -20;

                        Physics.SphereCast(bdGhostInstantied.transform.position + Vector3.up * 40, 5, -Vector3.up, out ra, 50);
                        if (ra.collider.gameObject.GetComponent<RTSGameObject>())
                            hittedSomeGO = true;
                        else
                            hittedSomeGO = false;

                        cap++;
                    }

                    if (cap >= 7)
                        tooMuchBuilding = true;
                    else
                    {
                        constructor.SendBuildOrder(buildingToBuild.gameObject, bdGhostInstantied.gameObject,
                                                   bdGhostInstantied.transform.position, buildingToBuild.gameObject.transform.rotation, true, false);
                    }
                }
                else
                {
                    constructor.SendDirectMoveOrder(constructorBasePosition.position);
                }
            }
        }
    }

    public Collectable GetClosestCollectable(Vector3 position)
    {
        Collectable[] collectables = GameObject.FindObjectsOfType<Collectable>();
        if (collectables.Length <= 0)
            return null;
        Collectable closestCollectable = collectables[0];
        for (int i = 1; i < collectables.Length; i++)
        {
            if (Math.Abs(Vector3.Distance(collectables[i].transform.position, position)) <
               Math.Abs(Vector3.Distance(closestCollectable.transform.position, position)))
                closestCollectable = collectables[i];
        }
        return closestCollectable;
    }
}
