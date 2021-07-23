using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[Serializable]
public struct PhaseAttackMapping
{
    public Phases phases;
    public State attack;
}

public enum State
{
    Idle,
    Phase1_LongRange_Desc,
    Phase1_ShortRange_Desc,
    Phase2_LongRange_Desc,
    Phase2_ShortRange_Desc,
    Phase3_LongRange_Desc,
    Phase3_ShortRange_Desc,
}

public enum Phases
{
    Phase1,
    Phase2
}

public class Boss : BaseCharacter
{
    [SerializeField]
    private float _distanceForLongRangeAttacks = 5;

    [SerializeField]
    private List<PhaseAttackMapping> _attacksPerPhase;

    private State _currentState = State.Idle;
    private Player _playerRef;
    private Phases _currentPhase = Phases.Phase1;
    //serves as exit condition from each state for now
    private bool _tempChangeStateTrigger = true;

    private void Start()
    {
        SetState(State.Idle);
    }

    private void OnDrawGizmos()
    {
        //helps visualize the state of the AI
        Handles.Label(transform.position + new Vector3(0, 2, 0), $"{_currentState}");
    }

    private void ToggleStateChangeTrigger()
    {
        _tempChangeStateTrigger = !_tempChangeStateTrigger;
    }

    //helper function to choose attack based on phase
    private State ChooseAttack()
    {
        List<State> availableAttacks = new List<State>();
        foreach (var attacks in _attacksPerPhase)
        {
            if (attacks.phases == _currentPhase)
            {
                availableAttacks.Add(attacks.attack);
            }
        }
        int randomInt = UnityEngine.Random.Range(0, availableAttacks.Count);
        return availableAttacks[randomInt];
    }

    private void Update()
    {
        //artifically triggering phase change
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetPhase(Phases.Phase2);
        }
    }

    private void SetPhase(Phases newPhase)
    {
        _currentPhase = newPhase;
    }

    private void SetState(State newState)
    {
        //before we switch state, we should stop our current state
        StopAllCoroutines();

        _currentState = newState;
        Debug.Log(newState);
        //based on the state, we start a different coroutine
        switch (_currentState)
        {
            case State.Idle:
                {
                    StartCoroutine(OnIdle());
                    break;
                }
            case State.Phase1_LongRange_Desc:
                {
                    StartCoroutine(OnPhase1_LongRange_Desc());
                    break;
                }
            case State.Phase1_ShortRange_Desc:
                {
                    StartCoroutine(OnPhase1_ShortRange_Desc());
                    break;
                }
            case State.Phase2_LongRange_Desc:
                {
                    StartCoroutine(OnPhase2_LongRange_Desc());
                    break;
                }
            case State.Phase2_ShortRange_Desc:
                {
                    StartCoroutine(OnPhase2_ShortRange_Desc());
                    break;
                }
            case State.Phase3_LongRange_Desc:
                {
                    StartCoroutine(OnPhase3_LongRange_Desc());
                    break;
                }
            case State.Phase3_ShortRange_Desc:
                {
                    StartCoroutine(OnPhase3_ShortRange_Desc());
                    break;
                }
        }
    }

    IEnumerator OnIdle()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0f);
        Invoke("ToggleStateChangeTrigger", 2f);

        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }

    IEnumerator OnPhase3_ShortRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        Invoke("ToggleStateChangeTrigger", 2f);
        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }

    IEnumerator OnPhase3_LongRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        Invoke("ToggleStateChangeTrigger", 2f);
        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }

    IEnumerator OnPhase2_ShortRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        Invoke("ToggleStateChangeTrigger", 2f);
        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }

    IEnumerator OnPhase2_LongRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        Invoke("ToggleStateChangeTrigger", 2f);
        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }

    IEnumerator OnPhase1_ShortRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        Invoke("ToggleStateChangeTrigger", 2f);
        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }

    IEnumerator OnPhase1_LongRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        Invoke("ToggleStateChangeTrigger", 2f);
        while (_tempChangeStateTrigger)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
        ToggleStateChangeTrigger();
        SetState(ChooseAttack());
    }
}
