using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    [Header("Rotation")]
    [SerializeField] private bool _enabled = true;
    [SerializeField] private Vector3 _speed;
	
	// Update is called once per frame
	void Update () {
        if(_enabled)
            transform.Rotate(_speed * Time.deltaTime);
	}

    public void Enable()
    {
        _enabled = true;
    }

    public void Disable()
    {
        _enabled = false;
    }
}
