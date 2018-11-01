using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CryoPod : Interactable {

    [Header("Images")]
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private Sprite _image;
    private static Canvas _canvas;
    private static RectTransform _saveScreenTransform;
    private static Image _saveScreen;
    private bool _running;

    public override void Start()
    {
        base.Start();

        if (!_canvas)
        {
            _canvas = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
            _saveScreenTransform = new GameObject().AddComponent<RectTransform>();
            _saveScreenTransform.transform.SetParent(_canvas.transform);
            _saveScreenTransform.sizeDelta = new Vector2(10000, 10000);
            _saveScreenTransform.localPosition = Vector3.zero;
            _saveScreen = _saveScreenTransform.gameObject.AddComponent<Image>();
            _saveScreen.sprite = _image;
            _saveScreen.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public override void Interact(PlayerController player)
    {
        if (!_running)
        {
            player.sanity.ResetSanity();
            GameManager.Save();
            StartCoroutine(Save());
        }
    }

    public override string ActionType()
    {
        return "Rest in";
    }

    public override string GetName()
    {
        return "Cryo Pod";
    }

    private IEnumerator Save()
    {
        if (_running)
            yield break;

        _running = true;

        _saveScreen.color = new Color(1f, 1f, 1f, 0f);
        while(_saveScreen.color.a < 1f)
        {
            _saveScreen.color = new Color(1f, 1f, 1f, _saveScreen.color.a + _fadeSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        while (_saveScreen.color.a > 0f)
        {
            _saveScreen.color = new Color(1f, 1f, 1f, _saveScreen.color.a - _fadeSpeed * Time.deltaTime);
            yield return null;
        }

        _saveScreen.color = new Color(1f, 1f, 1f, 0f);
        _running = false;
        yield break;
    }
}
