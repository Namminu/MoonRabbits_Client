using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private UIMonsterInformation uiMonsterInfo;
    
    private Animator animator;

    

    private int[] animCodeList = new[]
    {
        Constants.MonsterAttack1,
        Constants.MonsterAttack2,
        Constants.MonsterTaunting,
        Constants.MonsterVictory,
        Constants.MonsterDie,
        Constants.MonsterHit
    };
    
    public UIMonsterInformation UiMonsterInfo => uiMonsterInfo;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetAnim(int idx)
    {
        if(idx < 0 || idx >= animCodeList.Length)
            return;
        
        var animCode = animCodeList[idx];
        animator.SetTrigger(animCode);
    }

    public void Hit()
    {
        animator.SetTrigger(Constants.MonsterHit);
    }
}
