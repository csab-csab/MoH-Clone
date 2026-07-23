using UnityEngine;
using UnityEngine.Serialization;
using Input = UnityEngine.Input;

//Responsible for rotating the player around the y-axis
// and the camera around the x-axis
public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    private InputHandler _inputHandler;
    private Transform _playerTransform;
    [SerializeField]private Transform cameraPivot;
    
    [Header("Control Properties")]
    [SerializeField]private Vector2 mouseSensitivity;
    
    [Header("Y-rotation Clamp")]
    [SerializeField]private float clampAnglePos = 90;
    [SerializeField]private float clampAngleNeg = -90;

    //Current y-rotation of the camera pivot
    private float _verticalRotation; 
    
    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _playerTransform = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {   
        //Get mouse input
        float mouseX = _inputHandler.lookInput.x *  mouseSensitivity.x * Time.deltaTime;
        float mouseY = _inputHandler.lookInput.y *  mouseSensitivity.y * Time.deltaTime;
        
        //Rotate players transform on the y-axis using the horizontal input from the mouse
        _playerTransform.Rotate(Vector3.up, mouseX);
        
        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, clampAngleNeg, clampAnglePos);
        
        cameraPivot.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }
 
}
