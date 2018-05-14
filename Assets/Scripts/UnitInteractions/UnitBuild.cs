using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMove))]
public class UnitBuild : UnitInteraction
{

    //private bool _isBuilding; // Is the unit building something ?
    [SerializeField] private UnitMove _unitMoveScript; // The script used to move constructor
    [SerializeField] private GameMaster gameMaster;
    private GameObject _building; // The building to build
    private GameObject _buildingGhost; // His ghost
    private Vector3 _positionToBuild; // The position to build the building
    private bool _constructionDone; // Is the construction finished ?
    private bool _onTheMove; // Is the unit moving toward his destination ?
    private bool startbuilding = false;

    void Awake()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        _unitMoveScript = GetComponent<UnitMove>();
    }

	// Use this for initialization
	void Start () {
        //_isBuilding = false;
        _constructionDone = false;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (startbuilding && _unitMoveScript.DestMoveReached)
        {
            if (CanBeBuildHere())
            {
                Buildingbuilding();
            }
            else
                StopAction();
        }

        //if (_isBuilding)
        //{
        //    // Move to the position to build
        //    Init();

        //    // If the constructor reached the destination

        //    if (startbuilding && _unitMoveScript.DestMoveReached && !_constructionDone)
        //    {
        //        print("dest reached");
        //        Buildingbuilding();
        //    }
        //}
	}

    //public void Init()
    //{
    //    if (!_onTheMove)
    //    {
    //        _unitMoveScript.Move(_positionToBuild);
    //        _onTheMove = true;
    //        Invoke("NextInit", 0.06f);
    //    }
    //}

    //private void NextInit()
    //{
    //    // Nothing, just wait for the damn navMesh to update his damn destination
    //    startbuilding = true;
    //}

    public void Build(GameObject building, GameObject buildingGhost, Vector3 position)
    {
        _building = building;
        _buildingGhost = buildingGhost;
        _positionToBuild = position;

        _unitMoveScript.Move(_positionToBuild);

        startbuilding = true;
        //_isBuilding = true;
        //_constructionDone = false;
        //_onTheMove = false;
        //_building = building;
        //_positionToBuild = position;
        //_buildingGhost = buildingGhost;
        //_buildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned++;
    }

    public void Buildingbuilding()
    {
        Destroy(_buildingGhost);

        if (GetComponent<RTSGameObject>().faction.Equals(Faction.Red))
            gameMaster.RedWool -= _building.GetComponent<CreationBuilding>().Cost;
        else if (GetComponent<RTSGameObject>().faction.Equals(Faction.Blue))
            gameMaster.BlueWool -= _building.GetComponent<CreationBuilding>().Cost;

        gameMaster.addUnit(_building, _positionToBuild, _building.transform.rotation, GetComponent<RTSGameObject>().faction);
        //_constructionDone = true;
        startbuilding = false;
        transform.Translate(Vector3.right * 15);

        _unitMoveScript.StopAction();
    }

    public void StopAction()
    {
        //_isBuilding = false;
        _constructionDone = false;
        _onTheMove = false;
        _building = null;
        startbuilding = false;
        _unitMoveScript.StopAction();
        if(_buildingGhost)
            _buildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned--;
    }

    public bool CanBeBuildHere()
    {
        //GameObject tmp = Instantiate(_buildingGhost, _positionToBuild + (Vector3.up * 0.5f), _buildingGhost.transform.rotation);
        //BuildingGhost bdGhostInstantiated = tmp.GetComponent<BuildingGhost>();
        //if (bdGhostInstantiated.canBeBuiltHere)
        if (_buildingGhost.GetComponent<BuildingGhost>().canBeBuiltHere)
        {
            if (GetComponent<RTSGameObject>().faction.Equals(Faction.Red))
            {
                if (gameMaster.RedWool >= _building.GetComponent<CreationBuilding>().Cost)
                    return true;
            }

            else if (GetComponent<RTSGameObject>().faction.Equals(Faction.Blue))
            {
                if (gameMaster.BlueWool >= _building.GetComponent<CreationBuilding>().Cost)
                    return true;
            }
        }
        else
        {
            print("can't be built here");
            return false;
        }

        print("can't build cause no ressources");
        return false;
    }

    public override bool isInAction()
    {
        return startbuilding;
    }
}
