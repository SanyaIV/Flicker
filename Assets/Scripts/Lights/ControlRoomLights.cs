using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ControlRoomLights : Interactable {

    [Header("Lights")]
    [SerializeField] private LightController[] _lightControllers;
    [SerializeField] private UnityEvent method;

    public void On()
    {
        foreach (LightController light in _lightControllers)
        {
            light.On();
        }
    }

    public void Off()
    {
        foreach (LightController light in _lightControllers)
        {
            light.Off();
        }
    }

    public void Max()
    {
        foreach (LightController light in _lightControllers)
        {
            light.Max();
        }
    }

    public void FadeMax()
    {
        foreach (LightController light in _lightControllers)
        {
            light.FadeMax();
        }
    }

    public void Min()
    {
        foreach (LightController light in _lightControllers)
        {
            light.Min();
        }
    }

    public void FadeMin()
    {
        foreach (LightController light in _lightControllers)
        {
            light.FadeMin();
        }
    }

    public void ToggleMinMax()
    {
        foreach (LightController light in _lightControllers)
        {
            light.ToggleMinMax();
        }
    }

    public void ToggleOnOff()
    {
        foreach (LightController light in _lightControllers)
        {
            light.ToggleOnOff();
        }
    }

    public void StartFlicker()
    {
        foreach (LightController light in _lightControllers)
        {
            light.StartFlicker();
        }
    }

    public void StartFlickerMinMax()
    {
        foreach (LightController light in _lightControllers)
        {
            light.StartFlickerMinMax();
        }
    }

    public void StartFlickerForSeconds(float timeInSeconds, bool leaveOn = false)
    {
        foreach (LightController light in _lightControllers)
        {
            light.StartFlickerForSeconds(timeInSeconds, leaveOn);
        }
    }

    public void StartFlickerMinMaxForSeconds(float timeInSeconds, bool leaveOn = false)
    {
        foreach (LightController light in _lightControllers)
        {
            light.StartFlickerMinMaxForSeconds(timeInSeconds, leaveOn);
        }
    }

    public void StopFlicker(bool leaveOn = false)
    {
        foreach (LightController light in _lightControllers)
        {
            light.StopFlicker(leaveOn);
        }
    }

    public override void Interact()
    {
        method.Invoke();
    }

    public override string ActionType()
    {
        return "Use";
    }

    public override string GetName()
    {
        return "Light Switch";
    }
}
