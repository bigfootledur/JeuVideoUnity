using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireInvokation : Skill
{
    private GameMaster gameMaster;

    [SerializeField] private Unit invokation;   

    protected override void Awake()
    {
        base.Awake();
        gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource.clip)
            AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);
    }

    protected override void Start()
    {
        base.Start();
        float randomX;
        float randomZ;

        int range = 30;

        System.Random random = new System.Random();
        Vector3 positionInvokation = Vector3.zero;

        while (positionInvokation.x < 18 || positionInvokation.z > 82 || positionInvokation.z < 16)
        {
            randomX = random.Next(-(int)transform.position.x + range, (int)transform.position.x + range);
            randomZ = random.Next(-(int)transform.position.z + range, (int)transform.position.z + range);
            positionInvokation = new Vector3(randomX, 0, randomZ);
        }

        transform.position = positionInvokation;

        if (Sender.faction.Equals(Faction.Red))
            gameMaster.spawnUnit(Invokation.gameObject, transform.position, Invokation.transform.rotation, gameMaster.RallyPointBlueUnits, Faction.Red);
        else if (Sender.faction.Equals(Faction.Blue))
            gameMaster.spawnUnit(Invokation.gameObject, transform.position, Invokation.transform.rotation, gameMaster.RallyPointRedUnits, Faction.Blue);

        Destroy(this);
    }

    public override void SoftEndOfTheSkill()
    {
        // Do nothing
    }

    public Unit Invokation
    {
        get
        {
            return invokation;
        }

        set
        {
            invokation = value;
        }
    }
}
