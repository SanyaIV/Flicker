using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

	void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
            coll.GetComponent<Sanity>().DepleteSanity(10000f);
    }
}
