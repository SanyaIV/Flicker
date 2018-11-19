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

    public override void Initialize(Controller owner)
    {
        controller = (PlayerController)owner;
    }

    protected Interactable GetInteractible()
    {
        RaycastHit hit;
        if (Physics.Raycast(controller.cam.transform.position, controller.cam.transform.forward, out hit, _range, _interactableLayerMask))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                Interactable actable = hit.collider.GetComponent<Interactable>();

                if (actable && actable.IsEnabled())
                {
                    actable.Indicate();
                    return actable;
                }
            }
        }

        Interactable.Conceal();

        return null;
    }

    protected void Interact()
    {
        Interactable actable = GetInteractible();
        if (actable && actable.IsEnabled())
            actable.Interact(controller);
    }
}
