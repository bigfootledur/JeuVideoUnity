using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Townhall : Building {
    
    [SerializeField] private Collector creation;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Collectable _rallyObject;
    [SerializeField] private bool _creatingUnitOrder;
    [SerializeField] private float _createTimer;
    private bool timeCreationSet = false;
    private int collectorQueue;
    private const int COLLECTOR_QUEUE_CAP = 5; 

    protected override void Awake()
    {
        base.Awake();
        IsDumpingBuilding = true;
        _creatingUnitOrder = false;
        collectorQueue = 0;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (this.faction.Equals(Faction.Blue))
        {
            foreach(Transform obj in GetComponentInChildren<Transform>())
                if(obj.GetComponent<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
        }
        else if (this.faction.Equals(Faction.Red))
        {
            foreach (Transform obj in GetComponentInChildren<Transform>())
                if (obj.GetComponent<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().materials[0].color = Color.red;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (collectorQueue > 0 && !timeCreationSet)
        {
            _createTimer = creation.CreationTime;
            timeCreationSet = true;
        }

        if (_createTimer > 0)
            _createTimer -= Time.deltaTime;

        if (_createTimer < 0)
            _createTimer = 0;

        if (_createTimer <= 0 && collectorQueue > 0)
        {
            collectorQueue--;
            timeCreationSet = false;
            gameMaster.spawnUnit(creation.gameObject, _spawnPoint.position, Quaternion.identity, _rallyObject, faction);
        }
    }

    public override bool doingNothing()
    {
        throw new NotImplementedException();
    }

    public override void InitialiseButtons()
    {
        throw new NotImplementedException();
    }

    public override void SendMouse1Order(Vector3 position, bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public override void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public override void SendStopOrder(bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public void CreateCollector()
    {
        AddCollectorInQueue();
    }

    public void CancelCreateCollector()
    {
        RemoveCollectorFromQueue();
    }

    public void AddCollectorInQueue()
    {

        // Cost gestion
        float playerMoney = faction.Equals(Faction.Blue) ? gameMaster.BlueWool : gameMaster.RedWool;
        if (playerMoney < creation.Cost)
        {
            print("Not enough money");
            return;
        }



        // Add queue gestion
        if (collectorQueue < COLLECTOR_QUEUE_CAP)
        {
            if (faction.Equals(Faction.Blue))
                gameMaster.BlueWool -= creation.Cost;
            else if (faction.Equals(Faction.Red))
                gameMaster.RedWool -= creation.Cost;

            collectorQueue++;
        }
        else
            print("Queue full");

        if (collectorQueue == 1) // Reset timer if it's the first unit in the queue
        {
            timeCreationSet = false;
        }
    }

    public void RemoveCollectorFromQueue()
    {
        if (collectorQueue > 0)
        {
            collectorQueue--;

            if (faction.Equals(Faction.Blue))
                gameMaster.BlueWool += creation.Cost;
            else if (faction.Equals(Faction.Red))
                gameMaster.RedWool += creation.Cost;
        }
            
    }
}

//public override void InitialiseButtons()
//{
//    _GUI.ProductionBar = new GameObject[6];
//    _GUI.GUIButtons = new Dictionary<ButtonSet, List<GameObject>>();
//    _GUI.GUIButtons.Add(ButtonSet.Basic, new List<GameObject>());

//    List<GameObject> basicSet = null;

//    _GUI.GUIButtons.TryGetValue(ButtonSet.Basic, out basicSet);

//    for (int i = 0; i < Actions.Count; i++)
//    {

//        // Instantiate the correct button from the available button prefabs
//        GameObject btn = GameObject.Instantiate(gameMaster.GetButtonPrefab((OrderName)Actions[i]));

//        // Put the button in the panel "UnitButtons"
//        btn.transform.SetParent(GameObject.Find("UnitButtons").transform, false);
//        btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = ((OrderName)Actions[i]).ToString();
//        gameMaster.AddButtonListener(btn, (OrderName)Actions[i]);

//        // Anchors configuration
//        RectTransform btnTransform = btn.GetComponent<RectTransform>();
//        btnTransform.anchorMin = new Vector2((0 + 0.25f * (i % 4)), 0.75f - 0.25f * (i / 4));
//        btnTransform.anchorMax = new Vector2((0.25f + 0.25f * (i % 4)), 1 - 0.25f * (i / 4));
//        btnTransform.pivot = new Vector2(0.5f, 0.5f);

//        btnTransform.offsetMin = new Vector2(0, 0);
//        btnTransform.offsetMax = new Vector2(0, 0);

//        btn.SetActive(false);

//        Regex regexBuild = new Regex("^Build[0-9]+");
//        Regex regexCreate = new Regex("^Create[0-9]+");

//        if (regexBuild.IsMatch(((OrderName)Actions[i]).ToString()))
//        {
//            if (!_GUI.GUIButtons.ContainsKey(ButtonSet.Build))
//            {
//                _GUI.GUIButtons.Add(ButtonSet.Build, new List<GameObject>());

//                // Instantiate the correct button from the available button prefabs
//                GameObject btnCancel = GameObject.Instantiate(gameMaster.GetButtonPrefab((OrderName)Actions[i])); // Parameter can be left like this

//                // Put the button in the panel "UnitButtons"
//                btnCancel.transform.SetParent(GameObject.Find("UnitButtons").transform, false);
//                btnCancel.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Cancel";
//                gameMaster.AddButtonListener(btnCancel, (OrderName.Cancel));

//                // Anchors configuration
//                RectTransform btnCancelTransform = btnCancel.GetComponent<RectTransform>();
//                btnCancelTransform.anchorMin = new Vector2((0 + 0.25f * (0 % 4)), 0.75f - 0.25f * (2 / 4));
//                btnCancelTransform.anchorMax = new Vector2((0.25f + 0.25f * (4 % 4)), 1 - 0.25f * (2 / 4));
//                btnCancelTransform.pivot = new Vector2(0.5f, 0.5f);

//                btnCancelTransform.offsetMin = new Vector2(0, 0);
//                btnCancelTransform.offsetMax = new Vector2(0, 0);

//                btnCancel.SetActive(false);

//                List<GameObject> buildSetBis = null;
//                _GUI.GUIButtons.TryGetValue(ButtonSet.Build, out buildSetBis);
//                buildSetBis.Add(btnCancel);
//            }

//            List<GameObject> buildSet = null;
//            _GUI.GUIButtons.TryGetValue(ButtonSet.Build, out buildSet);
//            buildSet.Add(btn);
//        }
//        else
//        {
//            if (regexCreate.IsMatch(((OrderName)Actions[i]).ToString()))
//                btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = GetCreation((OrderName)Actions[i]).ToString();
//            else
//                btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = ((OrderName)Actions[i]).ToString();
//            basicSet.Add(btn);
//        }
//    }
//}
