using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

//Collects inputs and stores them so they can be accessed by other scripts
public class InputHandler : MonoBehaviour
{
    [Header("Script References")]
    //So we don't need to make it a single, and we can still access it where needed
    [SerializeField]private InputDeviceTracker _inputDeviceTracker;
    public InputDeviceTracker inputDeviceTracker => _inputDeviceTracker;
    private InputReader.InputReader _controls;
    //this allows us to access input actions such as interact, reload so we can get the binds
    //this setup allows us to assign in the inspector while also allowing other scripts to access it through a reference
    [SerializeField] private InputActionAsset _inputActionAsset;
    public InputActionAsset  inputActionAsset => _inputActionAsset;

    //Data available to other scripts (read-only)
    public Vector2 moveInput {get; private set;}
    public Vector2 lookInput {get; private set;}
    public bool isSprinting {get; private set;}
    public bool isCrouching {get; private set;}
    public bool jumpTriggered {get; private set;}
    public Vector2 scrollInputNorm {get; private set;}
    
    public bool isFireHeld {get; private set;}
    
    public bool interactPressed {get; private set;}
    

    #region Event Declarations

    //Event used for triggering a single shot for semi or bolt action weapons
    public delegate void FireAction();
    public event FireAction OnFireTriggered;
    
    public delegate void ReloadAction();
    public event ReloadAction OnReloadTriggered;

    public delegate void SwitchToPrimaryAction(int pos);
    public event SwitchToPrimaryAction OnSwitchToPrimaryBtnPressed;
    
    public delegate void SwitchToSecondaryAction(int neg);
    public event SwitchToSecondaryAction OnSwitchToSecondaryBtnPressed;
    #endregion
   
    
    
    
    void Awake()
    {
        //create instance of Input Reader
        _controls = new InputReader.InputReader();

        #region Player Inputs

        //subscribe to input events
        _controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        _controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        //Hold button for sprint; forces players to feel tense? 
        _controls.Player.Sprint.started += ctx =>
        {
            isSprinting = true;
            isCrouching = false;
        };
        _controls.Player.Sprint.canceled += ctx => isSprinting = false;

        //Toggle Crouch
        _controls.Player.Crouch.started += ctx => isCrouching = !isCrouching;
        
        //Trigger jump
        //Uncrouch player
        _controls.Player.Jump.started += ctx =>
        {
            isCrouching = false;
            jumpTriggered = true;
        };  
        
        //Interact
        _controls.Player.Interact.started += ctx => interactPressed = true;
        _controls.Player.Interact.canceled += ctx => interactPressed = false;

        #endregion

        #region Weapon Inputs

        //Used for full auto weapon check
        _controls.Weapons.Fire.started += ctx => isFireHeld = true;
        _controls.Weapons.Fire.canceled += ctx => isFireHeld = false;

        _controls.Weapons.Fire.performed += ctx => OnFireTriggered?.Invoke();
        
        //Reload
        _controls.Weapons.Reload.performed += ctx => OnReloadTriggered?.Invoke();

        //Switch Weapon
        _controls.Weapons.ScrollWheelSwitch.started += ctx => scrollInputNorm = ctx.ReadValue<Vector2>().normalized;
        _controls.Weapons.ScrollWheelSwitch.canceled += ctx => scrollInputNorm = Vector2.zero;
        
        _controls.Weapons.SwitchToPrimary.performed += ctx => OnSwitchToPrimaryBtnPressed?.Invoke(1);
        _controls.Weapons.SwitchToSecondary.performed += ctx => OnSwitchToSecondaryBtnPressed?.Invoke(-1);
       
        _controls.Weapons.SwitchToPrimary.performed += ctx => isSprinting = false;

        #endregion
    }

    public void ResetJump()
    {
        jumpTriggered = false;
    }
    
    //Tells unity when to listen for inputs
    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}
