using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnitMove))]
public abstract class WalkingUnit : Unit {

    private UnitCollectableObject collectableObjectScript;
    private UnitMove _moveScript;

    [SerializeField] private CollectableObject[] inventory;

    private LinkedList<Order> _orders;

    private float _moveSpeed = 10;
    private float _rotationSpeed = 10;

    [SerializeField] private float _creationTime = 1;

    protected override void Awake()
    {
        base.Awake();

        MoveScript = GetComponent<UnitMove>();
        CollectableObjectScript = GetComponent<UnitCollectableObject>();
        Orders = new LinkedList<Order>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        HandlingPostponeOrders();
    }

    public virtual void HandlingPostponeOrders()
    {
        if (Orders.Count > 0)
        {
            if (doingNothing())
            {
                // Execute the next order
                Order order = Orders.First.Value;

                if (order.Name.Equals(OrderName.Mouse1))
                {
                    if (order.GameObject == null && !(order.Position.Equals(Vector3.zero)))
                        DoPostponeMoveOrder(order.Position);
                }
                else if (order.Name.Equals(OrderName.Stop))
                    DoPostponeStopOrder();
                else if (order.Name.Equals(OrderName.Move))
                    DoPostponeMoveOrder(order.Position);
                else
                    Debug.Log("Order not yet implemented");

                // Remove this order when it has been executed
                Orders.RemoveFirst();
            }
        }
    }

    public void SendDirectDropItemOrder(CollectableObject collectableObject, Vector3 position)
    {
        SendDropItemOrder(collectableObject, position, true, false);
    }

    public void SendDropItemOrder(CollectableObject collectableObject, Vector3 position, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();

            SendMessage("StopAction");
            CollectableObjectScript.DropObject(collectableObject, position);
        }
    }

    public void SendMoveOrder(Vector3 position, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();

            SendMessage("StopAction");
            MoveScript.Move(position);
        }
        else
            Orders.AddLast(new Order(OrderName.Move, position, null));
    }
    public void SendDirectMoveOrder(Vector3 position)
    {
        SendMoveOrder(position, true, false);
    }
    public void SendPostponeMoveOrder(Vector3 position)
    {
        SendMoveOrder(position, false, true);
    }
    public void DoPostponeMoveOrder(Vector3 position)
    {
        SendMoveOrder(position, false, false);
    }

    public void SendDirectPatrolOrder(Vector3 position)
    {
        SendPatrolOrder(position, true, false);
    }
    public void SendPatrolOrder(Vector3 position, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();

            SendMessage("StopAction");
            MoveScript.Patrol(gameObject.transform.position, position);
        }
        else
            Orders.AddLast(new Order(OrderName.Patrol, position, null));
    }
    public void SendPostponePatrolOrder(Vector3 position)
    {
        SendPatrolOrder(position, false, true);
    }
    public void DoPostponePatrolOrder(Vector3 position)
    {
        SendPatrolOrder(position, false, false);
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
                btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = ((OrderName)Actions[i]).ToString();
                basicSet.Add(btn);
            }
        }
    }

    public override void SendStopOrder(bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();

            SendMessage("StopAction");
        }
        else
            Orders.AddLast(new Order(OrderName.Stop, Vector3.zero, null));
    }

    public void GoTakeObject(CollectableObject collectableObject)
    {
        int i = 0;
        while (i < Inventory.Length && Inventory[i] != null) i++;

        if (i < Inventory.Length)
        {
            CollectableObjectScript.TakeObject(collectableObject);
        }
        else
            print("Inventory full");
    }

    public void DropObject(CollectableObject collectableObject, Vector3 pos)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] != null)
            {
                if (Inventory[i].Equals(collectableObject))
                {
                    if (Inventory[i].GetComponent<CollectableObject>().BuffObject != null)
                        Buffs.Remove(Inventory[i].GetComponent<CollectableObject>().BuffObject);

                    Inventory[i] = null;
                    break;
                }
            }
        }
        collectableObject.transform.position = new Vector3(pos.x, 11, pos.z);
        collectableObject.transform.localScale = new Vector3(1, 1, 1);
        collectableObject.ObjectTaken = false;
        collectableObject.gameObject.SetActive(true);
    }

    public void TakeObject(CollectableObject collectableObject)
    {
        if (!collectableObject.ObjectTaken)
        {
            int i = 0;
            while (i < Inventory.Length && Inventory[i] != null) i++;

            if (i < Inventory.Length)
            {
                Inventory[i] = collectableObject;

                if (collectableObject.BuffObject != null)
                {
                    Buffs.Add(collectableObject.BuffObject);
                }

                collectableObject.ObjectTaken = true;
                collectableObject.TakeObjectAnimation();
            }
            else
                print("Inventory full");
        }
    }
    public bool HasInventory()
    {
        if (Inventory != null)
            return Inventory.Length > 0;
        return false;
        //return Inventory != null || Inventory.Length > 0;
    }

    #region Getters/Setters
    public float MoveSpeed
    {
        get
        {
            float moveSpeedBuff = 1;
            for (int i = 0; i < Buffs.Count; i++)
                moveSpeedBuff *= Buffs[i].SpeedMultiplier;

            return _moveSpeed * moveSpeedBuff;
        }

        set { _moveSpeed = value; }
    }

    public float RotationSpeed
    {
        get { return _rotationSpeed; }
        set { _rotationSpeed = value; }
    }
    public UnitMove MoveScript
    {
        get { return _moveScript; }
        set { _moveScript = value; }
    }
    public LinkedList<Order> Orders
    {
        get { return _orders; }
        set { _orders = value; }
    }
    public CollectableObject[] Inventory
    {
        get { return inventory; }
        set { inventory = value; }
    }
    public UnitCollectableObject CollectableObjectScript
    {
        get
        {
            return collectableObjectScript;
        }

        set
        {
            collectableObjectScript = value;
        }
    }

    public float CreationTime
    {
        get
        {
            return _creationTime;
        }

        set
        {
            _creationTime = value;
        }
    }
    #endregion
}
