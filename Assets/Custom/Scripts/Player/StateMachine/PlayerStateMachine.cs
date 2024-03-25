using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// context file: script responsible for handling player input and switching to the appropriate player state
// TODO: keep following this https://youtu.be/kV06GiJgFhc?si=XO6J8nTtnmjVC2e5 tutorial and finish this movement and camera junk (27:05)

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

    // variable to store the Vector2 produced by the player input
    Vector2 _currentMovementInput;
    // variable used to apply to the player's transform
    Vector3 _currentMovement;
    // boolean to store whether movement is detected from player input
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpPressed;
    bool _requireNewJumpPress = false;
    // controls the strength of character rotation when moving
    float _rotationFactorPerFrame = 10.0f;
    // factor to increase movement by when running
    float _runMultiplier = 3.0f;
    float _movementSpeed = 1.5f;

    bool _isJumping = false;
    float _initialJumpVelocity = 5.0f;

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
    public Animator Animator { get { return _animator; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public bool IsJumping { set { _isJumping = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public float GroundedGravity { get { return _groundedGravity; } }
    public float Gravity { get { return _gravity; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }

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

        // expression to listen when the player input triggers the "Move" action,
        // and pass the context to the onMovementInput function
        _playerInput.PlayerControls.Move.started += onMovementInput;
        _playerInput.PlayerControls.Move.canceled += onMovementInput;
        _playerInput.PlayerControls.Move.performed += onMovementInput;
        _playerInput.PlayerControls.Run.started += onRun;
        _playerInput.PlayerControls.Run.canceled += onRun;
        _playerInput.PlayerControls.Jump.started += onJump;
        _playerInput.PlayerControls.Jump.canceled += onJump;
    }
    void HandleRotation()
    {
        // the x and z vector for the player to face
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;

        // the current rotation of the player
        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            // new quaternion based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            // spherical interpolation applied to player's rotation
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (!_isJumping && _characterController.isGrounded && _isJumpPressed)
        {
            _animator.SetBool(_isJumpingHash, true);
            _isJumping = true;
            _currentMovement.y = _initialJumpVelocity;
        }
        else if (!_isJumpPressed && _isJumping && _characterController.isGrounded)
        {
            _isJumping = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        _currentState.UpdateState();
        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        // read the Vector2 from player input
        _currentMovementInput = context.ReadValue<Vector2>();
        // translate player input into player transform movement
        _currentMovement.x = _currentMovementInput.x * _movementSpeed;
        _currentMovement.z = _currentMovementInput.y * _movementSpeed;
        // if player input x or y does not equal 0 (negative or positive), movement pressed is true
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
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
