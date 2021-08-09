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
    Phase1_LongRange_SwordBoomerang,
    Phase2_ShortRange_Desc,
    Phase3_LongRange_Desc,
    Phase3_ShortRange_Desc,
    StartTaunt,
    Defeated
}

public enum Phases
{
    Phase1,
    Phase2
}

public class Boss : BaseCharacter
{
    // [SerializeField]
    // private LayerMask whatIsPlayer;

    [Header("General---------------------------------------------------------")]
    [SerializeField]
    private float knockBackPower = 100;

    [SerializeField]
    private float knockBackDuration = 1;

    [SerializeField]
    private float _distanceForLongRangeAttacks = 5;

    [SerializeField]
    private List<PhaseAttackMapping> _attacksPerPhase;

    [SerializeField]
    private LayerMask _groundLayer;

    [SerializeField]
    private float _timeBetweenAttacks = 1;

    [SerializeField]
    private float _randomOffsetBetweenAttacks = 0.3f;

    [SerializeField]
    private Transform _startXPosForRoom;

    [SerializeField]
    private Transform _endXPosForRoom;

    [SerializeField]
    private Transform _startYPosForRoom;

    [SerializeField]
    private Transform _endYPosForRoom;
    //------------------------------------------------------------------------------

    [Space]
    [Header("Slam Attack-----------------------------------------------------")]
    [SerializeField]
    private int _slamsInFirstPhase = 3;

    // [SerializeField]
    // private Transform[] _slamCurveTransforms;

    [SerializeField]
    private float _slamRange = 6;

    [SerializeField]
    private float _slamRangeRandomness = 1.5f;

    [SerializeField]
    private float _slamHeight = 5;

    [SerializeField]
    private float _slamLerpTime = 1;

    // [SerializeField]
    // private Ease _slamMovementEasing = Ease.InCirc;

    [SerializeField]
    private AnimationCurve _curve;
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
    [Space]
    [Header("Boomerang Attack-----------------------------------------------------")]

    [SerializeField]
    private float _boomerangAttackRange = 7;

    [SerializeField]
    private float _randomBoomerangDistanceOffsetRange = 1f;

    [SerializeField]
    private float _boomerangLerpTime = 0.5f;

    [SerializeField]
    private int _boomerangRevs = 5;

    [SerializeField]
    private float _boomerangMidInterval = 0.25f;

    [SerializeField]
    private float _boomerangRotationForce = 150;

    [SerializeField]
    private GameObject _swordGO;
    //------------------------------------------------------------------------------

    private State _currentState = State.Idle;

    private Phases _currentPhase = Phases.Phase1;
    //serves as exit condition from each state for now
    private bool _tempChangeStateTrigger = true;

    //private ParabolaController _pc;
    private bool _committedInAttack = false;
    private bool _repositioning = false;

    //boss current health
    private int _currentHealth;
    //material
    private Material matWhite;
    private List<Material> matDefault = new List<Material>();
    //private bool _isBossInactive = true;
    private Rect room;


    //------------------------------------------------------------------------------

    private void OnEnable()
    {
        //_pc = GetComponent<ParabolaController>();
    }

    private void Start()
    {
        // matDefault = new Material[_sr.Length];

        //hit feedback set up
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;

        for (int i = 0; i < _sr.Length; i++)
        {
            matDefault.Add(_sr[i].material);
        }

        _currentHealth = maxHealth;

        // SetState(State.Idle);

        room = new Rect(_startXPosForRoom.position.x, _startYPosForRoom.position.y, _endXPosForRoom.position.x - _startXPosForRoom.position.x, _endYPosForRoom.position.y - _startYPosForRoom.position.y);
    }

    //knockBack when overlap with boss  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            other.gameObject.GetComponent<Player>().KnockBack(knockBackDuration, knockBackPower, m_FacingRight ? Vector2.right : Vector2.left);

            Player.instance.TakeDamage(1);
        }
    }


    private void HitDetection()
    {
        //attack trigger
        // Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsPlayer);

        Player.instance.TakeDamage(attackDamage);

        SoundManagerScript.PlaySound("Hit");
    }



    private void OnDrawGizmos()
    {
        //helps visualize the state of the AI
        Handles.Label(transform.position + new Vector3(0, 2, 0), $"{m_FacingRight}");
        Handles.Label(transform.position + new Vector3(0, 2.5f, 0), $"{_currentState}");

        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        // /DrawRect(room);
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
        //camera shake effect
        CameraShake.instance.StartShake(.2f, .1f);

        //hit feedback
        for (int i = 0; i < _sr.Length; i++)
        {
            _sr[i].material = matWhite;
        }
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
        for (int i = 0; i < _sr.Length; i++)
        {
            _sr[i].material = matDefault[i];
        }
    }

    //Die
    private void Die()
    {
        Debug.Log("Enemy Died!");
        _anim.SetTrigger("DownState");
        SetState(State.Defeated);
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
            case State.Phase1_LongRange_SwordBoomerang:
                {
                    StartCoroutine(OnPhase2_LongRange_SwordBoomerang());
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
            case State.StartTaunt:
                {
                    StartCoroutine(OnStartTaunt());
                    break;
                }
            case State.Defeated:
                {
                    StartCoroutine(OnDefeated());
                    break;
                }
        }
    }

    IEnumerator OnIdle()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0f);
        Invoke("ToggleStateChangeTrigger", UnityEngine.Random.Range(_timeBetweenAttacks - _randomOffsetBetweenAttacks, _timeBetweenAttacks + _randomOffsetBetweenAttacks));

        while (_tempChangeStateTrigger) //|| _isBossInactive)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
            print("in idle`");
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

    IEnumerator OnStartTaunt()
    {
        //disable input of player
        Player.instance.SetInputMode(false);

        //play taunt animation
        _anim.SetTrigger("BossIntro");

        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);

        //makes sure this state remains until the animation and movement is complete
        _committedInAttack = true;

        while (_committedInAttack)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }

        //enable input of player
        Player.instance.SetInputMode(true);

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

    IEnumerator OnPhase2_LongRange_SwordBoomerang()
    {
        float boomerangeFinalDistance = _boomerangAttackRange + UnityEngine.Random.Range(-_randomBoomerangDistanceOffsetRange, _randomBoomerangDistanceOffsetRange);

        //play animation of build up plus attack


        DetermineAndMoveToAttackRange(boomerangeFinalDistance);

        //making sure attack doesnt happen until repositioning is finished
        if (_repositioning == true)
        {
            while (_repositioning)
            {
                //this is 'Start' of this state
                yield return new WaitForSeconds(0.0f);
            }
        }

        float finalXPos = m_FacingRight ? _swordGO.transform.position.x + boomerangeFinalDistance : _swordGO.transform.position.x - boomerangeFinalDistance;

        //setup vars
        var parent = _swordGO.transform.parent;
        float initPosX = _swordGO.transform.position.x;

        //unparent the GO first to avoid inheriting animation movement
        _swordGO.transform.parent = null;

        //boomerang tween
        Sequence seq = DOTween.Sequence();
        var xMovementTween = _swordGO.transform.DOMoveX(finalXPos, _boomerangLerpTime);
        seq.Append(xMovementTween);
        var zRotationTween = _swordGO.transform.DOPunchRotation(new Vector3(0, 0, _boomerangRotationForce), _boomerangLerpTime);
        seq.Insert(0, zRotationTween);
        seq.AppendInterval(_boomerangMidInterval);
        var xBackMovementTween = _swordGO.transform.DOMoveX(initPosX, _boomerangLerpTime);
        seq.Append(xBackMovementTween);
        var zBackRotationTween = _swordGO.transform.DOPunchRotation(new Vector3(0, 0, _boomerangRotationForce), _boomerangLerpTime);
        seq.Insert((seq.Duration() / 2) + (_boomerangMidInterval / 2), zBackRotationTween);

        //makes sure this state remains until the animation and movement is complete
        _committedInAttack = true;

        AttackComplete(seq);

        while (_committedInAttack)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }

        //parent it back to the original thing after attack is over
        _swordGO.transform.parent = parent;

        SetState(State.Idle);
    }

    IEnumerator OnPhase1_ShortRange_JumpAndCrush()
    {
        float slamFinalDistance = _slamRange + UnityEngine.Random.Range(-_slamRangeRandomness, _slamRangeRandomness);

        //play animation of build up plus attack

        //reposition the boss first
        //if (Mathf.Abs(Player.instance.transform.position.x - transform.position.x) > slamFinalDistance)
        //{
        DetermineAndMoveToAttackRange(slamFinalDistance);
        // }

        //making sure attack doesnt happen until repositioning is finished
        if (_repositioning == true)
        {
            while (_repositioning)
            {
                //this is 'Start' of this state
                yield return new WaitForSeconds(0.0f);
            }
        }

        //slam tween
        Sequence tween = _rb.DOJump(new Vector2(m_FacingRight ? transform.position.x + slamFinalDistance : transform.position.x - slamFinalDistance, transform.position.y), _slamHeight, 1, _slamLerpTime);
        tween.SetEase(_curve);

        //makes sure this state remains until the animation and movement is complete
        _committedInAttack = true;

        //this is 'Start' of this state
        // yield return new WaitForSeconds(0.0f);

        AttackComplete(tween);

        while (_committedInAttack)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }

        SetState(State.Idle);
    }

    IEnumerator OnPhase1_LongRange_DashTowardsPlayer()
    {
        //setup
        float randomOffset = UnityEngine.Random.Range(-_randomDashDistanceOffsetRange, _randomDashDistanceOffsetRange);
        _dashDistanceForAttack = _dashFixedDistance + randomOffset;

        //Debug.Log($"new dash + {Mathf.Abs(Player.instance.transform.position.x - transform.position.x)}");

        //first verify if the boss if close enough to do attack and still within the room
        //if not, reposition
        bool isPlayerNotWithinRange = Mathf.Abs(Player.instance.transform.position.x - transform.position.x) > _dashDistanceForAttack;
        bool isPlayerInsideRoom = room.Contains(new Vector2(Player.instance.transform.position.x, Player.instance.transform.position.y));
        if (!isPlayerNotWithinRange || !isPlayerInsideRoom)
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
        float finalXPos = m_FacingRight ? transform.position.x + _dashDistanceForAttack : transform.position.x - _dashDistanceForAttack;
        finalXPos += randomOffset;

        //TODO trigger animation

        //makes sure this state remains until the animation and movement is complete
        _committedInAttack = true;

        //lerp position
        var tweener = transform.DOMoveX(finalXPos, _dashLerpTime);

        //this is what will actually make the while loop stop after lerping is complete
        AttackComplete(tweener);

        while (_committedInAttack)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }

        SetState(State.Idle);
    }

    IEnumerator OnDefeated()
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
    }

    //generic method to determine point where boss needs to move before he can execute that particular attack
    private void DetermineAndMoveToAttackRange(float attackRange)
    {
        Vector2 reposPoint;
        float reposPointX;

        float min = Player.instance.transform.position.x - attackRange;
        float max = Player.instance.transform.position.x + attackRange;

        //works if the player is in the room
        if (room.Contains(new Vector2(min, transform.position.y)) && room.Contains(new Vector2(max, transform.position.y)))
        {
            reposPointX = m_FacingRight ? Player.instance.transform.position.x - attackRange : Player.instance.transform.position.x + attackRange;
        }
        //takes care of the position if the player is out of the room
        else
        {
            reposPointX = m_FacingRight ? Player.instance.transform.position.x + attackRange : Player.instance.transform.position.x - attackRange;
        }

        //atrifically makes sure the end point ends up in the room
        if (!room.Contains(new Vector2(reposPointX, transform.position.y)))
        {
            //needs to make sure the boss doesn't exit the battle arena
            reposPoint = MakeSureItIsWithinRoom(new Vector2(reposPointX, transform.position.y), attackRange);
        }
        else
        {
            reposPoint = new Vector2(reposPointX, transform.position.y);
        }
        _repositioning = true;
        Reposition(reposPoint);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    private Vector2 MakeSureItIsWithinRoom(Vector2 reposPoint, float attackRange)
    {
        Vector2 newPoint = reposPoint + new Vector2(attackRange, 0);

        if (newPoint.x < _startXPosForRoom.position.x || reposPoint.x < _startXPosForRoom.position.x)
        {
            newPoint = new Vector2(_startXPosForRoom.position.x + attackRange, reposPoint.y);
        }
        else if (newPoint.x > _endXPosForRoom.position.x || reposPoint.x > _endXPosForRoom.position.x)
        {
            newPoint = new Vector2(_endXPosForRoom.position.x - attackRange, reposPoint.y);
        }
        else if (newPoint.y < _startYPosForRoom.position.y || reposPoint.y < _startYPosForRoom.position.y)
        {
            newPoint = new Vector2(reposPoint.x, _startYPosForRoom.position.y);
        }
        else if (newPoint.y > _endYPosForRoom.position.y || reposPoint.y > _endYPosForRoom.position.y)
        {
            newPoint = new Vector2(reposPoint.x, _endYPosForRoom.position.y);
        }

        return newPoint;
    }

    //marks the dash attack complete after lerping to the destination is complete
    async private void AttackComplete(Tweener tween)
    {
        if (tween != null)
        {
            await tween.AsyncWaitForCompletion();
        }
        _committedInAttack = false;
    }
    async private void AttackComplete(Sequence tween)
    {
        if (tween != null)
        {
            await tween.AsyncWaitForCompletion();
        }
        _committedInAttack = false;
    }

    private void BossIntroComplete()
    {
        _committedInAttack = false;
    }

    //should it account for Y repositioning as well?
    async private void Reposition(Vector2 pos)
    {

        //using local distance for calculation of time to be taken for 
        Tweener tweener = transform.DOMove(pos, Mathf.Abs(pos.x - transform.position.x) / speed);
        await tweener.AsyncWaitForCompletion();
        _repositioning = false;
    }

    public void Init()
    {
        SetState(State.StartTaunt);
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


    //version 2 logic

        //adjust start and end points
        if (_slamCurveTransforms.Length >= 3)
        {
            //start point
            _slamCurveTransforms[0].position = new Vector3(
                x: transform.position.x,
                y: _slamCurveTransforms[0].position.y,
                z: _slamCurveTransforms[0].position.z);

            //end point
            _slamCurveTransforms[_slamCurveTransforms.Length - 1].position = new Vector3(
                x: m_FacingRight ? transform.position.x + slamFinalDistance : transform.position.x - slamFinalDistance,
                y: _slamCurveTransforms[_slamCurveTransforms.Length - 1].position.y,
                z: _slamCurveTransforms[_slamCurveTransforms.Length - 1].position.z);

            //midpoint(s)
            for (int i = 1; i < _slamCurveTransforms.Length - 1; i++)
            {
                _slamCurveTransforms[i].position = new Vector3(
                    x: (_slamCurveTransforms[0].position.x + _slamCurveTransforms[_slamCurveTransforms.Length - 1].position.x) / 2,
                    y: _slamHeight,
                    z: _slamCurveTransforms[i].position.z);
            }
        }

        //copy the transforms into a Vector3 array for tweening
        Vector3[] path = new Vector3[_slamCurveTransforms.Length];

        for (int i = 0; i < _slamCurveTransforms.Length; i++)
        {
            path[i] = _slamCurveTransforms[i].position;
            // print(path[i]);
        }

        //Tween
        var tween = transform.DOPath(path, 2, PathType.CatmullRom);
        tween.SetEase(_slamMovementEasing);
}*/

//jump and slam player
//dash and use melee weapon on ground
//add repositioning of boss towards the player
//initially, there will be a stone in the center player can use to climb
//while entering phase 2 boss will destroy it and make the fight difficult
//starting of the boss fight sequence should switch 1) input off 2) lerp camera a little back to a fixed point with a bigger FOV and 3) have boss intro animation
//integrate animations
//and make the boss go into down mode when the fight is finished
//do the jump and slam 3 times or whatever the number is int the inspector
