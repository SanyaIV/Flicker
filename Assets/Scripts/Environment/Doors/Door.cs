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

    [Header("Save")]
    private bool _savedOpen;
    private bool _saveLocked;

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
            _openPos = transform.localPosition;
            _closePos = _openPos + Vector3.forward * (reverseDirection ? -1 : 1) * (doorLength > 0 ? doorLength : transform.lossyScale.z);
        }
        else
        {
            _closePos = transform.localPosition;
            _openPos = _closePos - Vector3.forward * (reverseDirection ? -1 : 1) * (doorLength > 0 ? doorLength : transform.lossyScale.z);
        }

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
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
            if (Vector3.Distance(transform.localPosition, _openPos) > MathHelper.FloatEpsilon)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _openPos, speed * Time.deltaTime);
            }
            else
            {
                transform.localPosition = _openPos;
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
            if (Vector3.Distance(transform.localPosition, _closePos) > MathHelper.FloatEpsilon)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _closePos, speed * Time.deltaTime);
                AutomaticOpen();
            }
            else
            {
                transform.localPosition = _closePos;
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
            if(_closeCoroutine != null)
                StopCoroutine(_closeCoroutine);
            Open();
        }
    }

    public bool GetSavedLockState()
    {
        return _saveLocked;
    }

    public void Save()
    {
        _savedOpen = isOpen;
        _saveLocked = locked;
    }

    public void ReloadSave()
    {
        StopAllCoroutines();
        locked = _saveLocked;
        isOpen = _savedOpen;

        if (isOpen)
            transform.localPosition = _openPos;
        else
            transform.localPosition = _closePos;
    }
}
