using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private Vector3 openPos;
    private Vector3 closePos;
    private Coroutine openCoroutine;
    private Coroutine closeCoroutine;

    [Header("Movement")]
    public float doorLength;
    public bool reverseDirection = false;
    public float speed;
    public bool isOpen;
    public bool closing;
    public bool opening;
    public LayerMask automaticOpenLayers;

    [Header("State")]
    public bool locked;

	// Use this for initialization
	void Start () {
        if (doorLength == 0)
        {
            BoxCollider box = GetComponent<BoxCollider>();

            Vector3 vertice1 = transform.TransformPoint(box.center + new Vector3(box.size.x, -box.size.y, box.size.z) * 0.5f);
            Vector3 vertice2 = transform.TransformPoint(box.center + new Vector3(box.size.x, -box.size.y, -box.size.z) * 0.5f);
            doorLength = Vector3.Distance(vertice1, vertice2);
        }

        if (isOpen)
        {
            openPos = transform.position;
            closePos = openPos + transform.forward * (reverseDirection ? -1 : 1) * (doorLength > 0 ? doorLength : transform.lossyScale.z);
        }
        else
        {
            closePos = transform.position;
            openPos = closePos - transform.forward * (reverseDirection ? -1 : 1) * (doorLength > 0 ? doorLength : transform.lossyScale.z);
        }
	}

    private void OnDrawGizmosSelected()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(box.center + new Vector3(box.size.x, -box.size.y, box.size.z) * 0.5f), 0.05f);
        Gizmos.DrawSphere(transform.TransformPoint(box.center + new Vector3(-box.size.x, -box.size.y, box.size.z) * 0.5f), 0.05f);
        Gizmos.DrawSphere(transform.TransformPoint(box.center + new Vector3(-box.size.x, -box.size.y, -box.size.z) * 0.5f), 0.05f);
        Gizmos.DrawSphere(transform.TransformPoint(box.center + new Vector3(box.size.x, -box.size.y, -box.size.z) * 0.5f), 0.05f);
    }

    void Update()
    {
        //For debugging
        if (Input.GetKeyDown(KeyCode.O))
            Open();
        if (Input.GetKeyDown(KeyCode.C))
            Close();
    }

    public void Open()
    {
        if(!locked)
            openCoroutine = StartCoroutine(OpenCoroutine());
    }

    public void Close()
    {
        closeCoroutine = StartCoroutine(CloseCoroutine());
    }

    public void Lock()
    {
        locked = true;
        Close();
    }

    public void Unlock()
    {
        locked = false;
    }

    private IEnumerator OpenCoroutine()
    {
        closing = false;
        opening = true;

        while (opening)
        {
            if (Vector3.Distance(transform.position, openPos) > MathHelper.FloatEpsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, openPos, speed * Time.deltaTime);
            }
            else
            {
                transform.position = openPos;
                opening = false;
                isOpen = true;
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator CloseCoroutine()
    {
        opening = false;
        closing = true;

        while (closing)
        {
            if (Vector3.Distance(transform.position, closePos) > MathHelper.FloatEpsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, closePos, speed * Time.deltaTime);
                AutomaticOpen();
            }
            else
            {
                transform.position = closePos;
                closing = false;
                isOpen = false;
            } 

            yield return null;
        }

        yield break;
    }

    private void AutomaticOpen()
    {
        if (Physics.Raycast(transform.position, reverseDirection ? -transform.forward : transform.forward, doorLength, automaticOpenLayers))
        {
            StopCoroutine(closeCoroutine);
            Open();
        }
    }
}
