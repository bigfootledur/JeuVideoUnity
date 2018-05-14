using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSteal : CollectableObject {    

    protected override void Awake()
    {
        base.Awake();
        BuffObject = new LifeStealBuff(1, 1, 1, 1, 0.5f);
        print(BuffObject == null);
    }
}