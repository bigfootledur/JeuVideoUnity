using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    private float _damageMultiplier;
    private float _attackSpeedMultiplier;
    private float _defenseMultiplier;
    private float _speedMultiplier;

    public Buff(float damageMultiplier, float attackSpeedMultiplier, float defenseMultiplier, float speedMultiplier)
    {
        _damageMultiplier = damageMultiplier;
        _attackSpeedMultiplier = attackSpeedMultiplier;
        _defenseMultiplier = defenseMultiplier;
        _speedMultiplier = speedMultiplier;
    }

    public float DamageMultiplier
    {
        get { return _damageMultiplier; }
        set { _damageMultiplier = value; }
    }

    public float AttackSpeedMultiplier
    {
        get { return _attackSpeedMultiplier; }
        set { _attackSpeedMultiplier = value; }
    }

    public float DefenseMultiplier
    {
        get { return _defenseMultiplier; }
        set { _defenseMultiplier = value; }
    }

    public float SpeedMultiplier
    {
        get { return _speedMultiplier; }
        set { _speedMultiplier = value; }
    }

    public virtual void GetDamage(Unit sender, float damage) { } // Add a behavior when a unit deal damage
}