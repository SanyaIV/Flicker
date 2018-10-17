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

        if (CheckIfVisible())
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    private bool CheckIfVisible()
    {
        if (_rend.isVisible) //Check if Unity thinks the renderer is visible (Not perfect but works as a quick and easy out in case it's not)
        {
            if (Vector3.Dot((_cam.transform.position - transform.position).normalized, _cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(_cam.transform.position, transform.position) / 10)) //Bad attempt at checking if the player is looking towards the enemy through the dot products of directions
                return visible = false; //Set visible to false and return visible (which is false)

            int taskNumber = Time.frameCount % colls.Length; //Run one box per frame

            if (taskNumber == 0)
                visible = false; //Reset visibility status at start of the "loop"
            if (visible)
                return true; //If visible is true then just return since it means the enemy has been seen for this round of the "loop"

            _planes = GeometryUtility.CalculateFrustumPlanes(_cam); //Get the frustum planes of the camera
            
            if (GeometryUtility.TestPlanesAABB(_planes, colls[taskNumber].bounds)) //Test if the current boxcollider is within the camera's frustum
            {
                BoxCollider coll = colls[taskNumber]; //Get the current boxcollider
                _points[0] = transform.TransformPoint(colls[taskNumber].center);
                _points[1] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f); //One corner of the boxcollider
                _points[2] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _points[3] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                _points[4] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _points[5] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _points[6] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                _points[7] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _points[8] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                foreach (Vector3 point in _points) //Loop through the points array
                    if (!Physics.Linecast(point, _cam.transform.position, ignoreLayers)) //Linecast between the enemy and the camera to check if there is anything in the way
                        return visible = true; //If there is nothing in the way then the enemy is visible, so set visible to true and return it.        
            }
        }

        return visible = false; //Set visible to false and return it
    }
}
