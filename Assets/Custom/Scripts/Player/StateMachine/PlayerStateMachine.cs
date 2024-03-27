using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// context file: script responsible for handling player input and switching to the appropriate player state

public class PlayerStateMachine : MonoBehaviour
{
    // reference variables
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;

    // hashes to store animation states insead of booleans for optimization
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _onAttackHash;

    // variable to store the Vector2 produced by the player input
    Vector2 _currentMovementInput;
    // variable used to apply to the player's transform
    Vector3 _currentMovement;
    // variable to store movement vector converted relative to camera position
    Vector3 _cameraRelativeMovement;

    // booleans to store whether movement is detected from player input
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpPressed;
    bool _isAttackPressed;

    // logic booleans to prevent state repetition
    bool _requireNewJumpPress = false;
    bool _requireNewAttack = false;

    // controls the strength of character rotation when moving
    float _rotationFactorPerFrame = 10.0f;
    // movement variables
    float _runMultiplier = 3.0f;
    float _movementSpeed = 1.5f;
    float _initialJumpVelocity = 5.0f;

    bool _isAttacking = false;
    bool _attackLanded = false;

    float _groundedGravity = -.05f;
    float _gravity = -9.8f;

    // state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool IsAttackPressed { get { return _isAttackPressed; } }
    public Animator Animator { get { return _animator; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int OnAttackHash { get { return _onAttackHash; } }
    public bool IsAttacking { get { return _isAttacking; } set { _isAttacking = value; } }
    public bool AttackLanded { get { return _attackLanded; } set { _attackLanded = value; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float CurrentMovementZ { get { return _currentMovement.z; } set { _currentMovement.z = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public float GroundedGravity { get { return _groundedGravity; } }
    public float Gravity { get { return _gravity; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public bool RequireNewAttack { get { return _requireNewAttack; } set { _requireNewAttack = value; } }
    public float RunMultiplier { get { return _runMultiplier; } }
    public float MovementSpeed { get { return _movementSpeed; } }

    private void Awake()
    {
        // initialize reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // assign animation hashes
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _onAttackHash = Animator.StringToHash("onAttack");

        // expression to listen when the player input triggers the "Move" action,
        // and pass the context to the onMovementInput function
        _playerInput.PlayerControls.Move.started += onMovementInput;
        _playerInput.PlayerControls.Move.canceled += onMovementInput;
        _playerInput.PlayerControls.Move.performed += onMovementInput;
        _playerInput.PlayerControls.Run.started += onRun;
        _playerInput.PlayerControls.Run.canceled += onRun;
        _playerInput.PlayerControls.Jump.started += onJump;
        _playerInput.PlayerControls.Jump.canceled += onJump;
        _playerInput.PlayerControls.Attack.started += onAttack;
        _playerInput.PlayerControls.Attack.canceled += onAttack;
        _currentMovement.y = _groundedGravity;
    }
    void HandleRotation()
    {
        // the x and z vector for the player to face
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _cameraRelativeMovement.z;

        // the current rotation of the player
        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed && !_isAttacking)
        {
            // new quaternion based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            // spherical interpolation applied to player's rotation
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();
        _cameraRelativeMovement = ConvertToCameraSpace(_currentMovement);
        _characterController.Move(_cameraRelativeMovement * Time.deltaTime);
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        // read the Vector2 from player input
        _currentMovementInput = context.ReadValue<Vector2>();
        // if player input x or y does not equal 0 (negative or positive), movement pressed is true
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }


    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraFZproduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRXproduct = vectorToRotate.x * cameraRight;

        Vector3 vectorCameraSpace = cameraFZproduct + cameraRXproduct;
        vectorCameraSpace.y = vectorToRotate.y;
        return vectorCameraSpace;
    }

    void onRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void onJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    void onAttack(InputAction.CallbackContext context)
    {
        _isAttackPressed = context.ReadValueAsButton();
        _requireNewAttack = false;
    }

    private void OnEnable()
    {
        // enable listening when the player object is initialized
        _playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        // disable listening when the player object is uninitialized
        _playerInput.PlayerControls.Disable();
    }
}
