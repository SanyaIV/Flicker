using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour {

    [Header("Rotation")]
    [SerializeField] private Vector3 _speed;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(_speed * Time.deltaTime);
	}
}
