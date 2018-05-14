using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitAttack))]
public class AttackingUnit : WalkingUnit {

    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float range = 30f;
    [SerializeField] private float damage = 10f;

    private UnitAttack _attackScript;

    [SerializeField] private float rangeAggroWhenAttack;

    protected override void Awake()
    {
        base.Awake();

        AttackScript = gameObject.GetComponent<UnitAttack>();

        addAction(OrderName.Attack);
        addAction(OrderName.Stop);
        addAction(OrderName.Move);
        addAction(OrderName.Patrol);
    }

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
                    else if (order.GameObject != null && (order.Position.Equals(Vector3.zero)))
                        DoPostponeAttackOrder(order.GameObject);
                }

                else if (order.Name.Equals(OrderName.Attack))
                {
                    if (order.GameObject == null && !(order.Position.Equals(Vector3.zero)))
                        DoPostponeAttackOrder(order.Position);
                    else if (order.GameObject != null && (order.Position.Equals(Vector3.zero)))
                        DoPostponeAttackOrder(order.GameObject);
                }
                else if (order.Name.Equals(OrderName.Stop))
                    DoPostponeStopOrder();
                else if (order.Name.Equals(OrderName.Move))
                    DoPostponeMoveOrder(order.Position);
                else if (order.Name.Equals(OrderName.Patrol))
                    DoPostponePatrolOrder(order.Position);
                else
                    Debug.Log("Order not yet implemented");

                // Remove this order when it has been executed
                Orders.RemoveFirst();
            }
        }
    }

    public void SendAttackOrder(Vector3 position, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();
            // To be implemented
            SendMessage("StopAction");
            AttackScript.MoveAttack(position);
        }
        else
            Orders.AddLast(new Order(OrderName.Attack, position, null));
    }
    public void SendAttackOrder(RTSGameObject target, bool directOrder, bool postpone)
    {
        if (!postpone)
        {
            if (directOrder)
                Orders.Clear();
            if (target.GetComponent<Unit>())
            {
                if (target.faction == Faction.Neutral)
                    Debug.Log("Can't attack this target");

                else if (target.faction != this.faction)
                {
                    SendMessage("StopAction"); // Interrupt any other actions
                    AttackScript.Attack(target);
                }
            }
            else if (target.GetComponent<Collectable>())
            {

            }
            else if (target.GetComponent<CollectableObject>())
            {

            }
        }
        else
            Orders.AddLast(new Order(OrderName.Attack, Vector3.zero, target));
    }

    public void SendDirectAttackOrder(Vector3 position)
    {
        SendAttackOrder(position, true, false);
    }
    public void SendDirectAttackOrder(RTSGameObject target)
    {
        SendAttackOrder(target, true, false);
    }
    public void SendPostponeAttackOrder(Vector3 position)
    {
        SendAttackOrder(position, false, true);
    }
    public void SendPostponeAttackOrder(RTSGameObject target)
    {
        SendAttackOrder(target, false, true);
    }
    public void DoPostponeAttackOrder(Vector3 position)
    {
        SendAttackOrder(position, false, false);
    }
    public void DoPostponeAttackOrder(RTSGameObject target)
    {
        SendAttackOrder(target, false, false);
    }

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
        if (!postpone)
        {
            if (target.GetComponent<Unit>())
            {
                if (target.faction == Faction.Neutral)
                    Debug.Log("Can't attack this target");

                else if (target.faction != this.faction)
                {
                    SendMessage("StopAction"); // Interrupt any other actions
                    AttackScript.Attack(target);
                }
            }
            else if (target.GetComponent<Collectable>()) // Goldmine, wood
            {
                MoveScript.Move(target.transform.position);
            }

            else if (target.GetComponent<CollectableObject>()) // Collectable object
            {
                // Take the Object
            }
        }
        else
            Orders.AddLast(new Order(OrderName.Mouse1, Vector3.zero, target));
    }

    public override void GetDamage(Unit sender, float damage)
    {
        sender.TriggerGetDamageBuffs(sender, damage);
        CurrentHealth -= damage;
        //UpdateHealth();
        if (CurrentHealth <= 0)
            UnitDie();

        if (Vector3.Distance(transform.position, sender.transform.position) < RangeAggroWhenAttack)
        {
            AttackScript.Aggro(sender);
        }
    }

    public override bool doingNothing()
    {
        return !AttackScript.isInAction() && !MoveScript.isInAction();
    }

    #region Getters/Setters
    public float AttackSpeed
    {
        get
        {
            float attackSpeedBuff = 1;
            for (int i = 0; i < Buffs.Count; i++)
            {
                attackSpeedBuff *= Buffs[i].AttackSpeedMultiplier;
            }
            return attackSpeed * attackSpeedBuff;
        }
        set { attackSpeed = value; }
    }

    public float Range
    {
        get { return range; }
        set { range = value; }
    }

    public float Damage
    {
        get
        {
            float damageBuff = 1;
            for (int i = 0; i < Buffs.Count; i++)
                damageBuff *= Buffs[i].DamageMultiplier;
            return damage * damageBuff;
        }
        set { damage = value; }
    }
    public UnitAttack AttackScript
    {
        get { return _attackScript; }
        set { _attackScript = value; }
    }
    public float RangeAggroWhenAttack
    {
        get { return rangeAggroWhenAttack; }
        set { rangeAggroWhenAttack = value; }
    }
    #endregion
}
