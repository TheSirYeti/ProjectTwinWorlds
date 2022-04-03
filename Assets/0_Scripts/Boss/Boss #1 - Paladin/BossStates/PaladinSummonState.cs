using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinSummonState : MonoBehaviour, IState
{
    private PaladinLogic paladin;
    private GameObject shieldPrefab;
    private FiniteStateMachine fsm;
    
    private float currentTime;
    private float timeSummoning;
    
    public void OnStart()
    {
        
    }

    public void OnUpdate()
    {
        currentTime += timeSummoning;

        if (currentTime < timeSummoning)
        {
            //summon
        }
        else
        {
            fsm.ChangeState(PaladinState.CHASE);
        }
    }

    public void OnExit()
    {
        
    }
}
