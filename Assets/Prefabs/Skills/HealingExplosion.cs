using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingExplosion : Skill {

    float timeStart;

    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        timeStart = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - timeStart >= 1)
            Destroy(this.gameObject);
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<WalkingUnit>() &&
            collider.GetComponent<WalkingUnit>().faction.Equals(Sender.faction) &&
            TargetablesFriendlies.Contains(Sender.UnitType))
            collider.gameObject.GetComponent<WalkingUnit>().Heal(HealAmount(SkillLevel));
    }
}
