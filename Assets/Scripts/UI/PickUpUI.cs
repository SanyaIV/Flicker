using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PickUpUI : MonoBehaviour
{
    [Header("Private Variables")]
    protected bool _pickedUp = false;
    protected PlayerController _player;
    protected Image _image;

    public void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Start()
    {
        _player = GameManager.player.GetComponent<PlayerController>();
    }

    public abstract bool HasBeenPickedUp();

    public Image GetImage()
    {
        return _image;
    }
}
