using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class NewSkillWindow : EditorWindow {

    static string nameSkill;
    static string folderName;
    static string description = "";
    static int skillLevel;
    static bool skillShot;
    static int indexAreaSkillshot; // Index of the popupEnum
    static bool targetted;
    static bool passive;
    static bool targetSelf;
    static bool targetEnemies;
    static List<bool> targettableEnemies = new List<bool>();
    static bool targetFriendlies;
    static List<bool> targettableFriendlies = new List<bool>();
    static UnitType unitTypes;
    static InitialPositionState initialPosition;
    static int levelRequired;
    static List<ListSkillAttributes> attributesModifiers = new List<ListSkillAttributes>(); // Modifiers when skill lvl up
    static int levelAttributesModifiersSize;
    static List<int> attributesModifiersSizes = new List<int>();
    static GameObject ps_skill;


    bool flag = false;

    void OnGUI()
    {
        nameSkill = EditorGUILayout.TextField("Name", nameSkill);
        folderName = EditorGUILayout.TextField("Save folder", folderName);
        description = EditorGUILayout.TextField("Description", description);
        skillLevel = EditorGUILayout.IntField("Start skill level", skillLevel);
        skillShot = EditorGUILayout.Toggle("Is a skillshot", skillShot);

        string assetPath = "Assets/Prefabs/Skills/SkillGUI/";
        string[] search_results = null;
        search_results = System.IO.Directory.GetFiles(assetPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        indexAreaSkillshot = EditorGUILayout.Popup("Area skillshot", indexAreaSkillshot, search_results);

        targetted = EditorGUILayout.Toggle("Is a targetted skill", targetted);
        passive = EditorGUILayout.Toggle("Is a passive skill", passive);

        targetSelf = EditorGUILayout.Toggle("Can hit sender", targetSelf);
        targetEnemies = EditorGUILayout.Toggle("Can hit enemies", targetEnemies);
        if (targetEnemies)
        {
            int cpt = 0;
            foreach(UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                targettableEnemies[cpt] = EditorGUILayout.Toggle(type.ToString(), targettableEnemies[cpt]);
                cpt++;
            }
            EditorGUILayout.Space();
        }

        targetFriendlies = EditorGUILayout.Toggle("Can hit friendlies", targetFriendlies);
        if (targetFriendlies)
        {
            int cpt = 0;
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                targettableFriendlies[cpt] = EditorGUILayout.Toggle(type.ToString(), targettableFriendlies[cpt]);
                cpt++;
                
            }
            EditorGUILayout.Space();
        }

        initialPosition = (InitialPositionState)EditorGUILayout.EnumPopup("Initial position", initialPosition);
        levelRequired = EditorGUILayout.IntField("Level required", levelRequired);

        levelAttributesModifiersSize = EditorGUILayout.IntField("Skill levels", levelAttributesModifiersSize);
        if (levelAttributesModifiersSize > attributesModifiers.Count)
        {
            while (levelAttributesModifiersSize > attributesModifiers.Count)
            {
                attributesModifiers.Add(new ListSkillAttributes());
                attributesModifiersSizes.Add(new int());
            }
        }
        else if (levelAttributesModifiersSize < attributesModifiers.Count)
        {
            while (levelAttributesModifiersSize < attributesModifiers.Count)
            {
                attributesModifiers.RemoveAt(attributesModifiers.Count - 1);
                attributesModifiersSizes.RemoveAt(attributesModifiersSizes.Count - 1);
            }
        }

        for (int i = 0; i < attributesModifiers.Count; i++)
        {
            GUIStyle gstyleTextLevel = new GUIStyle(GUI.skin.textArea);
            gstyleTextLevel.margin.left += 10;
            GUIStyle gstyleTextAttributes = new GUIStyle(GUI.skin.textArea);
            gstyleTextAttributes.margin.left += 20;
            GUIStyle gstyleEnum = new GUIStyle(GUI.skin.GetStyle("MiniToolbarPopup"));
            gstyleEnum.margin.left += 24;

            attributesModifiersSizes[i] = EditorGUILayout.IntField("Level " + (i + 1) + " modifiers", attributesModifiersSizes[i], gstyleTextLevel);
            if (attributesModifiers[i].skillAttributes == null)
            {
                ListSkillAttributes lsa = new ListSkillAttributes();
                lsa.skillAttributes = new List<SkillAttribute>();
                attributesModifiers[i] = lsa;
            }

            if (attributesModifiersSizes[i] > attributesModifiers[i].skillAttributes.Count)
            {
                while (attributesModifiersSizes[i] > attributesModifiers[i].skillAttributes.Count)
                {
                    attributesModifiers[i].skillAttributes.Add(new SkillAttribute());
                }

            }
            else if (attributesModifiersSizes[i] < attributesModifiers[i].skillAttributes.Count)
            {
                while (attributesModifiersSizes[i] < attributesModifiers[i].skillAttributes.Count)
                {
                    attributesModifiers[i].skillAttributes.RemoveAt(attributesModifiers[i].skillAttributes.Count - 1);
                }
            }

            for (int j = 0; j < attributesModifiers[i].skillAttributes.Count; j++)
            {
                SkillAttribute sa = new SkillAttribute();
                sa.nameSkillAttribute = (NameSkillAttribute)EditorGUILayout.EnumPopup("Attribute name", attributesModifiers[i].skillAttributes[j].nameSkillAttribute, gstyleEnum);
                sa.value = EditorGUILayout.FloatField("Value", attributesModifiers[i].skillAttributes[j].value, gstyleTextAttributes);
                attributesModifiers[i].skillAttributes[j] = sa;
            }

            EditorGUILayout.Space();
        }

        ps_skill = (GameObject)EditorGUILayout.ObjectField("Particle System", ps_skill, typeof(GameObject), true);

        if (GUILayout.Button("Add new skill"))
        {
            AddNewSkill(search_results);
        }
    }

    private static void AddNewSkill(string[] search_results)
    {
        GameObject emptyGO = new GameObject();
        GameObject tmp = PrefabUtility.CreatePrefab(folderName, emptyGO);
        Skill skill = tmp.AddComponent<Skill>();
        skill.NameSkill = nameSkill;
        skill.Description = description;
        skill.SkillLevel = skillLevel;
        skill.SkillShot = skillShot;
        skill.Targetted = targetted;
        skill.Passive = passive;
        skill.AreaSkillShot = AssetDatabase.LoadAssetAtPath<GameObject>(search_results[indexAreaSkillshot]);
        skill.TargetSelf = targetSelf;
        skill.TargetEnemies = targetEnemies;
        skill.TargetFriendlies = targetFriendlies;

        List<UnitType> tmpEnemies = new List<UnitType>();
        for (int i = 0; i < targettableEnemies.Count; i++)
            if (targettableEnemies[i])
                tmpEnemies.Add((UnitType)i);
        List<UnitType> tmpFriendlies = new List<UnitType>();
        for (int i = 0; i < targettableFriendlies.Count; i++)
            if (targettableFriendlies[i])
                tmpFriendlies.Add((UnitType)i);

        skill.TargetablesEnemies = tmpEnemies;
        skill.TargetablesFriendlies = tmpFriendlies;

        List<ListSkillAttributes> lsa = new List<ListSkillAttributes>();
        for (int i = 0; i < attributesModifiers.Count; i++)
        {
            lsa.Add(new ListSkillAttributes());
            for (int j = 0; j < attributesModifiers[i].skillAttributes.Count; j++)
            {
                if (lsa[i].skillAttributes == null)
                {
                    ListSkillAttributes haha = new ListSkillAttributes();
                    haha.skillAttributes = new List<SkillAttribute>();
                    lsa[i] = haha;
                }
                lsa[i].skillAttributes.Add(attributesModifiers[i].skillAttributes[j]);
            }
        }
        skill.ListSkillAttributes1 = lsa;

        
        if (ps_skill)
        {
            GameObject instantiatedTmp = Instantiate(tmp);
            Instantiate(ps_skill, instantiatedTmp.transform);

            PrefabUtility.ReplacePrefab(instantiatedTmp, tmp);
            DestroyImmediate(instantiatedTmp);
        }

        DestroyImmediate(emptyGO);
    }

    [MenuItem("New/Skill")]
    public static void Init()
    {
        //EditorWindow.GetWindow(typeof(NewSkillWindow), false, "Skills");
        EditorWindow.GetWindow(typeof(NewSkillWindow));
        nameSkill = "Fireball_editor";
        folderName = "Assets/LALALALA/" + nameSkill + ".prefab";

        foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
        {
            targettableEnemies.Add(new bool());
            targettableFriendlies.Add(new bool());
        }
    }
}

public enum InitialPositionState
{
    Sender,
    MousePosition
}