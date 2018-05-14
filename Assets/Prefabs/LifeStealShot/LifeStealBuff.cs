using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LifeStealBuff : Buff
{
    private float _percentageLifeSteal;

    public LifeStealBuff(float damageMultiplier, float attackSpeedMultiplier, float defenseMultiplier, float speedMultiplier, float percentageLifeSteal) : base(damageMultiplier, attackSpeedMultiplier, defenseMultiplier, speedMultiplier)
    {
        PercentageLifeSteal = percentageLifeSteal;
    }

    public override void GetDamage(Unit sender, float damage)
    {
        sender.Heal(damage * _percentageLifeSteal);
    }

    public float PercentageLifeSteal
    {
        get
        {
            return _percentageLifeSteal;
        }

        set
        {
            _percentageLifeSteal = value;
        }
    }

}

