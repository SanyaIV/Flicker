using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour {

    [Header("Off-Screen Indicator")]
    [SerializeField] protected bool _enableIndicator;
    private bool _enabled;
    private Transform _source;

    [Header("Camera")]
    private Camera _cam;
    private Vector3 _cameraOffsetUp;
    private Vector3 _cameraOffsetRight;
    private Vector3 _cameraOffsetForward;

    [Header("Canvas")]
    private GameObject _canvas;

    [Header("Icon")]
    private Transform _icon;
    private Transform _iconsIcon;
    private Image _iconImage;
    private Image _iconsIconImage;
    public Sprite iconOnScreen;
    public Sprite iconOffScreen;
    public Sprite sprite;
    public Color iconColor = Color.yellow;
    public Color spriteColor = Color.yellow;
    public Vector3 iconScale;
    public Vector3 spriteScale;
    public float spriteOffset;
    public float maxRange;

    [Header("Edge")]
    public float edgeBuffer;


	// Use this for initialization
	public virtual void Start () {
        if (_enableIndicator)
        {
            _cam = Camera.main;
            _canvas = GameObject.FindGameObjectWithTag("HUD");

            InstantiateIcon();
            DisableIndicator();
        }
	}

    void LateUpdate()
    {
        //Test();
        if(_enableIndicator && _enabled)
            DrawIcon();
    }

    public void EnableIndicator(Transform source)
    {
        if (!_enableIndicator)
            return;

        _enabled = true;
        _source = source;

        Color tmp = _iconImage.color;
        tmp.a = 1f;
        _iconImage.color = tmp;
        _iconsIconImage.color = tmp;
    }

    public void DisableIndicator()
    {
        if (!_enableIndicator)
            return;

        _enabled = false;

        Color tmp = _iconImage.color;
        tmp.a = 0f;
        _iconImage.color = tmp;
        _iconsIconImage.color = tmp;
    }

    private void InstantiateIcon()
    {
        _icon = new GameObject().AddComponent<RectTransform>();
        _icon.SetParent(_canvas.transform);
        _icon.localScale = iconScale;
        _iconImage = _icon.gameObject.AddComponent<Image>();
        _iconImage.sprite = iconOnScreen;

        _iconsIcon = new GameObject().AddComponent<RectTransform>();
        _iconsIcon.SetParent(_icon);
        _iconsIcon.localScale = spriteScale;
        _iconsIconImage = _iconsIcon.gameObject.AddComponent<Image>();
        _iconsIconImage.sprite = sprite;
        _iconsIcon.localPosition = _icon.up * -15f;

        _iconImage.color = iconColor;
        _iconsIconImage.color = spriteColor;
    }

    private void DrawIcon()
    {
        Vector3 indicatorPos = Vector3.zero;
        Vector3 screenPos = _cam.WorldToScreenPoint(_source.position);

        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            _icon.transform.position = screenPos;
            _iconImage.sprite = iconOnScreen;
            _icon.rotation = Quaternion.identity;
            _iconsIcon.rotation = Quaternion.identity;
            _iconsIcon.localPosition = Vector2.zero;
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

            Vector2 padSize = new Vector2(Screen.width - edgeBuffer, Screen.height - edgeBuffer);

            if (screenPos.y < 0)
                indicatorPos = new Vector3((-padSize.y / 2) / slope, -padSize.y / 2, 0f);
            else
                indicatorPos = new Vector3((padSize.y / 2) / slope, padSize.y / 2);

            if (indicatorPos.x < -padSize.x / 2)
                indicatorPos = new Vector3(-padSize.x / 2, slope * -padSize.x / 2);
            else if (indicatorPos.x > padSize.x / 2)
                indicatorPos = new Vector3(padSize.x/2, slope * padSize.x / 2);

            indicatorPos += screenCenter;
            _icon.position = indicatorPos;
            _icon.rotation = Quaternion.identity;
            _iconsIcon.localPosition = _icon.up * spriteOffset;
            _icon.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
            _iconsIcon.rotation = Quaternion.identity;
        }

        Color tmp = _iconImage.color;
        tmp.a = Mathf.Lerp(0f, 1f, (maxRange - Vector3.Distance(_cam.transform.position, _source.position)) / maxRange);
        _iconImage.color = tmp;
        _iconsIconImage.color = tmp;
    }
}
