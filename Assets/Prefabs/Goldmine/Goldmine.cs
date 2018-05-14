using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goldmine : Collectable {

    private float _quantityRessources;

    void Awake()
    {
        QuantityRessources = 100000;
        Animator = GetComponent<Animator>();
    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Getters/Setters
    public float QuantityRessources
    {
        get
        {
            return _quantityRessources;
        }
        set
        {
            _quantityRessources = value;
        }
    }
    #endregion
}
