using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMove))]
public class UnitCollect : UnitInteraction
{
    GameMaster gameMaster;

    private UnitMove _moveScript;
    private bool onTheMove = false;

    private float _moveSpeed = 10; // To change later
    private float _rotationSpeed = 10; // To change later

    [SerializeField] private int _amoutCarried;
    private int _maxAmountCarried = 10;
    private bool _isCollecting;
    private Collectable _targetCollectable;
    private bool _collectableReached;

    [SerializeField] private Building _targetDumpingBuilding;

    void Awake()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        MoveScript = GetComponent<UnitMove>();
        _amoutCarried = 0;
        _isCollecting = false;
        _collectableReached = false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // If we sent me to collect a collectable
        if (_targetCollectable)
        {
            // If I don't carry any ressources
            if (_amoutCarried <= 0)
            {
                if (!onTheMove)
                {
                    //print("Moving to the position");
                    MoveScript.Move(_targetCollectable.transform.position);
                    onTheMove = true;
                }

                if (Vector3.Distance(transform.position, _targetCollectable.transform.position) < 18)
                {
                    //print("Reached the position");
                    TakeRessourcesFromCollectable(_targetCollectable);
                    onTheMove = false;
                }
            }
            // If I carry any ressources
            else
            {
                //print("Carring something");
                // If I already got a dumping building
                if (_targetDumpingBuilding)
                {
                    if (!onTheMove)
                    {
                        //print("Moving to the dumping building");
                        MoveScript.Move(_targetDumpingBuilding.transform.position);
                        onTheMove = true;
                    }

                    //if (MoveScript.RemainingDistance() < 15)
                    if (Vector3.Distance(transform.position, _targetDumpingBuilding.transform.position) < 15)
                    {
                        DumpRessource();
                        onTheMove = false;
                    }
                }
                // If not, I search for one
                else
                {
                    //print("Getting the closest dumping building");
                    _targetDumpingBuilding = GetClosestDumpingBuilding();
                }
            }
        }
	}

    public void TakeRessourcesFromCollectable(Collectable collectable)
    {
        collectable.Quantity -= MaxAmountCarried;
        _amoutCarried = MaxAmountCarried;
    }

    public void DumpRessource()
    {
        // ++ ressources
        if (GetComponent<Unit>().faction.Equals(Faction.Red))
            gameMaster.RedWool += _amoutCarried;
        if (GetComponent<Unit>().faction.Equals(Faction.Blue))
            gameMaster.BlueWool += _amoutCarried;
        _amoutCarried = 0;

    }

    public void Collect(Collectable target)
    {
        _isCollecting = true;
        _collectableReached = false;
        _targetCollectable = target;
    }

    public void StopAction()
    {
        _isCollecting = false;
        _targetCollectable = null;
        _targetDumpingBuilding = null;
        _collectableReached = false;
        onTheMove = false;
    }

    public Building GetClosestDumpingBuilding()
    {
        ArrayList dumpingBuildingList = new ArrayList();
        Building[] dumpingBuildingsObjects = GameObject.FindObjectsOfType<Building>();
        Building[] dumpingBuildings = new Building[dumpingBuildingsObjects.Length];

        // Extract building component
        for (int i = 0; i < dumpingBuildingsObjects.Length; i++)
        {
            //Debug.Log(dumpingBuildingsObject[i]);
            
            if (dumpingBuildingsObjects[i].GetComponent<Building>().IsDumpingBuilding)
                dumpingBuildings[i] = dumpingBuildingsObjects[i].GetComponent<Building>();
        }

        // Create a list of the dumpingBuildings availables
        for (int i = 0; i < dumpingBuildings.Length; i++)
        {
            if (dumpingBuildings[i] != null && dumpingBuildings[i].IsDumpingBuilding)
            {
                if (Vector3.Distance(gameObject.transform.position, dumpingBuildings[i].gameObject.transform.position) < 2000f)
                {
                    dumpingBuildingList.Add(dumpingBuildings[i]);
                }
            }
        }

        // Select the closest one
        Building minDumpingBuilding = null;
        if (dumpingBuildingList.Count > 0)
        {
            float min = 2000f;
            foreach (Building c in dumpingBuildingList)
                if (Vector3.Distance(gameObject.transform.position, c.gameObject.transform.position) < min)
                {
                    min = Vector3.Distance(gameObject.transform.position, c.gameObject.transform.position);
                    minDumpingBuilding = c;
                }
        }

        return minDumpingBuilding;
    }

    public Collectable GetClosestCollectableAround()
    {
        ArrayList collectableList = new ArrayList();
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("Collectable");

        for (int i = 0; i < collectables.Length; i++)
        {
            if (collectables[i].GetComponent<Collectable>() != null)
            {
                if (Vector3.Distance(gameObject.transform.position, collectables[i].gameObject.transform.position) < 40f)
                    collectableList.Add(collectables[i]);
            }
            else
            {
                Debug.Log(collectables[i] + "doesn't have Collectable component");
            }
        }

        Collectable minCollectable = null;
        if (collectableList.Count > 0)
        {
            float min = 40f;
            foreach (GameObject c in collectableList)
                if (Vector3.Distance(gameObject.transform.position, c.transform.position) < min)
                {
                    min = Vector3.Distance(gameObject.transform.position, c.transform.position);
                    minCollectable = c.GetComponent<Collectable>();
                }
        }

        return minCollectable;
    }

    public override bool isInAction()
    {
        return onTheMove;
    }

    #region Getters/Setters
    public bool IsCollecting
    {
        get
        {
            return _isCollecting;
        }

        set
        {
            _isCollecting = value;
        }
    }

    public int AmountCarried
    {
        get
        {
            return _amoutCarried;
        }

        set
        {
            _amoutCarried = value;
        }
    }

    public int MaxAmountCarried
    {
        get
        {
            return _maxAmountCarried;
        }

        set
        {
            _maxAmountCarried = value;
        }
    }

    public UnitMove MoveScript
    {
        get
        {
            return _moveScript;
        }

        set
        {
            _moveScript = value;
        }
    }
    #endregion
}


// Alternative collecting style
//if (_isCollecting)
//{
//    if (_targetCollectable != null)
//    {
//        if (_amoutCarried > 0)
//            _destReached = true;
//        if(!_destReached)
//        {
//            if (Vector3.Distance(gameObject.transform.position, _targetCollectable.gameObject.transform.position) < 0.1f)
//            {
//                _destReached = true;
//                TakeRessourcesFromCollectable(_targetCollectable);
//            }
//            else
//            {
//                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_targetCollectable.gameObject.transform.position - gameObject.transform.position), _rotationSpeed * Time.deltaTime);
//                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
//            }
//        }
//        else
//        {
//            if(_targetDumpingBuilding != null && _targetDumpingBuilding.IsDumpingBuilding())
//            {
//                if (Vector3.Distance(transform.position, _targetDumpingBuilding.gameObject.transform.position) < 0.1f)
//                {
//                    _destReached = false;
//                    DumpRessource();
//                }
//                else
//                {
//                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_targetDumpingBuilding.gameObject.transform.position - transform.position), _rotationSpeed * Time.deltaTime);
//                    transform.position += transform.forward * _moveSpeed * Time.deltaTime;
//                }
//            }
//            else
//            {
//                if (GetClosestDumpingBuilding() == null)
//                {
//                    _isCollecting = false;
//                }
//                else
//                {
//                    _targetDumpingBuilding = GetClosestDumpingBuilding();
//                }
//            }
//        }
//    }
//    else
//    {
//        if (GetClosestCollectableAround() == null)
//        {
//            _isCollecting = false;
//        }
//        else
//        {
//            _targetCollectable = GetClosestCollectableAround();
//        }
//    }
//}
