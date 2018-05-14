using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerXp : MonoBehaviour {

    private List<HerosUnit> closeHeros = new List<HerosUnit>();

	void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<HerosUnit>())
            closeHeros.Add(collider.GetComponent<HerosUnit>());
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<HerosUnit>())
            closeHeros.Remove(collider.GetComponent<HerosUnit>());
    }

    public void GiveExp()
    {
        for (int i = 0; i < closeHeros.Count; i++)
        {
            closeHeros[i].GetXp(GetComponentInParent<Unit>().XpValue);
        }
    }
}
