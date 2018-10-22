using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour {

    [Header("Camera")]
    private Camera _cam;
    private Vector3 _cameraOffsetUp;
    private Vector3 _cameraOffsetRight;
    private Vector3 _cameraOffsetForward;

    [Header("Canvas")]
    private GameObject _canvas;

    [Header("Icon")]
    private Transform _icon;
    private Image _iconImage;
    public Sprite iconOnScreen;
    public Sprite iconOffScreen;
    public Vector3 iconScale;

    [Header("Edge")]
    public float edgeBuffer;

    [Header("Debug")]
    public bool showDebugLines;


	// Use this for initialization
	void Start () {
        _cam = Camera.main;
        _canvas = GameObject.FindGameObjectWithTag("HUD");

        InstantiateIcon();
	}

    void LateUpdate()
    {
        //Test();
        DrawIcon();
    }

    private void InstantiateIcon()
    {
        _icon = new GameObject().AddComponent<RectTransform>();
        _icon.transform.SetParent(_canvas.transform);
        _icon.localScale = iconScale;
        _iconImage = _icon.gameObject.AddComponent<Image>();
        _iconImage.sprite = iconOnScreen;
    }

    private void Test()
    {
        Debug.Log(_cam.WorldToViewportPoint(transform.position));

        Vector3 screenPos = _cam.WorldToViewportPoint(transform.position);

        if (screenPos.z < 0)
            screenPos *= -1f;

        screenPos.x = Mathf.Clamp(screenPos.x, 0f, 1f);
        screenPos.y = Mathf.Clamp(screenPos.y, 0f, 1f);

        _icon.transform.position = _cam.ViewportToScreenPoint(screenPos);
    }

    private void DrawIcon()
    {
        Vector3 indicatorPos = Vector3.zero;
        Vector3 screenPos = _cam.WorldToScreenPoint(transform.position);

        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            _icon.transform.position = screenPos;
            _iconImage.sprite = iconOnScreen;
            _icon.transform.rotation = Quaternion.identity;
        }
        else
        {
            _iconImage.sprite = iconOffScreen;

            if (screenPos.z < 0)
                screenPos *= -1;

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            screenPos -= screenCenter;

            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float slope = screenPos.y / screenPos.x;

            float padding = 30f;
            Vector2 padSize = new Vector2(Screen.width - padding, Screen.height - padding);

            if (screenPos.y < 0)
                indicatorPos = new Vector3((-padSize.y / 2) / slope, -padSize.y / 2, 0f);
            else
                indicatorPos = new Vector3((padSize.y / 2) / slope, padSize.y / 2);

            if (indicatorPos.x < -padSize.x / 2)
                indicatorPos = new Vector3(-padSize.x / 2, slope * -padSize.x / 2);
            else if (indicatorPos.x > padSize.x / 2)
                indicatorPos = new Vector3(padSize.x/2, slope * padSize.x / 2);

            indicatorPos += screenCenter;
            _icon.transform.position = indicatorPos;
            _icon.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
        }
    }
}
