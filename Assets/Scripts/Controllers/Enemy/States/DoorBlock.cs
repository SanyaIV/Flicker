using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/DoorBlock")]
public class DoorBlock : EnemyState
{
    [Header("Pounding")]
    [SerializeField] [Range(0, 100)] private int _percentOpenChance;
    [SerializeField] [Range(0, 100)] private int _percentGiveUpChance;
    [SerializeField] private MinMaxFloat _waitBetweenPound;

    [Header("Door")]
    [SerializeField] private LayerMask _interactableLayer;
    private DoorButton _doorButton;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _poundingSounds;
    private BasicAudio _basicAudio;

    [Header("Cooldown")]
    [SerializeField] private float _cooldown = 5f;
    private float _time;

    [Header("Private")]
    private Coroutine _coroutine;
    

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        _controller.navMeshAgent.isStopped = true;
        _doorButton = GetDoorButton();

        if (_doorButton)
        {
            _basicAudio = _doorButton.GetBasicAudio();
            _coroutine = _controller.StartCoroutine(PoundOnDoor());
        } 
    }

    public override void Exit()
    {
        base.Exit();
        if (_coroutine != null)
            _controller.StopCoroutine(_coroutine);

        _doorButton = null;
        _time = Time.time;
    }

    public override void Update()
    {
        if (_controller.IsVisible())
        {
            _controller.TransitionTo<Frozen>();
            return;
        }
    }

    private void TransitionToPrevious()
    {
        if (_controller.previousState is Patrol)
            _controller.TransitionTo<Patrol>();
        else if (_controller.previousState is Hunt)
            _controller.TransitionTo<Hunt>();
        else
            _controller.TransitionTo<Patrol>();
    }

    public bool IsCoolingDown()
    {
        return Time.time - _time < _cooldown;
    }

    private DoorButton GetDoorButton()
    {
        DoorButton doorButton = null;

        Collider[] colls = Physics.OverlapBox(_transform.position, new Vector3(5f, 5f, 5f), _transform.rotation, _interactableLayer);
        foreach(Collider coll in colls)
        {
            doorButton = coll.GetComponent<DoorButton>();
            if (doorButton)
                break;
        }

        if (!doorButton)
        {
            TransitionToPrevious();
            return null;
        }

        return doorButton.GetMaster();
    }

    private void PlayPoundingSound()
    {
        if(_poundingSounds.Length > 1)
        {
            int n = Random.Range(1, _poundingSounds.Length);
            AudioClip tmp = _poundingSounds[0];
            _poundingSounds[0] = _poundingSounds[n];
            _poundingSounds[n] = tmp;
        }

        _basicAudio.PlayOneShot(_poundingSounds[0]);
    }

    private IEnumerator PoundOnDoor()
    {
        bool firstPound = true;
        while (true)
        {
            PlayPoundingSound();

            if (!firstPound && !_doorButton.IsLocked() && Random.Range(1, 101) <= _percentOpenChance)
            {
                _doorButton.OpenDoors();
                TransitionToPrevious();
                yield break;
            } 

            yield return new WaitForSeconds(Random.Range(_waitBetweenPound.Min, _waitBetweenPound.Max));

            if (!firstPound && Random.Range(1, 101) <= _percentGiveUpChance)
            {
                _controller.TransitionTo<Patrol>();
                yield break;
            }

            firstPound = false;
        }
    }

}
