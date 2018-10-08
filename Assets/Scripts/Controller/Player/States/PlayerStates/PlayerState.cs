using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : State {

    protected const int INVERSE = -1;
   
    protected PlayerController _controller;
    protected CharacterController _charCtrl { get { return _controller._charCtrl;  } }
    protected Transform transform { get { return _controller.transform; } }
    protected Vector3 MoveDir { get { return _controller.MoveDir; } set { _controller.MoveDir = value; } }

    public override void Initialize(Controller owner)
    {
        _controller = (PlayerController)owner;
    }
}
