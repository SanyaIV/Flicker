using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeepingAngel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        /*int i = 0;
        int j = 0;

        
        _controller.planes = GeometryUtility.CalculateFrustumPlanes(_controller.cam);
        if (_controller.renderer.isVisible)
        {
            foreach (BoxCollider coll in _controller.colls)
            {
                if (i < 1 && GeometryUtility.TestPlanesAABB(_controller.planes, coll.bounds))
                {
                    i++;
                }

                if (i > 0)
                {
                    RaycastHit hit;
                    Physics.Linecast(coll.bounds.center, _controller.cam.transform.position, out hit);
                    if (hit.collider.tag == "ThirdPersonCamera")
                    {
                        j++;
                        break;
                    }
                }

            }
        }

        if (j > 0)
            Debug.Log("Visible");
        else
            Debug.Log("Invisible");*/
    }
}
