using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnitCollect))]
[RequireComponent(typeof(UnitMove))]
public class Collector : WalkingUnit {

    private UnitCollect collectScript;

    protected override void Awake()
    {
        base.Awake();

        CollectScript = GetComponent<UnitCollect>();

        addAction(OrderName.Stop);
        addAction(OrderName.Move);
        addAction(OrderName.Collect);

        //if (!GameMaster.Player1Hero)
        if(GameMaster.PlayerFaction.Equals(faction))
            InitialiseGUI();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void HandlingPostponeOrders()
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
                else if (order.Name.Equals(OrderName.Collect))
                    DoPostponeCollectOrder(order.GameObject.GetComponent<Collectable>());
                else
                    Debug.Log("Order not yet implemented");

                // Remove this order when it has been executed
                Orders.RemoveFirst();
            }
        }
    }

    #region Orders Implementation
    public override void SendMouse1Order(Vector3 position, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();

            SendMessage("StopAction"); // Interrupt any other actions
            MoveScript.Move(position);
        }
        else
            Orders.AddLast(new Order(OrderName.Mouse1, position, null));
    }
    public override void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone)
    {
        print("got tf heer");
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();
            if (target.GetComponent<Collectable>()) // Goldmine, wood
            {
                print("got here");
                SendMessage("StopAction"); // Interrupt any other actions
                CollectScript.Collect(target.GetComponent<Collectable>());
            }
        }
        else
            Orders.AddLast(new Order(OrderName.Mouse1, Vector3.zero, target));
    }

    public void SendDirectCollectOrder(Collectable target)
    {
        SendCollectOrder(target, true, false);
    }
    public void SendPostponeCollectOrder(Collectable target)
    {
        SendCollectOrder(target, false, true);
    }
    public void DoPostponeCollectOrder(Collectable target)
    {
        SendCollectOrder(target, false, false);
    }
    public void SendCollectOrder(Collectable target, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();

            SendMessage("StopAction"); // Interrupt any other actions
            CollectScript.Collect(target);
        }
        else
            Orders.AddLast(new Order(OrderName.Collect, Vector3.zero, target));
    }
    #endregion

    public override bool doingNothing()
    {
        return !MoveScript.isInAction(); // && !CollectScript.IsCollecting;
    }

    #region BaseCharacter Getters and Setters

    public UnitCollect CollectScript
    {
        get
        {
            return collectScript;
        }

        set
        {
            collectScript = value;
        }
    }


    #endregion
}
