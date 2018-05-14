using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public abstract class RTSGameObject : MonoBehaviour, IEqualityComparer<RTSGameObject>
{
    public GameMaster gameMaster; 
    public Faction faction;
    private int _unitNumber;

    private Animator _animator;

    [SerializeField] private string _nameGo;
    [SerializeField] protected GUIRTSGameObject _GUI;
    [SerializeField] private UnitType _unitType;
    [SerializeField] private bool interactionable;

    protected virtual void Awake()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        Animator = GetComponent<Animator>();
        GiveNumber();

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public bool Interactionable
    {
        get
        {
            return interactionable;
        }

        set
        {
            interactionable = value;
        }
    }

    public void GiveNumber()
    {
        UnitNumber = gameMaster.InitUnitNumber;
        gameMaster.InitUnitNumber++;
    }
    public int UnitNumber
    {
        get
        {
            return _unitNumber;
        }
        set
        {
            if (value != gameMaster.InitUnitNumber)
                return;
            _unitNumber = value;
        }
    }
    public UnitType UnitType
    {
        get
        {
            return _unitType;
        }
        set
        {
            _unitType = value;
        }
    }

    #region Getters/Setters
    public string NameGo
    {
        get
        {
            return _nameGo;
        }

        set
        {
            _nameGo = value;
        }
    }
    #endregion

    #region GUI
    public GUIRTSGameObject GetGUI
    {
        get
        {
            return _GUI;
        }
    }

    public Animator Animator
    {
        get
        {
            return _animator;
        }

        set
        {
            _animator = value;
        }
    }



    public void LoadButtonSet(ButtonSet nameButton)
    {  
        _GUI.ActualButtonSetActive = nameButton;
    } // No one, just for builder
    public ButtonSet GetButtonSet()
    {
        return _GUI.ActualButtonSetActive;
    } // No one, just for builder
    #endregion

    #region IEqualityComparer
    public bool Equals(RTSGameObject x, RTSGameObject y)
    {
        if ((x == null && y != null) || (y == null && x != null))
            return false;
        if (x.GetType() != y.GetType())
            return false;

        return x.UnitNumber == y.UnitNumber;
    }

    public int GetHashCode(RTSGameObject obj)
    {
        return obj.UnitNumber.GetHashCode();
    }
    #endregion
}

public enum OrderName
{
    Mouse1, // Right click
    Attack,
    Move,
    Stop,
    Patrol,
    Collect,
    Skill0,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5,
    Skill6,
    RallyPoint,
    Build,
    Create0,
    Create1,
    Create2,
    Create3,
    Create4,
    Create5,
    Create6,
    Build0,
    Build1,
    Build2,
    Cancel
} 

public enum ButtonSet
{
    Basic, // Basic set of buttons (move, attack, etc)
    Build // Buildings availables
}

public enum UnitType
{
    WalkingUnit,
    AttackingUnit,
    Building,
    HerosUnit,
    BasicUnit,
    CollectableObject,
    Collectable
}

public enum Faction
{
    Red,
    Blue,
    Neutral
}

public struct Order
{
    private OrderName _name;
    private Vector3 _position; // The position where the order was given
    private RTSGameObject _gameObject; // The potential gameObject found on the way of the mouse when the order was given
    private GameObject _buildingGhost;

    public Order(OrderName _name, Vector3 _position, RTSGameObject _gameObject, GameObject _buildingGhost)
    {
        this._name = _name;
        this._position = _position;
        this._gameObject = _gameObject;
        this._buildingGhost = _buildingGhost;
    }
    public Order(OrderName _name, Vector3 _position, RTSGameObject _gameObject)
    {
        this._name = _name;
        this._position = _position;
        this._gameObject = _gameObject;
        this._buildingGhost = null;
    }

    #region Getters/Setters
    public OrderName Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }
    public Vector3 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }
    public RTSGameObject GameObject
    {
        get
        {
            return _gameObject;
        }
        set
        {
            _gameObject = value;
        }
    }
    public GameObject BuildingGhost
    {
        get { return _buildingGhost; }
        set { _buildingGhost = value; }

    }
    #endregion
}

public struct GUIRTSGameObject
{
    private Texture _preview;
    private ButtonSet actualButtonSetActive; // Which set of button should this unit shows ?
    private Dictionary<ButtonSet, List<GameObject>> _GUIButtons;
    private GameObject[] _productionBar;

    public GUIRTSGameObject(Texture preview, Dictionary<ButtonSet, List<GameObject>> buttons)
    {
        _preview = preview;
        actualButtonSetActive = ButtonSet.Basic;
        _GUIButtons = buttons;
        _productionBar = new GameObject[6];
    }

    public void ActivateSetButton()
    {
        if (_GUIButtons != null)
        {
            DesactivateButtons();

            List<GameObject> buttons = null;

            _GUIButtons.TryGetValue(ActualButtonSetActive, out buttons);
            foreach (GameObject btn in buttons)
                btn.SetActive(true);

            ActivateProductionBar();
        }
    }

    public void DesactivateButtons()
    {
        if (_GUIButtons != null)
        {
            if (_GUIButtons.Values.Count > 0)
            {
                foreach (List<GameObject> lists in _GUIButtons.Values)
                {
                    foreach (GameObject btn in lists)
                    {
                        if (btn != null)
                            btn.SetActive(false);
                        else
                            Debug.LogWarning("A button was null");
                    }
                }
                DesctivateProductionBar();
            }
            else
                Debug.LogWarning("No Buttons");
        }
    }
    public void ActivateProductionBar()
    {
        for (int i = 0; i < ProductionBar.Length; i++)
        {
            if (ProductionBar[i] != null)
            {
                ProductionBar[i].SetActive(true);
            }
        }
    }
    public void DesctivateProductionBar()
    {
        for (int i = 0; i < _productionBar.Length; i++)
        {
            if (_productionBar[i] != null)
            {
                _productionBar[i].SetActive(false);
            }
        }
    }

    #region Getters/Setters
    public Dictionary<ButtonSet, List<GameObject>> GUIButtons
    {
        get
        {
            return _GUIButtons;
        }
        set
        {
            _GUIButtons = value;
        }
    }
    public GameObject[] ProductionBar
    {
        get
        {
            return _productionBar;
        }
        set
        {
            _productionBar = value;
        }
    }
    public ButtonSet ActualButtonSetActive
    {
        get { return actualButtonSetActive; }
        set { actualButtonSetActive = value; }
    }

    public Texture Preview
    {
        get
        {
            return _preview;
        }

        set
        {
            _preview = value;
        }
    }

    #endregion
}

public enum BuildingName
{
    Barrack, TownHall
}