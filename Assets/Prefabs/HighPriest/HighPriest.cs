using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighPriest : SkilledBasicUnit {

    private Aggro aggroScript;

    protected override void Awake() 
    {
        base.Awake();

        aggroScript = GetComponentInChildren<Aggro>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (aggroScript.NearFriendliesUnit.Count >= 6)
        {
            if (CurrentMana >= UnitSkill.ManaCost(SkillLevel))
            {
                Skill skill = Instantiate(this.UnitSkill, transform.position, this.UnitSkill.transform.rotation);
                skill.Faction = this.faction;
                skill.Sender = this;
                skill.TargetPosition = transform.position;
                CurrentMana -= skill.ManaCost(SkillLevel);
            }
        }
    }
}
