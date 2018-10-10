using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private Vector3 openPos;
    private Vector3 closePos;
    private Coroutine openCoroutine;
    private Coroutine closeCoroutine;

    public float speed;
    public bool isOpen;
    public bool closing;
    public bool opening;
    public LayerMask automaticOpenLayers;

	// Use this for initialization
	void Start () {
        if (isOpen)
        {
            openPos = transform.position;
            closePos = openPos;
            closePos.z += transform.localScale.z;
        }
        else
        {
            closePos = transform.position;
            openPos = closePos;
            openPos.z -= transform.localScale.z;
        }
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Open();
        if (Input.GetKeyDown(KeyCode.C))
            Close();
    }

    public void Open()
    {
        openCoroutine = StartCoroutine(OpenCoroutine());
    }

    public void Close()
    {
        closeCoroutine = StartCoroutine(CloseCoroutine());
    }

    private IEnumerator OpenCoroutine()
    {
        Debug.Log(Time.deltaTime);
        closing = false;
        opening = true;

        while (opening)
        {
            if (transform.position.z >= openPos.z && Vector3.Distance(transform.position, openPos) > MathHelper.FloatEpsilon)
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
        Debug.Log(Time.deltaTime);
        opening = false;
        closing = true;

        while (closing)
        {
            if (transform.position.z <= closePos.z && Vector3.Distance(transform.position, closePos) > MathHelper.FloatEpsilon)
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
        Vector3 tmp = transform.position;
        tmp.z += transform.localScale.z / 2;

        if (Physics.Raycast(tmp, transform.forward, 0.5f, automaticOpenLayers))
        {
            StopCoroutine(closeCoroutine);
            Open();
        }
    }
}
