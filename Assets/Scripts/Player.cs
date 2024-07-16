using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Water Movement")]
    [SerializeField] private float forwardswimSpeed;
    [SerializeField] private float nonForwardSwimSpeed;
    [SerializeField] private float boostMultiplier;
    [SerializeField] private float boostActiveTime;
    [SerializeField] private float boostDelayTime;
    [SerializeField] private float massInWater;
    [SerializeField] private float dragInWater;

    [Header("Air Movement")]
    [SerializeField] private float massOutWater;
    [SerializeField] private float dragOutWater;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity;

    //references
    private PlayerInputHandler inputHandler;

    //internal variables
    private bool boostDoneDelay = true;
    private bool boostActive;
    private Rigidbody rb;
    private bool movementEnabled;


    //---------------------- METHODS ----------------------\\

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        while (inputHandler == null) //wait for inputHandler to exist
        {
            inputHandler = PlayerInputHandler.Instance;
        }

        if (movementEnabled) HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (boostDoneDelay && inputHandler.BoostTriggered)
        {
            StartCoroutine(HandleBoost());
        }

        Vector3 moveVector = //multiply by forward/non-forward speeds
            (inputHandler.MoveInput.y > 0 ? forwardswimSpeed : nonForwardSwimSpeed) * inputHandler.MoveInput.y * transform.forward
            + inputHandler.MoveInput.x * nonForwardSwimSpeed * transform.right;

        moveVector *= (boostActive ? boostMultiplier : 1); //add boost multiplier

        rb.AddForce(Time.deltaTime * moveVector, ForceMode.VelocityChange);
    }

    private IEnumerator HandleBoost()
    {
        boostDoneDelay = false;
        boostActive = true;
        yield return new WaitForSeconds(boostActiveTime);
        boostActive = false;
        yield return new WaitForSeconds(boostDelayTime);
        boostDoneDelay = true;
    }

    //method to rotate player dolphin to mouse movement
    private void HandleRotation()
    {
        float mouseXRotation = inputHandler.LookInput.x * mouseSensitivity;
        float mouseYRotation = -inputHandler.LookInput.y * mouseSensitivity;
        rb.rotation *= Quaternion.Euler(mouseYRotation, mouseXRotation, 0);
    }

    //method to handle in water physics
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Water"))
        {
            movementEnabled = true;
            rb.drag = dragInWater;
            rb.mass = massInWater;
            rb.useGravity = false;
        }
    }

    //method to handle out of water physics
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            movementEnabled = false;
            rb.drag = dragOutWater;
            rb.mass = massOutWater;
            rb.useGravity = true;
        }
    }
}
