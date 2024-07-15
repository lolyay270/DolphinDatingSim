using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Controls _controls;

    private InputAction _moveAction, _lookAction;

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _moveAction = _controls.Player.Move;
        _moveAction.Enable();
        _lookAction = _controls.Player.Look;
        _lookAction.Enable();
        _controls.Player.Bite.performed += OnJump;
        _controls.Player.Bite.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _controls.Player.Bite.Disable();
    }

    private void OnJump(InputAction.CallbackContext contect)
    {
        print("bite");
    }

    private void FixedUpdate()
    {
        Vector2 moveDir = _moveAction.ReadValue<Vector2>();
        print($"move: {moveDir}");
        Vector2 lookDir = _lookAction.ReadValue<Vector2>();
        print ($"look: {lookDir}");
    }
}
