using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

[Serializable]
public struct PhaseAttackMapping
{
    public Phases phases;
    public State attack;
}

public enum State
{
    Idle,
    Phase1_LongRange_DashTowardsPlayer,
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
    //------------------------------------------------------------------------------

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
    //------------------------------------------------------------------------------

    [Space]
    [Header("Dash Attack-----------------------------------------------------")]
    [SerializeField]
    private float _dashFixedDistance = 4.2f;

    [SerializeField]
    private float _randomDashDistanceOffsetRange = 1f;

    [SerializeField]
    private float _dashLerpTime = 0.5f;

    private float _dashDistanceForAttack = 0;
    //------------------------------------------------------------------------------

    private State _currentState = State.Idle;

    private Phases _currentPhase = Phases.Phase1;
    //serves as exit condition from each state for now
    private bool _tempChangeStateTrigger = true;

    private ParabolaController _pc;
    private bool _committedInAttack = false;
    private bool _repositioning = false;

    //boss current health
    private int _currentHealth;
    //material
    private Material matWhite;
    private Material matDefault;
    //------------------------------------------------------------------------------

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
        Handles.Label(transform.position + new Vector3(0, 2, 0), $"{m_FacingRight}");
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

        FaceTowardsPlayer();
        if (!_committedInAttack)
        {
        }
    }

    private void FaceTowardsPlayer()
    {
        if (Player.instance.transform.position.x < transform.position.x && m_FacingRight)
        {
            Flip();
        }
        if (Player.instance.transform.position.x > transform.position.x && !m_FacingRight)
        {
            Flip();
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
        //Debug.Log(newState);
        //based on the state, we start a different coroutine
        switch (_currentState)
        {
            case State.Idle:
                {
                    StartCoroutine(OnIdle());
                    break;
                }
            case State.Phase1_LongRange_DashTowardsPlayer:
                {
                    StartCoroutine(OnPhase1_LongRange_DashTowardsPlayer());
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
        // i think I should make him jump a fixed distance based on which direction the player is wrt the boss rather than
        //just the player position

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

    IEnumerator OnPhase1_LongRange_DashTowardsPlayer()
    {
        //setup
        float randomOffset = UnityEngine.Random.Range(-_randomDashDistanceOffsetRange, _randomDashDistanceOffsetRange);
        _dashDistanceForAttack = _dashFixedDistance + randomOffset;

        //Debug.Log($"new dash + {Mathf.Abs(Player.instance.transform.position.x - transform.position.x)}");

        //first verify if the boss if close enough to do attack
        //if not, reposition
        if (Mathf.Abs(Player.instance.transform.position.x - transform.position.x) > _dashDistanceForAttack)
        {
            DetermineAndMoveToAttackRange(_dashDistanceForAttack);
        }

        //making sure attack doesnt happen until repositioning is finished
        if (_repositioning == true)
        {
            while (_repositioning)
            {
                //this is 'Start' of this state
                yield return new WaitForSeconds(0.0f);
            }
        }

        //the position boss has to move after dash is complete based on the direction he is facing wrt to player
        float finalXPos = m_FacingRight ? transform.position.x + _dashFixedDistance : transform.position.x - _dashFixedDistance;
        finalXPos += randomOffset;

        //TODO trigger animation

        //makes sure this state remains until the animation and movement is complete
        _committedInAttack = true;

        //lerp position
        var tweener = transform.DOMoveX(finalXPos, _dashLerpTime);

        //this is what will actually make the while loop stop after lerping is complete
        DashAttackComplete(tweener);

        while (_committedInAttack)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }

        SetState(ChooseAttack());
    }

    private void DetermineAndMoveToAttackRange(float attackRange)
    {
        float reposPoint = m_FacingRight ? Player.instance.transform.position.x - attackRange : Player.instance.transform.position.x + attackRange;

        _repositioning = true;
        Reposition(new Vector2(reposPoint, transform.position.y));
    }

    //marks the dash attack complete after lerping to the destination is complete
    async private void DashAttackComplete(Tweener tween)
    {
        await tween.AsyncWaitForCompletion();
        _committedInAttack = false;
    }

    //should it account for Y repositioning as well?
    async private void Reposition(Vector2 pos)
    {
        //TODO needs to make sure the boss doesn't exit the battle arena as well

        //using local distance for calculation of time to be taken for 
        var tweener = transform.DOMove(pos, (pos.x - transform.position.x) / speed);
        await tweener.AsyncWaitForCompletion();
        _repositioning = false;
    }
}

/* oLD JUMP AND CRUSH WHICH FOLLOWS PLAYER POS ACCURATELY
IEnumerator OnPhase1_ShortRange_JumpAndCrush()
{
    // i think I should make him jump a fixed distance based on which direction the player is wrt the boss rather than
    //just the player position

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
}*/

//jump and slam player
//dash and use melee weapon on ground
//add repositioning of boss towards the player
//initially, there will be a stone in the center player can use to climb
//while entering phase 2 boss will destroy it and make the fight difficult
//starting of the boss fight sequence should switch 1) input off 2) lerp camera a little back to a fixed point with a bigger FOV and 3) have boss intro animation
