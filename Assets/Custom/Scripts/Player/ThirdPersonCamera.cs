using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerModel;
    public Rigidbody rb;
    PlayerInput playerInput = new PlayerInput();

    public float rotationSpeed;
    Vector2 movementInput;

    private void Awake()
    {
        playerInput.PlayerControls.Move.started += context =>
        {
            movementInput = context.ReadValue<Vector2>();
        };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDir = orientation.forward * movementInput.x + orientation.right * movementInput.y;
        if (inputDir != Vector3.zero)
        {
            playerModel.forward = Vector3.Slerp(playerModel.forward, inputDir.normalized, rotationSpeed * Time.deltaTime);        }
    }
}
