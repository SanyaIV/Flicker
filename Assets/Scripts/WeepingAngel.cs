using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeepingAngel : MonoBehaviour {

    private Plane[] _planes;
    private Camera _cam;
    private Renderer _rend;
    private Vector3[] _points = new Vector3[9];

    public BoxCollider[] colls;
    public LayerMask ignoreLayers;
    public bool visible = false;
    public Transform player;
    public float speed;

	// Use this for initialization
	void Start () {
        _cam = Camera.main;
        _rend = GetComponent<Renderer>();
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
        if (_rend.isVisible)
        {
            if (Vector3.Dot((_cam.transform.position - transform.position).normalized, _cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(_cam.transform.position, transform.position) / 10))
                return false;

            _planes = GeometryUtility.CalculateFrustumPlanes(_cam);

            foreach (BoxCollider coll in colls)
            {
                if (GeometryUtility.TestPlanesAABB(_planes, coll.bounds))
                {
                    _points[0] = transform.TransformPoint(coll.center);
                    _points[1] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                    _points[2] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                    _points[3] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                    _points[4] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                    _points[5] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                    _points[6] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                    _points[7] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                    _points[8] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                    foreach (Vector3 point in _points)
                        if (!Physics.Linecast(point, _cam.transform.position, ignoreLayers))
                            return true;
                }
            }
        }

        return false;
    }
}
