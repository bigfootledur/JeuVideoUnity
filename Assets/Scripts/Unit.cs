using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : RTSGameObject {

    //private UnitTakeCollectableObject collectableObjectScript;
    [SerializeField] private Material inAreaMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private float _regenManaPerSecond;

    [SerializeField] private float _healthMax;
    [SerializeField] private float _manaMax;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _currentMana;

    private ArrayList _actions = new ArrayList();

    [SerializeField] private GameObject smallHealthBarPrefab; 
    [SerializeField] private GameObject smallHealthBar;
    [SerializeField] private GameObject smallManaBarPrefab;
    [SerializeField] private GameObject smallManaBar;
    private List<Buff> _buffs = new List<Buff>(); 

    private float defense = 5;
    private float actualTime;
    private bool timeSet = false;
    //private CollectableObject[] inventory = new CollectableObject[6];
    [SerializeField] private int cost;
    [SerializeField] private float xpValue = 0;
    [SerializeField] private TriggerXp triggerXpArea;

    protected override void Awake()
    {
        base.Awake();

        if (this.faction.Equals(Faction.Red) && this.GetComponent<MeshRenderer>())
            this.GetComponent<MeshRenderer>().materials[0].color = Color.red;
        else if (this.faction.Equals(Faction.Blue) && this.GetComponent<MeshRenderer>())
            this.GetComponent<MeshRenderer>().materials[0].color = Color.blue;

    }

    protected override void Start()
    {
        base.Start();
        Vector3 pointOnUI = Camera.main.WorldToScreenPoint(transform.position);

        if (SmallHealthBarPrefab)
        {
            Vector3 healthBarPosition = pointOnUI + Vector3.up * 20;
            SmallHealthBar = Instantiate(SmallHealthBarPrefab, healthBarPosition, Quaternion.identity, GameObject.Find("Canvas").transform);
            SmallHealthBar.SetActive(true);
            SmallHealthBar.GetComponent<Image>().fillAmount = CurrentHealth / HealthMax;
        }

        if (SmallManaBarPrefab)
        {
            Vector3 manaBarPosition = pointOnUI + Vector3.up * 15;
            SmallManaBar = Instantiate(SmallManaBarPrefab, manaBarPosition, Quaternion.identity, GameObject.Find("Canvas").transform);
            SmallManaBar.SetActive(true);
            SmallManaBar.GetComponent<Image>().fillAmount = CurrentMana / ManaMax;
        }
    }

    protected override void Update()
    {
        base.Update();

        if(RegenManaPerSecond > 0)
            HandlingRegenHealthRegenMana();
        HandlingHealthBarPosition();
    }

    private void HandlingRegenHealthRegenMana()
    {
        if (!timeSet)
        {
            actualTime = Time.time;
            timeSet = true;
        }

        if (Time.time - actualTime >= 1 / RegenManaPerSecond)
        {
            if(CurrentMana < ManaMax)
                CurrentMana++;
            timeSet = false;
        }
    }
    private void HandlingHealthBarPosition()
    {
        Vector3 pointOnUI = Camera.main.WorldToScreenPoint(transform.position);
        if (SmallHealthBar)
        {
            Vector3 healthBarPosition = pointOnUI + Vector3.up * 20;
            SmallHealthBar.transform.position = healthBarPosition;
            SmallHealthBar.GetComponent<Image>().fillAmount = CurrentHealth / HealthMax;
        }

        if (SmallManaBar)
        {
            Vector3 manaBarPosition = pointOnUI + Vector3.up * 15;
            SmallManaBar.transform.position = manaBarPosition;
            SmallManaBar.GetComponent<Image>().fillAmount = CurrentMana / ManaMax;
        }
    }
    
    public abstract bool doingNothing();

    public void SendDirectStopOrder()
    {
        SendStopOrder(true, false);
    }
    public void SendPostponeStopOrder()
    {
        SendStopOrder(false, true);
    }
    public void DoPostponeStopOrder()
    {
        SendStopOrder(false, false);
    }
    public abstract void SendStopOrder(bool directOrder, bool postpone);

    public void SendDirectMouse1Order(Vector3 position)
    {
        SendMouse1Order(position, true, false);
    }
    public void SendDirectMouse1Order(RTSGameObject target)
    {
        SendMouse1Order(target, true, false);
    }
    public void SendPostponeMouse1Order(Vector3 position)
    {
        SendMouse1Order(position, false, true);
    }
    public void SendPostponeMouse1Order(RTSGameObject target)
    {
        SendMouse1Order(target, false, true);
    }

    public void DoPostponeMouse1Order(Vector3 position)
    {
        SendMouse1Order(position, false, false);
    }
    public void DoPostponeMouse1Order(RTSGameObject target)
    {
        SendMouse1Order(target, false, false);
    }

    public abstract void SendMouse1Order(Vector3 position, bool directOrder, bool postpone);
    public abstract void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone);

    public virtual void GetDamage(Unit sender, float damage)
    {
        // Reduce damage with defense

        sender.TriggerGetDamageBuffs(sender, damage);
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
            UnitDie();
    }

    public void TriggerGetDamageBuffs(Unit sender, float damage)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            sender.Buffs[i].GetDamage(sender, damage);
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > HealthMax)
            CurrentHealth = HealthMax;
    }

    public void RestoreMana(float amount)
    {
        CurrentMana += amount;
        if (CurrentMana > ManaMax)
            CurrentMana = ManaMax;
    }

    public void addAction(OrderName action)
    {
        for (int i = 0; i < Actions.Count; i++)
            if (Actions[i].Equals(action))
                return;

        Actions.Add(action);
    }

    public abstract void InitialiseButtons();

    public void InitialiseGUI()
    {
        InitialiseButtons();
    }
    public virtual void UnitDie()
    {
        //if (!GameMaster.Player1Hero)
        //{
        if (faction.Equals(Faction.Blue))
            ScoreRegister.AddRedScore(1);
        else if (faction.Equals(Faction.Red))
            ScoreRegister.AddBlueScore(1);

        if (Interactionable && !this.GetComponent<HerosUnit>())
        {
            GetGUI.DesactivateButtons();
            GetGUI.DesctivateProductionBar();
            GetGUI.GUIButtons.Clear();
        }
        //}
        if(GetComponentInChildren<TriggerXp>())
            GetComponentInChildren<TriggerXp>().GiveExp();

        DestroyIt();

    }

    /// <summary>
    /// To call after aniamtion and every shit needed when a unit die
    /// </summary>
    public void DestroyIt()
    {
        Destroy(SmallHealthBar);
        Destroy(SmallManaBar);
        Destroy(gameObject);
    }
    #region Getters/Setters
    public float Defense
    {
        get
        {
            float defenseBuff = 1;
            for (int i = 0; i < Buffs.Count; i++)
                defenseBuff *= Buffs[i].DefenseMultiplier;

            return defense * defenseBuff;
        }

        set
        {
            defense = value;
        }
    }

    public List<Buff> Buffs
    {
        get
        {
            return _buffs;
        }

        set
        {
            _buffs = value;
        }
    }
    public GameObject SmallHealthBarPrefab
    {
        get
        {
            return smallHealthBarPrefab;
        }

        set
        {
            smallHealthBarPrefab = value;
        }
    }

    public GameObject SmallHealthBar
    {
        get
        {
            return smallHealthBar;
        }

        set
        {
            smallHealthBar = value;
        }
    }
    public ArrayList Actions
    {
        get
        {
            return _actions;
        }
    }
    public float HealthMax
    {
        get
        {
            return _healthMax;
        }

        set
        {
            _healthMax = value;
        }
    }

    public float ManaMax
    {
        get
        {
            return _manaMax;
        }

        set
        {
            _manaMax = value;
        }
    }

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            _currentHealth = value;
        }
    }

    public float CurrentMana
    {
        get
        {
            return _currentMana;
        }

        set
        {
            _currentMana = value;
        }
    }

    public GameObject SmallManaBar
    {
        get
        {
            return smallManaBar;
        }

        set
        {
            smallManaBar = value;
        }
    }

    public GameObject SmallManaBarPrefab
    {
        get
        {
            return smallManaBarPrefab;
        }

        set
        {
            smallManaBarPrefab = value;
        }
    }

    public float RegenManaPerSecond
    {
        get
        {
            return _regenManaPerSecond;
        }

        set
        {
            _regenManaPerSecond = value;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }

        set
        {
            cost = value;
        }
    }

    public float XpValue
    {
        get
        {
            return xpValue;
        }

        set
        {
            xpValue = value;
        }
    }

    public Material InAreaMaterial
    {
        get
        {
            return inAreaMaterial;
        }

        set
        {
            inAreaMaterial = value;
        }
    }

    public Material DefaultMaterial
    {
        get
        {
            return defaultMaterial;
        }

        set
        {
            defaultMaterial = value;
        }
    }

    //public CollectableObject[] Inventory
    //{
    //    get { return inventory; }
    //    set { inventory = value; }
    //}

    //public UnitTakeCollectableObject CollectableObjectScript
    //{
    //    get
    //    {
    //        return collectableObjectScript;
    //    }

    //    set
    //    {
    //        collectableObjectScript = value;
    //    }
    //}

    //public void GoTakeObject(CollectableObject collectableObject)
    //{
    //    int i = 0;
    //    while (i < Inventory.Length && Inventory[i] != null) i++;

    //    if (i < Inventory.Length)
    //    {
    //        CollectableObjectScript.TakeObject(collectableObject);
    //    }
    //    else
    //        print("Inventory full");
    //}

    //public void TakeObject(CollectableObject collectableObject)
    //{
    //    int i = 0;
    //    while (i < Inventory.Length && Inventory[i] != null) i++;

    //    if (i < Inventory.Length)
    //    {
    //        Inventory[i] = collectableObject;
    //        if(collectableObject.GetComponent<PassiveCollectableObject>())
    //            Buffs.Add(collectableObject.GetComponent<PassiveCollectableObject>().Buff);

    //        collectableObject.TakeObjectAnimation();
    //    }
    //    else
    //        print("Inventory full");
    //}
    //public CollectableObject[] Inventory()
    //{
    //    return inventory;
    //}
    #endregion


    //public bool HasInventory()
    //{
    //    return Inventory != null || Inventory.Length > 0;
    //}


}