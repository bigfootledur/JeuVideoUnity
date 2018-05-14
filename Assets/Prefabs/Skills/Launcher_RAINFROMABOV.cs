using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher_RAINFROMABOV : Skill {

    [SerializeField] private RAINFROMABOV fireBallPrefab;

    //[SerializeField] private float areaRadius;
    [SerializeField] private float timeBetweenFireballs = 0.2f;
    [SerializeField] private float timePoint;
    //[SerializeField] 

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        timePoint = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time - timePoint > timeBetweenFireballs)
        {
            //Vector3 meteorTarget = new Vector3(this.TargetPosition.x + 0.1f, this.TargetPosition.y, this.TargetPosition.z + 0.2f);
            Vector2 randPos = Random.insideUnitCircle * AreaRadius(SkillLevel);
            Skill skill = Instantiate(fireBallPrefab, new Vector3(TargetPosition.x + randPos.x, TargetPosition.y + 200f, TargetPosition.z + randPos.y)  , Quaternion.identity);
            skill.Faction = this.Faction;
            skill.Sender = this.Sender;
            skill.TargetPosition = new Vector3(this.TargetPosition.x, this.TargetPosition.y, this.TargetPosition.z);
            skill.TargetPosition += new Vector3(randPos.x, 0, randPos.y);
            timePoint = Time.time;
        }
    }

    public float AreaRadius(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.AreaRadius))
                    return list[i].value;
            }
        }
        return 0;
    }
}
