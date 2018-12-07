using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasscardUI : PickUpUI
{
    [SerializeField] private string _passcard;

    public override bool HasBeenPickedUp()
    {
        return _player.HasPasscard(_passcard);
    }
}
