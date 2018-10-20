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
    private Canvas _canvas;

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
        _canvas = FindObjectOfType<Canvas>();

        InstantiateIcon();
	}

    void LateUpdate()
    {
        LetsTryThisAgain();
    }

    private void InstantiateIcon()
    {
        _icon = new GameObject().AddComponent<RectTransform>();
        _icon.transform.SetParent(_canvas.transform);
        _icon.localScale = iconScale;
        _iconImage = _icon.gameObject.AddComponent<Image>();
        _iconImage.sprite = iconOnScreen;
    }

    private void LetsTryThisAgain()
    {
        Vector3 indicatorPos = Vector3.zero;
        Vector3 screenPos = _cam.WorldToScreenPoint(transform.position);

        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            _icon.transform.position = screenPos;
        }
        else
        {
            if (screenPos.z < 0)
                screenPos *= -1;

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            screenPos -= screenCenter;

            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle *= Mathf.Rad2Deg;

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



            /*Vector3 screenBounds = screenCenter * 0.9f;

            if (cos > 0)
                screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            else
                screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

            if (screenPos.x > screenBounds.x)
                screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            else if (screenPos.x < -screenBounds.x)
                screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);

            screenPos += screenCenter;

            _icon.transform.position = screenPos;
            _icon.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);*/
        }
    }

    private void Paint()
    {
        Vector3 screenPos = _cam.WorldToScreenPoint(transform.position);

        if(screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            _icon.transform.position = screenPos;
        }
        else
        {
            if(screenPos.z < 0)
                screenPos *= -1;

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
            screenPos -= screenCenter;

            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            screenPos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            if (cos > 0)
                screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            else
                screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

            if (screenPos.x > screenBounds.x)
                screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            else if (screenPos.x < -screenBounds.x)
                screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);

            screenPos += screenCenter;

            _icon.transform.position = screenPos;
            _icon.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }
}
