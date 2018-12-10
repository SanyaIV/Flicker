using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAllRigidBodies : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(Rigidbody rb in GameObject.Find("Ship").GetComponentsInChildren<Rigidbody>())
        {
            if(rb.isKinematic && rb.collisionDetectionMode != CollisionDetectionMode.Discrete)
                Debug.Log(rb.collisionDetectionMode, rb.gameObject);
        }
    }
}
