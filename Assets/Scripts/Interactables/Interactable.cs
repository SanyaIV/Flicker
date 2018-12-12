using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour {

    [Header("Interactable")]
    [SerializeField] protected bool _enabled = true;
    [SerializeField] protected bool _showMouse = true;
    [SerializeField] protected bool _holdToActivate = false;
    [SerializeField] protected float _holdDownTimeToActivate = 0f;
    private static Text _interactableText;
    private static Image _mouseImage;
    private static Image _crosshair;
    private static Slider _slider;
    private static bool _initialized = false;
    private bool _showing = false;
    private float _heldDownCounter = 0f;

    [Header("Player")]
    private static PlayerController _player;

    public abstract string ActionType();
    public abstract string GetName();

    public virtual void Start()
    {
        Initialize();
    }

    public static void Initialize()
    {

        if (_interactableText == null)
        {
            _interactableText = GameObject.Find("InteractText").GetComponent<Text>();
            _slider = _interactableText.GetComponentInChildren<Slider>();
            _slider.gameObject.SetActive(false);
            _interactableText.text = "";
        }
        if (_mouseImage == null)
        {
            _mouseImage = GameObject.Find("MouseImage").GetComponent<Image>();
            _mouseImage.enabled = false;
        }
        if (_crosshair == null)
            _crosshair = GameObject.Find("Crosshair").GetComponent<Image>();

        if(_player == null)
           _player = GameManager.player.GetComponent<PlayerController>();

    }

    public virtual void Interact(PlayerController player)
    {
        Conceal();
    }

    public void Update()
    {
        if (_showing && !GameManager.pausePlayerMovement)
        {
            if (!_holdToActivate && Input.GetButtonDown("Fire1"))
                Interact(_player);

            if (_holdToActivate && Input.GetButton("Fire1"))
            {
                _heldDownCounter += Time.deltaTime;
                _slider.gameObject.SetActive(true);
                _slider.value = _heldDownCounter / _holdDownTimeToActivate;
                if (_heldDownCounter >= _holdDownTimeToActivate + MathHelper.FloatEpsilon)
                    Interact(_player);
            }
            else
            {
                _heldDownCounter = 0f;
                _slider.gameObject.SetActive(false);
            }

        }
    }

    public bool IsEnabled()
    {
        return _enabled;
    }

    public void Indicate()
    {
        if (!_enabled || _showing)
            return;

        _interactableText.text = ActionType() + " " + GetName();
        if(_showMouse)
            _mouseImage.enabled = true;
        _crosshair.enabled = false;
        _showing = true;
    }

    public void Conceal()
    {
        if (!_initialized)
            Initialize();

        _interactableText.text = "";
        _mouseImage.enabled = false;
        _crosshair.enabled = true;
        _slider.gameObject.SetActive(false);
        _showing = false;
        _heldDownCounter = 0f;
    }

    public void Enable()
    {
        _enabled = true;
    }

    public void Disable()
    {
        _enabled = false;
    }
}
