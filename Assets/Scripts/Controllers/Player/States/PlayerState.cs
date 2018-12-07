using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State {

    protected const int INVERSE = -1;
   
    protected PlayerController controller;
    protected CharacterController charCtrl { get { return controller.charCtrl;  } }
    protected Transform transform { get { return controller.transform; } }
    protected Vector3 moveDir { get { return controller.moveDir; } set { controller.moveDir = value; } }

    [Header("Interactable")]
    [SerializeField] private float _range;
    [SerializeField] private LayerMask _interactableLayerMask;
    private Interactable _lastInteractable;
    private Interactable _currentInteractable;

    public override void Initialize(Controller owner)
    {
        controller = (PlayerController)owner;
    }

    protected Interactable GetInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(controller.cam.transform.position, controller.cam.transform.forward, out hit, _range, _interactableLayerMask))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                _currentInteractable = hit.collider.GetComponent<Interactable>();
                if (_lastInteractable != null && _lastInteractable != _currentInteractable)
                    _lastInteractable.Conceal();

                if (_currentInteractable && _currentInteractable.IsEnabled())
                {
                    _currentInteractable.Indicate();
                    return _lastInteractable = _currentInteractable;
                }
            }
        }

        if(_lastInteractable != null)
            _lastInteractable.Conceal();
        _lastInteractable = null;

        return null;
    }
}
