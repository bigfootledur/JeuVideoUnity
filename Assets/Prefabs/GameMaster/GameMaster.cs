using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    public static Faction PlayerFaction; // Blue for hero, red for constructor
    public LevelManager levelManager;
    public GameObject blueWinCondition;
    public GameObject redWinCondition;

    //public static bool Player1Hero = true;
    //public static bool Player2Hero = false;

    [SerializeField] private int redWool = 100;
    [SerializeField] private int blueWool = 100;

    [SerializeField] private Text displayRedWool;
    [SerializeField] private Text displayBlueWool;

    // Useful stuff for the game
    public GameObject herosPrefab;
    public GameObject basicUnitPrefab;
    public GameObject barackPrefab;
    public GameObject townHallPrefab;
    public GameObject constructorPrefab;
    public GameObject gameSettingsPrefab;
    public GameObject buttonPrefab;
    public GameObject projectorPrefab;

    public Camera mainCamera;
    public Material unitPreSelectionMaterial;
    public Material unitAfterSelectionMaterial;
    public Material collectableObjectPreSelectionMaterial;
    public Material collectableObjectAfterSelectionMaterial;
    public Material enemyPreSelectionMaterial;
    public Material enemyAfterSelectionMaterial;
    public Terrain terrain;
    public GameObject selectunit_text;

    public Texture2D cursorAttackTexture;
    public Texture2D cursorDestinationTexture;
    public Texture2D cursorRallyPointTexture;
    public Texture2D cursorCollectTexture;

    public Vector3 RallyPointBlueUnits;
    public Vector3 RallyPointRedUnits;

    [SerializeField] private bool playingAsHero = true;

    // Units gestion
    public int InitUnitNumber;
    private List<RTSGameObject> _unitsInGame;
    public List<BuildingName> buildingsAvailables;

    public MeshRenderer meshBuildingGhost;
    private GameObject buildingGhost;
    private Shader buildingGhostShader;

    private GameObject buildingToCreate;
    private bool buildingGhostCreated = false;

    // Unit selection
    private List<RTSGameObject> _selectedRTSGameObjects;
    private Vector3 originalPosition; // The position of the square to select units (when the user first press the mouse button)
    private Vector3 currentPosition; // The second position of the square to select units (when the user release the mouse button)
    private bool isSelecting;
    private bool isAPreSelectedUnit;

    // Order token
    private bool _orderMoveGiven; // If the player click on "Move" Button
    private bool _orderAttackGiven;
    private bool _orderPatrolGiven;
    private bool _orderCollectGiven;
    private bool _orderRallyPointGiven;
    private bool _orderBuildGiven;
    [SerializeField] private bool _orderSkill0Given;
    [SerializeField] private bool _orderSkill1Given;
    [SerializeField] private bool _orderSkill2Given;
    [SerializeField] private bool _orderSkill3Given;
    [SerializeField] private GameObject skillAreaEffect;
    private CollectableObject _orderDropItemGiven = null;
    private bool post;

    // GUI
    private bool GUIprinted;
    private RTSGameObject _firstUnit;
    private bool firstUnitToken;
    private RTSGameObject _unitChanged;
    [SerializeField] private GameObject herosStuff;
    private Vector2 selectedInformationsPanelHerosAnchorMin = new Vector2(0.08487595f, 0);
    private Vector2 selectedInformationsPanelHerosAnchorMax = new Vector2(0.2697105f, 0.2093246f);
    private Vector2 unitFocusPanelHerosAnchorMin = new Vector2(0.268f, 0);
    private Vector2 unitFocusPanelHerosAnchorMax = new Vector2(0.362f, 0.2093246f);
    [SerializeField] private GameObject constructorStuff;
    private Vector2 selectedInformationsPanelConstructorAnchorMin = new Vector2(0.5975625f, 0);
    private Vector2 selectedInformationsPanelConstructorAnchorMax = new Vector2(0.782125f, 0.2093246f);
    private Vector2 unitFocusPanelConstructorAnchorMin = new Vector2(0.5026034f, 0);
    private Vector2 unitFocusPanelConstructorAnchorMax = new Vector2(0.5975625f, 0.2093246f);

    [SerializeField] private GameObject selectedInformationsPanel;
    [SerializeField] private GameObject unitFocusPanel;
    [SerializeField] private RawImage unitPreview;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text hitPoints;
    [SerializeField] private GameObject shopInformationsPanel;
    [SerializeField] private GameObject nameSelectedDisplay;
    [SerializeField] private GameObject attackSelectedDisplay;
    [SerializeField] private GameObject attackSpeedSelectedDisplay;
    [SerializeField] private GameObject moveSpeedSelectedDisplay;
    [SerializeField] private GameObject defenseSelectedDisplay;

    // Inventory
    public Image[] items = new Image[6];

    void Awake() {
        PlayerFaction = (Faction)PlayerPrefsManager.GetFaction();

        // Variables initialisation
        _unitsInGame = new List<RTSGameObject>();

        SelectedRTSGameObjects = new List<RTSGameObject>();

        isSelecting = false;
        isAPreSelectedUnit = false;

        _orderMoveGiven = _orderAttackGiven = _orderPatrolGiven = _orderCollectGiven = _orderRallyPointGiven = false;

        GUIprinted = false;
        firstUnitToken = false;
        _unitChanged = null;

        buildingsAvailables = new List<BuildingName>();
        UnlockBuilding(BuildingName.Barrack);
        UnlockBuilding(BuildingName.TownHall);

        RallyPointBlueUnits = new Vector3(35f, 9.54f, 97f);
        RallyPointRedUnits = new Vector3(400f, 9.54f, 97f);

        if (PlayerFaction.Equals(Faction.Blue))
        {
            displayBlueWool.gameObject.SetActive(true);
            displayRedWool.gameObject.SetActive(false);
        }

        else if (PlayerFaction.Equals(Faction.Red))
        {
            displayBlueWool.gameObject.SetActive(false);
            displayRedWool.gameObject.SetActive(true);
        }

    }

    void Start()
    {
        if (PlayerFaction.Equals(Faction.Red))
            displayRedWool.text = RedWool.ToString();
        else if (PlayerFaction.Equals(Faction.Blue))
            displayBlueWool.text = BlueWool.ToString();
        RTSGameObject[] gameObjectsOnScene = GameObject.FindObjectsOfType<RTSGameObject>();
        for (int i = 0; i < gameObjectsOnScene.Length; i++)
        {
            if (!UnitsInGame.Contains(gameObjectsOnScene[i])) 
                UnitsInGame.Add(gameObjectsOnScene[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandlingWinCondition();
        UpdatePlayersInformationsDisplay();

        CleanSelection();
        HandlingPostponeOrders();

        SelectMouseSymbol();

        if (isAPreSelectedUnit)
        {
            // Unpreselect everything
            HandlingUnpreselectRTSGameObjects();
        }

        // Preselect the unit under the cursor
        HandlingPreselectRTSGameObjects();

        HandlingHerosSkillsInputs();
        HandlingBasicUnitsInteractionsInputs();
        
        HandlingLeftClickDown();
        HandlingLeftClickHold();
        HandlingLeftClickUp();

        if (UnitsSelectedAreInFaction())
            HandlingRightClickDown();
    }

    public void HandlingWinCondition()
    {
        if(blueWinCondition == null)
        {
            ScoreRegister.Winner = Faction.Blue;
            levelManager.LoadScene("GameOver");
        }
        else if (redWinCondition == null)
        {
            ScoreRegister.Winner = Faction.Red;
            levelManager.LoadScene("GameOver");
        }
        
    }

    public void UpdatePlayersInformationsDisplay()
    {            
        if (PlayerFaction.Equals(Faction.Blue))
        {
            displayBlueWool.gameObject.SetActive(true);
            displayRedWool.gameObject.SetActive(false);
            displayBlueWool.text = BlueWool.ToString();
            herosStuff.SetActive(true);
            constructorStuff.SetActive(false);
            selectedInformationsPanel.GetComponent<RectTransform>().anchorMin = selectedInformationsPanelHerosAnchorMin;
            selectedInformationsPanel.GetComponent<RectTransform>().anchorMax = selectedInformationsPanelHerosAnchorMax;
            unitFocusPanel.GetComponent<RectTransform>().anchorMin = unitFocusPanelHerosAnchorMin;
            unitFocusPanel.GetComponent<RectTransform>().anchorMax = unitFocusPanelHerosAnchorMax;
        }

        else if (PlayerFaction.Equals(Faction.Red))
        {
            displayBlueWool.gameObject.SetActive(false);
            displayRedWool.gameObject.SetActive(true);
            displayRedWool.text = RedWool.ToString();
            herosStuff.SetActive(false);
            constructorStuff.SetActive(true);
            selectedInformationsPanel.GetComponent<RectTransform>().anchorMin = selectedInformationsPanelConstructorAnchorMin;
            selectedInformationsPanel.GetComponent<RectTransform>().anchorMax = selectedInformationsPanelConstructorAnchorMax;
            unitFocusPanel.GetComponent<RectTransform>().anchorMin = unitFocusPanelConstructorAnchorMin;
            unitFocusPanel.GetComponent<RectTransform>().anchorMax = unitFocusPanelConstructorAnchorMax;
        }
    }

    public bool UnitsSelectedAreInFaction()
    {
        return SelectedRTSGameObjects.Count > 0 && SelectedRTSGameObjects[0] 
               && SelectedRTSGameObjects[0].GetComponent<RTSGameObject>().faction.Equals(PlayerFaction);
    }

    /// <summary>
    /// Remove any empty case
    /// </summary>
    private void CleanSelection()
    {
        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
            if(SelectedRTSGameObjects[i] == null)
                SelectedRTSGameObjects.Remove(SelectedRTSGameObjects[i]);
    }

    void SelectMouseSymbol()
    {
        // Printing the mouse symbol
        int symbol = 0;
        if (_orderMoveGiven || _orderPatrolGiven)
            symbol = 1;
        else if (_orderAttackGiven)
            symbol = 2;
        else if (_orderSkill0Given || _orderSkill1Given || _orderSkill2Given || _orderSkill3Given)
        {
            if (skillAreaEffect)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                {
                    skillAreaEffect.transform.position = new Vector3(hit.point.x, hit.point.y + 5f, hit.point.z);
                }
            }
            symbol = 2;

        }
        else if (_orderRallyPointGiven)
            symbol = 3;
        else if (_orderCollectGiven)
            symbol = 4;

        //Prebuilding Render
        else if (_orderBuildGiven)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                bool constructorHere = false;
                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                {
                    if (SelectedRTSGameObjects[0].GetComponent<Constructor>())
                        constructorHere = true;

                }
                if (buildingGhost && constructorHere)
                    buildingGhost.transform.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
                else
                {
                    Destroy(buildingGhost);
                    _orderBuildGiven = false;
                }
            }
        }

        else if (_orderDropItemGiven)
        {
            symbol = 1;
        }
        if (!isSelecting)
            ChangingMouseSymbol(symbol);
    }

    private void HandlingUnpreselectRTSGameObjects()
    {
        for (int i = 0; i < UnitsInGame.Count; i++)
        {
            if (UnitsInGame[i] != null)
            {
                if (UnitsInGame[i].gameObject.GetComponentInChildren<Projector>().enabled)
                {
                    if(UnitsInGame[i].gameObject.GetComponent<Unit>() && UnitsInGame[i].gameObject.GetComponent<Unit>().faction.Equals(PlayerFaction))
                        UnitsInGame[i].gameObject.GetComponentInChildren<Projector>().material = unitAfterSelectionMaterial;
                    else if(UnitsInGame[i].gameObject.GetComponent<CollectableObject>() ||
                            UnitsInGame[i].gameObject.GetComponent<Collectable>())
                        UnitsInGame[i].gameObject.GetComponentInChildren<Projector>().material = collectableObjectAfterSelectionMaterial;
                    else if(UnitsInGame[i].gameObject.GetComponent<Unit>() && UnitsInGame[i].gameObject.GetComponent<Unit>().faction.Equals(PlayerFaction) &&
                            UnitsInGame[i].gameObject.GetComponent<CollectableObject>() && UnitsInGame[i].gameObject.GetComponent<CollectableObject>())
                        UnitsInGame[i].gameObject.GetComponentInChildren<Projector>().material = enemyAfterSelectionMaterial;
                }
                if (!IsSelected(UnitsInGame[i]))
                    unpreselectRTSGameObject(UnitsInGame[i]);
            }
        }
        isAPreSelectedUnit = false;
    }

    private void HandlingPostponeOrders()
    {
        post = false;
        if (Input.GetKey(KeyCode.LeftShift))
            post = true;
    }

    private void HandlingRightClickDown()
    {
        // Right mouse button pressed
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_orderAttackGiven || _orderMoveGiven || _orderCollectGiven || _orderPatrolGiven || _orderRallyPointGiven || 
                _orderBuildGiven || _orderSkill0Given || _orderSkill1Given || _orderSkill2Given || _orderSkill3Given || _orderDropItemGiven)
            {
                _orderAttackGiven = _orderMoveGiven = _orderCollectGiven = _orderPatrolGiven = _orderRallyPointGiven = _orderBuildGiven = _orderSkill0Given = _orderSkill1Given = _orderSkill2Given = _orderSkill3Given = false;
                _orderDropItemGiven = null;
                if(SelectedRTSGameObjects[0].GetComponent<Constructor>())
                    SelectedRTSGameObjects[0].GetComponent<Constructor>().LoadButtonSet(ButtonSet.Basic);
                skillAreaEffect.GetComponent<ColorUnitsWhenEnterArea>().DestroyIt();
                Destroy(buildingGhost);
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.gameObject.layer == 9) // Terrain's layer
                        {
                            if (!post)
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                        if (SelectedRTSGameObjects[i].GetComponent<Unit>())
                                            SelectedRTSGameObjects[i].GetComponent<Unit>().SendDirectMouse1Order(hit.point);
                            }
                            else
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                        if (SelectedRTSGameObjects[i].GetComponent<Unit>())
                                            SelectedRTSGameObjects[i].GetComponent<Unit>().SendPostponeMouse1Order(hit.point);
                            }
                        }

                        else if (hit.transform.gameObject.GetComponent<Unit>()) // Unit layer
                        {
                            if (!post)
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                        if (SelectedRTSGameObjects[i].GetComponent<Unit>())
                                            SelectedRTSGameObjects[i].GetComponent<Unit>().SendDirectMouse1Order(hit.transform.gameObject.GetComponent<RTSGameObject>());
                            }
                            else
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                        if (SelectedRTSGameObjects[i].GetComponent<Unit>())
                                            SelectedRTSGameObjects[i].GetComponent<Unit>().SendPostponeMouse1Order(hit.transform.gameObject.GetComponent<RTSGameObject>());
                            }
                        }
                        else if (hit.transform.gameObject.GetComponent<CollectableObject>())
                        {
                            for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                            {
                                if (SelectedRTSGameObjects[i] &&
                                    SelectedRTSGameObjects[i].GetComponent<WalkingUnit>() &&
                                    SelectedRTSGameObjects[i].GetComponent<WalkingUnit>().HasInventory())
                                {
                                    SelectedRTSGameObjects[i].GetComponent<Unit>().SendDirectMouse1Order(hit.transform.gameObject.GetComponent<CollectableObject>());
                                }
                            }
                        }

                        else if (hit.transform.gameObject.GetComponent<Collectable>())
                        {
                            if (!post)
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                {
                                    if (SelectedRTSGameObjects[i] &&
                                        SelectedRTSGameObjects[i].GetComponent<Collector>())
                                        SelectedRTSGameObjects[i].GetComponent<Collector>().SendDirectCollectOrder(hit.transform.gameObject.GetComponent<Collectable>());
                                }
                            }
                            else
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                {
                                    if (SelectedRTSGameObjects[i] &&
                                        SelectedRTSGameObjects[i].GetComponent<Collector>())
                                        SelectedRTSGameObjects[i].GetComponent<Collector>().SendPostponeCollectOrder(hit.transform.gameObject.GetComponent<Collectable>());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void HandlingLeftClickUp()
    {
        // Left mouse button released
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            // Simple clic
            if (originalPosition.Equals(Input.mousePosition))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform.gameObject.GetComponent<Unit>() ||
                        hit.transform.gameObject.GetComponent<CollectableObject>())
                    {
                        if (!Input.GetKey(KeyCode.LeftControl))
                        {
                            List<RTSGameObject> toRemove = new List<RTSGameObject>();
                            for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                toRemove.Add(SelectedRTSGameObjects[i]);

                            unselectRTSGameObjects(toRemove);
                        }

                        selectRTSGameObject(hit.transform.gameObject.GetComponent<RTSGameObject>());
                    }
                }
            }

            // Drag select
            else
            {
                bool isAHerosUnitHere = false;
                bool isAConstructorHere = false;
                bool isACollectorHere = false;
                bool isACollectableObjectHere = false;

                List<RTSGameObject> tmpSelectedRTSGameObjects = new List<RTSGameObject>();

                for (int i = 0; i < UnitsInGame.Count; i++)
                {
                    if (UnitsInGame[i] != null)
                    {
                        if (IsWithinSelectionBounds(UnitsInGame[i].gameObject))
                        {
                            if (UnitsInGame[i].GetComponent<HerosUnit>())
                                isAHerosUnitHere = true;
                            else if (UnitsInGame[i].GetComponent<Constructor>())
                                isAConstructorHere = true;
                            else if (UnitsInGame[i].GetComponent<Collector>())
                                isACollectorHere = true;
                            else if (UnitsInGame[i].GetComponent<CollectableObject>())
                                isACollectableObjectHere = true;
                            tmpSelectedRTSGameObjects.Add(UnitsInGame[i]);
                        }
                    }
                }

                if (tmpSelectedRTSGameObjects.Count > 0)
                {

                    if (isACollectableObjectHere && !Input.GetKey(KeyCode.LeftControl))
                    {
                        List<RTSGameObject> tmpCollectableObjectSelection = new List<RTSGameObject>();
                        for (int i = 0; i < tmpSelectedRTSGameObjects.Count; i++)
                        {
                            if (tmpSelectedRTSGameObjects[i].GetComponent<CollectableObject>())
                            {
                                tmpCollectableObjectSelection.Add(tmpSelectedRTSGameObjects[i]);
                            }
                        }
                        List<RTSGameObject> toRemove = new List<RTSGameObject>();
                        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                        {
                            toRemove.Add(SelectedRTSGameObjects[i]);
                        }
                        unselectRTSGameObjects(toRemove);
                        selectRTSGameObjects(tmpCollectableObjectSelection);
                    }

                    else if (isAHerosUnitHere && !Input.GetKey(KeyCode.LeftControl))
                    {
                        List<RTSGameObject> tmpHerosSelection = new List<RTSGameObject>();
                        for (int i = 0; i < tmpSelectedRTSGameObjects.Count; i++)
                        {
                            if (tmpSelectedRTSGameObjects[i].GetComponent<HerosUnit>())
                            {
                                tmpHerosSelection.Add(tmpSelectedRTSGameObjects[i]);
                            }
                        }
                        List<RTSGameObject> toRemove = new List<RTSGameObject>();
                        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                        {
                            toRemove.Add(SelectedRTSGameObjects[i]);
                        }
                        unselectRTSGameObjects(toRemove);
                        selectRTSGameObjects(tmpHerosSelection);
                    }

                    else if (isAConstructorHere && !Input.GetKey(KeyCode.LeftControl))
                    {

                        List<RTSGameObject> tmpBasicUnitSelection = new List<RTSGameObject>();
                        for (int i = 0; i < tmpSelectedRTSGameObjects.Count; i++)
                        {
                            if (tmpSelectedRTSGameObjects[i].GetComponent<Constructor>())
                            {
                                tmpBasicUnitSelection.Add(tmpSelectedRTSGameObjects[i]);
                            }
                        }
                        List<RTSGameObject> toRemove = new List<RTSGameObject>();
                        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                        {
                            toRemove.Add(SelectedRTSGameObjects[i]);
                        }
                        unselectRTSGameObjects(toRemove);
                        selectRTSGameObjects(tmpBasicUnitSelection);
                    }
                    else if (isACollectorHere && !Input.GetKey(KeyCode.LeftControl))
                    {

                        List<RTSGameObject> tmpBasicUnitSelection = new List<RTSGameObject>();
                        for (int i = 0; i < tmpSelectedRTSGameObjects.Count; i++)
                        {
                            if (tmpSelectedRTSGameObjects[i].GetComponent<Collector>())
                            {
                                tmpBasicUnitSelection.Add(tmpSelectedRTSGameObjects[i]);
                            }
                        }
                        List<RTSGameObject> toRemove = new List<RTSGameObject>();
                        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                        {
                            toRemove.Add(SelectedRTSGameObjects[i]);
                        }
                        unselectRTSGameObjects(toRemove);
                        selectRTSGameObjects(tmpBasicUnitSelection);
                    }
                    else
                    {

                        if (!Input.GetKey(KeyCode.LeftControl))
                        {
                            List<RTSGameObject> toRemove = new List<RTSGameObject>();
                            for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                            {
                                toRemove.Add(SelectedRTSGameObjects[i]);
                            }
                            unselectRTSGameObjects(toRemove); 
                            //print("after removing : " + _selectedRTSGameObjects.Count);
                        }

                        //print("selecting " + tmpSelectedRTSGameObjects.Count + " units");
                        selectRTSGameObjects(tmpSelectedRTSGameObjects);
                    }
                }
            }
            isSelecting = false;


        }
    }
    private void HandlingLeftClickHold()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            for (int i = 0; i < UnitsInGame.Count; i++)
            {
                if (UnitsInGame[i] != null)
                {
                    if (IsWithinSelectionBounds(UnitsInGame[i].gameObject))
                    {
                        preselectRTSGameObject(UnitsInGame[i]);
                    }
                }
            }
        }
    }
    private void HandlingLeftClickDown()
    {
        // Left mouse button pressed
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!EventSystem.current.IsPointerOverGameObject()) // If the pointer isn't over an UI element
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (UnitsSelectedAreInFaction())
                    {
                        // Move
                        if (_orderMoveGiven)
                        {
                            _orderMoveGiven = false;
                            if (!post)
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                {
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                        if (SelectedRTSGameObjects[i].GetComponent<WalkingUnit>())
                                            SelectedRTSGameObjects[i].GetComponent<WalkingUnit>().SendDirectMoveOrder(hit.point);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                {
                                    {
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                            if (SelectedRTSGameObjects[i].GetComponent<WalkingUnit>())
                                                SelectedRTSGameObjects[i].GetComponent<WalkingUnit>().SendPostponeMoveOrder(hit.point);
                                    }
                                }
                            }
                        }
                        // Attack
                        else if (_orderAttackGiven)
                        {
                            if (hit.transform.gameObject.GetComponentInChildren<RTSGameObject>() != null)
                            {
                                // Target Attack
                                _orderAttackGiven = false;
                                if (!post)
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                            if (SelectedRTSGameObjects[i].GetComponent<AttackingUnit>())
                                                SelectedRTSGameObjects[i].GetComponent<AttackingUnit>().SendDirectAttackOrder(hit.transform.gameObject.GetComponent<RTSGameObject>());
                                }
                                else
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                            if (SelectedRTSGameObjects[i].GetComponent<AttackingUnit>())
                                                SelectedRTSGameObjects[i].GetComponent<AttackingUnit>().SendPostponeAttackOrder(hit.transform.gameObject.GetComponent<RTSGameObject>());
                                }
                            }
                            else
                            {
                                // Move Attack
                                _orderAttackGiven = false;
                                if (!post)
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                            if (SelectedRTSGameObjects[i].GetComponent<AttackingUnit>())
                                                SelectedRTSGameObjects[i].GetComponent<AttackingUnit>().SendDirectAttackOrder(hit.point);
                                }
                                else
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable)
                                            if (SelectedRTSGameObjects[i].GetComponent<AttackingUnit>())
                                                SelectedRTSGameObjects[i].GetComponent<AttackingUnit>().SendPostponeAttackOrder(hit.point);
                                }
                            }
                        }
                        // Patrol
                        else if (_orderPatrolGiven)
                        {
                            _orderPatrolGiven = false;
                            if (!post)
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable
                                        && SelectedRTSGameObjects[i].GetComponent<WalkingUnit>())
                                        SelectedRTSGameObjects[i].GetComponent<WalkingUnit>().SendDirectPatrolOrder(hit.point);
                            }
                            else
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable
                                        && SelectedRTSGameObjects[i].GetComponent<WalkingUnit>())
                                        SelectedRTSGameObjects[i].GetComponent<WalkingUnit>().SendPostponePatrolOrder(hit.point);
                            }
                        }
                        //Rally Point
                        else if (_orderRallyPointGiven)
                        {
                            _orderRallyPointGiven = false;
                            if (hit.transform.gameObject.layer == 9) // Terrain's layer
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<CreationBuilding>()
                                         && SelectedRTSGameObjects[i].Interactionable)
                                        SelectedRTSGameObjects[i].GetComponent<CreationBuilding>().SendDirectRallyPointOrder(hit.point);
                                // Add postpone order
                            }

                            else
                            {
                                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<CreationBuilding>()
                                         && SelectedRTSGameObjects[i].Interactionable)
                                        SelectedRTSGameObjects[i].GetComponent<CreationBuilding>().SendDirectRallyPointOrder(hit.transform.gameObject.GetComponent<RTSGameObject>());
                            }
                        }
                        // Collect
                        else if (_orderCollectGiven)
                        {
                            if (hit.transform.gameObject.GetComponent<Collectable>() != null)
                            {
                                _orderCollectGiven = false;
                                if (!post)
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable
                                            && SelectedRTSGameObjects[i].GetComponent<Collector>())
                                            SelectedRTSGameObjects[i].GetComponent<Collector>().SendDirectCollectOrder(hit.transform.gameObject.GetComponent<Collectable>());
                                }
                                else
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable
                                            && SelectedRTSGameObjects[i].GetComponent<Collector>())
                                            SelectedRTSGameObjects[i].GetComponent<Collector>().SendPostponeCollectOrder(hit.transform.gameObject.GetComponent<Collectable>());
                                }
                            }
                            else
                            {
                                Debug.Log("Select a collectable");
                            }
                        }
                        else if (_orderBuildGiven)
                        {
                            if (CanBeBuiltHere())
                            {
                                if (!post)
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable
                                            && SelectedRTSGameObjects[i].GetComponent<Constructor>())
                                            SelectedRTSGameObjects[i].GetComponent<Constructor>().SendDirectBuildOrder(buildingToCreate, buildingGhost, hit.point);
                                }
                                else
                                {
                                    for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                                        if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].Interactionable
                                            && SelectedRTSGameObjects[i].GetComponent<Constructor>())
                                            SelectedRTSGameObjects[i].GetComponent<Constructor>().SendPostponeBuildOrder(buildingToCreate, buildingGhost, hit.point);
                                }
                                _orderBuildGiven = false;
                                if (SelectedRTSGameObjects[0].GetComponent<Constructor>())
                                    SelectedRTSGameObjects[0].GetComponent<Constructor>().LoadButtonSet(ButtonSet.Basic);

                                //GameObject.Instantiate(buildingGhost.gameObject, hit.point, buildingGhost.gameObject.transform.rotation);
                            }

                        }
                        else if (_orderSkill0Given || _orderSkill1Given || _orderSkill2Given || _orderSkill3Given)
                        {
                            //for (int i = 0; i < _selectedRTSGameObjects.Count; i++)
                            //{
                            //    print(_selectedRTSGameObjects[i]);
                            //}
                            Skill skill = null;
                            if (SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<HerosUnit>() && _orderSkill0Given)
                                skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill0);
                            else if (SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<HerosUnit>() && _orderSkill1Given)
                                skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill1);
                            else if (SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<HerosUnit>() && _orderSkill2Given)
                                skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill2);
                            else if (SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<HerosUnit>() && _orderSkill3Given)
                                skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill3);

                            if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill))
                            {
                                if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                                {
                                    if (skill.SkillShot)
                                    {
                                        // Have to resend a raycast to avoid anything that's not the terrain
                                        RaycastHit hitSkillShot;
                                        Ray raySkillShot = Camera.main.ScreenPointToRay(Input.mousePosition);

                                        if (Physics.Raycast(raySkillShot, out hitSkillShot, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                                        {
                                            //_selectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkill0Order(hitSkillShot.point);
                                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkillOrder(skill, hitSkillShot.point);
                                            _orderSkill0Given = _orderSkill1Given = _orderSkill2Given = _orderSkill3Given = false;
                                            if (skillAreaEffect)
                                                skillAreaEffect.GetComponent<ColorUnitsWhenEnterArea>().DestroyIt();
                                        }

                                    }
                                    else if (skill.Targetted && hit.transform.gameObject.GetComponent<Unit>())
                                    {
                                        Unit targetHit = hit.transform.gameObject.GetComponent<Unit>();
                                        if (CanBeCastOn(skill, targetHit))
                                        {
                                            //_selectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkill0Order(targetHit);
                                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkillOrder(skill, targetHit);
                                            _orderSkill0Given = _orderSkill1Given = _orderSkill2Given = _orderSkill3Given = false;
                                        }
                                        else
                                        {
                                            Debug.Log("Wrong target");
                                        }
                                    }
                                }
                                else
                                {
                                    print(skill.NameSkill + " not ready yet");
                                }
                            }
                            else
                            {
                                print("Not enough mana to cast " + skill.NameSkill);
                            }
                        }
                        else if (_orderDropItemGiven)
                        {
                            if (!post)
                            {
                                if (SelectedRTSGameObjects[0])
                                {
                                    WalkingUnit unit = SelectedRTSGameObjects[0].GetComponent<WalkingUnit>();
                                    if (unit && unit.HasInventory())
                                    {
                                        unit.SendDirectDropItemOrder(_orderDropItemGiven, hit.point);
                                    }
                                }
                            }
                            else
                            {
                                print("Post not implemented yet");
                            }
                            _orderDropItemGiven = null;
                        }

                        else
                        {
                            originalPosition = Input.mousePosition;
                            isSelecting = true;
                        }
                    }

                    else
                    {
                        originalPosition = Input.mousePosition;
                        isSelecting = true;
                    }
                }
            }
        }
    }

    private void HandlingBasicUnitsInteractionsInputs()
    {
        if(UnitsSelectedAreInFaction()) { 
            if (Input.GetButtonDown("Attack"))
                _orderAttackGiven = true;
            if (Input.GetButtonDown("Stop"))
                btnStopClick();
            if (!_orderBuildGiven && Input.GetButtonDown("Build"))
                btnBuildClick();
            if (!_orderBuildGiven && Input.GetButtonDown("Build0") && SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetButtonSet().Equals(ButtonSet.Build))
                btnBuild0Click();
            if (!_orderBuildGiven && Input.GetButtonDown("Build1") && SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetButtonSet().Equals(ButtonSet.Build))
                btnBuild1Click();
        }
    }
    private void HandlingHerosSkillsInputs()
    {
        if (SelectedRTSGameObjects.Count > 0)
        {
            if (Input.GetButtonDown("Skill0"))
            {
                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                {
                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<HerosUnit>())
                    {
                        Skill skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill0);
                        if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().LevelsSkill[0] >= 0 &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill) &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                        {
                            if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill0).SkillShot ||
                                SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill0).Targetted)
                            {
                                _orderSkill0Given = true; _orderSkill1Given = false; _orderSkill2Given = false; _orderSkill3Given = false;
                                Destroy(skillAreaEffect);
                                if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill0).AreaSkillShot)
                                    skillAreaEffect = SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill0).AreaSkillShot;
                            }
                            else
                            {
                                if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill))
                                {
                                    if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                                        SelectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkillOrder(skill);
                                    else
                                    {
                                        print(skill.NameSkill + " not ready yet");
                                    }
                                }
                                else
                                {
                                    print("Not enough mana to cast " + skill.NameSkill);
                                }
                            }
                        }
                        else
                        {
                            print("Skill not learned yet or no mana or no cooldown");
                        }
                    }
                }
            }
            else if (Input.GetButtonDown("Skill1"))
            {
                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                {
                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<HerosUnit>())
                    {
                        Skill skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill1);
                        if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().LevelsSkill[1] >= 0 &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill) &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                        {
                            if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill1).SkillShot ||
                                SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill1).Targetted)
                            {
                                _orderSkill0Given = false; _orderSkill1Given = true; _orderSkill2Given = false; _orderSkill3Given = false;
                                Destroy(skillAreaEffect);
                                if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill1).AreaSkillShot)
                                    skillAreaEffect = SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill1).AreaSkillShot;
            }
                            else
                            {
                                //_selectedRTSGameObjects[i].GetComponent<HerosUnit>().SendDirectSkill0Order();
                                if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill))
                                {
                                    if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                                        SelectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkillOrder(skill);
                                    else
                                    {
                                        print(skill.NameSkill + " not ready yet");
                                    }
                                }
                                else
                                {
                                    print("Not enough mana to cast " + skill.NameSkill);
                                }
                            }
                        }
                        else
                        {
                            print("Skill not learned yet or no mana or no cooldown");
                        }
                    }
                }
            }
            else if (Input.GetButtonDown("Skill2"))
            {
                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                {
                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<HerosUnit>())
                    {
                        Skill skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill2);
                        if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().LevelsSkill[2] >= 0 &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill) &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                        {
                            if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill2).SkillShot ||
                                SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill2).Targetted)
                            {
                                _orderSkill0Given = false; _orderSkill1Given = false; _orderSkill2Given = true; _orderSkill3Given = false;
                                Destroy(skillAreaEffect);
                                if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill2).AreaSkillShot)
                                    skillAreaEffect = SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill2).AreaSkillShot;
                            }
                            else
                            {
                                //_selectedRTSGameObjects[i].GetComponent<HerosUnit>().SendDirectSkill0Order();
                                if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill))
                                {
                                    if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                                        SelectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkillOrder(skill);
                                    else
                                    {
                                        print(skill.NameSkill + " not ready yet");
                                    }
                                }
                                else
                                {
                                    print("Not enough mana to cast " + skill.NameSkill);
                                }
                            }
                        }
                        else
                        {
                            print("Skill not learned yet or no mana or no cooldown");
                        }
                    }
                }
            }
            else if (Input.GetButtonDown("Skill3"))
            {
                for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                {
                    if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<HerosUnit>())
                    {
                        Skill skill = SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GetSkill(OrderName.Skill3);
                        if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().LevelsSkill[3] >= 0 &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill) &&
                            SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                        {
                            if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill3).SkillShot ||
                                SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill3).Targetted)
                            {
                                _orderSkill0Given = false; _orderSkill1Given = false; _orderSkill2Given = false; _orderSkill3Given = true;
                                Destroy(skillAreaEffect);
                                if (SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill3).AreaSkillShot && !skillAreaEffect)
                                    skillAreaEffect = Instantiate(SelectedRTSGameObjects[i].GetComponent<HerosUnit>().GetSkill(OrderName.Skill3).AreaSkillShot,
                                                                  transform.position,Quaternion.identity);
                            }
                            else
                            {
                                
                                if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().GotEnoughMana(skill))
                                {
                                    if (SelectedRTSGameObjects[0].GetComponent<HerosUnit>().CooldownReady(skill))
                                        SelectedRTSGameObjects[0].GetComponent<HerosUnit>().SendDirectSkillOrder(skill);
                                    else
                                    {
                                        print(skill.NameSkill + " not ready yet");
                                    }
                                }
                                else
                                {
                                    print("Not enough mana to cast " + skill.NameSkill);
                                }
                            }
                        }
                        else
                        {
                            print("Skill not learned yet or no mana or no cooldown");
                        }
                    }
                }
            }
        }
    }

    private void HandlingPreselectRTSGameObjects()
    {
        RaycastHit hitmonlee;
        Ray rayoftheyear = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayoftheyear, out hitmonlee, Mathf.Infinity))
        {
            if (hitmonlee.transform.gameObject.GetComponent<Unit>() ||
                hitmonlee.transform.gameObject.GetComponent<CollectableObject>()) // Unit layer
                preselectRTSGameObject(hitmonlee.transform.gameObject.GetComponentInChildren<RTSGameObject>());
        }
        
    }

    public bool CanBeCastOn(Skill skill, RTSGameObject target)
    {
        if (target.faction.Equals(SelectedRTSGameObjects[0].faction))
        {
            if (skill.Sender.gameObject.Equals(target.gameObject))
            {
                if (skill.TargetSelf)
                {
                    return true;
                }
            }

            if (skill.TargetFriendlies)
            {
                if (target.GetComponent<CreationBuilding>())
                {
                    if (skill.TargetablesFriendlies.Contains(UnitType.Building))
                        return true;
                }

                else if (target.GetComponent<HerosUnit>())
                {
                    if (skill.TargetablesFriendlies.Contains(UnitType.HerosUnit))
                        return true;
                }
                else if (target.GetComponent<AttackingUnit>() && !target.GetComponent<HerosUnit>() && !target.GetComponent<CreationBuilding>())
                {
                    if (skill.TargetablesFriendlies.Contains(UnitType.BasicUnit))
                        return true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        else
        {
            if (skill.TargetEnemies)
            {
                if (target.GetComponent<CreationBuilding>())
                {
                    if (skill.TargetablesEnemies.Contains(UnitType.Building))
                        return true;
                }

                else if (target.GetComponent<HerosUnit>())
                {
                    if (skill.TargetablesEnemies.Contains(UnitType.HerosUnit))
                        return true;
                }
                else if (target.GetComponent<AttackingUnit>() && !target.GetComponent<HerosUnit>() && !target.GetComponent<CreationBuilding>())
                {
                    if (skill.TargetablesEnemies.Contains(UnitType.BasicUnit))
                        return true;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        return false;
    }

    public bool CanBeBuiltHere()
    {
        return buildingGhost.GetComponent<BuildingGhost>().canBeBuiltHere;
    }

    #region Gestion of selection
    public void preselectRTSGameObject(RTSGameObject rtsGameObject)
    {
        if (rtsGameObject.GetComponent<Unit>() && rtsGameObject.GetComponent<Unit>().faction.Equals(PlayerFaction))
            rtsGameObject.GetComponentInChildren<Projector>().material = unitPreSelectionMaterial;

        else if (rtsGameObject.GetComponent<CollectableObject>() || rtsGameObject.GetComponent<Collectable>())
            rtsGameObject.GetComponentInChildren<Projector>().material = collectableObjectPreSelectionMaterial;

        else if(rtsGameObject.GetComponent<Unit>() && !rtsGameObject.GetComponent<Unit>().faction.Equals(PlayerFaction) &&
                !(rtsGameObject.GetComponent<CollectableObject>() || rtsGameObject.GetComponent<Collectable>()))
            rtsGameObject.GetComponentInChildren<Projector>().material = enemyPreSelectionMaterial;

        else
            Debug.LogError("Don't know which projector to activate");
        rtsGameObject.GetComponentInChildren<Projector>().enabled = true;
        isAPreSelectedUnit = true;
    }
    public void unpreselectRTSGameObject(RTSGameObject rtsGameObject)
    {
        if (rtsGameObject.GetComponent<Unit>() && rtsGameObject.GetComponent<Unit>().faction.Equals(PlayerFaction))
            rtsGameObject.GetComponentInChildren<Projector>().material = unitAfterSelectionMaterial;

        else if (rtsGameObject.GetComponent<CollectableObject>() || rtsGameObject.GetComponent<Collectable>())
            rtsGameObject.GetComponentInChildren<Projector>().material = collectableObjectAfterSelectionMaterial;

        else if (rtsGameObject.GetComponent<Unit>() && !rtsGameObject.GetComponent<Unit>().faction.Equals(PlayerFaction) &&
                !(rtsGameObject.GetComponent<CollectableObject>() || rtsGameObject.GetComponent<Collectable>()))
            rtsGameObject.GetComponentInChildren<Projector>().material = enemyAfterSelectionMaterial;

        if (!IsSelected(rtsGameObject))
            rtsGameObject.GetComponentInChildren<Projector>().enabled = false;
    }
    public void selectRTSGameObject(RTSGameObject rtsGameObject)
    {
        if (rtsGameObject.GetComponent<Unit>() && rtsGameObject.GetComponent<Unit>().faction.Equals(PlayerFaction))
            rtsGameObject.GetComponentInChildren<Projector>().material = unitAfterSelectionMaterial;

        else if (rtsGameObject.GetComponent<CollectableObject>() || rtsGameObject.GetComponent<Collectable>())
            rtsGameObject.GetComponentInChildren<Projector>().material = collectableObjectAfterSelectionMaterial;

        else if (rtsGameObject.GetComponent<Unit>() && !rtsGameObject.GetComponent<Unit>().faction.Equals(PlayerFaction) &&
        !(rtsGameObject.GetComponent<CollectableObject>() || rtsGameObject.GetComponent<Collectable>()))
            rtsGameObject.GetComponentInChildren<Projector>().material = enemyAfterSelectionMaterial;

        rtsGameObject.GetComponentInChildren<Projector>().enabled = true;
        SelectedRTSGameObjects.Add(rtsGameObject);
    }
    public void selectRTSGameObjects(List<RTSGameObject> rtsGameObjects)
    {
        for(int i = 0; i < rtsGameObjects.Count; i++)
            if (rtsGameObjects[i] != null)
                selectRTSGameObject(rtsGameObjects[i]);
    }
    public void unselectRTSGameObject(RTSGameObject rtsGameObject)
    {
        //int position = 0;
        rtsGameObject.GetComponentInChildren<Projector>().enabled = false;
        rtsGameObject.LoadButtonSet(ButtonSet.Basic);
        SelectedRTSGameObjects.Remove(rtsGameObject);
    }
    public void unselectRTSGameObjects(List<RTSGameObject> rtsGameObjects)
    {
        int rtsGameObjectsCount = rtsGameObjects.Count;
        for (int i = 0; i < rtsGameObjectsCount; i++)
        {
            if (rtsGameObjects[i] != null)
            {
                unselectRTSGameObject(rtsGameObjects[i]);
            }
        }
    }

    /// <summary>
    /// Check weither the specific Rtsgameobject is selected by the player
    /// </summary>
    /// <param name="rtsGameObject"> The Rtsgameobject to check </param>
    /// <returns> True if selected, false otherwise </returns>
    public bool IsSelected(RTSGameObject rtsGameObject)
    {
        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
            if(SelectedRTSGameObjects[i] != null)
                if (SelectedRTSGameObjects[i].Equals(rtsGameObject))
                    return true;
        return false;
    }
    #endregion

    #region Listeneners
    public void btnCreate0Click()
    {
        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
            if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<CreationBuilding>())
                SelectedRTSGameObjects[i].GetComponent<CreationBuilding>().SendDirectCreate0Order();
    }
    public void btnCreate1Click()
    {
        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
            if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<CreationBuilding>())
                SelectedRTSGameObjects[i].GetComponent<CreationBuilding>().SendDirectCreate1Order();
    }
    public void btnCreate2Click()
    {
        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
            if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<CreationBuilding>())
                SelectedRTSGameObjects[i].GetComponent<CreationBuilding>().SendDirectCreate2Order();
    }
    public void btnCancelCreateClick(GameObject btn, int position)
    {
        for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
            if (SelectedRTSGameObjects[i] != null && SelectedRTSGameObjects[i].GetComponent<CreationBuilding>())
                SelectedRTSGameObjects[i].GetComponent<CreationBuilding>().CancelCreate(btn);
    }
    public void btnRallyPointClick()
    {
        _orderRallyPointGiven = true;
    }
    public void btnStopClick()
    {
        if (!post)
        {
            for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                if (SelectedRTSGameObjects[i] != null)
                    if(SelectedRTSGameObjects[i].GetComponent<Unit>())
                        SelectedRTSGameObjects[i].GetComponent<Unit>().SendDirectStopOrder();
        }
        else
        {
            for (int i = 0; i < SelectedRTSGameObjects.Count; i++)
                if (SelectedRTSGameObjects[i] != null)
                    if (SelectedRTSGameObjects[i].GetComponent<Unit>())
                        SelectedRTSGameObjects[i].GetComponent<Unit>().SendPostponeStopOrder();
        }
    }
    public void btnMoveClick()
    {
        _orderMoveGiven = true;
    }
    public void btnAttackClick()
    {
        _orderAttackGiven = true;
    }
    public void btnPatrolClick()
    {
        _orderPatrolGiven = true;
    }
    public void btnCollectClick()
    {
        _orderCollectGiven = true;
    }
    public void btnBuildClick()
    {
        if(SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<Constructor>())
            SelectedRTSGameObjects[0].LoadButtonSet(ButtonSet.Build);
    }
    public void btnCancelClick()
    {
        if (SelectedRTSGameObjects[0])
            SelectedRTSGameObjects[0].LoadButtonSet(ButtonSet.Basic);
    }
    public void btnBuild0Click()
    {
        if (SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<Constructor>())
        {
            buildingToCreate = SelectedRTSGameObjects[0].GetComponent<Constructor>().GetBuilding(OrderName.Build0).gameObject;
            if (!_orderBuildGiven && buildingToCreate.GetComponent<CreationBuilding>() && SelectedRTSGameObjects[0].GetComponent<Unit>().faction.Equals(Faction.Red)
                    && buildingToCreate.GetComponent<CreationBuilding>().Cost <= RedWool)
            {
                _orderBuildGiven = true;

                buildingGhost = Instantiate(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost,
                                            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z)),
                                            buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.rotation);

                buildingGhost.transform.position = new Vector3(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.x,
                                                               0, 
                                                               buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.z);
            }
            else if (!_orderBuildGiven && buildingToCreate.GetComponent<CreationBuilding>() && SelectedRTSGameObjects[0].GetComponent<RTSGameObject>().faction.Equals(Faction.Blue)
                    && buildingToCreate.GetComponent<CreationBuilding>().Cost <= BlueWool)
            {
                _orderBuildGiven = true;

                buildingGhost = Instantiate(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost,
                                            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z)),
                                            buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.rotation);

                buildingGhost.transform.position = new Vector3(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.x, 
                                                               0, 
                                                               buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.z);
            }
            else
            {
                print("can't build because no ressources");
            }
        }
    }
    public void btnBuild1Click()
    {
        if (SelectedRTSGameObjects[0] && SelectedRTSGameObjects[0].GetComponent<Constructor>())
        {
            buildingToCreate = SelectedRTSGameObjects[0].GetComponent<Constructor>().GetBuilding(OrderName.Build1).gameObject;
            if (!_orderBuildGiven && buildingToCreate.GetComponent<CreationBuilding>() && SelectedRTSGameObjects[0].GetComponent<Unit>().faction.Equals(Faction.Red)
                    && buildingToCreate.GetComponent<CreationBuilding>().Cost <= RedWool)
            {
                _orderBuildGiven = true;

                buildingGhost = Instantiate(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost,
                                            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z)),
                                            buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.rotation);

                buildingGhost.transform.position = new Vector3(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.x,
                                                               0,
                                                               buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.z);
            }
            else if (!_orderBuildGiven && buildingToCreate.GetComponent<CreationBuilding>() && SelectedRTSGameObjects[0].GetComponent<RTSGameObject>().faction.Equals(Faction.Blue)
                    && buildingToCreate.GetComponent<CreationBuilding>().Cost <= BlueWool)
            {
                _orderBuildGiven = true;

                buildingGhost = Instantiate(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost,
                                            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.z)),
                                            buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.rotation);

                buildingGhost.transform.position = new Vector3(buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.x,
                                                               0,
                                                               buildingToCreate.GetComponent<CreationBuilding>().BuildingGhost.transform.position.z);
            }
            else
            {
                print("can't build because no ressources");
            }
        }
    }
    public void btnBuild2Click()
    {
        if (SelectedRTSGameObjects[0])
        {
            _orderBuildGiven = true;
            meshBuildingGhost = SelectedRTSGameObjects[0].GetComponent<Constructor>().GetBuilding(OrderName.Build2).gameObject.GetComponent<MeshRenderer>();
            buildingGhost = SelectedRTSGameObjects[0].GetComponent<Constructor>().GetBuilding(OrderName.Build2).gameObject;
        }
    }
    public void btnSkill0Click()
    {
        HerosUnit heros = SelectedRTSGameObjects[0].GetComponent<HerosUnit>();
        
        if (heros.GetSkill(OrderName.Skill0).SkillShot || heros.GetSkill(OrderName.Skill0).Targetted)
            _orderSkill0Given = true;

        else
        {
            //heros.SendDirectSkill0Order();
            heros.SendDirectSkillOrder(heros.GetSkill(OrderName.Skill0));
        }
    }
    #endregion

    void OnGUI()
    {
        if (isSelecting)
        {
            Rect selectionBox = UtilsGUI.GetScreenRect(originalPosition, Input.mousePosition);
            UtilsGUI.DrawScreenRect(selectionBox, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            UtilsGUI.DrawScreenRectBorder(selectionBox, 2, new Color(0.8f, 0.8f, 0.95f));
        }

        if (SelectedRTSGameObjects.Count > 0 && SelectedRTSGameObjects[0] != null)
        {
            //if (_selectedRTSGameObjects[0].Interactionable() && !_selectedRTSGameObjects[0].GetComponent<HerosUnit>())
            //if(!GameMaster.Player1Hero)
            if (SelectedRTSGameObjects[0].faction.Equals(PlayerFaction))
            {
                printButtons(SelectedRTSGameObjects[0]);
                //if (SelectedRTSGameObjects[0].GetComponent<WalkingUnit>() &&
                //    SelectedRTSGameObjects[0].GetComponent<WalkingUnit>().HasInventory() ||
                //    SelectedRTSGameObjects[0].GetComponent<Shop>())
                PrintInventory();
            }
            //else
            //{
            //if (SelectedRTSGameObjects[0])
            //{
            //    for (int i = 0; i < items.Length; i++)
            //    {
            //        items[i].sprite = null;
            //        items[i].color = Color.white;
            //    }
            //}
            //}
            //if (SelectedRTSGameObjects.Count > 1)
            //    printSelectedUnits();
            //else
            printRTSGameObjectInformations(SelectedRTSGameObjects[0]);
        }
        else
        {
            selectedInformationsPanel.SetActive(false);
            unitFocusPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Print the informations of a game object on the appropriate panel
    /// </summary>
    /// <param name="unit"> The game object </param>
    private void printRTSGameObjectInformations(RTSGameObject rtsGameobject)
    {

        selectedInformationsPanel.SetActive(true);
        unitFocusPanel.SetActive(true);
        nameSelectedDisplay.GetComponent<Text>().text = SelectedRTSGameObjects[0].NameGo;

        Texture unitprev = SelectedRTSGameObjects[0].GetGUI.Preview;
        //print(unitprev);
        if(unitprev)
            unitPreview.texture = unitprev;
        Unit unit = SelectedRTSGameObjects[0].GetComponent<Unit>();
        if (unit)
        {
            healthBar.GetComponent<Image>().fillAmount = unit.CurrentHealth / unit.HealthMax;
            hitPoints.text = unit.CurrentHealth.ToString() + "/" + unit.HealthMax.ToString();
        }
        else
        {
            hitPoints.text = "0/0";
        }


        if (SelectedRTSGameObjects[0].GetComponent<AttackingUnit>())
        {
            shopInformationsPanel.SetActive(false);

            attackSelectedDisplay.SetActive(true);
            attackSpeedSelectedDisplay.SetActive(true);
            moveSpeedSelectedDisplay.SetActive(true);
            defenseSelectedDisplay.SetActive(true);

            attackSelectedDisplay.GetComponent<Text>().text = "Damage : " + SelectedRTSGameObjects[0].GetComponent<AttackingUnit>().Damage.ToString();
            attackSpeedSelectedDisplay.GetComponent<Text>().text = "Attack speed : " + SelectedRTSGameObjects[0].GetComponent<AttackingUnit>().AttackSpeed.ToString();
            moveSpeedSelectedDisplay.GetComponent<Text>().text = "Move speed : " + SelectedRTSGameObjects[0].GetComponent<AttackingUnit>().MoveSpeed.ToString();
            defenseSelectedDisplay.GetComponent<Text>().text = "Defense : " + SelectedRTSGameObjects[0].GetComponent<AttackingUnit>().Defense.ToString();
        }
        else if (SelectedRTSGameObjects[0].GetComponent<WalkingUnit>())
        {
            shopInformationsPanel.SetActive(false);

            moveSpeedSelectedDisplay.SetActive(true);
            defenseSelectedDisplay.SetActive(true);
            attackSelectedDisplay.SetActive(false);
            attackSpeedSelectedDisplay.SetActive(false);

            moveSpeedSelectedDisplay.GetComponent<Text>().text = "Move speed : " + SelectedRTSGameObjects[0].GetComponent<WalkingUnit>().MoveSpeed.ToString();
            defenseSelectedDisplay.GetComponent<Text>().text = "Defense : " + SelectedRTSGameObjects[0].GetComponent<WalkingUnit>().Defense.ToString();
        }

        else if (SelectedRTSGameObjects[0].GetComponent<Shop>())
        {
            attackSelectedDisplay.SetActive(false);
            attackSpeedSelectedDisplay.SetActive(false);
            moveSpeedSelectedDisplay.SetActive(false);
            defenseSelectedDisplay.SetActive(false);

            shopInformationsPanel.SetActive(true);
        }

        else
        {
            attackSelectedDisplay.SetActive(false);
            attackSpeedSelectedDisplay.SetActive(false);
            moveSpeedSelectedDisplay.SetActive(false);
            defenseSelectedDisplay.SetActive(false);

            shopInformationsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Print the selected unit on the appropriate panel
    /// </summary>
    private void printSelectedUnits()
    {

    }

    /// <summary>
    /// Activate the buttons of a specific unit
    /// </summary>
    /// <param name="unit"> The unit </param>
    private void printButtons(RTSGameObject unit)
    {
        try
        {
            // If the selected unit changed
            //if (!Equals(unit, _firstUnit))
            //{
            GUIRTSGameObject gui;

            if (_firstUnit != null)
            {
                gui = _firstUnit.GetGUI;
                DesactivateGUI(gui);
            }
            else
            {
                gui = unit.GetGUI;
            }

            _firstUnit = unit;

            ActivateGUI(_firstUnit.GetGUI);
            //}
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    private void DesactivateGUI(GUIRTSGameObject gui)
    {
        if (gui.GUIButtons != null)
            gui.DesactivateButtons();
    }

    private void ActivateGUI(GUIRTSGameObject gui)
    {
        if(gui.GUIButtons != null)
            gui.ActivateSetButton();
        if(SelectedRTSGameObjects[0].GetComponent<WalkingUnit>() && SelectedRTSGameObjects[0].GetComponent<WalkingUnit>().HasInventory())
            PrintInventory();
    }

    private void PrintInventory()
    {
        CollectableObject[] unitInventory = null;
        if (SelectedRTSGameObjects[0].GetComponent<WalkingUnit>() &&
            SelectedRTSGameObjects[0].GetComponent<WalkingUnit>().HasInventory())
            unitInventory = SelectedRTSGameObjects[0].GetComponent<WalkingUnit>().Inventory;

        else if (SelectedRTSGameObjects[0].GetComponent<Shop>() &&
                 SelectedRTSGameObjects[0].GetComponent<Shop>().TriggerShop.GetNearestOne())
            unitInventory = SelectedRTSGameObjects[0].GetComponent<Shop>().TriggerShop.GetNearestOne().Inventory;

        else
        {
            if (SelectedRTSGameObjects[0])
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].sprite = null;
                    items[i].color = Color.white;
                }
            }
        }

        for (int i = 0; i < unitInventory.Length; i++)
        {
            if(unitInventory[i] == null)
            {
                items[i].sprite = null;
                items[i].color = Color.yellow;
            }

            else
            {
                if (unitInventory[i].ImageDisplay)
                {
                    items[i].sprite = unitInventory[i].ImageDisplay;
                    items[i].color = Color.white;
                }
                else
                    items[i].color = Color.red;
                //items[i].sprite = unitInventory[i].ImageDisplay;
            }
        }
    }
    /// <summary>
    /// Add a listener for a specific button
    /// </summary>
    /// <param name="button"> The concerned button </param>
    /// <param name="listener"> The order given when pressing the button </param>
    public void AddButtonListener(GameObject button, OrderName listener)
    {
        AddButtonListener(button, listener, null, -1);
    }

    /// <summary>
    /// Add a listener for a specific button
    /// </summary>
    /// <param name="button"> The concerned button </param>
    /// <param name="listener"> The order given when pressing the button </param>
    /// <param name="pos"> If the pos is over -1, cancel the <c> position </c> creation (for buildings only)</param>
    public void AddButtonListener(GameObject button, OrderName listener, GameObject btn, int pos)
    {
        if (!btn || pos == -1)
        {
            if (listener.Equals(OrderName.Attack))
                button.GetComponent<Button>().onClick.AddListener(btnAttackClick);
            else if (listener.Equals(OrderName.Stop))
                button.GetComponent<Button>().onClick.AddListener(btnStopClick);
            else if (listener.Equals(OrderName.Move))
                button.GetComponent<Button>().onClick.AddListener(btnMoveClick);
            else if (listener.Equals(OrderName.Patrol))
                button.GetComponent<Button>().onClick.AddListener(btnPatrolClick);
            else if (listener.Equals(OrderName.Collect))
                button.GetComponent<Button>().onClick.AddListener(btnCollectClick);
            else if (listener.Equals(OrderName.Create0))
                button.GetComponent<Button>().onClick.AddListener(btnCreate0Click);
            else if (listener.Equals(OrderName.Create1))
                button.GetComponent<Button>().onClick.AddListener(btnCreate1Click);
            else if (listener.Equals(OrderName.Create2))
                button.GetComponent<Button>().onClick.AddListener(btnCreate2Click);
            else if (listener.Equals(OrderName.RallyPoint))
                button.GetComponent<Button>().onClick.AddListener(btnRallyPointClick);
            else if (listener.Equals(OrderName.Build))
                button.GetComponent<Button>().onClick.AddListener(btnBuildClick);
            else if (listener.Equals(OrderName.Build0))
                button.GetComponent<Button>().onClick.AddListener(btnBuild0Click);
            else if (listener.Equals(OrderName.Build1))
                button.GetComponent<Button>().onClick.AddListener(btnBuild1Click);
            else if (listener.Equals(OrderName.Cancel))
                button.GetComponent<Button>().onClick.AddListener(btnCancelClick);
        }
        else
            button.GetComponent<Button>().onClick.AddListener(delegate { btnCancelCreateClick(btn, pos); });
    }

    /// <summary>
    /// Change the cursor according to the situation
    /// </summary>
    /// <param name="symbol"> The situation </param>
    private void ChangingMouseSymbol(int symbol)
    {
        if (SelectedRTSGameObjects.Count > 0 && SelectedRTSGameObjects[0] != null)
        {
            if (symbol == 0)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    try
                    {
                        GameObject hitObject = hit.transform.gameObject;
                        if (hitObject.GetComponentInChildren<RTSGameObject>() != null)
                        {
                            if (!hitObject.GetComponentInChildren<RTSGameObject>().faction.Equals(PlayerFaction))
                            {
                                Vector2 cursorOffset = new Vector2(cursorAttackTexture.width / 2, cursorAttackTexture.height / 2); // Move the texture in order to make the pointer on the center of the texture
                                Cursor.SetCursor(cursorAttackTexture, cursorOffset, CursorMode.Auto);
                            }
                        }

                        else if (hitObject.GetComponent<Collectable>() && SelectedRTSGameObjects[0].GetComponent<Collector>()) // Collectable layer
                        {
                            Vector2 cursorOffset = new Vector2(cursorCollectTexture.width / 2, cursorCollectTexture.height / 2); // Move the texture in order to make the pointer on the center of the texture
                            Cursor.SetCursor(cursorCollectTexture, cursorOffset, CursorMode.Auto);
                        }
                        else
                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                    catch
                    {
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
            }
            else if (symbol == 1)
            {
                Vector2 cursorOffset = new Vector2(cursorDestinationTexture.width / 2, cursorDestinationTexture.height / 2);
                Cursor.SetCursor(cursorDestinationTexture, cursorOffset, CursorMode.Auto);
            }

            else if (symbol == 2)
            {
                Vector2 cursorOffset = new Vector2(cursorAttackTexture.width / 2, cursorAttackTexture.height / 2);
                Cursor.SetCursor(cursorAttackTexture, cursorOffset, CursorMode.Auto);
            }

            else if (symbol == 3)
            {
                Vector2 cursorOffset = new Vector2(cursorRallyPointTexture.width / 2, cursorRallyPointTexture.height / 2);
                Cursor.SetCursor(cursorRallyPointTexture, cursorOffset, CursorMode.Auto);
            }
            else if (symbol == 4)
            {
                Vector2 cursorOffset = new Vector2(cursorCollectTexture.width / 2, cursorCollectTexture.height / 2);
                Cursor.SetCursor(cursorCollectTexture, cursorOffset, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Check if the gameObject is in the bounds of a rectangle
    /// </summary>
    /// <param name="gameObject"> The gameObject tbat needs to be check </param>
    /// <returns></returns>
    private bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (gameObject.GetComponent<Collider>())
        {
            if (!isSelecting)
                return false;

            Camera camera = mainCamera;
            Bounds viewportBounds = UtilsGUI.GetViewportBounds(camera, originalPosition, Input.mousePosition);

            Vector3 bounds_min;
            Vector3 bounds_max;
            bounds_min = gameObject.GetComponent<Collider>().bounds.min;
            bounds_max = gameObject.GetComponent<Collider>().bounds.max;

            Bounds viewportBoundsObject =
                UtilsGUI.GetViewportBounds(camera, Camera.main.WorldToScreenPoint(bounds_min), Camera.main.WorldToScreenPoint(bounds_max));

            return viewportBounds.Intersects(viewportBoundsObject);
        }
        else
        {
            Debug.LogError("Missing Collider component on " + gameObject.name);
            return false;
        }
    }

    /// <summary>
    /// When the player left click on one the object of the inventory
    /// </summary>
    /// <param name="slotNumber"> The position of the object in the inventory </param>
    public void InventoryLeftClick(int slotNumber)
    {
        if (SelectedRTSGameObjects[0])
        {
            WalkingUnit unit = SelectedRTSGameObjects[0].GetComponent<WalkingUnit>();
            if (unit && unit.HasInventory() && unit.Inventory[slotNumber] != null &&
                unit.Inventory[slotNumber].GetComponent<UseableCollectableObject>())
            {
                unit.Inventory[slotNumber].GetComponent<UseableCollectableObject>().Use(unit);
                unit.Inventory[slotNumber] = null;
            }

        }
    }

    /// <summary>
    /// When the player right click on one the object of the inventory
    /// </summary>
    /// <param name="slotNumber"> The position of the object in the inventory </param>
    public void InventoryRightClick(int slotNumber)
    {
        if (SelectedRTSGameObjects[0])
        {
            WalkingUnit unit = SelectedRTSGameObjects[0].GetComponent<WalkingUnit>();
            if(unit && unit.HasInventory() && unit.Inventory[slotNumber] != null)
            {
                _orderDropItemGiven = unit.Inventory[slotNumber];
            }
        }
    }
    /// <summary>
    /// When the player left click on one of the object of the selected shop
    /// </summary>
    /// <param name="slotNumber"> The position of the object on the store </param>
    public void ShopLeftClick(int slotNumber)
    {
        if (SelectedRTSGameObjects[0])
        {
            Shop shop = SelectedRTSGameObjects[0].GetComponent<Shop>();
            if (shop)
            {
                shop.SellObject(shop.ObjectsToSell[slotNumber]);
            }
        }
    }


    #region Add/Spawn Units
    /// <summary>
    /// Add a new unit to the game at the position
    /// </summary>
    /// <param name="unit"> The unit to add </param>
    /// <param name="position"> The initial position of the unit </param>
    /// <param name="quaternion"> The initial rotation of the unit </param>
    public void addUnit(GameObject unit, Vector3 position, Quaternion quaternion, Faction faction)
    {
        if (unit.GetComponentInChildren<RTSGameObject>() != null)
        {
            GameObject refUnit = GameObject.Instantiate(unit, position, unit.transform.rotation);
            refUnit.GetComponentInChildren<Projector>().enabled = false;
            refUnit.GetComponentInChildren<RTSGameObject>().faction = faction;

            if (faction.Equals(Faction.Blue))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            else if (faction.Equals(Faction.Red))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.red;

            UnitsInGame.Add(refUnit.GetComponentInChildren<RTSGameObject>());
        }

        else
        {
            Debug.LogError(unit.name + " n'est pas de type Unit");
        }
    }

    /// <summary>
    /// Spawn an unit at the position that go to the rallyPoint 
    /// </summary>
    /// <param name="unit"> The unit to spawn </param>
    /// <param name="position"> The initial position to spawn the unit </param>
    /// <param name="quaternion"> The initial rotation of the unit </param>
    /// <param name="rallyPoint"> The position where the unit will go on </param>
    public void spawnUnit(GameObject unit, Vector3 position, Quaternion quaternion, Vector3 rallyPoint, Faction faction)
    {
        if (unit.GetComponentInChildren<AttackingUnit>() != null)
        {
            GameObject refUnit = GameObject.Instantiate(unit, position, quaternion);

            refUnit.GetComponentInChildren<Projector>().enabled = false;
            refUnit.GetComponentInChildren<AttackingUnit>().faction = faction;

            if (faction.Equals(Faction.Blue))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            else if (faction.Equals(Faction.Red))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.red;

            UnitsInGame.Add(refUnit.GetComponentInChildren<RTSGameObject>());
            refUnit.GetComponentInChildren<AttackingUnit>().SendDirectAttackOrder(rallyPoint);
        }

        else
        {
            Debug.LogError(unit.name + " n'est pas de type AttackingUnit");
        }

    }

    /// <summary>
    /// Spawn an unit at the position that go to the rallyPoint 
    /// </summary>
    /// <param name="unit"> The unit to spawn </param>
    /// <param name="position"> The initial position to spawn the unit </param>
    /// <param name="quaternion"> The initial rotation of the unit </param>
    /// <param name="rallyPoint"> The position where the unit will go </param>
    public void spawnUnit(GameObject unit, Vector3 position, Quaternion quaternion, RTSGameObject rallyPoint, Faction faction)
    {
        if (unit.GetComponentInChildren<AttackingUnit>() != null)
        {
            GameObject refUnit = Instantiate(unit, position, quaternion);
            refUnit.GetComponentInChildren<Projector>().enabled = false;
            refUnit.GetComponentInChildren<AttackingUnit>().faction = faction;

            if (faction.Equals(Faction.Blue))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            else if (faction.Equals(Faction.Red))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.red;

            refUnit.GetComponentInChildren<AttackingUnit>().SendDirectAttackOrder(rallyPoint.transform.position);
            UnitsInGame.Add(refUnit.GetComponentInChildren<RTSGameObject>());
        }

        if (unit.GetComponentInChildren<Collector>() && rallyPoint.GetComponent<Collectable>())
        {
            GameObject refUnit = Instantiate(unit, position, quaternion);
            refUnit.GetComponentInChildren<Projector>().enabled = false;
            refUnit.GetComponentInChildren<Collector>().faction = faction;

            if (faction.Equals(Faction.Blue))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            else if (faction.Equals(Faction.Red))
                refUnit.GetComponent<MeshRenderer>().materials[0].color = Color.red;

            refUnit.GetComponentInChildren<Collector>().SendDirectCollectOrder(rallyPoint.GetComponent<Collectable>());
            UnitsInGame.Add(refUnit.GetComponentInChildren<RTSGameObject>());
        }

        else
        {
            Debug.LogError(unit.name + " n'est pas de type Unit");
        }

    }

    #endregion
    
    /// <summary>
    /// Unlock a new building ready to be built
    /// </summary>
    /// <param name="buildingName"> The name of the building </param>
    public void UnlockBuilding(BuildingName buildingName)
    {
        if (!buildingsAvailables.Contains(buildingName))
            buildingsAvailables.Add(buildingName);
    }
    /// <summary>
    /// Lock a building
    /// </summary>
    /// <param name="buildingName"> The name of the building to lock </param>
    public void LockBuilding(BuildingName buildingName)
    {
        if (buildingsAvailables.Contains(buildingName))
            buildingsAvailables.Remove(buildingName);
    }

    /// <summary>
    /// Unlock a series of buildings
    /// </summary>
    /// <param name="buildings"> The buildings to unlock </param>
    public void UnlockBuildings(List<BuildingName> buildings)
    {
        foreach (BuildingName bn in buildings)
            if (!buildingsAvailables.Contains(bn))
                buildingsAvailables.Add(bn);
    }
    /// <summary>
    /// Lock a series of buildings
    /// </summary>
    /// <param name="buildings"> The buildings to lock </param>
    public void LockBuildings(List<BuildingName> buildings)
    {
        foreach (BuildingName bn in buildings)
            if (buildingsAvailables.Contains(bn))
                buildingsAvailables.Remove(bn);
    }
    #region Getters and Setters
    public GameObject ButtonPrefab
    {
        get
        {
            return buttonPrefab;
        }
    }

    public GameObject GetButtonPrefab(OrderName Btn)
    {
        // Return only 1 button for now
        return ButtonPrefab;
    }
    public List<RTSGameObject> UnitsInGame
    {
        get
        {
            return _unitsInGame;
        }
    }

    public int RedWool
    {
        get
        {
            return redWool;
        }

        set
        {
            redWool = value;
            displayRedWool.text = RedWool.ToString();
        }
    }

    public int BlueWool
    {
        get
        {
            return blueWool;
        }

        set
        {
            blueWool = value;
            displayBlueWool.text = BlueWool.ToString();
        }
    }

    public List<RTSGameObject> SelectedRTSGameObjects
    {
        get
        {
            return _selectedRTSGameObjects;
        }

        set
        {
            _selectedRTSGameObjects = value;
        }
    }
    #endregion
}

//public void LoadCharacter() {
//    GameObject gs = GameObject.Find("GameSettings");
//    if (gs == null)
//    {
//        gs = Instantiate(gameSettingsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
//        gs.name = "GameSettings";
//    }

//    GameSettings gsScript = gs.GetComponent<GameSettings>();

//    gsScript.LoadCharacterData();
//}


//private void ActivateButton(int position, string NameGUI)
//{
//    if (NameGUI.Equals("Heros"))
//    {
//        _GUIHerosBtns[position].SetActive(true);
//        _GUIBuildingBtns[position].SetActive(false);
//        _GUIUnitBtns[position].SetActive(false);
//    }

//    else if (NameGUI.Equals("Building"))
//    {
//        _GUIBuildingBtns[position].SetActive(true);
//        _GUIHerosBtns[position].SetActive(false);
//        _GUIUnitBtns[position].SetActive(false);
//    }
//    else if (NameGUI.Equals("Unit"))
//    {
//        _GUIUnitBtns[position].SetActive(true);
//        _GUIHerosBtns[position].SetActive(false);
//        _GUIBuildingBtns[position].SetActive(false);
//    }
//}



//return viewportBounds.Contains(
//    camera.WorldToViewportPoint(gameObject.transform.position));

//Vector3 position1;
//Vector3 position2;

//position1 = new Vector3(camera.WorldToScreenPoint(gameObject.transform.position).x - gameObject.GetComponent<BoxCollider>().size.x / 2,
//                        camera.WorldToScreenPoint(gameObject.transform.position).y - gameObject.GetComponent<BoxCollider>().size.z / 2);
//position2 = new Vector3(camera.WorldToScreenPoint(gameObject.transform.position).x + gameObject.GetComponent<BoxCollider>().size.x / 2,
//                        camera.WorldToScreenPoint(gameObject.transform.position).y + gameObject.GetComponent<BoxCollider>().size.z / 2);

//position1 = new Vector3(camera.WorldToScreenPoint(gameObject.transform.position).x - 10,
//                       camera.WorldToScreenPoint(gameObject.transform.position).y - 10);
//position2 = new Vector3(camera.WorldToScreenPoint(gameObject.transform.position).x + 10,
//                        camera.WorldToScreenPoint(gameObject.transform.position).y + 10);

//Bounds viewportGameObjectBound = UtilsGUI.GetViewportBounds(camera, position1, position2);
//Debug.Log(gameObject.GetComponent<Renderer>().bounds);

//return viewportBounds.Intersects(gameObject.GetComponent<Collider>().bounds);