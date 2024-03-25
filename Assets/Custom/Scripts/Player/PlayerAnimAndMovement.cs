using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// source: https://youtu.be/bXNFxQpp2qk?si=JkSSlhzQxIkHS-vS

public class PlayerAnimAndMovement : MonoBehaviour
{
    // reference variables
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;
    public Camera PlayerCamera;

    // hashes to store animation states insead of booleans for optimization
    int _isWalkingHash;
    int _isRunningHash;

    // variable to store the Vector2 produced by the player input
    Vector2 _currentMovementInput;
    // variable used to apply to the player's transform
    Vector3 _currentMovement;
    // boolean to store whether movement is detected from player input
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpedPressed;
    // controls the strength of character rotation when moving
    float _rotationFactorPerFrame = 10.0f;
    // factor to increase movement by when running
    float _runMultiplier = 3.0f;
    float _movementSpeed = 1.5f;

    float _groundedGravity = -.05f;
    float _gravity = -9.8f;


    private void Awake()
    {
        // initialize reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // assign animation hashes
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");

        // expression to listen when the player input triggers the "Move" action,
        // and pass the context to the onMovementInput function
        _playerInput.PlayerControls.Move.started += onMovementInput;
        _playerInput.PlayerControls.Move.canceled += onMovementInput;
        _playerInput.PlayerControls.Move.performed += onMovementInput;
        _playerInput.PlayerControls.Run.started += onRun;
        _playerInput.PlayerControls.Run.canceled += onRun;
        _playerInput.PlayerControls.Jump.started += onJump;
        _playerInput.PlayerControls.Jump.canceled+= onJump;
    }

    void handleAnimation()
    {
        // local storage of defined animation booleans
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        // set walking animation
        if (_isMovementPressed && !isWalking)
        {
            _animator.SetBool(_isWalkingHash, true);
        }

        else if (!_isMovementPressed && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false);
        }

        // set running animation
        if (_isMovementPressed && _isRunPressed && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }

        else if ((!_isMovementPressed || !_isRunPressed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }
    }

    void handleRotation()
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

    void handleGravity()
    {
        if (_characterController.isGrounded)
        {
            _currentMovement.y = _groundedGravity;
        } 
        else
        {
            _currentMovement.y += _gravity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _movementVector;
        handleRotation();
        handleAnimation();
        if (_isRunPressed)
        {
            _movementVector = ConvertToCameraSpace(_currentMovement * _runMultiplier * Time.deltaTime);
            _characterController.Move(_movementVector);
        }
        else
        {
            _movementVector = ConvertToCameraSpace(_currentMovement * Time.deltaTime);
            _characterController.Move(_movementVector);
        }
        handleGravity();
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

        return cameraFZproduct + cameraRXproduct;
    }

    void onJump(InputAction.CallbackContext context)
    {
        _isJumpedPressed = context.ReadValueAsButton();
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
