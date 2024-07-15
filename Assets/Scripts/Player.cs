using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostActiveTime = 2f;
    [SerializeField] private float boostDelayTime = 5f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticleRange = 80f;

    //references
    private PlayerInputHandler inputHandler;

    //internal variables
    private bool boostDoneDelay = true;

    private float currentSpeed;

    private void Awake()
    {
        inputHandler = PlayerInputHandler.Instance;
        currentSpeed = swimSpeed;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (boostDoneDelay && inputHandler.BoostTriggered)
        {
            StartCoroutine(HandleBoostRunAndDelay());
        }

        Vector3 inputDirection = new(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
        inputDirection.Normalize();

        transform.Translate(currentSpeed * Time.deltaTime * inputDirection);
    }

    private IEnumerator HandleBoostRunAndDelay()
    { 
        boostDoneDelay = false;
        currentSpeed *= boostMultiplier;
        yield return new WaitForSeconds(boostActiveTime);
        currentSpeed = swimSpeed;
        yield return new WaitForSeconds(boostDelayTime);
        boostDoneDelay = true;
    }

    private void HandleRotation() 
    {
        float mouseXRotation = inputHandler.LookInput.x * mouseSensitivity;
        float mouseYRotation = -inputHandler.LookInput.y * mouseSensitivity;
        transform.Rotate(mouseYRotation, mouseXRotation, 0);
    }
}
