using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
       : base(currentContext, playerStateFactory) { }
    public override void EnterState()
    {
        _ctx.Animator.SetBool(_ctx.IsWalkingHash, true);
        _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
    }

    public override void UpdateState()
    {
        _ctx.CurrentMovementX = _ctx.CurrentMovementInput.x * _ctx.MovementSpeed;
        _ctx.CurrentMovementZ = _ctx.CurrentMovementInput.y * _ctx.MovementSpeed;
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SwitchState(_factory.Run());
        }
        else if (!_ctx.IsMovementPressed)
        {
            SwitchState(_factory.Idle());
        }
    }
}
