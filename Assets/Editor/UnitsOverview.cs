using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitsOverview : EditorWindow
{
    static List<Unit> units;
    Vector2 scrollPos;

    float layoutWidth;

    float sizeOfEntries = 110;

    bool showHerosSkills = false;
    int nbSkillsMax = 0;

    void OnGUI()
    {
        layoutWidth = sizeOfEntries * (units.Count + 1);
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        GUIStyle labelNameStyle = new GUIStyle(GUI.skin.label);
        labelNameStyle.alignment = TextAnchor.MiddleCenter;

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(1500), GUILayout.Height(790)); // 1500 is close to the width of the window

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("", labelNameStyle);
        for (int i = 0; i < units.Count; i++)
            EditorGUILayout.TextField(units[i].NameGo, labelNameStyle);
        GUILayout.EndHorizontal();
        

        GUILayout.Space(100);
        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (AssetPreview.GetAssetPreview(units[i].gameObject))
                EditorGUI.DrawPreviewTexture(new Rect(sizeOfEntries + 15 + (sizeOfEntries * i), 30, 90, 90), AssetPreview.GetAssetPreview(units[i].gameObject));
            else
            {
                Debug.Log("No image preview for " + units[i].gameObject);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Unit Type", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].UnitType = (UnitType)EditorGUILayout.EnumPopup(units[i].UnitType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Interactionable", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].Interactionable = EditorGUILayout.Toggle(units[i].Interactionable);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("In Area Material", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].InAreaMaterial = (Material)EditorGUILayout.ObjectField(units[i].InAreaMaterial, typeof(Material), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Default Material", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].DefaultMaterial = (Material)EditorGUILayout.ObjectField(units[i].DefaultMaterial,typeof(Material),true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Regen Man Per Second", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].RegenManaPerSecond = EditorGUILayout.FloatField(units[i].RegenManaPerSecond);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Health Max", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].HealthMax = EditorGUILayout.FloatField(units[i].HealthMax);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Mana Max", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].ManaMax = EditorGUILayout.FloatField(units[i].ManaMax);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Current Health", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].CurrentHealth = EditorGUILayout.FloatField(units[i].CurrentHealth);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Current Mana", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].CurrentMana = EditorGUILayout.FloatField(units[i].CurrentMana);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Cost", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].Cost = EditorGUILayout.IntField(units[i].Cost);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Xp Value", labelStyle);
        for (int i = 0; i < units.Count; i++)
            units[i].XpValue = EditorGUILayout.FloatField(units[i].XpValue);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Creation Time", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if(units[i].GetComponent<WalkingUnit>())
                units[i].GetComponent<WalkingUnit>().CreationTime = EditorGUILayout.FloatField(units[i].GetComponent<WalkingUnit>().CreationTime);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        
        EditorGUILayout.TextField("Attack Speed", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<AttackingUnit>())
                units[i].GetComponent<AttackingUnit>().AttackSpeed = EditorGUILayout.FloatField(units[i].GetComponent<AttackingUnit>().AttackSpeed);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Range", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<AttackingUnit>())
                units[i].GetComponent<AttackingUnit>().Range = EditorGUILayout.FloatField(units[i].GetComponent<AttackingUnit>().Range);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Damage", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<AttackingUnit>())
                units[i].GetComponent<AttackingUnit>().Damage = EditorGUILayout.FloatField(units[i].GetComponent<AttackingUnit>().Damage);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Range Aggro When attack", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<AttackingUnit>())
                units[i].GetComponent<AttackingUnit>().RangeAggroWhenAttack = EditorGUILayout.FloatField(units[i].GetComponent<AttackingUnit>()
                                                                                                                 .RangeAggroWhenAttack);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Skill", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<SkilledBasicUnit>())
                units[i].GetComponent<SkilledBasicUnit>().UnitSkill = (Skill)EditorGUILayout.ObjectField(units[i].GetComponent<SkilledBasicUnit>()
                                                                                                                 .UnitSkill, typeof(Skill), true);
            else if (units[i].GetComponent<HerosUnit>())
            {
                GUIStyle gstyle = new GUIStyle(EditorStyles.foldout);
                gstyle.margin.right += (int)(sizeOfEntries / 2);
                
                showHerosSkills = EditorGUILayout.Foldout(showHerosSkills, "Skills", gstyle);
                if (units[i].GetComponent<HerosUnit>().Skills.Count > nbSkillsMax)
                    nbSkillsMax = units[i].GetComponent<HerosUnit>().Skills.Count;
            }

            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        if (showHerosSkills)
        {
            for (int i = 0; i < nbSkillsMax; i++)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
                EditorGUILayout.TextField("", labelStyle);
                for (int j = 0; j < units.Count; j++)
                {
                    if (units[j].GetComponent<HerosUnit>())
                    {
                        units[j].GetComponent<HerosUnit>().Skills[i] = (Skill)EditorGUILayout.ObjectField(units[j].GetComponent<HerosUnit>().Skills[i]
                                                                                                                 , typeof(Skill), true);
                    }
                    else
                    {
                        EditorGUILayout.TextField("", labelStyle);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Skill Level", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<SkilledBasicUnit>())
                units[i].GetComponent<SkilledBasicUnit>().SkillLevel = EditorGUILayout.IntField(units[i].GetComponent<SkilledBasicUnit>().SkillLevel);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Xp to pass level", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<HerosUnit>())
                units[i].GetComponent<HerosUnit>().XpToPassLevel = EditorGUILayout.FloatField(units[i].GetComponent<HerosUnit>().XpToPassLevel);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(GUILayout.Width(layoutWidth));
        EditorGUILayout.TextField("Level coef", labelStyle);
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<HerosUnit>())
                units[i].GetComponent<HerosUnit>().LevelCoef = EditorGUILayout.FloatField(units[i].GetComponent<HerosUnit>().LevelCoef);
            else
            {
                EditorGUILayout.TextField("", labelStyle);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    [MenuItem("View/UnitsOverview")]
    public static void Init()
    {
        //EditorWindow.GetWindow(typeof(NewSkillWindow), false, "Skills");
        EditorWindow.GetWindow(typeof(UnitsOverview));

        units = new List<Unit>();

        string assetPath = "Assets/";
        string[] search_results = null;
        search_results = System.IO.Directory.GetFiles(assetPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < search_results.Length; i++)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(search_results[i]);
            if (obj && obj.GetComponent<Unit>())
            {
                units.Add(obj.GetComponent<Unit>());
            }
        }
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
