using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodPartUI : PickUpUI
{
    [Header("Escape Pod Part")]
    [SerializeField] private EscapePodPart _part;

    public override bool HasBeenPickedUp()
    {
        return _player.HasEscapePodPart(_part);
    }
}
