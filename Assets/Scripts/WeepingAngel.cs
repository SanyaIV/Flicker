using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeepingAngel : MonoBehaviour {

    private Plane[] planes;
    private Camera cam;
    private Renderer rend;
    public BoxCollider[] colls;
    public LayerMask ignoreLayers;
    public bool visible = false;
    public Transform player;
    public float speed;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
        rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        int i = 0;
        visible = false;
        
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (rend.isVisible)
        {
            foreach (BoxCollider coll in colls)
            {
                if (i < 1 && GeometryUtility.TestPlanesAABB(planes, coll.bounds))
                {
                    i++;
                }

                if (i > 0)
                {
                    RaycastHit hit;
                    if (Physics.Linecast(coll.bounds.center, cam.transform.position, out hit, ignoreLayers))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            visible = true;
                            break;
                        }
                    }
                    else
                    {
                        visible = true;
                        break;
                    }
                }
            }
        }

        if (!visible)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        if (visible)
            Debug.Log("Visible");
        else
            Debug.Log("Invisible");
    }
}
