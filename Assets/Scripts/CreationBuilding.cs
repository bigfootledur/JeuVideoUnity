using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CreationBuilding : Building {

    public static int NB_ORDER_MAX = 12; // Buildings
    public static int NB_BUTTONS_MAX = 19; // Buildings

    [SerializeField] private bool _autoBuild = true;

    private int ordersCount;
    private int ordersCanceled;
    [SerializeField] private OrderName[] orders;
    private Vector3 _spawnPoint;

    [SerializeField] private WalkingUnit creation;
    [SerializeField] private WalkingUnit[] _creations;
    [SerializeField] private RTSGameObject rallyPointFlag;

    private Vector3 _rallyPoint;
    private RTSGameObject _rallyObject;

    public bool _creatingUnit;

    public double _createTimer;

    [SerializeField] private GameObject buildingGhost;

    [SerializeField] private bool isDumpingBuilding;

    protected override void Awake()
    {
        base.Awake();

        ordersCanceled = 0;
        ordersCount = 0;

        _rallyObject = null;

        _createTimer = 0;
        _creatingUnit = false;

        orders = new OrderName[6];

        //addAction(OrderName.Create0);
        //addAction(OrderName.Create1);
        //addAction(OrderName.Create2);
        addAction(OrderName.Create0);
        addAction(OrderName.RallyPoint);

        //if (!GameMaster.Player1Hero)

    }

    protected override void Start()
    {
        base.Start();

        if (GameMaster.PlayerFaction.Equals(faction))
            InitialiseGUI();

        if (this.faction.Equals(Faction.Blue))
        {
            _spawnPoint = new Vector3(this.transform.position.x + 10, this.transform.position.y, this.transform.position.z);
            _rallyPoint = gameMaster.RallyPointRedUnits;
            if(this.GetComponent<MeshRenderer>())
                this.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
        }
        else if (this.faction.Equals(Faction.Red))
        {
            _spawnPoint = new Vector3(this.transform.position.x - 10, this.transform.position.y, this.transform.position.z + 5);
            _rallyPoint = gameMaster.RallyPointBlueUnits;
            if(this.GetComponent<MeshRenderer>())
                this.GetComponent<MeshRenderer>().materials[0].color = Color.red;
        }
    }

    protected override void Update()
    {
        base.Update();

        //if (!_creatingUnit)
        //{
        //    if (AutoBuild && Creations.Length > 0)
        //    {
        //        creation = GetCreation(OrderName.Create0);
        //        _createTimer = creation.CreationTime;
        //        _creatingUnit = true;
        //    }
        //    else
        //    {
        //        if (ordersCount > 0)
        //        {
        //            creation = GetCreation(orders[0]);
        //            _createTimer = creation.CreationTime;
        //            _creatingUnit = true;
        //        }
        //        else
        //        {
        //            _creatingUnit = false;
        //        }
        //    }
        //}

        //if (_createTimer > 0)
        //    _createTimer -= Time.deltaTime;

        //if (_createTimer < 0)
        //    _createTimer = 0;

        //if (_createTimer <= 0 && _creatingUnit)
        //{
        //    _creatingUnit = false;

        //    gameMaster.spawnUnit(creation.gameObject, _spawnPoint, Quaternion.identity, _rallyPoint, this.faction);

        //    CancelCreate();
        //}

        if (!_creatingUnit)
        {
            _createTimer = creation.CreationTime;
            _creatingUnit = true;
        }

        if (_createTimer > 0)
            _createTimer -= Time.deltaTime;

        if (_createTimer < 0)
            _createTimer = 0;

        if (_createTimer <= 0 && _creatingUnit)
        {
            _creatingUnit = false;
            gameMaster.spawnUnit(creation.gameObject, _spawnPoint, Quaternion.identity, _rallyPoint, this.faction);
        }
    }

    public void CancelCreate(GameObject btn)
    {
        GameObject.Destroy(btn);
        Invoke("Next", 0.05f);
    }

    public void Next()
    {
        int position = 0;
        for (int i = 0; i < GetGUI.ProductionBar.Length; i++)
        {
            if (GetGUI.ProductionBar[i] == null)
            {
                position = i;
                break;
            }
        }

        if (position == 0)
            _creatingUnit = false;
        orders[position] = OrderName.Mouse1;

        for (int i = position + 1; i < orders.Length; i++)
        {
            OrderName tmp = orders[i];
            orders[i] = orders[i - 1];
            orders[i - 1] = tmp;

            GameObject tmpG = GetGUI.ProductionBar[i];
            GetGUI.ProductionBar[i] = GetGUI.ProductionBar[i - 1];
            GetGUI.ProductionBar[i - 1] = tmpG;
        }

        ordersCount--;

        UpdateButtons(position);
    }

    public void CancelCreate()
    {
        _creatingUnit = false;

        orders[0] = OrderName.Mouse1;
        //if (!GameMaster.Player1Hero && (!GetGUI.Equals(null)) && (!GetGUI.ProductionBar[0].Equals(null)))
        //if ((!GetGUI.Equals(null)) && (!GetGUI.ProductionBar[0].Equals(null)))
        //    GameObject.Destroy(GetGUI.ProductionBar[0]);
        //else
        //    Debug.LogWarning("No production for the building");

        ordersCanceled++;

        //if (!GameMaster.Player1Hero)
        //{
        for (int i = 1; i < orders.Length; i++)
        {
            OrderName tmp = orders[i];
            orders[i] = orders[i - 1];
            orders[i - 1] = tmp;

            GameObject tmpG = GetGUI.ProductionBar[i];
            GetGUI.ProductionBar[i] = GetGUI.ProductionBar[i - 1];
            GetGUI.ProductionBar[i - 1] = tmpG;
        }

        UpdateButtons(0);
        //}
        ordersCount--;
    }

    public void UpdateButtons(int position)
    {
        for (int i = position; i < GetGUI.ProductionBar.Length; i++)
        {
            if (GetGUI.ProductionBar[i] != null)
                GetGUI.ProductionBar[i].GetComponent<RectTransform>().localPosition = new Vector3(GetGUI.ProductionBar[i].GetComponent<RectTransform>().localPosition.x - 100,
                                                                                                  GetGUI.ProductionBar[i].GetComponent<RectTransform>().localPosition.y,
                                                                                                  GetGUI.ProductionBar[i].GetComponent<RectTransform>().localPosition.z);
        }
    }

    private void AddProduction(OrderName order)
    {
        if (ordersCount < 6)
        {
            GameObject btn = Instantiate(gameMaster.buttonPrefab);
            btn.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = GetCreation(order).name;
            gameMaster.AddButtonListener(btn, order, btn, ordersCount);
            btn.GetComponent<RectTransform>().localPosition = new Vector3((ordersCount * 100) - 60f, -355.9f, 0);
            btn.SetActive(false);
            print(ordersCount);
            GetGUI.ProductionBar[ordersCount] = btn;
            orders[ordersCount] = order;
            ordersCount++;
        }
    }

    public WalkingUnit GetCreation(OrderName order)
    {
        if (order.Equals(OrderName.Create0))
            return Creations[0];
        else if (order.Equals(OrderName.Create1))
            return Creations[1];
        else if (order.Equals(OrderName.Create2))
            return Creations[2];

        return null;
    }
    public override void InitialiseButtons()
    {
        _GUI.ProductionBar = new GameObject[6];
        _GUI.GUIButtons = new Dictionary<ButtonSet, List<GameObject>>();
        _GUI.GUIButtons.Add(ButtonSet.Basic, new List<GameObject>());

        List<GameObject> basicSet = null;

        _GUI.GUIButtons.TryGetValue(ButtonSet.Basic, out basicSet);

        for (int i = 0; i < Actions.Count; i++)
        {
            // Instantiate the correct button from the available button prefabs
            GameObject btn = GameObject.Instantiate(gameMaster.GetButtonPrefab((OrderName)Actions[i]));

            // Put the button in the panel "UnitButtons"
            btn.transform.SetParent(GameObject.Find("UnitButtons").transform, false);
            btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = ((OrderName)Actions[i]).ToString();
            gameMaster.AddButtonListener(btn, (OrderName)Actions[i]);

            // Anchors configuration
            RectTransform btnTransform = btn.GetComponent<RectTransform>();
            btnTransform.anchorMin = new Vector2((0 + 0.25f * (i % 4)), 0.75f - 0.25f * (i / 4));
            btnTransform.anchorMax = new Vector2((0.25f + 0.25f * (i % 4)), 1 - 0.25f * (i / 4));
            btnTransform.pivot = new Vector2(0.5f, 0.5f);

            btnTransform.offsetMin = new Vector2(0, 0);
            btnTransform.offsetMax = new Vector2(0, 0);

            btn.SetActive(false);

            Regex regexBuild = new Regex("^Build[0-9]+");
            Regex regexCreate = new Regex("^Create[0-9]+");

            if (regexBuild.IsMatch(((OrderName)Actions[i]).ToString()))
            {
                if (!_GUI.GUIButtons.ContainsKey(ButtonSet.Build))
                {
                    _GUI.GUIButtons.Add(ButtonSet.Build, new List<GameObject>());

                    // Instantiate the correct button from the available button prefabs
                    GameObject btnCancel = GameObject.Instantiate(gameMaster.GetButtonPrefab((OrderName)Actions[i])); // Parameter can be left like this

                    // Put the button in the panel "UnitButtons"
                    btnCancel.transform.SetParent(GameObject.Find("UnitButtons").transform, false);
                    btnCancel.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Cancel";
                    gameMaster.AddButtonListener(btnCancel, (OrderName.Cancel));

                    // Anchors configuration
                    RectTransform btnCancelTransform = btnCancel.GetComponent<RectTransform>();
                    btnCancelTransform.anchorMin = new Vector2((0 + 0.25f * (0 % 4)), 0.75f - 0.25f * (2 / 4));
                    btnCancelTransform.anchorMax = new Vector2((0.25f + 0.25f * (4 % 4)), 1 - 0.25f * (2 / 4));
                    btnCancelTransform.pivot = new Vector2(0.5f, 0.5f);

                    btnCancelTransform.offsetMin = new Vector2(0, 0);
                    btnCancelTransform.offsetMax = new Vector2(0, 0);

                    btnCancel.SetActive(false);

                    List<GameObject> buildSetBis = null;
                    _GUI.GUIButtons.TryGetValue(ButtonSet.Build, out buildSetBis);
                    buildSetBis.Add(btnCancel);
                }

                List<GameObject> buildSet = null;
                _GUI.GUIButtons.TryGetValue(ButtonSet.Build, out buildSet);
                buildSet.Add(btn);
            }
            else
            {
                if (regexCreate.IsMatch(((OrderName)Actions[i]).ToString()))
                    btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = GetCreation((OrderName)Actions[i]).ToString();
                else
                    btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = ((OrderName)Actions[i]).ToString();
                basicSet.Add(btn);
            }
        }
    }
    #region Orders Implementation

    public void SendCreate0Order()
    {
        AddProduction(OrderName.Create0);
    }
    public void SendCreate1Order()
    {
        AddProduction(OrderName.Create1);
    }
    public void SendCreate2Order()
    {
        AddProduction(OrderName.Create2);
    }

    public void SendRallyPointOrder(Vector3 position)
    {
        _rallyPoint = position;
        _rallyObject = null;
    }

    public void SendRallyPointOrder(RTSGameObject target)
    {
        _rallyObject = target;
        _rallyPoint = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 5);
    }

    public override void SendMouse1Order(Vector3 position, bool directOrder, bool postpone)
    {
        SendRallyPointOrder(position);
    }

    public override void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone)
    {
        SendRallyPointOrder(target);
    }

    #endregion

    #region Getters/Setters

    public override bool doingNothing()
    {
        throw new NotImplementedException();
    }
    #endregion
    #region Orders Implementation
    public override void SendStopOrder(bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public void SendDirectCreate0Order()
    {
        SendCreate0Order();
    }
    public void SendDirectCreate1Order()
    {
        SendCreate1Order();
    }
    public void SendDirectCreate2Order()
    {
        SendCreate2Order();
    }
    public void SendDirectRallyPointOrder(Vector3 position)
    {
        SendRallyPointOrder(position);
    }
    public void SendDirectRallyPointOrder(RTSGameObject target)
    {
        SendRallyPointOrder(target);
    }
    #endregion

    public bool AutoBuild
    {
        get
        {
            return _autoBuild;
        }

        set
        {
            _autoBuild = value;
        }
    }

    public GameObject BuildingGhost
    {
        get
        {
            return buildingGhost;
        }

        set
        {
            buildingGhost = value;
        }
    }

    public bool IsDumpingBuilding
    {
        get
        {
            return isDumpingBuilding;
        }

        set
        {
            isDumpingBuilding = value;
        }
    }

    public WalkingUnit[] Creations
    {
        get
        {
            return _creations;
        }

        set
        {
            _creations = value;
        }
    }
}
