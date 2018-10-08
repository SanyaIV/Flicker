using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    [SerializeField] private State[] _states;
    protected readonly Dictionary<Type, State> _stateDictionary = new Dictionary<Type, State>();
    public State CurrentState;
    public State PreviousState;

    public virtual void Awake()
    {
        foreach(State state in _states)
        {
            State instance = Instantiate(state);
            PreviousState = instance;
            instance.Initialize(this);
            _stateDictionary.Add(instance.GetType(), instance);

            if (CurrentState != null) continue;
            CurrentState = instance;
            CurrentState.Enter();
        }
    }

    public virtual void Start()
    {
        foreach(KeyValuePair<Type, State> entry in _stateDictionary)
        {
            entry.Value.LateInitialize();
        }
    }

    public virtual void Update()
    {
        if(CurrentState != null)
            CurrentState.Update();
    }

    public virtual void FixedUpdate()
    {
        if(CurrentState != null)
            CurrentState.FixedUpdate();
    }

    public T GetState<T>()
    {
        Type type = typeof(T);
        if (!_stateDictionary.ContainsKey(type)) throw new NullReferenceException("No state of type: " + type + " found");
        return (T) Convert.ChangeType(_stateDictionary[type], type);
    }

    public void TransitionTo<T>()
    {
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = GetState<T>() as State;
        CurrentState.Enter();
    }
}
