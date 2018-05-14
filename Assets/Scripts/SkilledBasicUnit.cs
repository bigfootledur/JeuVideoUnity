using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkilledBasicUnit : AttackingUnit {

    [SerializeField] private Skill unitSkill;
    [SerializeField] private float cdSKill;
    [SerializeField] private int skillLevel = 1;

    public Skill UnitSkill
    {
        get
        {
            return unitSkill;
        }

        set
        {
            unitSkill = value;
        }
    }

    public float CdSKill
    {
        get
        {
            return cdSKill;
        }

        set
        {
            cdSKill = value;
        }
    }

    public int SkillLevel
    {
        get
        {
            return skillLevel;
        }

        set
        {
            skillLevel = value;
        }
    }
}
