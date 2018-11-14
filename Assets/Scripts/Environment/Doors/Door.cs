using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private Vector3 _openPos;
    private Vector3 _closePos;
    private Coroutine _closeCoroutine;
    private BoxCollider _coll;

    [Header("Movement")]
    public float doorLength;
    public bool reverseDirection = false;
    public float speed;
    public bool isOpen;
    public bool closing;
    public bool opening;

    [Header("Automatic Open")]
    public float checkDistance = 0f;
    public Vector3 offset = Vector3.zero;
    public LayerMask automaticOpenLayers;

    [Header("State")]
    public bool locked;

    [Header("Save")]
    private bool _savedOpen;
    private bool _saveLocked;

	// Use this for initialization

    void Awake()
    {
        _coll = GetComponent<BoxCollider>();

        if (doorLength == 0)
        {
            Vector3 vertice1 = transform.TransformPoint(_coll.center + new Vector3(_coll.size.x, -_coll.size.y, _coll.size.z) * 0.5f);
            Vector3 vertice2 = transform.TransformPoint(_coll.center + new Vector3(_coll.size.x, -_coll.size.y, -_coll.size.z) * 0.5f);
            doorLength = Vector3.Distance(vertice1, vertice2);
        }

        if (checkDistance == 0f)
            checkDistance = doorLength;

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
    }

	void Start () {
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

        if (Physics.BoxCast(_coll.bounds.center, _coll.bounds.extents, reverseDirection ? -transform.forward : transform.forward, Quaternion.identity, checkDistance, automaticOpenLayers, QueryTriggerInteraction.Collide))
        {
            if (_closeCoroutine != null)
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
