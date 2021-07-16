using System.Collections;
using UnityEngine;

public class Boss : BaseCharacter
{
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

    private State _currentState = State.Idle;

    private void Start()
    {
        SetState(State.Idle);
    }

    private void SetState(State newState)
    {
        //before we switch state, we should stop our current state
        StopAllCoroutines();

        _currentState = newState;
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
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator OnPhase3_ShortRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator OnPhase3_LongRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator OnPhase2_ShortRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator OnPhase2_LongRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator OnPhase1_ShortRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator OnPhase1_LongRange_Desc()
    {
        //this is 'Start' of this state
        yield return new WaitForSeconds(0.0f);
        while (true)
        {
            //this is fixedupdate for this state
            yield return new WaitForFixedUpdate();
        }
    }
}
