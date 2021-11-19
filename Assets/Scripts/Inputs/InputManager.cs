using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    [SerializeField] MoveController moveController;
    [SerializeField] InventoryManager inventoryManager;

    PlayerControls controls;
    PlayerControls.MovementActions movement;
    PlayerControls.TestActions test;

    Vector2 horizontalInput;

    private void Awake()
    {
        controls = new PlayerControls();
        movement = controls.Movement;
        test = controls.Test;

        movement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        movement.Jump.performed += _ => OnJump();
        movement.Jump.performed += _ => inventoryManager.AddRandomItemToInventory();

        test.Test1.performed += _ => inventoryManager.SaveInventories();
        test.Test2.performed += _ => inventoryManager.LoadInventories();
    }

    private void OnJump()
    {
        print("jump");
    }

    [Client]
    private void Update()
    {
        if (!hasAuthority) return;
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
