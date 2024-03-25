using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        InitializeSubState();
    }
    public override void EnterState()
    {
        HandleJump();
        _ctx.RequireNewJumpPress = true;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleGravity();
    }

    public override void ExitState()
    {
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
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
        if (_ctx.CharacterController.isGrounded && !_ctx.RequireNewJumpPress)
        {
            SwitchState(_factory.Grounded());
        }
    }

    void HandleJump()
    {
        _ctx.IsJumping = true;
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocity;
    }

    void HandleGravity()
    {
        if (!_ctx.CharacterController.isGrounded)
        {
            _ctx.CurrentMovementY += _ctx.Gravity * Time.deltaTime;
        }
        else
        {
            _ctx.CurrentMovementY += _ctx.GroundedGravity * Time.deltaTime;
        }
    }
}
