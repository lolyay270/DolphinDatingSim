using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    //internal actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction biteAction;
    private InputAction boostAction;

    //Values from Player
    [HideInInspector] public Vector2 MoveInput { get; private set; }
    [HideInInspector] public Vector2 LookInput { get; private set; }
    [HideInInspector] public bool BiteTriggered { get; private set; }
    [HideInInspector] public bool BoostTriggered { get; private set; }

    //singleton
    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        //set up only one singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //link to actions in Controls
        moveAction = playerControls.FindActionMap(actionMapName).FindAction("Move");
        lookAction = playerControls.FindActionMap(actionMapName).FindAction("Look");
        biteAction = playerControls.FindActionMap(actionMapName).FindAction("Bite");
        boostAction = playerControls.FindActionMap(actionMapName).FindAction("Boost");

        RegisterInputActions();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        biteAction.Enable();
        boostAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        biteAction.Disable();
        boostAction.Disable();
    }

    //read and set values from player
    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        biteAction.performed += context => BiteTriggered = true;
        biteAction.canceled += context => BiteTriggered = false;

        boostAction.performed += context => BoostTriggered = true;
        boostAction.canceled += context => BoostTriggered = false;
    }
}
