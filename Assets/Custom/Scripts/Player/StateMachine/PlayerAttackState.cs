using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
      : base(currentContext, playerStateFactory) { }
    public override void EnterState()
    {
        //Debug.Log("ATTACK");
        _ctx.Animator.SetBool(_ctx.OnAttackHash, true);
        _ctx.Animator.SetBool(_ctx.IsWalkingHash, false);
        _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
        _ctx.IsAttacking = true;
        _ctx.RequireNewAttack = true;
        _ctx.CurrentMovementX = 0;
        _ctx.CurrentMovementZ = 0;
        _ctx.AttackLanded = false;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        _ctx.Animator.SetBool(_ctx.OnAttackHash, false);
        _ctx.IsAttacking = false;
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (_ctx.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            //Debug.Log("not attacking anymore");
            if (!_ctx.IsMovementPressed)
            {
                SwitchState(_factory.Idle());
            }
            else if (_ctx.IsMovementPressed)
            {
                SwitchState(_factory.Walk());
            }
        }
    }
}
