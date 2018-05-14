using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour {

    [SerializeField] private Image iconDisplay; // The icon of the skill
    [SerializeField] private string nameSkill;
    [SerializeField] private string description;
    [SerializeField] private int skillLevel = 0; // Use this only for GetDamage function
    [SerializeField] private Faction faction;

    [SerializeField] private bool skillShot;
    [SerializeField] private GameObject areaSkillShot;
    [SerializeField] private bool targetted;
    [SerializeField] private bool passive;

    [SerializeField] private Unit sender;
    [SerializeField] private bool targetSelf;

    [SerializeField] private bool targetEnemies;
    [SerializeField] private List<UnitType> targetablesEnemies;

    [SerializeField] private bool targetFriendlies;
    [SerializeField] private List<UnitType> targetablesFriendlies;

    [SerializeField] private Vector3 targetPosition;

    //[SerializeField] private float duration;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private int levelRequired;

    [SerializeField] private List<ListSkillAttributes> listSkillAttributes; // Mana, damge, cooldown, etc


    //public AudioClip audioclip;
    public AudioSource audioSource;

    protected virtual void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    protected virtual void Start()
    {
        Invoke("SoftEndOfTheSkill", Duration(SkillLevel));
    }

    public virtual void SoftEndOfTheSkill()
    {
        if (ps)
        {
            ps.Stop();
            Invoke("EndOfTheSkill", ps.main.startLifetimeMultiplier);
        }
        else
            EndOfTheSkill();
    }

    public virtual void EndOfTheSkill()
    {
        Destroy(this.gameObject);
    }

    #region Getters/Setters
    public bool SkillShot
    {
        get { return skillShot; }
        set { skillShot = value; }
    }

    public List<UnitType> TargetablesEnemies
    {
        get { return targetablesEnemies; }
        set { targetablesEnemies = value; }
    }

    public bool TargetFriendlies
    {
        get { return targetFriendlies; }
        set { targetFriendlies = value; }
    }

    public List<UnitType> TargetablesFriendlies
    {
        get { return targetablesFriendlies; }
        set { targetablesFriendlies = value; }
    }

    public string NameSkill
    {
        get { return nameSkill; }
        set { nameSkill = value; }
    }

    public Faction Faction
    {
        get
        {
            return faction;
        }

        set
        {
            faction = value;
        }
    }


    public Unit Sender
    {
        get
        {
            return sender;
        }

        set
        {
            sender = value;
        }
    }

    public bool TargetSelf
    {
        get
        {
            return targetSelf;
        }

        set
        {
            targetSelf = value;
        }
    }

    public bool TargetEnemies
    {
        get
        {
            return targetEnemies;
        }

        set
        {
            targetEnemies = value;
        }
    }

    public bool Passive
    {
        get
        {
            return passive;
        }

        set
        {
            passive = value;
        }
    }



    public bool Targetted
    {
        get
        {
            return targetted;
        }

        set
        {
            targetted = value;
        }
    }
    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }

        set
        {
            targetPosition = value;
        }
    }

    public float Cooldown(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.Cooldown))
                {
                    //print(list[i].value);
                    return list[i].value;
                }
            }
        }

        return 0;
    }

    public float ManaCost(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.ManaCost))
                    return list[i].value;
            }
        }

        return 0;
    }

    public float Duration(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.Duration))
                    return list[i].value;
            }
        }
        return 0;
    }

    public float HealAmount(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.HealAmount))
                    return list[i].value;
            }
        }
        return 0;
    }

    public string Description
    {
        get
        {
            return description;
        }

        set
        {
            description = value;
        }
    }

    public GameObject AreaSkillShot
    {
        get
        {
            return areaSkillShot;
        }

        set
        {
            areaSkillShot = value;
        }
    }

    public ParticleSystem Ps
    {
        get
        {
            return ps;
        }

        set
        {
            ps = value;
        }
    }

    public int LevelRequired
    {
        get
        {
            return levelRequired;
        }

        set
        {
            levelRequired = value;
        }
    }

    public List<ListSkillAttributes> ListSkillAttributes1
    {
        get
        {
            return listSkillAttributes;
        }

        set
        {
            listSkillAttributes = value;
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
    #endregion
}

public enum NameSkillAttribute
{
    AreaRadius,
    Damage,
    ProjectileSpeed,
    Cooldown,
    ManaCost,
    Range,
    Duration,
    LevelRequired,
    DamageMultiplier,
    AttackSpeedMultiplier,
    MoveSpeedMultiplier,
    DefenseMultiplier,
    HealAmount
}

[Serializable]
public struct SkillAttribute
{
    [SerializeField] public NameSkillAttribute nameSkillAttribute;
    [SerializeField] public float value;
}

[Serializable]
public struct ListSkillAttributes
{
    [SerializeField] public List<SkillAttribute> skillAttributes;
}
