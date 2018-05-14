using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HerosUnit : AttackingUnit {

    [SerializeField] private List<Skill> skills;
    private int _nbObjectCarried;

    private float[] cdSkill = new float[4];
    private int[] levelsSkill = new int[4];

    private bool hasPassiveSkill;

    [SerializeField] private Image[] DisplayCooldown;
    [SerializeField] private Image[] DisplaySkill;
    [SerializeField] private Image[] displaynewSkillButtons;

    [SerializeField] private int newSkillPoints;
    [SerializeField] private float currentExperiencePoints = 0;
    //[SerializeField] private int currentHerosLevel = 1;
    [SerializeField] private float xpToPassLevel = 500;
    [SerializeField] private float levelCoef = 0;
    [SerializeField] private Scrollbar currentExperiencePointsDisplayer;
    [SerializeField] private Text currentHerosLevelDisplayer;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < LevelsSkill.Length; i++)
        {
            LevelsSkill[i] = -1;
        }
        Inventory = new CollectableObject[6];

        for (int i = 0; i < Inventory.Length; i++)
            Inventory[i] = null;

        UpdateExperiencePointsDisplay();
        UpdateSkillAvailableDisplay();
        UpdateNewSkillButtonsDisplay();
        hasPassiveSkill = HasPassiveSkill();
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].Passive)
            {
                DisplaySkill[i].color = new Color(DisplaySkill[i].color.r, DisplaySkill[i].color.g, DisplaySkill[i].color.b, 0.5f);
            }
            //print(Skills[i].name + " is level " + Skills[i].SkillLevel);
        }
    }

    protected override void Update()
    {
        base.Update();
        //CurrentExperiencePoints += 10;
        HandlingPassiveSkills();
        HandlingCooldowns();
    }

    public override void HandlingPostponeOrders()
    {
        if (Orders.Count > 0)
        {
            print(Orders.Count);
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

                //if (Orders.First.Value.GameObject == null && Orders.First.Value.Position.Equals(Vector3.zero))
                //    this.DoPostponeOrder(Orders.First.Value.Name);
                //else if (Orders.First.Value.GameObject != null && Orders.First.Value.Position.Equals(Vector3.zero))
                //    this.DoPostponeOrder(Orders.First.Value.Name, Orders.First.Value.GameObject);
                //else if (Orders.First.Value.GameObject == null && !Orders.First.Value.Position.Equals(Vector3.zero))
                //    this.DoPostponeOrder(Orders.First.Value.Name, Orders.First.Value.Position);
                else
                    Debug.Log("Order not yet implemented");

                // Remove this order when it has been executed
                Orders.RemoveFirst();
            }
        }
    }

    private void HandlingCooldowns()
    {
        for (int i = 0; i < CdSkill.Length; i++)
        {
            if (CdSkill[i] > 0)
                CdSkill[i] -= Time.deltaTime;
            if (CdSkill[i] < 0)
                CdSkill[i] = 0;
        }
        UpdateCooldownDisplay();
    }

    private void UpdateSkillAvailableDisplay()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if(LevelsSkill[i] <= -1)
            {
                DisplaySkill[i].color = new Color(80, 80, 80);
                DisplaySkill[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                DisplaySkill[i].color = new Color(255, 255, 255);
                DisplaySkill[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    private void UpdateCooldownDisplay()
    {
        for (int i = 0; i < Skills.Count; i++)
            if (Skills[i])
            {
                DisplayCooldown[i].fillAmount = 1 - (Skills[i].Cooldown(LevelsSkill[i]) - CdSkill[i]) / Skills[i].Cooldown(LevelsSkill[i]);
            }
    }

    private void HandlingPassiveSkills()
    {
        for (int i = 0; i < Skills.Count; i++)
            if (LevelsSkill[i] >= 0 && Skills[i].Passive)
                SendDirectSkillOrder(Skills[i], transform.position);
    }

    public void SendSkillOrder(Skill skill, Vector3 position, bool directOrder, bool postpone)
    {
        if (CooldownReady(skill))
        {
            if (CurrentMana >= skill.ManaCost(LevelsSkill[GetSkillNumber(skill)]))
            {
                CurrentMana -= skill.ManaCost(LevelsSkill[GetSkillNumber(skill)]);
                if (!skill.Passive)
                    SendMessage("StopAction");
                transform.LookAt(position);
                Skill skillInstantiated = Instantiate(skill, transform.position, skill.transform.rotation);
                skillInstantiated.Faction = this.faction;
                skillInstantiated.Sender = this;
                skillInstantiated.TargetPosition = position;

                CdSkill[GetSkillNumber(skill)] = skill.Cooldown(LevelsSkill[GetSkillNumber(skill)]);
            }
            else
            {
                print("Not enough mana");
            }
        }

    }
    public void SendSkillOrder(Skill skill, Unit target, bool directOrder, bool postpone)
    {
        if (!skill.Passive)
            SendMessage("StopAction");
        print("sendskill0target");
    }
    public void SendSkillOrder(Skill skill, bool directOrder, bool postpone)
    {
        if (CooldownReady(skill))
        {
            if (CurrentMana >= skill.ManaCost(LevelsSkill[GetSkillNumber(skill)]))
            {
                CurrentMana -= skill.ManaCost(LevelsSkill[GetSkillNumber(skill)]);
                if (!skill.Passive)
                    SendMessage("StopAction");

                Skill skillInstantiated = Instantiate(skill, transform.position, skill.transform.rotation);
                skillInstantiated.Faction = this.faction;
                skillInstantiated.Sender = this;
                skillInstantiated.transform.position = this.transform.position;

                CdSkill[GetSkillNumber(skill)] = skill.Cooldown(LevelsSkill[GetSkillNumber(skill)]);
            }
            else
            {
                print("Not enough mana");
            }
        }
    }

    public override bool doingNothing()
    {
        return !MoveScript.isInAction() && !AttackScript.IsAttacking;
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
                SendMessage("StopAction");
                MoveScript.Move(target.transform.position);
            }

            else if (target.GetComponent<CollectableObject>()) // Collectable object
            {
                SendMessage("StopAction");
                GoTakeObject(target.GetComponent<CollectableObject>());
            }
        }
        else
            Orders.AddLast(new Order(OrderName.Mouse1, Vector3.zero, target));
    }

    public Skill GetSkill(OrderName skillName)
    {
        if (skillName.Equals(OrderName.Skill0))
            return Skills[0];
        else if (skillName.Equals(OrderName.Skill1))
            return Skills[1];
        else if (skillName.Equals(OrderName.Skill2))
            return Skills[2];
        else
            return Skills[3];
    }

    public void SendSkill0Order(Vector3 position, bool directOrder, bool postpone)
    {
        if (CdSkill[0] <= 0)
        {
            SendMessage("StopAction");
            transform.LookAt(position);
            Skill skill = Instantiate(GetSkill(OrderName.Skill0), transform.position, GetSkill(OrderName.Skill0).transform.rotation);
            skill.Faction = this.faction;
            skill.Sender = this;
            skill.TargetPosition = position;
            CdSkill[0] = skill.Cooldown(LevelsSkill[0]);
        }
    }
    public void SendSkill0Order(Unit target, bool directOrder, bool postpone)
    {
        SendMessage("StopAction");
        print("sendskill0target");
    }
    public void SendSkill0Order(bool directOrder, bool postpone)
    {
        SendMessage("StopAction");
        print("sendskill0");
    }

    public void SendSkill1Order(Vector3 position, bool directOrder, bool postpone)
    {
        SendMessage("StopAction");
        transform.LookAt(position);
        Skill skill = Instantiate(GetSkill(OrderName.Skill1), transform.position, GetSkill(OrderName.Skill1).transform.rotation);
        skill.Faction = this.faction;
        skill.Sender = this;

        CdSkill[1] = skill.Cooldown(LevelsSkill[1]);
    }
    public void SendSkill1Order(Unit target, bool directOrder, bool postpone)
    {
        SendMessage("StopAction");
        print("sendskill0target");
    }
    public void SendSkill1Order(bool directOrder, bool postpone)
    {
        Skill tmp = GameObject.Instantiate(GetSkill(OrderName.Skill1), transform.position, GetSkill(OrderName.Skill1).transform.rotation);
        tmp.Faction = this.faction;
        tmp.Sender = this;
    }

    public void RemoveObject(CollectableObject collectableObject)
    {
        if (collectableObject.BuffObject != null)
            Buffs.Remove(collectableObject.BuffObject);
    }

    #region Orders Implementation
    public void SendDirectSkillOrder(Skill skill, Unit target)
    {
        SendSkillOrder(skill, target, true, false);
    }
    public void SendDirectSkillOrder(Skill skill, Vector3 position)
    {
        SendSkillOrder(skill, position, true, false);
    }
    public void SendDirectSkillOrder(Skill skill)
    {
        SendSkillOrder(skill, true, false);
    }

    public void SendDirectSkill0Order(Unit target)
    {
        SendSkill0Order(target, true, false);
    }
    public void SendDirectSkill0Order(Vector3 position)
    {
        SendSkill0Order(position, true, false);
    }
    public void SendDirectSkill0Order()
    {
        SendSkill0Order(true, false);
    }

    public void SendDirectSkill1Order(Unit target)
    {
        SendSkill1Order(target, true, false);
    }
    public void SendDirectSkill1Order(Vector3 position)
    {
        SendSkill1Order(position, true, false);
    }
    public void SendDirectSkill1Order()
    {
        SendSkill1Order(true, false);
    }
    #endregion

    public bool CooldownReady(Skill skill)
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].Equals(skill))
            {
                if (CdSkill[i] <= 0)
                    return true;
                else
                    return false;
            }
        }
        throw new Exception("Couldn't find skill named " + skill.NameSkill);
    }

    public bool GotEnoughMana(Skill skill)
    {
        return CurrentMana >= skill.ManaCost(LevelsSkill[GetSkillNumber(skill)]);
    }

    public int GetSkillNumber(Skill skill)
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].Equals(skill))
                return i;
        }
        return -1;
    }
    public Skill GetSkillByNumber(int number)
    {
        if (number < Skills.Count)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (i == number)
                {
                    return Skills[i];
                }
            }

            // Won't go here
            return null;
        }
        else
        {
            Debug.LogError(number + " is too high");
            return null;
        }
    }

    public bool HasPassiveSkill()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if(Skills[i] != null)
                if (Skills[i].Passive)
                    return true;
        }
        return false;
    }

    public void UpdateExperiencePointsDisplay()
    {
        double discriminant;
        double b;
        b = (((2 * XpToPassLevel) - LevelCoef) / (2 * LevelCoef));
        discriminant = Math.Sqrt(((b * b) + (2 * (CurrentExperiencePoints / LevelCoef))));

        double levelAndExp = -b + discriminant;

        currentExperiencePointsDisplayer.size = Math.Abs(Convert.ToSingle(levelAndExp - Math.Truncate(levelAndExp)));
        currentHerosLevelDisplayer.text = "Level " + HerosLevel();
    }

    public int HerosLevel()
    {
        double discriminant;
        double b;
        b = (((2 * XpToPassLevel) - LevelCoef) / (2 * LevelCoef));
        discriminant = Math.Sqrt(((b * b) + (2 * (CurrentExperiencePoints / LevelCoef))));

        double levelAndExp = -b + discriminant;
        return Convert.ToInt32(Math.Truncate(levelAndExp)) + 1;
    }

    /* Method with coef */
    //public void UpdateExperiencePointsDisplay()
    //{
    //    if (CurrentExperiencePoints < xpToPassLevel)
    //    {
    //        currentExperiencePointsDisplayer.size = CurrentExperiencePoints / xpToPassLevel;
    //        currentHerosLevelDisplayer.text = "Level " + 1;
    //    }
    //    else
    //    {
    //        double levelAndExp = 0;
    //        if (levelCoef > 0)
    //            levelAndExp = (Math.Log(CurrentExperiencePoints / xpToPassLevel) + Math.Log(levelCoef + 1)) / Math.Log(levelCoef + 1);
    //        else
    //            levelAndExp = CurrentExperiencePoints / xpToPassLevel;
    //        currentExperiencePointsDisplayer.size = Math.Abs(Convert.ToSingle(levelAndExp - Math.Truncate(levelAndExp)));
    //        currentHerosLevelDisplayer.text = "Level " + HerosLevel();
    //    }
    //}

    //public int HerosLevel()
    //{
    //    double levelAndExp = 0;
    //    if (levelCoef > 0)
    //        levelAndExp = (Math.Log(CurrentExperiencePoints / xpToPassLevel) + Math.Log(levelCoef + 1)) / Math.Log(levelCoef + 1);
    //    else
    //        levelAndExp = CurrentExperiencePoints / xpToPassLevel;
    //    return Convert.ToInt32(Math.Truncate(levelAndExp)) + 1;
    //}

    public void GetXp(float amount)
    {
        int level = HerosLevel();
        CurrentExperiencePoints += amount;
        if(level < HerosLevel())
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        NewSkillPoints++;
    }

    public void SpendSkillPoint(int skillNumber)
    {
        if(skillNumber < Skills.Count && Skills[skillNumber])
        {
            if (Skills[skillNumber].LevelRequired <= HerosLevel() && LevelsSkill[skillNumber] + 1 < Skills[skillNumber].ListSkillAttributes1.Count)
            {
                LevelsSkill[skillNumber]++;
                NewSkillPoints--;
            }
            else
            {
                print("Not enough strongness");
            }
        }
    }


    private void UpdateNewSkillButtonsDisplay()
    {
        if (NewSkillPoints > 0)
            for (int i = 0; i < DisplaynewSkillButtons.Length; i++)
            {
                if (Skills[i].ListSkillAttributes1.Count > LevelsSkill[i] + 1
                 && Skills[i].LevelRequired <= HerosLevel())
                    DisplaynewSkillButtons[i].gameObject.SetActive(true);
                else
                    DisplaynewSkillButtons[i].gameObject.SetActive(false);
            }

        else
            for (int i = 0; i < DisplaynewSkillButtons.Length; i++)
                DisplaynewSkillButtons[i].gameObject.SetActive(false);
    }

    #region Getters/Setters

    public List<Skill> Skills
    {
        get { return skills; }
        set { skills = value; }
    }
    public float[] CdSkill
    {
        get { return cdSkill; }
        set { cdSkill = value; }
    }

    public float CurrentExperiencePoints
    {
        get
        {
            return currentExperiencePoints;
        }

        set
        {
            currentExperiencePoints = value;
            UpdateExperiencePointsDisplay();
        }
    }

    public Image[] DisplaynewSkillButtons
    {
        get
        {
            return displaynewSkillButtons;
        }

        set
        {
            displaynewSkillButtons = value;
        }
    }

    public int NewSkillPoints
    {
        get
        {
            return newSkillPoints;
        }

        set
        {
            newSkillPoints = value;
            UpdateSkillAvailableDisplay();
            UpdateNewSkillButtonsDisplay();
        }
    }

    public int[] LevelsSkill
    {
        get
        {
            return levelsSkill;
        }

        set
        {
            levelsSkill = value;
        }
    }

    public float XpToPassLevel
    {
        get
        {
            return xpToPassLevel;
        }

        set
        {
            xpToPassLevel = value;
        }
    }

    public float LevelCoef
    {
        get
        {
            return levelCoef;
        }

        set
        {
            levelCoef = value;
        }
    }

    #endregion
}