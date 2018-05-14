using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NewUnitWindow : EditorWindow
{
    static string nameUnit;
    static UnitType unitType;
    static Material defaultMaterial;
    static GameObject skin;
    static float regenManaPerSecond;
    static float healthMax;
    static float manaMax;
    static int cost;
    static float xpValue;
    static CollectableObject[] inventory = new CollectableObject[6];
    static bool showInventory = false;
    static float creationTime;
    static float attackSpeed;
    static float range;
    static float damage;
    static float defense;
    static float moveSpeed = 20;
    static float rangeAggroWhenAttack = 100;
    static int actualIndexGO;
    static int indexGO;
    static string[] search_results = null;

    //static bool showCreations = false;
    static WalkingUnit creation;
    static bool isDumpingBuilding;
    static Material ghostMaterial;

    void OnGUI()
    {
        switch (unitType) {
            case UnitType.AttackingUnit:
                CreateNewAttackingUnit();
                break;
            case UnitType.Building:
                CreateNewBuilding();
                break;
        }
    }

    private void CreateNewBuilding()
    {
        indexGO = EditorGUILayout.Popup("Model", indexGO, search_results);
        if (indexGO != actualIndexGO)
            UpdateEveryFields();
        actualIndexGO = indexGO;

        if (skin && AssetPreview.GetAssetPreview(skin))
        {
            EditorGUI.DrawPreviewTexture(new Rect(155, 25, 90, 90), AssetPreview.GetAssetPreview(skin));
        }

        for (int i = 0; i < 20; i++)
        {
            EditorGUILayout.Space();
        }

        nameUnit = EditorGUILayout.TextField("Name", nameUnit);
        defaultMaterial = (Material)EditorGUILayout.ObjectField("Default Material", defaultMaterial, typeof(Material), true);
        skin = (GameObject)EditorGUILayout.ObjectField("Skin", skin, typeof(GameObject), true);
        healthMax = EditorGUILayout.FloatField("Health Max", healthMax);
        manaMax = EditorGUILayout.FloatField("Mana Max", manaMax);
        regenManaPerSecond = EditorGUILayout.FloatField("Regen Mana Per Second", regenManaPerSecond);
        cost = EditorGUILayout.IntField("Cost", cost);
        xpValue = EditorGUILayout.FloatField("Xp value", xpValue);

        GUIStyle gstyle = new GUIStyle(EditorStyles.foldout);
        //showCreations = EditorGUILayout.Foldout(showCreations, "Creations", gstyle);
        //if (showCreations)
        //{
        //    for (int i = 0; i < inventory.Length; i++)
        //    {
        //        creations[i] = (AttackingUnit)EditorGUILayout.ObjectField(creations[i], typeof(AttackingUnit), true);
        //    }
        //}
        creation = (WalkingUnit)EditorGUILayout.ObjectField(creation, typeof(WalkingUnit), true);

        isDumpingBuilding = EditorGUILayout.Toggle("Is Dumping Building", isDumpingBuilding);

        if (GUILayout.Button("Add new building"))
        {
            AddNewBuilding();
        }
    }

    public void AddNewBuilding()
    {
        GameObject emptyGO = new GameObject();
        AssetDatabase.CreateFolder("Assets/Prefabs", nameUnit);
        GameObject tmp = PrefabUtility.CreatePrefab("Assets/Prefabs/" + nameUnit + "/" + nameUnit + ".prefab", emptyGO);

        tmp.AddComponent<Animator>();
        tmp.AddComponent<Rigidbody>();
        tmp.AddComponent<BoxCollider>();

        CreationBuilding building = tmp.AddComponent<CreationBuilding>();
        building.NameGo = nameUnit;
        building.UnitType = unitType;
        building.DefaultMaterial = defaultMaterial;
        building.RegenManaPerSecond = regenManaPerSecond;
        Material inAreaUnit = (Material)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skills/InAreaUnit.mat", typeof(Material));
        GameObject smallHealthBar = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/HealthBar/HealthBar.prefab", typeof(GameObject));
        building.InAreaMaterial = inAreaUnit;
        building.SmallHealthBarPrefab = smallHealthBar;
        building.HealthMax = healthMax;
        building.ManaMax = manaMax;
        building.Cost = cost;
        building.XpValue = xpValue;
        building.Creations = new WalkingUnit[1];
        building.Creations[0] = creation;

        // Create the building ghost and save it at the same folder as the original building
        GameObject emptyBuildingGhost = new GameObject();
        GameObject tmpBuildingGhost = PrefabUtility.CreatePrefab("Assets/Prefabs/" + nameUnit + "/" + nameUnit + "Ghost.prefab", emptyBuildingGhost);
        tmpBuildingGhost.AddComponent<BuildingGhost>();
        tmpBuildingGhost.AddComponent<BoxCollider>();

        GameObject instantiatedTmpBuildingGhost = Instantiate(tmpBuildingGhost);

        //GameObject emptySkin = new GameObject();
        //emptySkin.transform.SetParent(tmpBuildingGhost.transform);
        //instantiatedTmpBuildingGhost.AddComponent<MeshFilter>();
        //if(skin)
        //    instantiatedTmpBuildingGhost.GetComponent<MeshFilter>().mesh = 
        //GameObject skinInstantiated = null;
        //if (skin)
        //    skinInstantiated = Instantiate(skin, instantiatedTmpBuildingGhost.transform);
        //if (skinInstantiated && skinInstantiated.GetComponent<MeshRenderer>())
        //    skinInstantiated.GetComponent<MeshRenderer>().sharedMaterials[0] = ghostMaterial;


        PrefabUtility.ReplacePrefab(instantiatedTmpBuildingGhost, tmpBuildingGhost);

        building.BuildingGhost = tmpBuildingGhost;

        GameObject instantiatedTmp = Instantiate(tmp);
        if (skin)
            Instantiate(skin, instantiatedTmp.transform);

        GameObject projector = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Projector/ProjectorPrefab.prefab", typeof(GameObject));
        if (!projector)
        {
            Debug.LogError("No aggro or projector or triggerXpArea object");
            return;
        }
        Instantiate(projector, instantiatedTmp.transform);
        
        PrefabUtility.ReplacePrefab(instantiatedTmp, tmp);
        DestroyImmediate(instantiatedTmp);

        DestroyImmediate(emptyGO);
    }

    private void CreateNewAttackingUnit()
    {
        indexGO = EditorGUILayout.Popup("Model", indexGO, search_results);
        if (indexGO != actualIndexGO)
            UpdateEveryFields();
        actualIndexGO = indexGO;

        if (skin && AssetPreview.GetAssetPreview(skin))
        {
            EditorGUI.DrawPreviewTexture(new Rect(155, 25, 90, 90), AssetPreview.GetAssetPreview(skin));
        }

        for (int i = 0; i < 20; i++)
        {
            EditorGUILayout.Space();
        }

        nameUnit = EditorGUILayout.TextField("Name", nameUnit);
        defaultMaterial = (Material)EditorGUILayout.ObjectField("Default Material", defaultMaterial, typeof(Material), true);
        skin = (GameObject)EditorGUILayout.ObjectField("Skin", skin, typeof(GameObject), true);
        regenManaPerSecond = EditorGUILayout.FloatField("Regen Mana Per Seconds", regenManaPerSecond);
        healthMax = EditorGUILayout.FloatField("Health Max", healthMax);
        manaMax = EditorGUILayout.FloatField("Mana Max", manaMax);
        cost = EditorGUILayout.IntField("Cost", cost);
        xpValue = EditorGUILayout.FloatField("Xp value", xpValue);

        GUIStyle gstyle = new GUIStyle(EditorStyles.foldout);
        showInventory = EditorGUILayout.Foldout(showInventory, "Inventory", gstyle);
        if (showInventory)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = (CollectableObject)EditorGUILayout.ObjectField(inventory[i], typeof(CollectableObject), true);
            }
        }

        attackSpeed = EditorGUILayout.FloatField("Attack speed", attackSpeed);
        range = EditorGUILayout.FloatField("Range", range);
        damage = EditorGUILayout.FloatField("Damage", damage);
        defense = EditorGUILayout.FloatField("Defense", defense);
        moveSpeed = EditorGUILayout.FloatField("Move speed", moveSpeed);
        rangeAggroWhenAttack = EditorGUILayout.FloatField("Range Aggro When Attack", rangeAggroWhenAttack);
        creationTime = EditorGUILayout.FloatField("Creation time", creationTime);

        if (GUILayout.Button("Add new unit"))
        {
            AddNewAttackingUnit();
        }
    }

    public static void AddNewAttackingUnit()
    {
        GameObject emptyGO = new GameObject();
        AssetDatabase.CreateFolder("Assets/Prefabs", nameUnit);
        GameObject tmp = PrefabUtility.CreatePrefab("Assets/Prefabs/" + nameUnit + "/" + nameUnit + ".prefab", emptyGO);

        tmp.AddComponent<NavMeshAgent>();
        tmp.AddComponent<Rigidbody>();
        tmp.AddComponent<Animator>();
        tmp.AddComponent<AudioSource>();

        AttackingUnit atunit = tmp.AddComponent<AttackingUnit>();
        atunit.NameGo = nameUnit;
        atunit.UnitType = unitType;
        atunit.DefaultMaterial = defaultMaterial;
        atunit.RegenManaPerSecond = regenManaPerSecond;
        Material inAreaUnit = (Material)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Skills/InAreaUnit.mat", typeof(Material));
        GameObject smallHealthBar = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/HealthBar/HealthBar.prefab", typeof(GameObject));
        atunit.InAreaMaterial = inAreaUnit;
        atunit.SmallHealthBarPrefab = smallHealthBar;
        atunit.HealthMax = healthMax;
        atunit.ManaMax = manaMax;
        atunit.Cost = cost;
        atunit.XpValue = xpValue;

        bool inventoryEmpty = true;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
                inventoryEmpty = false;
        }

        if (!inventoryEmpty)
        {
            atunit.Inventory = new CollectableObject[6];
            for (int i = 0; i < atunit.Inventory.Length; i++)
            {
                atunit.Inventory[i] = inventory[i];
            }
        }

        atunit.CreationTime = creationTime;
        atunit.AttackSpeed = attackSpeed;
        atunit.Damage = damage;
        atunit.Range = range;
        atunit.Defense = defense;
        atunit.RangeAggroWhenAttack = rangeAggroWhenAttack;
        atunit.MoveSpeed = moveSpeed;

        NavMeshAgent navmesh = tmp.GetComponent<NavMeshAgent>();
        navmesh.speed = moveSpeed;
        navmesh.angularSpeed = 200;
        navmesh.acceleration = 100;
        navmesh.stoppingDistance = 0;
        navmesh.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;

        
        GameObject instantiatedTmp = Instantiate(tmp);
        if (skin)
            Instantiate(skin, instantiatedTmp.transform);

        GameObject aggro = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Aggro/Aggro.prefab", typeof(GameObject));
        GameObject projector = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Projector/ProjectorPrefab.prefab", typeof(GameObject));
        GameObject triggerXpArea = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/TriggerXpArea/TriggerXpArea.prefab", typeof(GameObject));

        if (!aggro || !projector || !triggerXpArea)
        {
            Debug.LogError("No aggro or projector or triggerXpArea object");
            return;
        }
        Instantiate(aggro, instantiatedTmp.transform);
        Instantiate(projector, instantiatedTmp.transform);
        Instantiate(triggerXpArea, instantiatedTmp.transform);

        PrefabUtility.ReplacePrefab(instantiatedTmp, tmp);
        DestroyImmediate(instantiatedTmp);

        DestroyImmediate(emptyGO);
    }

    public void UpdateEveryFields()
    {
        GameObject tmpShit = (GameObject)AssetDatabase.LoadAssetAtPath(search_results[indexGO], typeof(GameObject));

        if (unitType.Equals(UnitType.AttackingUnit))
        {
            AttackingUnit unit = tmpShit.GetComponent<AttackingUnit>();

            nameUnit = unit.NameGo;
            unitType = unit.UnitType;
            defaultMaterial = unit.DefaultMaterial;
            //GameObject tmpSkin = new GameObject();
            //tmpSkin.AddComponent<MeshFilter>();
            //tmpSkin.GetComponent<MeshFilter>().mesh = unit.GetComponent<MeshFilter>().mesh;
            //skin = tmpSkin;
            //DestroyImmediate(tmpSkin);
            //skin
            regenManaPerSecond = unit.RegenManaPerSecond;
            healthMax = unit.HealthMax;
            manaMax = unit.ManaMax;
            cost = unit.Cost;
            xpValue = unit.XpValue;

            bool inventoryEmpty = true;
            for (int i = 0; i < unit.Inventory.Length; i++)
            {
                if (unit.Inventory[i] != null)
                    inventoryEmpty = false;
            }

            if (!inventoryEmpty)
            {
                for (int i = 0; i < unit.Inventory.Length; i++)
                {
                    inventory[i] = unit.Inventory[i];
                }
            }

            creationTime = unit.CreationTime;
            attackSpeed = unit.AttackSpeed;
            damage = unit.Damage;
            range = unit.Range;
            defense = unit.Defense;
            rangeAggroWhenAttack = unit.RangeAggroWhenAttack;
            moveSpeed = unit.MoveSpeed;
        }
        else if (unitType.Equals(UnitType.Building))
        {
            CreationBuilding unit = tmpShit.GetComponent<CreationBuilding>();

            nameUnit = unit.NameGo;
            unitType = unit.UnitType;
            defaultMaterial = unit.DefaultMaterial;
            healthMax = unit.HealthMax;
            manaMax = unit.ManaMax;
            cost = unit.Cost;
            xpValue = unit.XpValue;
            creation = unit.Creations[0];
            //bool creationsEmpties = true;
            //for (int i = 0; i < unit.Creations.Length; i++)
            //{
            //    if (unit.Creations[i] != null)
            //        creationsEmpties = false;
            //}
            //if (!creationsEmpties)
            //{
            //    for (int i = 0; i < unit.Creations.Length; i++)
            //    {
            //        if (unit.Creations[i] != null)
            //            creations[i] = unit.Creations[i];
            //    }
            //}
        }
    }

    [MenuItem("New/Unit/AttackingUnit")]
    public static void InitAttackingUnit()
    {
        EditorWindow.GetWindow(typeof(NewUnitWindow));

        unitType = UnitType.AttackingUnit;
        string assetPath = "Assets/Prefabs";
        List<string> tmpStrings = new List<string>();
        string[] search_resultsTmp = System.IO.Directory.GetFiles(assetPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        List<AttackingUnit> units = new List<AttackingUnit>();
        for (int i = 0; i < search_resultsTmp.Length; i++)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(search_resultsTmp[i]);
            if (obj && obj.GetComponent<AttackingUnit>())
            {
                tmpStrings.Add(search_resultsTmp[i]);
            }
        }
        search_results = new string[tmpStrings.Count + 1];
        search_results[0] = "";
        for (int i = 0; i < tmpStrings.Count; i++)
        {
            search_results[i + 1] = tmpStrings[i];
        }
    }

    [MenuItem("New/Unit/Building")]
    public static void InitBuilding()
    {
        EditorWindow.GetWindow(typeof(NewUnitWindow));

        unitType = UnitType.Building;
        string assetPath = "Assets/Prefabs";
        List<string> tmpStrings = new List<string>();
        string[] search_resultsTmp = System.IO.Directory.GetFiles(assetPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        List<CreationBuilding> units = new List<CreationBuilding>();
        for (int i = 0; i < search_resultsTmp.Length; i++)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(search_resultsTmp[i]);
            if (obj && obj.GetComponent<CreationBuilding>())
            {
                tmpStrings.Add(search_resultsTmp[i]);
            }
        }
        search_results = new string[tmpStrings.Count + 1];
        search_results[0] = "";
        for (int i = 0; i < tmpStrings.Count; i++)
        {
            search_results[i + 1] = tmpStrings[i];
        }
    }
}