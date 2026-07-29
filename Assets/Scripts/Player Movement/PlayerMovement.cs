using UnityEngine;

//Handles player movement and player physics
public class PlayerMovement : MonoBehaviour
{
    //Reference to the script that captures our input
    //and character controller
    private InputHandler _inputHandler;
    private CharacterController _characterController;

    [Header("Controller Properties")]
    [SerializeField]private float baseHeight;
    
    [Header("Movement Properties")]
    private float _currentMoveSpeed;
    [SerializeField]private float movementSpeed;
    [SerializeField]private float crouchMoveSpeed;
    [SerializeField]private float sprintSpeed;
    private bool _isMoving;
    
    [Header("Crouch Properties")]
    [SerializeField]private float crouchHeight;
    //how quickly the player crouches
    [SerializeField]private float crouchSpeed;
    private bool _isCrouching;

    [Header("Jump Properties")]
    [SerializeField]private float jumpForce;
    
    [Header("Gravity Properties")]
    [SerializeField]private float gravity = -9.81f;
    [SerializeField]private float groundForce = -2f; // Force to push player down ensure stays grounded
    private float _verticalVelocity;

    //Events
    public delegate void CrouchAction(bool isCrouching);
    public event CrouchAction OnCrouchChanged;
    
    
    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        DetermineMovementSpeed();
        CalculateGravity();

        if (_inputHandler.jumpTriggered && _characterController.isGrounded)
        {
            HandleJump();
        }
        
        MovePlayer();
        HandleCrouch();    
    }

    /// <summary>
    /// Determines what the current movement speed of the player should be.
    /// E.g.: If the player is crouched, apply crouched movement speed.
    /// </summary>
    private void DetermineMovementSpeed()
    {
        _currentMoveSpeed = _inputHandler.isSprinting ? sprintSpeed : 
            _inputHandler.isCrouching ? crouchMoveSpeed : 
            movementSpeed;
    }

    /// <summary>
    /// Calculates the vertical velocity of the player. This is stored in a variable and applied by the
    /// character controller's move function.
    /// </summary>
    private void CalculateGravity()
    {
        //only player down if they are on the floor and not trying to jump (vertical velocity)
        if (_characterController.isGrounded && _verticalVelocity < 0)
        {
            // We stay at a small negative number so we don't 'bounce' 
            // while walking down slopes
            _verticalVelocity = groundForce;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Applies movement to character controller.
    /// </summary>
    private void MovePlayer()
    {
        if (_inputHandler.moveInput.x != 0 || _inputHandler.moveInput.y != 0)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }
        
        Vector3 move = transform.forward * (_currentMoveSpeed * _inputHandler.moveInput.y) + 
                       transform.right * (_currentMoveSpeed * _inputHandler.moveInput.x);
         
        //apply our y velocity calculated in ApplyGravity()
        //to the y axis (untouched before) to the move vector
        move.y = _verticalVelocity;
         
        _characterController.Move(move * Time.deltaTime);
    }

    private void HandleCrouch()
    { 
        _isCrouching = _inputHandler.isCrouching;
        
        var targetHeight = _isCrouching ? crouchHeight : baseHeight;
        
        // Optimization: If we are already at the height we want, stop here.
        if (Mathf.Approximately(_characterController.height, targetHeight)) return;
        
        _characterController.height = Mathf.MoveTowards(_characterController.height, targetHeight, crouchSpeed * Time.deltaTime);
        // Adjust center: if height is 1, center should be 0.5. If height is 2, center is 1.0.
        _characterController.center = new Vector3(0, _characterController.height / 2f, 0);
        
        //Trigger Crouch event
        //Used for moving the camera to the correct position
        OnCrouchChanged?.Invoke(_isCrouching);
    }

    private void HandleJump()
    {
        _inputHandler.ResetJump();
        _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    //Return Values
    public float ReturnCurrentMoveSpeed()
    {
        return _currentMoveSpeed;
    }

    public bool ReturnIsMoving()
    {
        return _isMoving;
    }
    
    public float ReturnCrouchSpeed()
    {
        return crouchSpeed;
    }
}
