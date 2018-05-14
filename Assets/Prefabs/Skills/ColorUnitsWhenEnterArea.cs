using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorUnitsWhenEnterArea : MonoBehaviour {

    List<Unit> units = new List<Unit>();

	void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Unit>())
        {
            collider.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.green;
            units.Add(collider.GetComponent<Unit>());
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Unit>())
        {
            if(collider.GetComponent<Unit>().faction.Equals(Faction.Red))
                collider.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            else if (collider.GetComponent<Unit>().faction.Equals(Faction.Blue))
                collider.gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            units.Remove(collider.GetComponent<Unit>());
        }
    }

    public void DestroyIt()
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<Unit>().faction.Equals(Faction.Red))
                units[i].gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            else if (units[i].GetComponent<Unit>().faction.Equals(Faction.Blue))
                units[i].gameObject.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
        }
        Destroy(gameObject);
    }
}
