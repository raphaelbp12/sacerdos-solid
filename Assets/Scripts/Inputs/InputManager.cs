using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] MoveController moveController;

    PlayerControls controls;
    PlayerControls.MovementActions movement;

    Vector2 horizontalInput;

    private void Awake()
    {
        controls = new PlayerControls();
        movement = controls.Movement;

        movement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        movement.Jump.performed += _ => OnJump();
    }

    private void OnJump()
    {
        print("jump");
    }

    private void Update()
    {
        moveController.ReceiveInput(horizontalInput);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
