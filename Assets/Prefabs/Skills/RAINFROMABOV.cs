using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAINFROMABOV : Skill {

    [SerializeField] private float projectileSpeed = 300;

    [SerializeField] private float damage = 50;

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        
        Vector3 heading = TargetPosition - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;

        GetComponent<Rigidbody>().velocity = direction * ProjectileSpeed;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Unit>())
        {
            if (collider.GetComponent<Unit>().faction.Equals(Faction))
            {
                if (TargetFriendlies)
                {
                    if (collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.Building))
                            DealDamage(collider.GetComponent<Unit>());
                    }

                    else if (collider.GetComponent<HerosUnit>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.HerosUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>() && !collider.GetComponent<HerosUnit>() && !collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.BasicUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<WalkingUnit>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.WalkingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.AttackingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {

                if (TargetEnemies)
                {
                    if (collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.Building))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<HerosUnit>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.HerosUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>() && !collider.GetComponent<HerosUnit>() && !collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.BasicUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<WalkingUnit>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.WalkingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.AttackingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }

    public void DealDamage(Unit targetHit)
    {
        print("deal dmg");
        targetHit.GetDamage(Sender, damage);
    }

    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }
    public float ProjectileSpeed
    {
        get
        {
            return projectileSpeed;
        }

        set
        {
            projectileSpeed = value;
        }
    }
}
