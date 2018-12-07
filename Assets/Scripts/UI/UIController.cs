using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Pickups")]
    [SerializeField] private List<PickUpUI> _pickupImages;

    [Header("Values")]
    [SerializeField] private float _pickUpTimeToFadeIn = 2f;
    [SerializeField] private float _pickUpTimeToFadeOut = 1f;
    [SerializeField] private float _notPickedUpAlpha = 0.3f;
    [SerializeField] private float _pickedUpAlpha = 1f;
    private const float MIN_ALPHA = 0f;
    private const float MAX_ALPHA = 1f;

    [Header("Temporary Variables")]
    private Color _c;

    void Start()
    {
        foreach(PickUpUI pickUp in _pickupImages)
        {
            _c = pickUp.GetImage().color;
            _c.a = MAX_ALPHA;
            pickUp.GetImage().color = _c;
            pickUp.GetImage().CrossFadeAlpha(MIN_ALPHA, 0f, false);
        }
    }
    
    void Update()
    {

        if (Input.GetKey(KeyCode.I))
        {
            foreach (PickUpUI pickUp in _pickupImages)
            {
                if (pickUp.HasBeenPickedUp())
                    pickUp.GetImage().CrossFadeAlpha(_pickedUpAlpha, _pickUpTimeToFadeIn, false);
                else
                    pickUp.GetImage().CrossFadeAlpha(_notPickedUpAlpha, _pickUpTimeToFadeIn, false);
            }
        }
        else
        {
            foreach (PickUpUI pickUp in _pickupImages)
                pickUp.GetImage().CrossFadeAlpha(MIN_ALPHA, _pickUpTimeToFadeOut, false);
        }
    }
}
