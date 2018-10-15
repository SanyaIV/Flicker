using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeepingAngel : MonoBehaviour {

    private Plane[] planes;
    private Camera cam;
    private Renderer rend;
    private Vector3[] points = new Vector3[9];

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
        visible = CheckIfVisible();

        if (!visible)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    private bool CheckIfVisible()
    {
        if (rend.isVisible)
        {
            Debug.Log(Vector3.Distance(cam.transform.position, transform.position) / 10 + " : " + Vector3.Dot((cam.transform.position - transform.position).normalized, cam.transform.forward));
            if (Vector3.Dot((cam.transform.position - transform.position).normalized, cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(cam.transform.position, transform.position) / 10))
                return false;

            planes = GeometryUtility.CalculateFrustumPlanes(cam);

            foreach (BoxCollider coll in colls)
            {
                if (GeometryUtility.TestPlanesAABB(planes, coll.bounds))
                {
                    points[0] = transform.TransformPoint(coll.center);
                    points[1] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                    points[2] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                    points[3] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                    points[4] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                    points[5] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                    points[6] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                    points[7] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                    points[8] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                    foreach (Vector3 point in points)
                        if (!Physics.Linecast(point, cam.transform.position, ignoreLayers))
                            return true;
                }
            }
        }

        return false;
    }
}
