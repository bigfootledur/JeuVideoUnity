using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameMaster gameMaster;
    [SerializeField] private int skillNumber;

    [SerializeField] private GameObject skillDescriptionTextPrefab;
    private GameObject skillDescriptionTextInstantiate;
    [SerializeField] private Text title;
    [SerializeField] private Text desc;
    [SerializeField] private Text manaCost;
    [SerializeField] private Text cooldown;
    [SerializeField] private Text skillLevel;

    void Awake()
    {
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();

        Text[] texts = skillDescriptionTextPrefab.GetComponentsInChildren<Text>();

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name.Equals("Title"))
                title = texts[i];
            else if (texts[i].name.Equals("Description"))
                desc = texts[i];
            else if (texts[i].name.Equals("ManaCost"))
                manaCost = texts[i];
            else if (texts[i].name.Equals("Cooldown"))
                cooldown = texts[i];
            else if (texts[i].name.Equals("SkillLevel"))
                skillLevel = texts[i];
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(gameMaster && gameMaster.SelectedRTSGameObjects.Count > 0 && gameMaster.SelectedRTSGameObjects[0] && 
           gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>())
        {
            if (skillNumber < gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>().Skills.Count
               && gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>().Skills[skillNumber])
            {
                Skill skill = gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>().Skills[skillNumber];
                title.text = skill.NameSkill;
                desc.text = skill.Description;
                manaCost.text = skill.ManaCost(gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>().LevelsSkill[skillNumber]).ToString();
                cooldown.text = skill.Cooldown(gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>().LevelsSkill[skillNumber]).ToString();
                skillLevel.text = (gameMaster.SelectedRTSGameObjects[0].GetComponent<HerosUnit>().LevelsSkill[skillNumber] + 1).ToString();
                
                Vector2 descriptionPosition = new Vector2(transform.position.x + 50, transform.position.y + 80);

                skillDescriptionTextInstantiate = Instantiate(skillDescriptionTextPrefab, descriptionPosition, Quaternion.identity,
                                                              GameObject.Find("Canvas").transform);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(skillDescriptionTextInstantiate);
    }
}
