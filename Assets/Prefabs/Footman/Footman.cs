using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Footman : AttackingUnit
{

    public Animator basicUnitAnim;

    protected override void Awake()
    {
        base.Awake();

        addAction(OrderName.Attack);
        addAction(OrderName.Stop);
        addAction(OrderName.Move);
        addAction(OrderName.Patrol);
        addAction(OrderName.Collect);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (!Interactionable)
        {
            GetGUI.GUIButtons.Clear();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    #region Orders Implementations

    //public override void SendMouse1Order(Vector3 position, bool directOrder, bool postpone)
    //{
    //    if (!postpone)
    //    {
    //        if (directOrder)
    //            Orders.Clear();

    //        SendMessage("StopAction"); // Interrupt any other actions
    //        MoveScript.Move(position);
    //    }
    //    else
    //        Orders.AddLast(new Order(OrderName.Mouse1, position, null));
    //}
    //public override void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone)
    //{
    //    if (!postpone)
    //    {
    //        if (target.GetComponent<Unit>())
    //        {
    //            if (target.faction == Faction.Neutral)
    //                Debug.Log("Can't attack this target");

    //            else if (target.faction != this.faction)
    //            {
    //                SendMessage("StopAction"); // Interrupt any other actions
    //                AttackScript.Attack(target);
    //            }
    //            else
    //            {
    //                SendMessage("StopAction");
    //                MoveScript.Move(target.transform.position);
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Order can't be interpreted");
    //        }
    //    }
    //    else
    //        Orders.AddLast(new Order(OrderName.Mouse1, Vector3.zero, target));
    //}
    #endregion

    //public override bool doingNothing()
    //{
    //    return !MoveScript.isInAction() && !MoveScript.IsPatroling && !AttackScript.IsAttacking;
    //}

    IEnumerator UnitDeath()
    {
        basicUnitAnim.Play("BasicUnitDeath");
        yield return new WaitForSeconds(1);
        GameObject.Destroy(gameObject);
    }

    public void DesactivateGUI()
    {
        GetGUI.DesactivateButtons();
    }

    #region Getters and Setters

    #endregion
}
