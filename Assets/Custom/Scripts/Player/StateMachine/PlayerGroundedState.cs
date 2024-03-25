using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    // constructor
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory)
    {
        InitializeSubState();
    }
    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {
        // if the player is not moving or running = idle
        if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        }
        // if the player is moving but not running = walk
        else if (_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        }
        // player must be moving and running = run
        else
        {
            SetSubState(_factory.Run());
        }
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsJumpPressed)
        {
            SwitchState(_factory.Jump());
        }
    }
}
