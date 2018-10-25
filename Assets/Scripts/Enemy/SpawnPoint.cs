using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 1);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
