using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiOUHAHAHA : Skill
{
    [SerializeField] private Buff skillBuff;

    //[SerializeField] private float damageMultiplier;
    //[SerializeField] private float attackSpeedMultiplier;
    //[SerializeField] private float defenseMultiplier;
    //[SerializeField] private float speedMultiplier;

    [SerializeField] private List<Unit> unitsInArea = new List<Unit>();

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        skillBuff = new Buff(DamageMultiplier(SkillLevel), AttackSpeedMultiplier(SkillLevel), DefenseMultiplier(SkillLevel), MoveSpeedMultiplier(SkillLevel));
    }
	
	// Update is called once per frame
	void Update () {
        if (Sender)
            transform.position = Sender.transform.position;
        else
            Destroy(gameObject);
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Unit>())
        {
            for (int i = 0; i < TargetablesFriendlies.Count; i++)
            {
                if (TargetablesFriendlies[i].Equals(collider.GetComponent<Unit>().UnitType) && 
                    collider.GetComponent<Unit>().faction.Equals(Sender.faction))
                {
                    unitsInArea.Add(collider.GetComponent<Unit>());
                    collider.GetComponent<Unit>().Buffs.Add(skillBuff);
                }
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Unit>())
        {
            for (int i = 0; i < TargetablesFriendlies.Count; i++)
            {
                if (TargetablesFriendlies[i].Equals(collider.GetComponent<Unit>().UnitType) &&
                    collider.GetComponent<Unit>().faction.Equals(Sender.faction))
                {
                    unitsInArea.Remove(collider.GetComponent<Unit>());
                    collider.GetComponent<Unit>().Buffs.Remove(skillBuff);
                }
            }
        }
    }

    public override void EndOfTheSkill()
    {
        for (int i = 0; i < unitsInArea.Count; i++)
        {
            unitsInArea[i].Buffs.Remove(skillBuff);
        }
        Destroy(this.gameObject);
    }

    public float DamageMultiplier(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.DamageMultiplier))
                    return list[i].value;
            }
        }
        return 0;
    }

    public float AttackSpeedMultiplier(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.AttackSpeedMultiplier))
                    return list[i].value;
            }
        }

        return 0;
    }

    public float MoveSpeedMultiplier(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.MoveSpeedMultiplier))
                    return list[i].value;
            }
        }
        return 0;
    }

    public float DefenseMultiplier(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.DefenseMultiplier))
                    return list[i].value;
            }
        }

        return 0;
    }
}
