using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    [SerializeField] private State[] _states;
    protected readonly Dictionary<Type, State> _stateDictionary = new Dictionary<Type, State>();
    public State currentState;
    public State previousState;

    public virtual void Awake()
    {
        foreach(State state in _states)
        {
            State instance = Instantiate(state);
            previousState = instance;
            instance.Initialize(this);
            _stateDictionary.Add(instance.GetType(), instance);

            if (currentState != null) continue;
            currentState = instance;
            currentState.Enter();
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
        if(currentState != null)
            currentState.Update();
    }

    public virtual void FixedUpdate()
    {
        if(currentState != null)
            currentState.FixedUpdate();
    }

    public T GetState<T>()
    {
        Type type = typeof(T);
        if (!_stateDictionary.ContainsKey(type)) throw new NullReferenceException("No state of type: " + type + " found");
        return (T) Convert.ChangeType(_stateDictionary[type], type);
    }

    public void TransitionTo<T>()
    {
        previousState = currentState;
        currentState.Exit();
        currentState = GetState<T>() as State;
        currentState.Enter();
    }
}
