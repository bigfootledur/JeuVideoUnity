using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseableCollectableObject : CollectableObject {

    public abstract void Use(Unit unit);
}
