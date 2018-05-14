using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPotion : UseableCollectableObject {

    [SerializeField] private float recoverHealth = 10;

    public override void Use(Unit unit)
    {
        unit.Heal(recoverHealth);
        Destroy(this.gameObject);
    }
}
