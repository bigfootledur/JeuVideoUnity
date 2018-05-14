using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : UseableCollectableObject
{
    [SerializeField] private float recoverMana = 100;

    public override void Use(Unit unit)
    {
        unit.RestoreMana(recoverMana);
        Destroy(this.gameObject);
    }
}
