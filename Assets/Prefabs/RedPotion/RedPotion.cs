using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : UseableCollectableObject {

    [SerializeField] private float recoverHealth = 100;

    public override void Use(Unit unit)
    {
        unit.Heal(recoverHealth);
        Destroy(this.gameObject);
    }
}