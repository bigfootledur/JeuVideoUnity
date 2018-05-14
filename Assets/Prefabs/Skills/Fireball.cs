using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Skill {

    [SerializeField] private ParticleSystem fireExplosion;

    [SerializeField] private Unit target;
    //[SerializeField] private float projectileSpeed = 100;

    //[SerializeField] private float damage = 50;

    private ParticleSystem myParticleSystem;

    protected override void Awake()
    {
        base.Awake();
        myParticleSystem = GetComponent<ParticleSystem>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource.clip)
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        
        Vector3 heading = TargetPosition - new Vector3(transform.position.x, 0, transform.position.z);
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;

        GetComponent<Rigidbody>().velocity = direction * ProjectileSpeed(SkillLevel);
    }
	
	// Update is called once per frame
	void Update () {
        //if (target)
        //{
        //    try
        //    {
        //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), ProjectileSpeed * Time.deltaTime);
        //        transform.position += transform.forward * ProjectileSpeed * Time.deltaTime;
        //    }
        //    catch
        //    {
        //        Debug.Log("Target dead");
        //    }
        //}
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Unit>()) // Ignore terrain
        {
            // If it's a targetted skill
            //if (target)
            //{
            //    try
            //    {
            //        if (collider.gameObject.Equals(target.gameObject))
            //        {
            //            collider.gameObject.GetComponent<Unit>().GetDamage(Sender, damage);
            //            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position, 10f);
            //            Destroy(this.gameObject);
            //        }
            //    }
            //    catch
            //    {
            //        print("Target dead");
            //    }
            //}
            // If an other thing
            //else
            //{
            if (collider.GetComponent<Unit>().faction.Equals(Faction))
            {
                if (TargetFriendlies)
                {
                    if (collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.Building))
                            DealDamage(collider.GetComponent<Unit>());
                    }

                    else if (collider.GetComponent<HerosUnit>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.HerosUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>() && !collider.GetComponent<HerosUnit>() && !collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.BasicUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<WalkingUnit>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.WalkingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>())
                    {
                        if (TargetablesFriendlies.Contains(UnitType.AttackingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {

                if (TargetEnemies)
                {
                    if (collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.Building))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<HerosUnit>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.HerosUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>() && !collider.GetComponent<HerosUnit>() && !collider.GetComponent<CreationBuilding>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.BasicUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<WalkingUnit>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.WalkingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else if (collider.GetComponent<AttackingUnit>())
                    {
                        if (TargetablesEnemies.Contains(UnitType.AttackingUnit))
                            DealDamage(collider.GetComponent<Unit>());
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }

    public void DealDamage(Unit targetHit)
    {
        targetHit.GetDamage(Sender, Damage(SkillLevel));
        ParticleSystem tmp = Instantiate(fireExplosion, this.transform.position, Quaternion.identity);
        tmp.Play();
    }

    void DestroyIt()
    {
        Destroy(this.gameObject);
    }

    public float Damage(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.Damage))
                    return list[i].value;
            }
        }
        return 0;
    }

    public float ProjectileSpeed(int level)
    {
        if (level >= 0 && level < ListSkillAttributes1.Count)
        {
            List<SkillAttribute> list = ListSkillAttributes1[level].skillAttributes;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].nameSkillAttribute.Equals(NameSkillAttribute.ProjectileSpeed))
                    return list[i].value;
            }
        }

        return 0;
    }
}
