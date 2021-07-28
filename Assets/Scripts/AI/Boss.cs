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
    Phase1_ShortRange_JumpAndCrush,
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
    [Header("General---------------------------------------------------------")]
    [SerializeField]
    private float _distanceForLongRangeAttacks = 5;

    [SerializeField]
    private List<PhaseAttackMapping> _attacksPerPhase;

    [SerializeField]
    private LayerMask _groundLayer;

    [Space]
    [Header("Slam Attack-----------------------------------------------------")]
    [SerializeField]
    private int _slamsInFirstPhase = 3;

    [SerializeField]
    private Transform _startPos;

    [SerializeField]
    private Transform _endPos;

    [SerializeField]
    private Transform _height;

    private State _currentState = State.Idle;
    private Phases _currentPhase = Phases.Phase1;
    //serves as exit condition from each state for now
    private bool _tempChangeStateTrigger = true;

    private ParabolaController _pc;

    //boss current health
    private int _currentHealth;
    //material
    private Material matWhite;
    private Material matDefault;

    private void OnEnable()
    {
        _pc = GetComponent<ParabolaController>();
    }

    private void Start()
    {
        //hit feedback set up
        _sr = GetComponent<SpriteRenderer>();
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        matDefault = _sr.material;


        _currentHealth = maxHealth;
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

        //TODO add range based decisions wrt to player and boss
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

    //take damage
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        //hit feedback
        _sr.material = matWhite;
        if (_currentHealth <= 0)
        {
            Die();
            Invoke("ResetMaterial", 0.1f);
        }
        else
        {
            Invoke("ResetMaterial", 0.1f);
        }
    }

    private void ResetMaterial()
    {
        _sr.material = matDefault;
    }

    //Die
    private void Die()
    {
        Debug.Log("Enemy Died!");
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
            case State.Phase1_ShortRange_JumpAndCrush:
                {
                    StartCoroutine(OnPhase1_ShortRange_JumpAndCrush());
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

    IEnumerator OnPhase1_ShortRange_JumpAndCrush()
    {
        //play animation of build up plus attack
        //adjust start and end points
        Ray2D ray = new Ray2D(Player.instance.transform.position, Vector2.down);
        var hitOut = Physics2D.Raycast(Player.instance.transform.position, Vector2.down, 100, _groundLayer);
        if (hitOut.collider.gameObject != null)
        {
            _startPos.position = hitOut.point + new Vector2(0, 1);
            _endPos.position = transform.position + new Vector3(0, 1, 0);
            _height.position = new Vector3((_startPos.position.x + _endPos.position.x) / 2, Player.instance.transform.position.y + 3.5f, 0);
            //trigger movement along parabola
            _pc.FollowParabola();
            //this is 'Start' of this state
            yield return new WaitForSeconds(0.0f);

            Invoke("ToggleStateChangeTrigger", 2f);
            while (_tempChangeStateTrigger)
            {
                //this is fixedupdate for this state
                yield return new WaitForFixedUpdate();
            }
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
//jump and slam player
//dash and use melee weapon on ground
