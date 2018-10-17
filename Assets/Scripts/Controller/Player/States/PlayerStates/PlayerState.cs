using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State {

    protected const int INVERSE = -1;
   
    protected PlayerController controller;
    protected CharacterController charCtrl { get { return controller.charCtrl;  } }
    protected Transform transform { get { return controller.transform; } }
    protected Vector3 moveDir { get { return controller.moveDir; } set { controller.moveDir = value; } }

    public override void Initialize(Controller owner)
    {
        controller = (PlayerController)owner;
    }
}
