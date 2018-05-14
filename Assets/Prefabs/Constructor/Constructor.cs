using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnitBuild))]
public class Constructor : WalkingUnit {
    
    public Animator basicUnitAnim;
    private UnitBuild _unitBuildScript;
    private GameObject _buildingGhost;
    [SerializeField] private CreationBuilding[] buildings;

    protected override void Awake()
    {
        base.Awake();
        UnitBuildScript = GetComponent<UnitBuild>();

        addAction(OrderName.Stop);
        addAction(OrderName.Move);
        addAction(OrderName.Build);
        addAction(OrderName.Build0);
        addAction(OrderName.Build1);

        //if(!GameMaster.Player1Hero)
        InitialiseGUI();
    }

    protected override void Start()
    {
        base.Start();

        //if (!Interactionable)
        //{
        //    GetGUI.GUIButtons.Clear();
        //}

    }

    // Update is called once per frame
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
                else if (order.Name.Equals(OrderName.Patrol))
                    DoPostponePatrolOrder(order.Position);
                else if (order.Name.Equals(OrderName.Build0))
                    DoPostponeBuildOrder(order.GameObject.gameObject, order.BuildingGhost, order.Position);
                else
                    Debug.Log("Order not yet implemented");

                // Remove this order when it has been executed
                Orders.RemoveFirst();
            }
        }
    }


    #region Orders Implementations
    public void SendBuildOrder(GameObject building, GameObject buildingGhost, Vector3 position, Quaternion quaternion, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
            {
                foreach (Order order in Orders)
                {
                    if (order.BuildingGhost)
                        order.BuildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned--;
                }
                Orders.Clear();
            }

            SendMessage("StopAction");
            UnitBuildScript.Build(building, buildingGhost, position);
        }
        else
        {
            buildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned++;
            Orders.AddLast(new Order(OrderName.Build0, position, building.GetComponent<RTSGameObject>(), buildingGhost));
        }
    }
    public void SendDirectBuildOrder(GameObject building, GameObject buildingGhost, Vector3 position)
    {
        SendBuildOrder(building, buildingGhost, position, building.transform.rotation, true, false);
    }
    public void SendPostponeBuildOrder(GameObject building, GameObject buildingGhost, Vector3 position)
    {
        SendBuildOrder(building, buildingGhost, position, building.transform.rotation, false, true);
    }
    public void DoPostponeBuildOrder(GameObject building, GameObject buildingGhost, Vector3 position)
    {
        SendBuildOrder(building, buildingGhost, position, building.transform.rotation, false, false);
    }

    public override void SendMouse1Order(Vector3 position, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
            {
                foreach (Order order in Orders)
                {
                    if(order.BuildingGhost)
                        order.BuildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned--;
                }
                Orders.Clear();
            }
            SendMessage("StopAction"); // Interrupt any other actions
            MoveScript.Move(position);
        }
        else
            Orders.AddLast(new Order(OrderName.Mouse1, position, null));
    }
    public override void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
            {
                foreach (Order order in Orders)
                    if (order.BuildingGhost)
                        order.BuildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned--;
                Orders.Clear();
            }

            if (target.GetComponent<Unit>())
            {
                
                SendMessage("StopAction");
                MoveScript.Move(target.transform.position);
                
            }
            else
            {
                Debug.Log("Order can't be interpreted");
            }
        }
        else
            Orders.AddLast(new Order(OrderName.Mouse1, Vector3.zero, target));
    }
    #endregion

    public override bool doingNothing()
    {
        return !MoveScript.isInAction() && !UnitBuildScript.isInAction();
    }

    IEnumerator UnitDeath()
    {
        basicUnitAnim.Play("BasicUnitDeath");
        yield return new WaitForSeconds(1);
        GameObject.Destroy(gameObject);
    }

    public override void UnitDie()
    {
        // TODO 
        if (faction.Equals(Faction.Blue))
            ScoreRegister.AddRedScore(1);
        else if (faction.Equals(Faction.Red))
            ScoreRegister.AddBlueScore(1);

        if (Interactionable && !this.GetComponent<HerosUnit>())
        {
            GetGUI.DesactivateButtons();
            GetGUI.DesctivateProductionBar();
            GetGUI.GUIButtons.Clear();
        }
        //}
        foreach (Order order in Orders)
        {
            if (order.BuildingGhost)
            {
                order.BuildingGhost.GetComponent<BuildingGhost>().NbWorkersAssigned--;
            }
        }
        SendMessage("StopAction");
        Destroy(SmallHealthBar);
        Destroy(gameObject);
    }

    #region Getters and Setters

    public CreationBuilding GetBuilding(OrderName order)
    {
        if (order.Equals(OrderName.Build0))
            return Buildings[0];
        else if (order.Equals(OrderName.Build1))
            return Buildings[1];
        else if (order.Equals(OrderName.Build2))
            return Buildings[2];

        return null;
    }
    public UnitBuild UnitBuildScript
    {
        get
        {
            return _unitBuildScript;
        }

        set
        {
            _unitBuildScript = value;
        }
    }

    public CreationBuilding[] Buildings
    {
        get
        {
            return buildings;
        }

        set
        {
            buildings = value;
        }
    }
    #endregion
}
