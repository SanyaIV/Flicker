using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private Vector3 _openPos;
    private Vector3 _closePos;
    private Coroutine _closeCoroutine;

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
            _openPos = transform.position;
            _closePos = _openPos + transform.forward * (reverseDirection ? -1 : 1) * (doorLength > 0 ? doorLength : transform.lossyScale.z);
        }
        else
        {
            _closePos = transform.position;
            _openPos = _closePos - transform.forward * (reverseDirection ? -1 : 1) * (doorLength > 0 ? doorLength : transform.lossyScale.z);
        }
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
            StartCoroutine(OpenCoroutine());
    }

    public void Close()
    {
        _closeCoroutine = StartCoroutine(CloseCoroutine());
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
            if (Vector3.Distance(transform.position, _openPos) > MathHelper.FloatEpsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, _openPos, speed * Time.deltaTime);
            }
            else
            {
                transform.position = _openPos;
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
            if (Vector3.Distance(transform.position, _closePos) > MathHelper.FloatEpsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, _closePos, speed * Time.deltaTime);
                AutomaticOpen();
            }
            else
            {
                transform.position = _closePos;
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
            StopCoroutine(_closeCoroutine);
            Open();
        }
    }
}
