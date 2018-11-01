using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour {

    [Header("Interactable")]
    [SerializeField] protected bool _enabled = true;
    [SerializeField] protected bool _showMouse = true;
    private static Text _interactableText;
    private static Image _mouseImage;
    private static Image _crosshair;

    public abstract void Interact(PlayerController player);
    public abstract string ActionType();
    public abstract string GetName();

    public virtual void Start()
    {
        if(!_interactableText)
            _interactableText = GameObject.Find("InteractText").GetComponent<Text>();
        if (!_mouseImage)
        {
            _mouseImage = GameObject.Find("MouseImage").GetComponent<Image>();
            _mouseImage.enabled = false;
        }
        if (!_crosshair)
            _crosshair = GameObject.Find("Crosshair").GetComponent<Image>();
            
    }

    public void Indicate()
    {
        if (!_enabled)
            return;

        _interactableText.text = ActionType() + " " + GetName();
        _mouseImage.enabled = true;
        _crosshair.enabled = false;
    }

    public static void Conceal()
    {
        _interactableText.text = "";
        _mouseImage.enabled = false;
        _crosshair.enabled = true;
    }
}
